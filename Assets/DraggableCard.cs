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

        BoxCollider bc;

        Vector3 startBcSize;

        void Start() {
            bc = GetComponent<BoxCollider>();
            startBcSize = bc.size;
        }

        Vector3 startDragPosition;

        float startYPos;

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

        bool willReturn = false;

        void Update() {

            if (Tutorial.instance && Tutorial.instance.OnMessage()) {
                return;
            }

            if (willReturn) {
                card.returnToStartPosition = true;
                card.ReturnToStartRotation();
                ResetBoxCollider();
                Tutorial.instance.ReleaseCard();
                isDragging = false;
                draggableGO = null;
                willReturn = false;
            }

            if (excludingProcess) {
                isDragging = false;
                draggableGO = null;
                excludingProcess = false;
            }

            RaycastHit hit;

            PlayerInput input = PlayerInput.instance;

            Ray ray = input.inputRay;

            if (Physics.Raycast(ray, out hit)) {

                if (input.tapScreen && hit.transform == transform && PlayerAction.instance.CanAct(card)) {
                    isDragging = true;
                    draggableGO = gameObject;
                    startYPos = transform.position.y;
                    bc.size = startBcSize * 3;
                    card.SetRotation(Quaternion.Euler(64, 8.616f, 3.693f));
                }
            }

            if (input.releaseFinger && draggableGO == gameObject) {
                willReturn = true;
            }

            if (isDragging && input.releaseFinger) {
                excludingProcess = true;
            }

            if (isDragging && hit.transform == transform) {
                transform.position = new Vector3(hit.point.x, startYPos + offset.y, hit.point.z);
                Tutorial.instance.DragCard();
            }
            else if (isDragging){
                isDragging = false;
                ResetBoxCollider();
            }
        }

        private void OnEnable() {
            ResetBoxCollider();
        }

        public void ResetBoxCollider() {
            if (bc != null) { 
                bc.size = startBcSize;
            }
        }
    }


}