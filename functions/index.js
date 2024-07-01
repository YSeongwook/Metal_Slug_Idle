const functions = require('firebase-functions');
const admin = require('firebase-admin');
admin.initializeApp();
const fs = require('fs');
const path = require('path');

// GachaData.json 파일 경로 설정
const gachaDataPath = path.join(__dirname, 'GachaData.json');
let gachaData;

// GachaData.json 파일 읽기 및 초기화
fs.readFile(gachaDataPath, 'utf8', (err, data) => {
    if (err) {
        console.error('Error reading gacha data file:', err);
        return;
    }
    const parsedData = JSON.parse(data).data;
    // 가중치를 할당하여 gachaData 초기화
    gachaData = parsedData.map(hero => ({
        id: hero.id,
        weight: getWeightByRank(hero.rank)
    }));
});

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

    if (!gachaData) {
        return res.status(500).send('Gacha data not initialized');
    }

    const picker = new WeightedRandomPicker(gachaData);
    const heroIds = picker.pickMultiple(drawCount);

    try {
        const heroCollectionRef = admin.database().ref(`/user_HeroCollection/${userId}`);
        const heroDataSnapshot = await heroCollectionRef.once('value');

        let heroCollection = heroDataSnapshot.val() || [];

        // heroCollection이 객체일 경우 배열로 변환
        if (!Array.isArray(heroCollection)) {
            heroCollection = Object.values(heroCollection);
        }

        // 뽑은 영웅을 컬렉션에 추가
        heroIds.forEach(heroId => {
            const heroExists = heroCollection.find(hero => hero.id === heroId);
            if (heroExists) {
                heroExists.owned = true;
            } else {
                heroCollection.push({ id: heroId, owned: true });
            }
        });

        const base64HeroCollection = Buffer.from(JSON.stringify(heroCollection)).toString('base64');

        await heroCollectionRef.set({ heroCollection: base64HeroCollection });

        res.status(200).json({ result: heroIds });
    } catch (error) {
        console.error('Error updating hero collection:', error);
        res.status(500).send('Internal Server Error');
    }
});
