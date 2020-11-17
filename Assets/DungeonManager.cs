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

        public Dealer dealer;

        public int enemyCards = 0;

        public enum GameState { rest, shop, battle, tutorial };

        public GameState state = GameState.battle;

        public int actualWaveDifficult, actualShopLevel;

        public uint[] dungeonMarkers, shopMarkers;

        public int challengesWon = 0;

        EnemyWave actualWave;

        private void Awake() {
            instance = this;
        }

        private void Start() {
            actualCards = new Card[3];
            actualWaveDifficult = 0;
            if (Tutorial.instance.active) {
                state = GameState.tutorial;
            }
            NewStage();
        }

        public void NewStage() {
            if (state == GameState.shop) {
                shopManager.StartShop();
            }
            else if (state == GameState.battle) {
                StartBattle();
            }
            playerDeck.StartNewTurn();
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
            CreateEnemyLevel(selectedOption.wave);
        }

        public void CreateEnemyLevel (Wave selectedOption) {
            if (selectedOption == null) {
                return;
            }

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

        public IEnumerator DestroyCard(Card card) {
            Transform playerCards = battleboard.GetChild(card.position).GetChild(0);

            for (int i = playerCards.childCount - 1; i >= 0; i--) {
                playerDeck.AddCardToDiscard(playerCards.GetChild(i).GetComponent<CardFramework.Card>());
                
            }
            yield return null;

            actualCards[card.position] = null;
            enemyCards--;
            Destroy(card.gameObject);

        }

        void CheckNewWave() {
            challengesWon++;
            NewStage();
        }

        void CreateCard(CardInfo c, int id) {
            Transform enemyTransform = battleboard.GetChild(id);
            GameObject go = Instantiate(CardManager.instance.enemyCardPrefab, enemyTransform );
            Card card = go.GetComponent<Card>();

            card.position = id;
            card.cardInfo = c;
            card.GetComponent<CardFramework.Card>().FrontBecameVisible();
            actualCards[id] = card;

            card.GetComponent<EnemyCard>().remainingShields = card.cardInfo.shieldValue;

            Vector3 shieldOffset = new Vector3(-0.08f, 0, -0.008f);

            for (int i = 0; i < card.cardInfo.shieldValue; i++) {
                GameObject shieldGO = Instantiate(CardManager.instance.shieldPrefab, enemyTransform.GetChild(1));
                shieldGO.transform.localPosition += shieldOffset * i;
            }
        }

        public bool WillKillEveryEnemyCard() {
            foreach (Card c in actualCards) {
                if (c != null && c.GetComponent<EnemyCard>() && !c.GetComponent<EnemyCard>().WillDie()) {
                    return false;
                }
            }
            return true;
        }

        IEnumerator PlayerAttackEnemyCards() {
            foreach (Card c in actualCards) {
                if (c == null)
                    continue;

                EnemyCard en = c.GetComponent<EnemyCard>();
                en.strength = 0;
                en.attachedCards = 0;
                Transform shieldTransform = en.transform.parent.GetChild(1);

                foreach (CardFramework.Card playerCard in en.playerCards) {
                    playerCard.SetDamp(.1f);

                    if (en != null) {
                        Vector3 targetPosition = en.transform.position;
                        Transform shieldToBeDestroyed = null;

                        if (en.remainingShields > 0) {
                            shieldToBeDestroyed = shieldTransform.GetChild(shieldTransform.childCount - 1);
                            targetPosition = shieldToBeDestroyed.position;
                        }

                        playerCard.SetNewTarget(targetPosition, playerCard.transform.rotation);

                        while (!playerCard.OnTargetPosition()) {
                            yield return null;
                        }

                        if (playerCard.IsClub) {
                            en.remainingShields = 0;
                            DestroyAllShield(shieldTransform);
                        }

                        if (en.remainingShields > 0) {
                            en.remainingShields--;

                            Destroy(shieldToBeDestroyed.gameObject);
                        }
                        else {
                            en.ReduceHp((int)playerCard.cardInfo.rank + 1);
                        }

                        if (en.IsDead()) {
                            DestroyEnemyCard(en);
                        }
                    }

                    dealer.MoveCardToDiscardSlot(playerCard);
                    while (!playerCard.OnTargetPosition()) {
                        yield return null;
                    }
                    playerCard.SetDamp(.2f);

                }
                en.ResetPlayerCards();
            }
        }

        void DestroyAllShield (Transform t) {
            foreach (Transform child in t) {
                Destroy(child.gameObject);
            }
        }

        IEnumerator Combat() {

            yield return StartCoroutine(PlayerAttackEnemyCards());
            yield return StartCoroutine(EnemiesAttack());
            playerDeck.StartNewTurn();
        }

        void DestroyEnemyCard(EnemyCard en) {
            Destroy(en.GetComponent<CardFramework.Card>().TargetTransform.gameObject);
            Destroy(en.gameObject);
        }

        public void EndRound() {
           if (state == GameState.battle) {

                StartCoroutine(Combat());
                    
                if (enemyCards <= 0) {
                    Debug.Log("shop");
                    state = GameState.shop;
                    NewStage();
                }
            }
            else if (state == GameState.shop) {
                shopManager.CloseShop();
                state = GameState.battle;
                NewStage();
            }
           else if (state == GameState.tutorial) {
                StartCoroutine(Combat());
            }
        }

        public IEnumerator EnemiesAttack() {

            foreach (CardFramework.Card c in actualCards) {
                if (c != null) { 
                    Vector3 previousPos = c.transform.position;
                    c.SetDamp(0.1f);
                    c.SetNewTarget(dealer.GetDeckPosition(), c.transform.rotation);
                    while (!c.OnTargetPosition()) {
                        yield return null;
                    }
                    playerDeck.DamageDeck(1);

                    if (Tutorial.instance.active) {
                        yield return StartCoroutine(Tutorial.instance.TakeDmg());
                    }

                    yield return new WaitForSeconds(0.3f);
                    c.SetNewTarget(previousPos, c.transform.rotation);
                    while (!c.OnTargetPosition()) {
                        yield return null;
                    }
                    c.SetDamp(0.2f);
                }
            }
        }
    }
}