using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DeckInfo {
    public Rank rank;
    public Suit suit;
    public int qtd;
}

public class PlayerDeck : MonoBehaviour {


    public List<DeckInfo> starterDeck;

    public List<CardInfo> playerDeck;

    public CardInfo[] hand;


    List<Card> discardPile;

    List<Card> lostPile;

    public Transform deckTransform;

    public Transform discardTransform;

    public Vector2 cardDistance;

    public Transform handTransform, lostTransform;

    public PlayerStatus playerStatus;

    public static PlayerDeck instance;

    private void Start() {
        instance = this;
        playerDeck = new List<CardInfo>();
        hand = new CardInfo[5];
        discardPile = new List<Card>();
        lostPile = new List<Card>();
        CreateCards();
        InstantiateDeck();
        DrawCards();
    }

    void DrawCards() {
        for (int i = 0; i < 5; i++) {
            if (hand[i] == null || hand[i].card == null) {
                CardInfo drawCard = DrawCard();
                if (drawCard == null) {
                    if (discardPile.Count > 0) {
                        ShuffleDiscardToDeck();
                        drawCard = DrawCard();
                    }
                    else {
                        return;
                    }
                }
                hand[i] = drawCard;
                hand[i].card = drawCard.card;
                StartCoroutine(DrawCardCoroutine(drawCard.card, i));
            }
        }
    }

    void ShuffleDiscardToDeck() {
        for (int i = discardPile.Count - 1; i >= 0; i--) {
            Card card = discardPile[i];
            AddCardToDeck(card);
            discardPile.RemoveAt(i);
        }

        ShuffleDeck();
    }


    CardInfo DrawCard() {
        if (playerDeck.Count > 0) {
            CardInfo card = playerDeck[0];
            playerDeck.RemoveAt(0);
            return card;
        }
        return null;
    }

    void CreateCards() {
        foreach (DeckInfo di in starterDeck) {

            for (int i = 0; i < di.qtd; i++) {
                CardInfo card = new CardInfo(di.rank, di.suit);
                playerDeck.Add(card);
            }
        }

        ShuffleDeck();
    }

    void InstantiateDeck() {
        int card = 0;
        foreach (CardInfo c in playerDeck) {
            GameObject go = Instantiate(CardManager.instance.cardPrefab, deckTransform);
            go.transform.localPosition = cardDistance * card;
            go.GetComponent<Card>().cardInfo = c;
            c.card = go.GetComponent<Card>();
            go.GetComponent<Card>().UpdateSprite();
            card++;
        }
    }

    public void AddCardToDiscard (Card card) {
        card.transform.SetParent(discardTransform);
        card.transform.localPosition = Vector3.zero;
        discardPile.Add(card);
    }

    public void AddCardToDeck (Card card) {
        card.flipUp = false;
        card.position = -1;
        card.transform.SetParent(deckTransform);
        card.transform.localPosition = Vector3.zero;
        CardInfo newCardInfo = new CardInfo(card.cardInfo.rank, card.cardInfo.suit);
        newCardInfo.card = card;
        card.GetComponent<DraggableCard>().enabled = true;
        card.GetComponent<CanvasGroup>().blocksRaycasts = true;
        playerDeck.Add(newCardInfo);

    }


    public void DiscardCard (Card card) {
        AddCardToDiscard(card);
        hand[card.position] = new CardInfo(Rank.ace, Suit.clubs);
        UpdateHand();
    }

    public void LoseCard(Card card) {
        card.transform.SetParent(lostTransform);
        card.flipUp = true;
        card.transform.localPosition = Vector3.zero;
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
                }
            }
            if (hand[i].card != null) {
                hand[i].card.transform.SetParent(handTransform.GetChild(i));
                hand[i].card.transform.localPosition = Vector3.zero;
            }
        }
    }

    public void PlayDiamondCard (Card card) {
        PlayerStatus.instance.gold += (int)card.cardInfo.rank + 1;
        PlayerAction.instance.Act(card);
        DiscardCard(card);
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
            LoseCard(cardInfo.card);
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
    }

    public void PlayHeartCard(Card card) {
        PlayerStatus.instance.life += (int)card.cardInfo.rank + 1;
        PlayerAction.instance.Act(card);
        DiscardCard(card);
    }

    public bool HasCardInLostZone() {
        return lostPile.Count > 0;
    }

    public void PlayHeartCardInLost (Card card) {
        PlayerAction.instance.Act(card);
        for (int i = 0; i < (int)card.cardInfo.rank + 1; i++) {
            AddCardToDeck(lostPile[lostPile.Count - 1]);
            lostPile.RemoveAt(lostPile.Count - 1);
        }
        ShuffleDeck();
        Destroy(card.gameObject);
    }

    IEnumerator DrawCardCoroutine(Card card, int position) {


        bool rotate = false;
        /*
        while (true) {
            card.transform.localScale = 
        }
        */

        yield return null;

        card.flipUp = true;

        card.transform.position = handTransform.GetChild(position).position;
        card.transform.SetParent(handTransform.GetChild(position));
        card.canUse = true;
        card.position = position;
    }

    public void StartNewTurn() {

        Invoke("DrawCards", 0.5f);
    }

}
