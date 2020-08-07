using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CardEvent", menuName = "New Card Event")]
public class CardEvent : ScriptableObject {
    public Option[] options;
    public int level;


    public Option GetRandomOption() {
        return options[Random.Range(0, options.Length - 1)];
    }
}

[System.Serializable]
public class Option {
    public List<CardInfo> cards;
    public int cost = 1;


}
