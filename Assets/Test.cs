using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour {

    

    private void Start() {
        CardInfo c = new CardInfo(Rank.ace, Suit.clubs);
        CardInfo c2 = null;
        c2 = c;
        c2 = null;

    }

}
