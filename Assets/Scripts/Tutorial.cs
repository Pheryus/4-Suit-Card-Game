using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Pheryus {

    /// <summary>
    /// TUTORIAL STEPS
    /// 
    /// First Round: Kill a ace card
    /// Second Round: Use multiple cards
    /// Third Round: Use multiple cards on multiple enemy cards
    /// Fourth Round: Use all your cards and dont kill enemy.
    /// </summary>


    public class Tutorial : MonoBehaviour {

        public bool active;

        public List<CardInfo> tutorialDeck;

        public PlayerDeck playerDeck;
        public DungeonManager dungeonManager;
        public PlayerAction playerAction;

        public int tutorialRound = 5;

        public enum TutorialSteps { start, holdCard, dropCard, playAllCards, showingTapMessages, takeDmgMessage};

        public TutorialSteps tutorialStep;

        public GameObject tutorialWindow;
        public TextMeshProUGUI tutorialText;

        public List<Wave> tutorialWaves;

        public static Tutorial instance;

        public Animator messageAnimator;
        public TextMeshProUGUI messageText;

        public int tapMessageIndex;

        public string[] tapMessages = { "Agora são duas ameaças.", "Tente neutralizá-las." };

        public string fourthRoundTapMessage = "Now is a tough battle!";

        public string fifthRoundTapMessage = "At least the card is weaker now. Aproveite!";

        public string[] takeDmgMessages = { "Ouch! This hurts.", "Every time you don't defeat a card, it will attack you.",
            "It will reduce your extra life. When it reach 0, you will start losing cards from your deck." };

        public string[] sixthRoundTapMessage = { "Oh, não tem nenhuma ameaça iminente.", "Aproveite para aumentar sua vida extra." };

        public string[] seventhRoundTapMessage = {"Vários inimigos dessa vez.", "Melhor se concentrar em acabar com um de uma vez.",
            "Perceba que você tem cartas de naipes diferentes em sua mão." };

        public string[] lostCardMessage = { "Você perdeu uma carta forte de seu baralho.", "Mas não se preocupe: ainda é possível recuperá-la!" };

        public string[] teachingRecoverLostCard = { "Você pode se desfazer de uma carta de coração para retornar uma carta perdida para seu descarte." };

        public TextMeshProUGUI tapToPassText;

        public List<EnemyCard> cardsThatPlayedOver;

        public Message message;


        private void Awake() {
            instance = this;
        }

        private IEnumerator Start() {
            if (!active) {
                yield break;
            }
            yield return new WaitForSeconds(0.1f);
            StartNewTurn();
        }

        void EndTapMessage() {
            tapMessageIndex = 0;
            tutorialStep = TutorialSteps.start;
            tapToPassText.gameObject.SetActive(false);
            UpdateTutorialText();
        }

        void UpdateTapMessage() {
            if (tutorialRound == 2) {
                if (tapMessageIndex >= tapMessages.Length) {
                    EndTapMessage();
                    return;
                }
                tutorialText.text = tapMessages[tapMessageIndex];
            }

            else if (tutorialRound == 3) {
                if (tutorialStep == TutorialSteps.takeDmgMessage) {
                    if (tapMessageIndex >= takeDmgMessages.Length) {
                        tutorialStep = TutorialSteps.start;
                        return;
                    }
                    tutorialText.text = takeDmgMessages[tapMessageIndex];
                }
                else {
                    if (tapMessageIndex > 0) {
                        EndTapMessage();
                        return;
                    }
                    tutorialText.text = fourthRoundTapMessage;
                }

            }
            else if (tutorialRound == 4) {
                if (tapMessageIndex > 0) {
                    EndTapMessage();
                    return;
                }
                tutorialText.text = fifthRoundTapMessage;
            }
            else if (tutorialRound == 5) {
                if (tapMessageIndex >= sixthRoundTapMessage.Length) {
                    EndTapMessage();
                    return;
                }
                else {
                    tutorialText.text = sixthRoundTapMessage[tapMessageIndex];
                }
            }
            else if (tutorialRound == 6) {
                if (tapMessageIndex >= seventhRoundTapMessage.Length) {
                    EndTapMessage();
                    return;
                }
                else {
                    tutorialText.text = seventhRoundTapMessage[tapMessageIndex];
                }
            }
            else if (tutorialRound == 8) {
                if (tapMessageIndex >= lostCardMessage.Length) {
                    EndTapMessage();
                    return;
                }
                else {
                    tutorialText.text = lostCardMessage[tapMessageIndex];
                }
            }
            else if (tutorialRound == 9) {
                if (tapMessageIndex >= teachingRecoverLostCard.Length) {
                    EndTapMessage();
                    return;
                }
                else {
                    tutorialText.text = teachingRecoverLostCard[tapMessageIndex];
                }
            }

            tapToPassText.gameObject.SetActive(true);
        }

        void UpdateTutorialText() {

            if (tutorialStep == TutorialSteps.showingTapMessages || tutorialStep == TutorialSteps.takeDmgMessage) {
                UpdateTapMessage();
                return;
            }

            switch (tutorialRound) {
                case 0:
                    if (tutorialStep == TutorialSteps.start) {
                        tutorialText.text = "Hold your card.";
                    }
                    else if (tutorialStep == TutorialSteps.holdCard) {
                        tutorialText.text = "Now play it on the field!";
                    }
                    else if (tutorialStep == TutorialSteps.dropCard) {
                        tutorialText.text = "Now end your turn!";
                    }
                    break;

                case 1:
                    if (tutorialStep == TutorialSteps.start) {
                        tutorialText.text = "Use all your cards to beat the enemy card!";
                    }
                    else if (tutorialStep == TutorialSteps.playAllCards) {
                        tutorialText.text = "Now end your turn!";
                    }
                    break;

                case 2:
                    if (tutorialStep == TutorialSteps.start) {
                        tutorialText.text = "Use all your cards";
                    }
                    break;
                case 3:
                    if (tutorialStep == TutorialSteps.start) {
                        tutorialText.text = "Use all you got to damage the enemy card.";
                    }
                    break;
                case 4:
                    if (tutorialStep == TutorialSteps.start) {
                        tutorialText.text = "Aproveite!";
                    }
                    break;
                case 5:
                    if (tutorialStep == TutorialSteps.start) {
                        tutorialText.text = "Jogue suas cartas de coração na mesa.";
                    }
                    break;
                case 6:
                    if (tutorialStep == TutorialSteps.start) {
                        tutorialText.text = "Você só pode usar cartas do mesmo naipe durante uma rodada.";
                    }
                    break;
                case 7:
                    if (tutorialStep == TutorialSteps.start) {
                        tutorialText.text = "Tente se defender.";
                    }
                    break;
                case 9:
                    if (tutorialStep == TutorialSteps.start) {
                        tutorialText.text = "Jogue ela aqui!";
                    }
                    else if (tutorialStep == TutorialSteps.dropCard) {
                        tutorialText.text = "Agora, você tem que se defender!";
                    }
                    break;
                default:
                    tutorialText.text = string.Empty;
                    break;
            }
        }

        public void DragCard() {
            if (active && tutorialRound == 0) {
                tutorialStep = TutorialSteps.holdCard;
                UpdateTutorialText();
            }
        }

        public bool CanPlayHeartCard() {
            if (tutorialRound == 6) {
                return false;
            }
            else if (tutorialRound == 9 && tutorialStep == TutorialSteps.start) {
                message.ShowWarningMessage("Coloque aqui!");
                return false;
            }
            return true;
        }

        public bool CanPlayHeartCardInLost() {
            if (tutorialRound == 9 && tutorialStep == TutorialSteps.start) {
                return true;
            }
            return false;
        }

        public void PlayHeartCardInLost() {
            if (tutorialRound == 9 && tutorialStep == TutorialSteps.start) {
                tutorialStep = TutorialSteps.dropCard;
                UpdateTutorialText();
            }
        }

        public void WarningMessage() {
            if (tutorialRound == 6) {
                message.ShowWarningMessage("Use suas cartas de espada!");
            }
        }

        void ShowUpMessage(string msg) {
            messageAnimator.enabled = true;
            messageAnimator.gameObject.SetActive(true);
            messageText.text = msg;
            StartCoroutine(FadeOutMessage());
        }

        IEnumerator FadeOutMessage() {
            yield return new WaitForSeconds(2);
            messageAnimator.enabled = false;
            messageAnimator.Play("ShopUp");
            messageAnimator.gameObject.SetActive(false);
        }

        public bool CanPlayCard(EnemyCard target) {
            bool playCardHere = true;
            if (tutorialRound == 2) {
                foreach (EnemyCard en in cardsThatPlayedOver) {
                    if (en == target && en.WillDie()) {
                        message.ShowWarningMessage("Wrong! Try again!");
                        return false;
                    }
                }
            }
            else if (tutorialRound == 6 || tutorialRound == 8) {
                if (cardsThatPlayedOver.Count == 0) {
                    return true;
                }
                foreach (EnemyCard en in cardsThatPlayedOver) {
                    if (en == target) {
                        return true;
                    }
                }
                message.ShowWarningMessage("Tente focar em uma única carta inimiga.");
                return false;
            }
            else if (tutorialRound == 7) {
                return false;
            }

            return playCardHere;
        }

        public IEnumerator TakeDmg() {
            if (tutorialRound == 3 || tutorialRound == 8) {
                tutorialStep = TutorialSteps.takeDmgMessage;
                tapMessageIndex = 0;
                UpdateTutorialText();
                while (tutorialStep != TutorialSteps.start) {
                    yield return null;
                }
                tutorialText.text = string.Empty;
            }
        }

        public void PlayHeartCardFromHand() {
            if (tutorialRound == 5 && playerDeck.totalCardsInHand == 0) {
                playerAction.SetEndTurn(true);
                tutorialText.text = string.Empty;
            }
            if (tutorialRound == 7) {
                playerAction.SetEndTurn(true);
                tutorialText.text = string.Empty;
            }
            if (tutorialRound == 9 && playerDeck.totalCardsInHand == 0) {
                playerAction.SetEndTurn(true);
                tutorialText.text = string.Empty;
            } 
        }

        public void DropCard(EnemyCard card, bool canKillEnemy) {

            if (!cardsThatPlayedOver.Contains(card)) {
                cardsThatPlayedOver.Add(card);
            }
           
            if (active) { 
                if (canKillEnemy) {
                    if (tutorialRound == 0) {
                        tutorialStep = TutorialSteps.dropCard;
                        UpdateTutorialText();
                        playerAction.SetEndTurn(true);
                        ShowUpMessage("OMG YOURE AWESOME!");
                    }
                    else if (tutorialRound == 1) {
                        ShowUpMessage("Fucking brilliant!");
                        tutorialStep = TutorialSteps.playAllCards;
                        UpdateTutorialText();
                        playerAction.SetEndTurn(true);
                    }
                    else if (tutorialRound == 4) {
                        ShowUpMessage("You are amazing!");
                        tutorialStep = TutorialSteps.playAllCards;
                        playerAction.SetEndTurn(true);
                        UpdateTutorialText();

                    }
                }
                if (tutorialRound == 2 && dungeonManager.WillKillEveryEnemyCard()) {
                    ShowUpMessage("Nice!");
                    tutorialStep = TutorialSteps.playAllCards;
                    UpdateTutorialText();
                    playerAction.SetEndTurn(true);
                }
                if (tutorialRound == 3 && playerDeck.totalCardsInHand == 0) {
                    playerAction.SetEndTurn(true);
                }
                if (tutorialRound == 6 && playerDeck.totalCardsInHand == 1) {
                    playerAction.SetEndTurn(true);
                }
                if (tutorialRound == 8 && playerDeck.totalCardsInHand == 0) {
                    playerAction.SetEndTurn(true);
                }
            }

        }

        public void ReleaseCard() {
            if (active && tutorialRound == 0) {
                tutorialStep = TutorialSteps.start;
                UpdateTutorialText();
            }
        }

        public void StartNewTurn() {
            tutorialRound++;
            tapMessageIndex = 0;
            cardsThatPlayedOver.Clear();
            playerAction.SetEndTurn(false);
            dungeonManager.CreateEnemyLevel(tutorialWaves[tutorialRound]);

            if (tutorialRound == 0) { 
                tutorialWindow.SetActive(true);
                PlayerStatus.instance.life = 2;
                UpdateTutorialText();
            }


            if (tutorialRound == 1) {
                tutorialStep = TutorialSteps.start;
                UpdateTutorialText();
                StartCoroutine(playerDeck.DrawUpToXCards(3));
            }
            else if (tutorialRound == 2) {
                StartCoroutine(playerDeck.DrawUpToXCards(3));
                tutorialStep = TutorialSteps.showingTapMessages;
                UpdateTutorialText();
            }
            else if (tutorialRound == 3) {
                StartCoroutine(playerDeck.DrawUpToXCards(3));
                tutorialStep = TutorialSteps.showingTapMessages;
                UpdateTutorialText();
                
            }
            else if (tutorialRound == 4) {
                StartCoroutine(playerDeck.DrawUpToXCards(2));
                tutorialStep = TutorialSteps.showingTapMessages;
                UpdateTutorialText();
            }
            else if (tutorialRound == 5){
                StartCoroutine(playerDeck.DrawUpToXCards(2));
                tutorialStep = TutorialSteps.showingTapMessages;
                UpdateTutorialText();
            }
            else if (tutorialRound == 6) {
                StartCoroutine(playerDeck.DrawUpToXCards(3));
                tutorialStep = TutorialSteps.showingTapMessages;
                UpdateTutorialText();
            }
            else if (tutorialRound == 7) {
                StartCoroutine(playerDeck.DrawUpToXCards(2));
                tutorialStep = TutorialSteps.start;
                UpdateTutorialText();
            }
            else if (tutorialRound == 8) {
                StartCoroutine(playerDeck.DrawUpToXCards(2));
                tutorialStep = TutorialSteps.start;
                UpdateTutorialText();
            }
            else if (tutorialRound == 9) {
                StartCoroutine(playerDeck.DrawUpToXCards(2));
                tutorialStep = TutorialSteps.showingTapMessages;
                UpdateTutorialText();
            }
        }

        private void Update() {
            if (tutorialStep == TutorialSteps.showingTapMessages || tutorialStep == TutorialSteps.takeDmgMessage) {
                if (PlayerInput.instance.tapScreen) {
                    tapMessageIndex++;
                    UpdateTutorialText();
                }
            }
        }

        public bool OnMessage() {
            return active && tutorialStep == TutorialSteps.showingTapMessages;
        }

        public void CreateTutorialDeck() {
            foreach (CardInfo ci in tutorialDeck) {
                CardInfo card = new CardInfo(ci.rank, ci.suit);
                playerDeck.playerDeck.Add(card);
            }
        }
    }
}
