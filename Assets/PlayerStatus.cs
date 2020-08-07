using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerStatus : MonoBehaviour {

    public TextMeshProUGUI goldText, heartText;

    public static PlayerStatus instance;

    private void Awake() {
        instance = this;
    }


    private int _gold, _life;

    public int gold {
        get {
            return _gold;
        }
        set {
            _gold = value;
            UpdateGoldText();
        }
    }

    public int life {
        get {
            return _life;
        }

        set {
            _life = value;
            if (_life < 0) {
                _life = 0;
            }
            UpdateLifeText();
        }

    }

    public int ReducePlayerLife(int dmg) {
        int remaining = 0;
        if (life < dmg) {
            remaining = dmg - life;
        }
        life -= dmg;
        return remaining;
    }

    void UpdateGoldText() {
        goldText.text = gold.ToString();
    }

    void UpdateLifeText() {
        heartText.text = life.ToString();
    }



}
