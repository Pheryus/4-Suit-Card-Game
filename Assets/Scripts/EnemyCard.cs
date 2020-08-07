using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EnemyCard : Card, IDropHandler {

    public int attachedCards;

    public int strength;

    public void OnDrop(PointerEventData eventData) {
        GameObject go = eventData.pointerDrag.gameObject;

        Card c = go.GetComponent<Card>();
        DraggableCard dc = go.GetComponent<DraggableCard>();

        if (c != null && (c.IsSpades || c.IsClub) && dc.isDragging){
            PutPlayerCardBelowEnemy(go, c);
            dc.correctPosition = true;
            dc.enabled = false;
            
        }
        else if (c != null && c.IsDiamonds && dc.isDragging) {
            if ((int)c.cardInfo.rank + 1 >= cardInfo.power) {
                dc.correctPosition = true;
                dc.enabled = false;
                dc.isLost = true;
                GiveMoneyToCard(go, c);
            }
            
        }
    }

    void GiveMoneyToCard (GameObject go, Card card) {
        if (PlayerAction.instance) {
            PlayerAction.instance.Act(card);
        }
        PlayerDeck.instance.LoseCardFromHand(card);
        DungeonManager.instance.DestroyCard(this);
        PlayerAction.instance.DisableDroppableAreas();
    }

    void PutPlayerCardBelowEnemy(GameObject go, Card card) {
        if (DungeonManager.instance) {

            if (PlayerAction.instance) {
                PlayerAction.instance.Act(card);
            }

            go.transform.SetParent(DungeonManager.instance.battleboard.GetChild(position).GetChild(0));
            go.transform.localPosition = Vector3.zero + new Vector3(0, 1f, 0) * attachedCards;
            attachedCards++;
            strength += (int)card.cardInfo.rank + 1;

            if (strength >= (int)cardInfo.rank + 1) {
                DungeonManager.instance.DestroyCard(this);
            }
        }
    }
}
