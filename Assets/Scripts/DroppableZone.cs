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
                        dc.enabled = false;
                    }
                    else if (c.IsHearts) {
                        if (Tutorial.instance.active && Tutorial.instance.CanPlayHeartCard()) { 
                            PlayerDeck.instance.PlayHeartCard(c);
                        }
                        else {
                            ResetDroppedCard(c, dc);
                            Tutorial.instance.WarningMessage();
                            return;
                        }
                        if (Tutorial.instance.active) {
                            Tutorial.instance.PlayHeartCardFromHand();
                        }
                        dc.enabled = false;
                    }
                }
                else if (droppapleArea == DroppableArea.lostZone) {
                    if (c.IsHearts && PlayerDeck.instance.HasCardInLostZone()) {
                        if (Tutorial.instance.active) { 
                            if (Tutorial.instance.CanPlayHeartCardInLost()) {
                                PlayerDeck.instance.PlayHeartCardInLost(c);
                                Tutorial.instance.PlayHeartCardInLost();
                            }
                            else {
                            }
                        }
                        else {
                            PlayerDeck.instance.PlayHeartCardInLost(c);
                        }
                        dc.enabled = false;
                    }
                }

                ResetDroppedCard(c, dc);
            }
        }

        void ResetDroppedCard(Card c, DraggableCard dc) {
            c.GetComponent<CardFramework.Card>().returnToStartPosition = true;
            DraggableCard.draggableGO = null;

            PlayerAction.instance.DisableDroppableAreas();

            dc.ResetBoxCollider();
        }

        private void Update() {

            RaycastHit hit;
            PlayerInput input = PlayerInput.instance;
            Ray ray = input.inputRay;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, collisionMask) && hit.transform == transform && input.releaseFinger) {
                if (DraggableCard.draggableGO != null) {
                    OnDrop();
                }
            }
        }




    }
}