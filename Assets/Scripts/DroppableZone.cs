using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Pheryus { 
    public class DroppableZone : MonoBehaviour {

        public enum DroppableArea { none, commonArea, lostZone};

        public DroppableArea droppapleArea;

        public LayerMask collisionMask;


        public void OnDrop() {
            GameObject go = DraggableCard.draggableGO;

            Card c = go.GetComponent<Card>();
            DraggableCard dc = go.GetComponent<DraggableCard>();

            if (c != null && (c.IsDiamonds || c.IsHearts) && dc.isDragging) {

                if (droppapleArea == DroppableArea.commonArea) {
                    if (c.IsDiamonds) {
                        PlayerDeck.instance.PlayDiamondCard(c);
                    }
                    else if (c.IsHearts) {
                        PlayerDeck.instance.PlayHeartCard(c);
                    }
                }
                else if (droppapleArea == DroppableArea.lostZone) {
                    if (c.IsHearts && PlayerDeck.instance.HasCardInLostZone()) {
                        PlayerDeck.instance.PlayHeartCardInLost(c);
                    }
                }
                c.GetComponent<CardFramework.Card>().returnToStartPosition = true;
                dc.enabled = false;

                PlayerAction.instance.DisableDroppableAreas();


            }
        }

        private void Update() {
            RaycastHit hit;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, collisionMask) && hit.transform == transform && Input.GetMouseButtonUp(0)) {
                if (DraggableCard.draggableGO != null) {
                    OnDrop();
                }
            }
        }




    }
}