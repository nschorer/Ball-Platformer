using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingText : MonoBehaviour {

    private Camera mainCamera;
    private TextMesh floatingText;

	// Use this for initialization
	void Start () {
        mainCamera = Camera.main;
        floatingText = GetComponent<TextMesh>();
	}
	
	// Update is called once per frame
	void Update () {
        // If there's nothing to show, don't even worry about the orientation
		if (floatingText.text != null && floatingText.color.a != 0f){
            Vector3 fwd = mainCamera.transform.forward;
            transform.rotation = Quaternion.LookRotation(fwd);
        }
	}
}
