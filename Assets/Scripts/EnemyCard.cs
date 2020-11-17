using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


namespace Pheryus { 
    public class EnemyCard : MonoBehaviour {

        public int attachedCards;

        public int strength;

        public int remainingShields;

        public CardFramework.Card enemyCard;

        public LayerMask collisionMask;

        public Vector3 offset;

        public List<CardFramework.Card> playerCards;


        public int damageTook;

        private void Start() {
            playerCards = new List<CardFramework.Card>();
        }


        public void OnDrop() {
            GameObject go = DraggableCard.draggableGO;

            Card c = go.GetComponent<Card>();
            DraggableCard dc = go.GetComponent<DraggableCard>();

            if (c != null && (c.IsSpades || c.IsClub) && dc.isDragging){

                if (Tutorial.instance.CanPlayCard(this)) {
                    PutPlayerCardBelowEnemy(go, c);
                    dc.correctPosition = true;
                    dc.enabled = false;
                    dc.ResetBoxCollider();
                    Tutorial.instance.DropCard(this, canKillEnemy: WillDie());
                    DraggableCard.draggableGO = null;
                }
            }
            else if (c != null && c.IsDiamonds && dc.isDragging) {
                if ((int)c.cardInfo.rank + 1 >= enemyCard.cardInfo.power) {
                    dc.correctPosition = true;
                    dc.enabled = false;
                    dc.isLost = true;
                    dc.ResetBoxCollider();
                    GiveMoneyToCard(go, c);
                    DraggableCard.draggableGO = null;
                }

            }
        }

        public void ResetPlayerCards() {
            playerCards = new List<CardFramework.Card>();
        }

        public bool IsDead() {
            return enemyCard.cardInfo.rank < 0;
        }

        public void ReduceHp(int strength) {
            enemyCard.cardInfo.rank -= strength;
            if (!IsDead()) {
                enemyCard.FrontBecameVisible();
            }
        }

        public bool WillDie() {
            return strength >= (int)enemyCard.cardInfo.rank + 1;
        }

        void GiveMoneyToCard (GameObject go, Card card) {
            if (PlayerAction.instance) {
                PlayerAction.instance.Act(card);
            }
            PlayerDeck.instance.LoseCardFromHand(card);
            DungeonManager.instance.DestroyCard(this.enemyCard);
            PlayerAction.instance.DisableDroppableAreas();
        }

        void PutPlayerCardBelowEnemy(GameObject go, Card card) {
            if (DungeonManager.instance) {

                if (PlayerAction.instance) {
                    PlayerAction.instance.Act(card);
                }

                PlayerDeck.instance.DiscardCardFromHand(card.position);

                Transform fieldCardParent = DungeonManager.instance.battleboard.GetChild(enemyCard.position).GetChild(0);

                Vector3 playerCardPos = fieldCardParent.position + offset * attachedCards;
                Quaternion playerCardRot = DungeonManager.instance.battleboard.GetChild(enemyCard.position).GetChild(3).rotation;

                go.transform.SetParent(fieldCardParent);

                ((CardFramework.Card)card).SetNewTarget(playerCardPos, playerCardRot);
                
                attachedCards++;
                strength += (int)card.cardInfo.rank + 1;
                playerCards.Add((CardFramework.Card)card);


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