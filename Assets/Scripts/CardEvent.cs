using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pheryus {
    public enum OptionType { card, diamond, heart, loseCard, substituteCard };

    [CreateAssetMenu(fileName = "CardEvent", menuName = "New Card Event")]
    public class CardEvent : ScriptableObject {
        public Option[] options;

        public Option[] GetRandomOptions(int choices) {
            Option[] selectedCards = new Option[3];
            for (int i = 0; i < choices; i++) {
                selectedCards[i] = GetRandomOption(selectedCards);
            }
            return selectedCards;

        }

        public Option GetRandomOption (Option[] excludedList) {

            if (PlayerStatus.instance && PlayerStatus.instance.gold == 0) {
                return GetPurchasableOption(0, true, excludedList);
            }

            float freeOptionPct = 0.3f;
            if (Random.Range(0f, 1f) < freeOptionPct) {
                return GetPurchasableOption(0, true, excludedList);
            }
            return GetPurchasableOption(PlayerStatus.instance.gold, false, excludedList);
        }

        public Option GetPurchasableOption(int cost, bool freeAvailable, Option[] excludedList) {
            Option selectedOption = null;
            int tries = 0;
            while (true) {
                selectedOption = options[Random.Range(0, options.Length)];
                if (selectedOption.cost <= cost) {

                    if ((selectedOption.cost == 0 && freeAvailable) || selectedOption.cost != 0) {
                        if (!IsInListOfOption(selectedOption, excludedList)) { 
                            return selectedOption;
                        }
                    }
                }
                tries++;
                if (tries > 100000) {
                    Debug.LogWarning("Not found available option");
                    return null;
                }
            }
        }

        public bool IsInListOfOption(Option opt, Option[] listOfOptions) {
            foreach (Option option in listOfOptions) {
                if (opt == option) {
                    return true;
                }
            }
            return false;
        }
    }

    [System.Serializable]
    public class Option {
        public Wave wave;
        public OptionType option;
        public int cost = 1;
        public int qty;

        public bool IsOfType (OptionType type) {
            return option == type;
        }
    }

    [System.Serializable]
    public class Wave {
        public List<CardInfo> cards;
    }
}
