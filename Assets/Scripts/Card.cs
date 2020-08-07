using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum Suit { hearts, diamonds, clubs, spades, none };

public enum Rank { ace, two, three, four, five, six, seven, eight, nine, ten, jack, queen, king, joker };

public class Card : MonoBehaviour {

    public CardInfo cardInfo;

    public DraggableCard draggableCard;

    bool _flipUp;

    public int position;

    public bool flipUp {
        get {
            return _flipUp;
        }

        set {
            _flipUp = value;
            UpdateSprite();
        }
    }

    bool _canUse;

    public bool canUse {
        get {
            return _canUse;
        }

        set {
            _canUse = value;
            if (draggableCard != null) {
                draggableCard.enabled = _canUse;
            }
        }
    }

    public Image image;

    private void Start() {
        if (image == null) {
            image = GetComponent<Image>();
        }
    }


    public void UpdateSprite() {

        if (CardManager.instance == null) {
            Debug.LogError("Card Manager not found");
            return;
        }

        if (!flipUp) {
            image.sprite = CardManager.instance.flipDown;
            return;
        }

        if (cardInfo.suit == Suit.hearts) {
            image.sprite = CardManager.instance.heartCards[(int)cardInfo.rank];
        }
        else if (cardInfo.suit == Suit.spades) {
            image.sprite = CardManager.instance.spadeCards[(int)cardInfo.rank];
        }
        else if (cardInfo.suit == Suit.clubs) {
            image.sprite = CardManager.instance.clubCards[(int)cardInfo.rank];
        }
        else if (cardInfo.suit == Suit.diamonds) {
            image.sprite = CardManager.instance.diamondCards[(int)cardInfo.rank];
        }
    }

    public bool IsClub {
        get {
            return cardInfo.suit == Suit.clubs;
        }
    }

    public bool IsHearts {
        get {
            return cardInfo.suit == Suit.hearts;
        }
    }

    public bool IsDiamonds {
        get {
            return cardInfo.suit == Suit.diamonds;
        }
    }

    public bool IsSpades {
        get {
            return cardInfo.suit == Suit.spades;
        }
    }


}
