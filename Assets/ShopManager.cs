using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Pheryus { 
    public class ShopManager : MonoBehaviour {
        public CardEvent shopEvent;

        public Transform battleboard;

        CardEvent activeEvent;

        public Transform[] costCardTransform;

        public Option[] availableOptions;

        public Dealer _dealer;

        public GameObject shopUI, shopArea;

        public TextMeshProUGUI[] optionInfo;

        public GameObject[] cost;

        public TextMeshProUGUI[] costText;

        public GameObject passButton;

        public void StartShop() {
            activeEvent = shopEvent;
            CreateShopOptions();
            _dealer.SetCardSlotActive(false);
            shopUI.SetActive(true);
            shopArea.SetActive(true);
            passButton.SetActive(false);
        }

        Option[] GetOptions() {
            return shopEvent.GetRandomOptions(3);
        }

        public void TryToBuy(int position) {
            Option selectedOption = availableOptions[position];
            if (PlayerStatus.instance.gold >= selectedOption.cost) {
                PlayerStatus.instance.gold -= selectedOption.cost;

                if (availableOptions[position].IsOfType(OptionType.card)) {
                    foreach (Card c in shopArea.transform.GetChild(position).GetComponentsInChildren<Card>()) {
                        PlayerDeck.instance.AddCardToDiscard(c.GetComponent<CardFramework.Card>());
                    }
                }
                else if (availableOptions[position].IsOfType(OptionType.diamond)){
                    PlayerStatus.instance.gold += availableOptions[position].qty;
                }
                else if (availableOptions[position].IsOfType(OptionType.heart)) {
                    PlayerStatus.instance.life += availableOptions[position].qty;
                }

                DeleteCards(position);
                CloseShop();

            }
        }

        private void AddBoughtCard() {

        }

        public void CloseShop() {
            _dealer.SetCardSlotActive(true);
            shopUI.SetActive(false);
            shopArea.SetActive(false);
            passButton.SetActive(true);
        }

        public void DeleteCards(int selectedPosition) {
            for (int i = 0; i < 3; i++) {
                optionInfo[i].text = string.Empty;
                if (i == selectedPosition) {
                    continue;
                }
                for (int j = shopArea.transform.GetChild(i).childCount - 1; j >= 0; j--) {
                    Destroy(shopArea.transform.GetChild(i).GetChild(j).gameObject);
                }
            }
        }

        private void CreateCards(int i) {
            int qty = 0;

            foreach (CardInfo cardInfo in availableOptions[i].wave.cards) {
                GameObject newCardToBuy = Instantiate(CardManager.instance.cardPrefab, shopArea.transform.GetChild(i));

                CardFramework.Card newCard = newCardToBuy.GetComponent<CardFramework.Card>();
                Vector3 newCardPos = newCard.transform.position - new Vector3(0, -0.002f, 0.08f) * qty;
                newCard.cardInfo = new CardInfo(cardInfo);
                newCard.position = i;
                newCard.FrontBecameVisible();
                newCard.canUse = false;
                newCard.cardInfo.card = newCard;
                newCard.SetNewTarget(newCardPos, newCardToBuy.transform.rotation);
                qty++;

            }
        }

        private void SetOptionCost(int i) {
            cost[i].SetActive(true);

            if (availableOptions[i].cost > 0) {
                costText[i].text = availableOptions[i].cost.ToString();
            }
            else {
                costText[i].text = "FREE";
            }
        }

        private void CreateResourceOption (int i, OptionType type) {
            int qty = availableOptions[i].qty;
            string text = "Gain " + qty.ToString();

            if (type == OptionType.diamond) {
                text += " diamonds";
            }
            else if (type == OptionType.heart) {
                text += " extra lifes";
            }
            optionInfo[i].text = text;
        }

        public void CreateShopOptions() {
        
            availableOptions = new Option[3];
            availableOptions = GetOptions();
            for (int i = 0; i < 3; i++) {

                SetOptionCost(i);

                if (availableOptions[i].IsOfType(OptionType.card)) {
                    CreateCards(i);
                }
                else if (availableOptions[i].IsOfType(OptionType.diamond) || availableOptions[i].IsOfType(OptionType.heart)) {
                    CreateResourceOption(i, availableOptions[i].option);
                }

            }
        
        }
    }
}