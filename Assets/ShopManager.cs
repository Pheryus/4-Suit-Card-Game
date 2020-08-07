using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour {
    public CardEvent shopEvent;

    public Transform battleboard;

    CardEvent activeEvent;

    public Transform[] costCardTransform;

    public Option[] availableOptions;

    public void StartShop() {
        activeEvent = shopEvent;
        CreateShopCards();
    }

    Option GetOption(int lvl) {
        return shopEvent.GetRandomOption();
    }

    public void TryToBuy(int position) {
        Option selectedOption = availableOptions[position];
        if (PlayerStatus.instance.gold >= selectedOption.cost) {
            PlayerStatus.instance.gold -= selectedOption.cost;
            foreach (Card c in battleboard.GetChild(position).GetComponentsInChildren<Card>()) {
                if (c.gameObject.transform.parent.name == "Cost") {
                    Destroy(c.gameObject, 0.3f);
                }
                else {
                    Destroy(c.GetComponent<Button>());
                    Destroy(c.GetComponent<ForSale>());
                    PlayerDeck.instance.AddCardToDiscard(c);
                }
            }

        }
    }

    public void CloseShop() {
        foreach (Card c in battleboard.GetComponentsInChildren<Card>()) {
            Destroy(c.gameObject);
        }
    }


    public void CreateShopCards() {
        
        availableOptions = new Option[3];

        for (int i = 0; i < 3; i++) {

            availableOptions[i] = GetOption(i);

            GameObject costGO = Instantiate(CardManager.instance.cardPrefab, costCardTransform[i]);
            costGO.transform.localPosition = Vector3.zero;
            costGO.transform.localRotation = Quaternion.identity;
            Card costGOCard = costGO.GetComponent<Card>();
            costGOCard.cardInfo = new CardInfo((Rank)availableOptions[i].cost - 1, Suit.diamonds);
            costGOCard.flipUp = true;
            costGOCard.UpdateSprite();
            ForSale forSaleCard = costGO.AddComponent<ForSale>();
            forSaleCard.SetSale(this, i);

            int qty = 0;

            foreach (CardInfo cardInfo in availableOptions[i].cards) {
                GameObject newCardToBuy = Instantiate(CardManager.instance.cardPrefab, battleboard.GetChild(i));
                newCardToBuy.transform.localPosition = new Vector3(0, qty * 0.5f, 0);
                Card newCard = newCardToBuy.GetComponent<Card>();
                newCard.cardInfo = new CardInfo(cardInfo);
                newCard.position = i;
                newCard.flipUp = true;
                newCardToBuy.AddComponent<ForSale>().SetSale(this, i);
                qty++;
                
            }

        }
        
    }
}
