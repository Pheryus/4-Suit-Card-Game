using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using Pheryus;

using CardFramework;
public class CardDeck : MonoBehaviour 
{
	[SerializeField]
	private GameObject _cardPrefab;

    public readonly List<CardFramework.Card> CardList = new List<CardFramework.Card>();

    public PlayerDeck playerDeck;

    public Transform deckTransform;

    public CardSlot deckSlot;

    public void InstantiateDeck(string cardBundlePath) {
        foreach (CardInfo c in playerDeck.playerDeck) {
            GameObject cardInstance = Instantiate(_cardPrefab);
            CardFramework.Card card = cardInstance.GetComponent<CardFramework.Card>();
            card.transform.localPosition = deckTransform.position;
            card.transform.SetParent(deckTransform);
			CardList.Add(card);

            deckSlot.AddCard(card);
            //c.card = card;
		}


    }

    private int StringToFaceValue(string input)
	{
		for (int i = 2; i < 11; ++i)
		{
			if (input.Contains(i.ToString()))
			{
				return i;
			}
		}
		if (input.Contains("jack") ||
		    input.Contains("queen") ||
		    input.Contains("king"))
		{
			return 10;
		}
		if (input.Contains("ace"))
		{
			return 11;
		}
		return 0;
	}	
}
