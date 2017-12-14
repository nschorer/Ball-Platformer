using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Speed : MonoBehaviour, IPad {

	private float boostSpeed = 50f;
	private Vector3 boostDirection = Vector3.up;
    private bool paused;

	// Use this for initialization
	void Start () {
		// Top of Pad is an empty object that is simply there to allow us to identify what direction the boost pad is facing.
		// Note: for some reason the vector goes the opposite way of what I'd think.
		GameObject padTop = transform.GetChild(0).gameObject;
		boostDirection = -(padTop.transform.position - transform.position).normalized;
	}

	void OnTriggerStay (Collider collider){

        if (!paused)
        {

            if (collider.tag == "Player")
            {
                // Every frame that the player sits on the pad, the player accelerates in that direction.
                collider.attachedRigidbody.GetComponent<PlayerController>().Boost(boostDirection * boostSpeed);

                // In addition the pad will get redder the longer the player sits on it.
                // We make it redder by reducing the other two color components (since it starts out as white)
                Color darkerRed = GetComponent<Renderer>().material.color;
                darkerRed.g = darkerRed.g - .05f;
                darkerRed.b = darkerRed.b - .05f;
                GetComponent<Renderer>().material.color = darkerRed;
            }

        }
	}

    void OnTriggerExit(Collider collider)
    {

        if (!paused)
        {

            if (collider.tag == "Player")
            {
                // After the player leaves the pad, we make it white again.
                GetComponent<Renderer>().material.color = Color.white;
            }
        }
	}

    public void PausePad(bool pauseOn)
    {
        paused = pauseOn;
    }
}
