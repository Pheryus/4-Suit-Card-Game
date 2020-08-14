using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Pheryus { 
    public class DraggableCard : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {

        public bool correctPosition;

        public bool isLost;

        public CardFramework.Card card;

        public bool mouseOver, isDragging;

        public Vector3 offset;

        public static GameObject draggableGO = null;

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

        bool excludingProcess = false;

        void Update() {

            if (excludingProcess) {
                isDragging = false;
                draggableGO = null;
                excludingProcess = false;
            }

            RaycastHit hit;

            Vector3 newOffset = Vector3.zero;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit)) {

                if (!isDragging && hit.transform == transform && card.OnTargetPosition()) {
                    newOffset = offset;
                    card.returnToStartPosition = false;
                }

                if (Input.GetMouseButtonDown(0) && hit.transform == transform && PlayerAction.instance.CanAct(card)) {
                    isDragging = true;
                    draggableGO = gameObject;
                }
            }

            if (Input.GetMouseButtonUp(0)) {
                card.returnToStartPosition = true;
            }

            if (isDragging && Input.GetMouseButtonUp(0)) {
                excludingProcess = true;
            }

            if (isDragging && hit.transform == transform) {
                Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                transform.position = new Vector3(hit.point.x, hit.point.y + newOffset.y, hit.point.z);
               
            }
        }
    }


}