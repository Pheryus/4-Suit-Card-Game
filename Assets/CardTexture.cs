using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardTexture : MonoBehaviour {

    public Texture[] diamondCards, heartCards, clubCards, spadeCards;

    public static CardTexture instance;

    private void Awake() {
        instance = this;
    }

    public Texture GetTextureFromCard (Pheryus.CardInfo cardInfo) {

        if (cardInfo == null) {
            Debug.LogError("cardInfo null");
            return null;
        }
        int cardRank = (int)cardInfo.rank;
        if (cardRank < 0) {
            cardRank = 0;
        }

        if (cardInfo.suit == Suit.clubs) {
            return clubCards[cardRank];
        }
        else if (cardInfo.suit == Suit.diamonds) {
            return diamondCards[cardRank];
        }
        else if (cardInfo.suit == Suit.hearts) {
            return heartCards[cardRank];
        }
        else {
            return spadeCards[cardRank];
        }
    }
}
