using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour {

    public static PlayerInput instance;

    public bool releaseFinger, tapScreen, isTapping;

    public Ray inputRay;

    private void Awake() {
        instance = this;
    }

    private void Update() {

#if UNITY_STANDALONE || UNITY_EDITOR

        inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        tapScreen = Input.GetMouseButtonDown(0);
        isTapping = Input.GetMouseButton(0);
        releaseFinger = Input.GetMouseButtonUp(0);

#elif UNITY_ANDROID || UNITY_IPHONE
        if (Input.touchCount > 0) {
            Touch t = Input.GetTouch(0);
            inputRay = Camera.main.ScreenPointToRay(t.position);

            tapScreen = t.phase == TouchPhase.Began;
            releaseFinger = t.phase == TouchPhase.Ended || t.phase == TouchPhase.Canceled;
            isTapping = t.phase == TouchPhase.Stationary || t.phase == TouchPhase.Moved;
        }
#endif
    }
}
