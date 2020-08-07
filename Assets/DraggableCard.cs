using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableCard : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {

    public bool correctPosition;

    public bool isDragging;

    public bool isLost;

    public Card card;

    void Start() {
        ;
    }

    Vector3 startDragPosition;

    public void OnBeginDrag(PointerEventData eventData) {

        if (PlayerAction.instance && PlayerAction.instance.CanAct(card)) {
            startDragPosition = transform.position;
            GetComponent<CanvasGroup>().blocksRaycasts = false;
            isDragging = true;
            if (card.IsDiamonds) {
                PlayerAction.instance.ShowCommonDroppableArea(true);
            }
            else if (card.IsHearts) {
                PlayerAction.instance.ShowCommonDroppableArea(true);
                PlayerAction.instance.ShowLostDroppableArea(true);
            }
        }
    }

    void IDragHandler.OnDrag(PointerEventData eventData) {
        if (PlayerAction.instance && PlayerAction.instance.CanAct(card)) {
            Vector3 v = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.position = new Vector3(v.x, v.y, transform.position.z);
        }
    }

    void IEndDragHandler.OnEndDrag(PointerEventData eventData) {
        if (PlayerAction.instance && PlayerAction.instance.CanAct(card)) {
            Debug.Log("EndDrag");
            GetComponent<CanvasGroup>().blocksRaycasts = true;

            if (!correctPosition) {
                transform.position = startDragPosition;
            }
            isDragging = false;
            PlayerAction.instance.DisableDroppableAreas();
        }
    }
}
