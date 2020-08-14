using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DeckInfo {
    public Rank rank;
    public Suit suit;
    public int qtd;
}
namespace Pheryus { 
    public class PlayerDeck : MonoBehaviour {


        public List<DeckInfo> starterDeck;

        public List<CardInfo> playerDeck;

        public CardInfo[] hand;


        List<Card> discardPile;

        List<Card> lostPile;

        public Vector2 cardDistance;

        public PlayerStatus playerStatus;

        public static PlayerDeck instance;

        public CardSlot cardSlot;

        [SerializeField]
        private Dealer _dealer;

        [SerializeField]
        private CardDeck _cardDeck;

        private IEnumerator Start() {
            instance = this;
            SetupCardManager();
            CreateDeck();
            ShuffleDeck();
            InstantiateDeck();
            
            yield return StartCoroutine(DrawCards());
        }

        void SetupCardManager() {
            playerDeck = new List<CardInfo>();
            hand = new CardInfo[5];
            discardPile = new List<Card>();
            lostPile = new List<Card>();
        }


        IEnumerator DrawCards() {
            yield return new WaitForSeconds(0.3f);
            for (int i = 0; i < 5; i++) {
                if (hand[i] == null || hand[i].card == null) {
                    CardInfo drawCard = DrawCard();
                    if (drawCard == null) {
                        if (discardPile.Count > 0) {
                            yield return StartCoroutine(ShuffleDiscardToDeck());
                            drawCard = DrawCard();
                        }
                        else {
                            yield break;
                        }
                    }
                    hand[i] = drawCard;
                    hand[i].card = drawCard.card;
                    yield return StartCoroutine(DrawCardCoroutine(drawCard, i));
                }
            }
        }

        IEnumerator ShuffleDiscardToDeck() {
            for (int i = discardPile.Count - 1; i >= 0; i--) {
                Card card = discardPile[i];
                AddCardToDeck(card);
                discardPile.RemoveAt(i);
            }
            yield return new WaitForSeconds(0.5f);
            ShuffleDeck();
            yield return StartCoroutine(_dealer.ShuffleCoroutine());
        }

        Vector3[] GetDeckCardPosition() {
            Vector3[] cardsPosition = new Vector3[playerDeck.Count];
            for (int i = 0; i < playerDeck.Count; i++) {
                if (playerDeck[i].card != null) {
                    cardsPosition[i] = playerDeck[i].card.transform.localPosition;
                }
            }
            return cardsPosition;
        }

        CardInfo DrawCard() {
            //return cardSlot.TopCard().cardInfo;
            
            if (playerDeck.Count > 0) {
                CardInfo card = playerDeck[0];
                playerDeck.RemoveAt(0);
                return card;
            }
            return null;
            
        }

        void CreateDeck() {
            foreach (DeckInfo di in starterDeck) {

                for (int i = 0; i < di.qtd; i++) {
                    CardInfo card = new CardInfo(di.rank, di.suit);
                    playerDeck.Add(card);
                }
            }

        }

        void InstantiateDeck() {
            _cardDeck.InstantiateDeck("cards");
            _dealer.DropCards();
        }

        public void AddCardToDiscard (CardFramework.Card card) {
            _dealer.MoveCardToDiscardSlot(card);
            _dealer.SetCardToPlayer(card);
            discardPile.Add(card);
        }

        public void AddCardToDeck (Card card) {
            card.position = -1;
            CardInfo newCardInfo = new CardInfo(card.cardInfo.rank, card.cardInfo.suit);
            newCardInfo.card = card;
            card.GetComponent<DraggableCard>().enabled = false;
            _dealer.MoveCardToDeck(card.GetComponent<CardFramework.Card>());
            playerDeck.Add(newCardInfo);
        }

        public void DiscardCardFromHand (int position) {
            hand[position] = new CardInfo(Rank.ace, Suit.clubs);
        }


        public void DiscardCard (CardFramework.Card card) {
            AddCardToDiscard(card);
            hand[card.position] = new CardInfo(Rank.ace, Suit.clubs);
            UpdateHand();
        }

        public void LoseCard(Card card) {
            //card.transform.SetParent(lostTransform);
            //card.flipUp = true;
            //card.transform.localPosition = Vector3.zero;
            _dealer.MoveCardToLoseSlot(card.GetComponent<CardFramework.Card>());
            lostPile.Add(card);
        }

        public void LoseCardFromHand(Card card) {
            hand[card.position] = new CardInfo(Rank.ace, Suit.clubs);
            LoseCard(card);
        }

        void UpdateHand() {
            for (int i = 0; i < 4; i++) {
                if (hand[i].card == null) {
                    int index = GetIndexFromNextCard(i+1);
                    if (index != -1) {
                        hand[i] = new CardInfo(hand[index].rank, hand[index].suit);
                        hand[i].card = hand[index].card;
                        hand[index].card = null;
                        hand[i].card.position = i;
                        StartCoroutine(_dealer.MoveCardInHand(hand[i].card, i));
                    }
                }
                /*
                if (hand[i].card != null) {
                    //hand[i].card.transform.SetParent(handTransform.GetChild(i));
                    hand[i].card.transform.localPosition = Vector3.zero;
                }
                */
            }
        }

        public void PlayDiamondCard (Card card) {
            PlayerStatus.instance.gold += (int)card.cardInfo.rank + 1;
            PlayerAction.instance.Act(card);
            DiscardCard(card.GetComponent<CardFramework.Card>());
        }

        int GetIndexFromNextCard(int id) {
            for (int i = id; i < 5; i++) {
                if (hand[i].card != null) {
                    return i;
                }
            }
            return -1;
        }

        public void DamageDeck (int dmg) {

            int remainingDamage = playerStatus.ReducePlayerLife(dmg);

            bool die = false;

            for (int i = 0; i < remainingDamage; i++) {
                if (playerDeck.Count == 0) {
                    if (discardPile.Count == 0) {
                        die = true;
                        break;
                    }
                    ShuffleDiscardToDeck();
                }
            

                CardInfo cardInfo = playerDeck[0];
                playerDeck.RemoveAt(0);
                CardFramework.Card card = _dealer.GetTopCardFromDeck();
                card.cardInfo = cardInfo;
                LoseCard(card);
            }

            if (die) {
            
            }
        }

        void ShuffleDeck() {
             System.Random r = new System.Random();
            for (int n = playerDeck.Count - 1; n > 0; --n) {
                int k = r.Next(n + 1);
                CardInfo temp = playerDeck[n];
                playerDeck[n] = playerDeck[k];
                playerDeck[k] = temp;
            }
            //yield return StartCoroutine(_dealer.ShuffleCoroutine());
            
        }

        public void PlayHeartCard(Card card) {
            PlayerStatus.instance.life += (int)card.cardInfo.rank + 1;
            PlayerAction.instance.Act(card);
            DiscardCard(card.GetComponent<CardFramework.Card>());
        }

        public bool HasCardInLostZone() {
            return lostPile.Count > 0;
        }

        public void PlayHeartCardInLost (Card card) {
            PlayerAction.instance.Act(card);
            DiscardCardFromHand(card.position);
            UpdateHand();
            for (int i = 0; i < (int)card.cardInfo.rank + 1; i++) {
                AddCardToDeck(lostPile[lostPile.Count - 1]);
                lostPile.RemoveAt(lostPile.Count - 1);
            }
            ShuffleDeck();
            StartCoroutine(_dealer.ShuffleCoroutine());
            
            Destroy(card.gameObject);
        }


        IEnumerator DrawCardCoroutine(CardInfo card, int position) {
            yield return StartCoroutine(_dealer.DrawCoroutine(card, position));
        }

        public void StartNewTurn() {
            StartCoroutine(DrawCards());
        }

        private void Update() {
            if (Input.GetKeyDown(KeyCode.S)) {
                StartCoroutine(_dealer.ShuffleCoroutine());
            }
        }

    }
}