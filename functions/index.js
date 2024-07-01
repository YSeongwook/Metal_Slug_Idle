const functions = require('firebase-functions');
const admin = require('firebase-admin');
admin.initializeApp();
const db = admin.database();

// 등급에 따른 가중치를 반환하는 함수
function getWeightByRank(rank) {
    switch(rank) {
        case 'S': return 0.01; // 1%
        case 'A': return 0.04; // 4%
        case 'B': return 0.15; // 15%
        case 'C': return 0.30; // 30%
        case 'D': return 0.50; // 50%
        default: return 0.0;
    }
}

// WeightedRandomPicker 클래스 정의
class WeightedRandomPicker {
    constructor(items) {
        this.items = items;
        // 전체 가중치 합 계산
        this.totalWeight = items.reduce((sum, item) => sum + item.weight, 0);
    }

    // 단일 항목을 랜덤하게 선택하는 메서드
    pick() {
        const random = Math.random() * this.totalWeight;
        let weightSum = 0;

        for (const item of this.items) {
            weightSum += item.weight;
            if (random < weightSum) {
                return item.id;
            }
        }
    }

    // 여러 항목을 랜덤하게 선택하는 메서드
    pickMultiple(count) {
        let results = [];
        for (let i = 0; i < count; i++) {
            results.push(this.pick());
        }
        return results;
    }
}

// gacha Firebase Function 정의
exports.gacha = functions.region('asia-northeast1').https.onRequest(async (req, res) => {
    const userId = req.body.userId;
    const drawCount = req.body.drawCount || 1; // 기본 뽑기 횟수는 1회

    if (!userId) {
        return res.status(400).send('Missing userId');
    }

    try {
        // Firebase Realtime Database에서 GachaData 읽어오기
        const gachaDataSnapshot = await db.ref('GachaData').once('value');
        const gachaData = gachaDataSnapshot.val().data;

        if (!gachaData) {
            return res.status(500).send('Gacha data not found');
        }

        // 가중치를 할당하여 gachaData 초기화
        const weightedGachaData = gachaData.map(hero => ({
            id: hero.id,
            weight: getWeightByRank(hero.rank)
        }));

        const picker = new WeightedRandomPicker(weightedGachaData);
        const heroIds = picker.pickMultiple(drawCount);

        const heroCollectionRef = db.ref(`/user_HeroCollection/${userId}`);
        const heroDataSnapshot = await heroCollectionRef.once('value');

        let heroCollection = heroDataSnapshot.val().heroCollection || [];
        if (!Array.isArray(heroCollection)) {
            heroCollection = new Array(15).fill().map((_, index) => ({ id: index, owned: false }));
        }

        // 뽑은 영웅을 컬렉션에 추가
        heroIds.forEach(heroId => {
            if (heroId >= 0 && heroId < heroCollection.length) {
                heroCollection[heroId].owned = true;
            }
        });

        const base64HeroCollection = Buffer.from(JSON.stringify(heroCollection)).toString('base64');

        await heroCollectionRef.set({ heroCollection: base64HeroCollection });

        res.status(200).json({ result: heroIds });
    } catch (error) {
        console.error('Error processing gacha:', error);
        res.status(500).send('Internal Server Error');
    }
});
