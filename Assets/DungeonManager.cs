using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pheryus { 
    public class DungeonManager : MonoBehaviour {

        public List<EnemyWave> waves;

        public Transform battleboard;

        Card[] actualCards;

        public static DungeonManager instance;

        public PlayerDeck playerDeck;

        public ShopManager shopManager;

        public int enemyCards = 0;

        public enum GameState { rest, shop, battle };

        public GameState state = GameState.shop;

        public int actualWaveDifficult, actualShopLevel;

        public uint[] dungeonMarkers, shopMarkers;

        public int challengesWon = 0;

        EnemyWave actualWave;

        private void Awake() {
            instance = this;
        }

        private void Start() {
            actualWaveDifficult = 0;
            NewStage();
        }

        public void NewStage() {
            if (state == GameState.shop) {
                shopManager.StartShop();
            }
            else if (state == GameState.battle) {
                StartBattle();
            }
        }

        void StartBattle() {
            if (dungeonMarkers.Length > actualWaveDifficult - 1 && challengesWon >= dungeonMarkers[actualWaveDifficult]) {
                actualWaveDifficult++;
            }
            actualCards = new Card[3];
            CreateWave();
        }

        void CreateWave() {
            if (actualWaveDifficult >= waves.Count) {
                actualWave = waves[waves.Count - 1];
            }
            else {
                actualWave = waves[actualWaveDifficult];
            }
            Option selectedOption = actualWave.GetRandomOption();

            if (selectedOption.cards.Count == 1) {
                CreateCard(selectedOption.cards[0], 1);
            }
            else {
                for (int i = 0; i < selectedOption.cards.Count; i++) {
                    CreateCard(selectedOption.cards[i], i);
                }
            }
            enemyCards = selectedOption.cards.Count;
        }

        public void DestroyCard(Card card) {
            Transform playerCards = battleboard.GetChild(card.position).GetChild(0);
            for (int i = playerCards.childCount - 1; i >= 0; i--) {
                playerDeck.DiscardCard(playerCards.GetChild(i).GetComponent<CardFramework.Card>());
            }

            actualCards[card.position] = null;

            enemyCards--;
 
            Destroy(card.gameObject);

        }

        void CheckNewWave() {
            challengesWon++;
            NewStage();
        }

        void CreateCard(CardInfo c, int id) {
            GameObject go = Instantiate(CardManager.instance.enemyCardPrefab, battleboard.GetChild(id));
            Card card = go.GetComponent<Card>();

            card.position = id;
            card.cardInfo = c;
            actualCards[id] = card;
        }


        public void EndRound() {
           if (state == GameState.battle) {

                foreach (Card c in actualCards) {
                    if (c != null && c.GetComponent<EnemyCard>() && c.GetComponent<EnemyCard>().IsAlive()) {
                        continue;
                    }
                }
            
                if (enemyCards <= 0) {
                    Debug.Log("shop");
                    state = GameState.shop;
                    NewStage();
                }
                else {
                    EnemiesAttack();
                }
            }
            else if (state == GameState.shop) {
                shopManager.CloseShop();
                state = GameState.battle;
                NewStage();
            }
        }

        public void EnemiesAttack() {
            int dmg = 0;
            if (actualCards == null) {
                Debug.Break();
            }

            foreach (Card c in actualCards) {
                if (c != null) {
                    dmg++;
                }
            }
            playerDeck.DamageDeck(dmg);
        }
    }
}