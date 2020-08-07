using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CardInfo {

    public Rank rank;
    public Suit suit;

    public Card card;

    public bool singleUse;

    public int power = 1;

    public CardInfo (Rank _rank, Suit _suit) {
        this.rank = _rank;
        this.suit = _suit;
    }
    public CardInfo (CardInfo cardInfo) {
        rank = cardInfo.rank;
        suit = cardInfo.suit;
        if (cardInfo.card != null) {
            card = cardInfo.card;
        }
    }


}

[System.Serializable]
public class EnemyCardInfo : CardInfo {

    public EnemyCardInfo(Rank _rank, Suit _suit) : base(_rank, _suit) {

    }

}