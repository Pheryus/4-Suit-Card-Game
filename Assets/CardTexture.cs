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
        if (cardInfo.suit == Suit.clubs) {
            return clubCards[(int)cardInfo.rank];
        }
        else if (cardInfo.suit == Suit.diamonds) {
            return diamondCards[(int)cardInfo.rank];
        }
        else if (cardInfo.suit == Suit.hearts) {
            return heartCards[(int)cardInfo.rank];
        }
        else {
            return spadeCards[(int)cardInfo.rank];
        }
    }
}
