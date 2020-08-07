using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DroppableZone : MonoBehaviour, IDropHandler {

    public enum DroppableArea { none, commonArea, lostZone};

    public DroppableArea droppapleArea;

    public void OnDrop(PointerEventData eventData) {
        GameObject go = eventData.pointerDrag.gameObject;

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

            dc.correctPosition = true;
            dc.enabled = false;

            PlayerAction.instance.DisableDroppableAreas();


        }
    }


}
