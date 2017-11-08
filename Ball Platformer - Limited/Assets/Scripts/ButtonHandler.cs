using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class ButtonHandler {

	public static void DisableButton(Button button) {
        button.enabled = false;
        button.GetComponent<Image>().color = new Color(.4f, .4f, .4f, .5f);
        button.GetComponentInChildren<Text>().color = new Color(0f, 0f, 0f, .5f);
    }

}
