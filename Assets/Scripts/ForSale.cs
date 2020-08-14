using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Pheryus { 
    public class ForSale : MonoBehaviour {

        public int position;

        ShopManager shopManager;

        public void SetSale(ShopManager shop, int id) {
            position = id;
            shopManager = shop;
        }

        private void Update() {
            if (Input.GetMouseButtonDown(0)) {

                RaycastHit hit;

                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out hit) && hit.collider != null && hit.collider.transform == transform) {
                    ClickToBuy();
                }
            }
        }

        public void ClickToBuy() {
            Debug.Log("Click to Buy");
            shopManager.TryToBuy(position);
        }

    }
}