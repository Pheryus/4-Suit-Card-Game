using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAction : MonoBehaviour {

    public bool canAct = true;

    public static PlayerAction instance;

    public DungeonManager dungeonManager;

    Suit suitPlayed = Suit.none;

    public PlayerDeck playerDeck;

    public GameObject commonDroppapleArea, lostArea;

    private void Awake() {
        instance = this;
    }

    public void Act (Card cardPlayed) {
        suitPlayed = cardPlayed.cardInfo.suit;
    }

    public void ShowCommonDroppableArea(bool active) {
        commonDroppapleArea.SetActive(active);
    }

    public void ShowLostDroppableArea(bool active) {
        lostArea.SetActive(active);
    }

    public void DisableDroppableAreas() {
        ShowLostDroppableArea(false);
        ShowCommonDroppableArea(false);
    }

    public bool CanAct (Card card) {
        return card.cardInfo.suit == suitPlayed || suitPlayed == Suit.none;
    }

    public void EndTurn() {
        suitPlayed = Suit.none;
        dungeonManager.EndRound();
        playerDeck.StartNewTurn();
    }
}
