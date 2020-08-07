using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardManager : MonoBehaviour {

    public static CardManager instance;

    public Sprite[] heartCards, diamondCards, clubCards, spadeCards;

    public Texture2D cardTexture;

    public Sprite flipDown;

    public GameObject cardPrefab, enemyCardPrefab;


    private void Awake() {
        instance = this;

        heartCards = new Sprite[12];
        diamondCards = new Sprite[12];
        clubCards = new Sprite[12];
        spadeCards = new Sprite[12];

        Sprite[] sprites = Resources.LoadAll<Sprite>(cardTexture.name);

        Debug.Log(sprites.Length);
        for (int i = 0; i < 12; i++) {
            clubCards[i] = sprites[i];
        }
        for (int i = 0; i < 12; i++) {
            diamondCards[i] = sprites[i + 13];
        }
        for (int i = 0; i < 12; i++) {
            heartCards[i] = sprites[i + 26];
        }
        for (int i = 0; i < 12; i++) {
            spadeCards[i] = sprites[i + 39];
        }

    }

}
