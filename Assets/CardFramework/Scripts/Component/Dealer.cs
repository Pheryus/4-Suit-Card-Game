using UnityEngine;
using System.Collections;
using System.IO;

using CardFramework;

public class Dealer : MonoBehaviour 
{
	public DealerUI DealerUIInstance { get; set; }
    
	[SerializeField]
	private CardDeck _cardDeck;	

	[SerializeField]
	private CardSlot _pickupCardSlot;		

	[SerializeField]
	private CardSlot _stackCardSlot;

    [SerializeField]
    private CardSlot _shuffleStackCardSlot;

	[SerializeField]
	private CardSlot _discardStackCardSlot;		

	[SerializeField]
	private CardSlot _discardHoverStackCardSlot;			

	[SerializeField]
	private CardSlot _rightHandCardSlot;

	[SerializeField]
	private CardSlot _leftHandCardSlot;

	[SerializeField]
	private CardSlot _currentCardSlot;	

	[SerializeField]
	private CardSlot _prior0CardSlot;	

	[SerializeField]
	private CardSlot _prior1CardSlot;	

	[SerializeField]
	private CardSlot _prior2CardSlot;		

	[SerializeField]
	private CardSlot _prior3CardSlot;

	[SerializeField]
	private CardSlot _prior4CardSlot;

    [SerializeField]
    private CardSlot _lostCardSlot;

    public Transform cardParent;

    public Transform cardSlots;

	private const float CardStackDelay = .01f;
	
	/// <summary>
	/// Counter which keeps track current dealing movements in progress.
	/// </summary>
	public int DealInProgress { get; set; }

	private void Awake()
	{
        /*
		_cardDeck.InstantiateDeck("cards");
        */
	}

    private void Start() {
    }

    public void DropCards() {
        StartCoroutine(StackCardRangeOnSlot(0, _cardDeck.CardList.Count, _stackCardSlot));
    }

    private void MoveCardSlotToCardSlot(CardSlot sourceCardSlot, CardSlot targerCardSlot) 
	{
		Card card;
		while ((card = sourceCardSlot.TopCard()) != null)
		{
			targerCardSlot.AddCard(card);
		}
	}

    public void SetCardSlotActive (bool b) {
        cardSlots.gameObject.SetActive(b);
    }

    public void MoveCardToCardSlot(Card card, CardSlot targetCardSlot) {
        targetCardSlot.AddCard(card);
    }

    public void MoveCardToDeck (Card card) {
        MoveCardToCardSlot(card, _stackCardSlot);
    }

    public void MoveCardToDiscardSlot (Card card) {
        MoveCardToCardSlot(card, _discardStackCardSlot);
    }

    public void MoveCardToLoseSlot (Card card) {
        MoveCardToCardSlot(card, _lostCardSlot);
    }

    public void SetCardToPlayer (Card card) {
        card.transform.SetParent(cardParent);
    }

    public Vector3 GetDeckPosition() {
        return _stackCardSlot.transform.position;
    }

    private IEnumerator StackCardRangeOnSlot(int start, int end, CardSlot cardSlot) 
	{
		DealInProgress++;
		for (int i = start; i < end; ++i)
		{
			cardSlot.AddCard(_cardDeck.CardList[i]);
			yield return new WaitForSeconds(CardStackDelay);
		}
		DealInProgress--;
    }


    /// <summary>
    /// Shuffle Coroutine.
    /// Moves all card to pickupCardSlot. Then shuffles them back
    /// to cardStackSlot.
    /// </summary>
    public IEnumerator ShuffleCoroutine()
	{
        Pheryus.PlayerDeck.instance.HideHand(false);
        DealInProgress++;
		MoveCardSlotToCardSlot(_stackCardSlot, _pickupCardSlot);		
		MoveCardSlotToCardSlot(_discardStackCardSlot, _pickupCardSlot);	
		yield return new WaitForSeconds(.4f);	
		int halfLength = _pickupCardSlot.CardList.Count / 2;
        int fullSize = _pickupCardSlot.CardList.Count;
		for (int i = 0; i < halfLength; ++i)
		{
			_leftHandCardSlot.AddCard(_pickupCardSlot.TopCard());
		}
		yield return new WaitForSeconds(.2f);	
		for (int i = 0; i < fullSize - halfLength; ++i)
		{
			_rightHandCardSlot.AddCard(_pickupCardSlot.TopCard());
		}
		yield return new WaitForSeconds(.2f);	
		for (int i = 0; i < fullSize; i++)
		{
			if (i % 2 == 0)
			{
				_shuffleStackCardSlot.AddCard(_rightHandCardSlot.TopCard());
			}
			else
			{
                _shuffleStackCardSlot.AddCard(_leftHandCardSlot.TopCard());
			}
			yield return new WaitForSeconds(CardStackDelay);
		}

        yield return new WaitForSeconds(.3f);
        for (int i = 0; i < fullSize; i++) {
            _stackCardSlot.AddCard(_shuffleStackCardSlot.TopCard());
        }
		DealInProgress--;
        yield return new WaitForSeconds(.3f);
        Pheryus.PlayerDeck.instance.HideHand(true);
    }

    public IEnumerator MoveCardInHand(Pheryus.Card card, int position) {
        card.position = position;
        if (position == 4) {
            _prior0CardSlot.AddCard(card.GetComponent<Card>());
        }
        else if (position == 3) {
            _prior1CardSlot.AddCard(card.GetComponent<Card>());

        }
        else if (position == 2) {
            _prior2CardSlot.AddCard(card.GetComponent<Card>());
        }
        else if (position == 1) {
            _prior3CardSlot.AddCard(card.GetComponent<Card>());
        }
        else if (position == 0) {
            _prior4CardSlot.AddCard(card.GetComponent<Card>());
        }
        yield return null;
    }

    public Card GetTopCardFromDeck() {
        return _stackCardSlot.TopCard();
    }

	public IEnumerator DrawCoroutine(Pheryus.CardInfo card, int position)
	{
		DealInProgress++;
      
        card.card = _stackCardSlot.TopCard();
        card.card.position = position;
        _stackCardSlot.TopCard().cardInfo = card;
        ((Card)card.card).returnToStartPosition = true;
        card.card.GetComponent<Pheryus.DraggableCard>().enabled = true;

        yield return StartCoroutine(MoveCardInHand(card.card, position));

        DealInProgress--;
	}	
}
