using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Climb : MonoBehaviour, IPad {

    private Renderer rend;
    private Vector3 lastPos;
    private Vector3 displacementVec;
    private bool paused;

	// Use this for initialization
	void Start () {
        rend = transform.GetComponent<Renderer>();
        lastPos = transform.position;
    }

    private void Update(){

        if (!paused)
        {
            displacementVec = transform.position - lastPos;
            lastPos = transform.position;
        }
    }

    void OnCollisionStay(Collision collision){

        if (!paused)
        {

            if (collision.collider.tag == "Player")
            {
                Vector3 pointOfContact = collision.contacts[0].point;
                collision.collider.transform.GetComponent<PlayerController>().Climb(pointOfContact, displacementVec);

                Color lightBrown = new Color(.80392f, .52156f, .24705f);
                if (rend.material.color != lightBrown) rend.material.color = lightBrown;
            }

        }
    }

    void OnCollisionExit(Collision collision){

        if (!paused)
        {

            if (collision.collider.tag == "Player")
            {
                rend.material.color = Color.white;
                collision.collider.transform.GetComponent<PlayerController>().EndClimb();
            }
        }
    }

    public void PausePad(bool pauseOn)
    {
        paused = pauseOn;
    }
}
