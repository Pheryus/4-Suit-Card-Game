using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum Suit { hearts, diamonds, clubs, spades, none };

public enum Rank { ace, two, three, four, five, six, seven, eight, nine, ten, jack, queen, king, joker };

namespace Pheryus { 

    public class Card : MonoBehaviour {

        public CardInfo cardInfo;

        public DraggableCard draggableCard;

        public int position;

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
}