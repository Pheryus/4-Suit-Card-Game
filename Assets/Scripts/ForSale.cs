using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ForSale : MonoBehaviour {

    public int position;

    ShopManager shopManager;

    Button button;

    public void SetSale(ShopManager shop, int id) {
        position = id;
        shopManager = shop;
        button = gameObject.AddComponent<Button>();
        button.onClick.AddListener(() => ClickToBuy());

    }

    public void ClickToBuy() {
        shopManager.TryToBuy(position);
    }

}
