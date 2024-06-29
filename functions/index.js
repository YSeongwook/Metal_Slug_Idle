const functions = require('firebase-functions');
const admin = require('firebase-admin');
admin.initializeApp();

class WeightedRandomPicker {
    constructor() {
        this.itemWeightDict = new Map();
        this.sumOfWeights = 0;
    }

    add(item, weight) {
        if (weight <= 0) throw new Error('Weight must be positive');
        this.itemWeightDict.set(item, weight);
        this.sumOfWeights += weight;
    }

    getRandomPick() {
        let rand = Math.random() * this.sumOfWeights;
        let cumulativeWeight = 0;
        for (let [item, weight] of this.itemWeightDict.entries()) {
            cumulativeWeight += weight;
            if (rand < cumulativeWeight) {
                return item;
            }
        }
    }
}

const picker = new WeightedRandomPicker();
picker.add("HeroA", 50);
picker.add("HeroB", 30);
picker.add("HeroC", 20);

exports.gacha = functions.https.onRequest(async (req, res) => {
    const userId = req.body.userId;
    const gachaResult = picker.getRandomPick();

    const userRef = admin.database().ref(`users/${userId}/heroCollection`);
    const snapshot = await userRef.once('value');
    const heroCollection = snapshot.val() || [];

    heroCollection.push(gachaResult);
    await userRef.set(heroCollection);

    res.json({ result: gachaResult });
});
