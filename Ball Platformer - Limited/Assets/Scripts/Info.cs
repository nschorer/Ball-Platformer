using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Info : MonoBehaviour {

	public Image infoImage;

	// While player is standing on pad, display some image.
	void OnTriggerEnter (Collider collider){
		if (collider.tag  == "Player") {
			if (infoImage != null){
				Color visible = infoImage.color;
				visible.a = 255f;
				infoImage.color = visible;
			}
		}
	}

	void OnTriggerExit (Collider collider){
		if (collider.tag  == "Player") {
			if (infoImage != null){
				Color invisible = infoImage.color;
				invisible.a = 0f;
				infoImage.color = invisible;
			}
		}
	}
}
