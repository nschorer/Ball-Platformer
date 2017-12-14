using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class MenuHandler {

    public static readonly Color buttonColor = new Color(.6f, .6f, .6f);
    public static readonly float buttonFlash = .2f;

    public static void DisableButton(Button button) {
        button.enabled = false;
        button.GetComponent<Image>().color = new Color(.4f, .4f, .4f, .5f);
        button.GetComponentInChildren<Text>().color = new Color(0f, 0f, 0f, .5f);
    }

    public static void EnableButton(Button button) {
        button.enabled = true;
        button.GetComponent<Image>().color = Color.white;
        button.GetComponentInChildren<Text>().color = Color.black;
    }

    public static void ShowCG(CanvasGroup cg, bool showCG) {
        cg.alpha = showCG ? 1f : 0f;
        cg.blocksRaycasts = showCG;
        cg.interactable = showCG;
    }
}
