using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuController : MonoBehaviour {

    private bool heldUp, heldDown, heldLeft, heldRight;

    private const float BUTTON_THRESHOLD = 0.75f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (heldUp && !UpPressed()) heldUp = false;
        if (heldDown && !DownPressed()) heldDown = false;
        if (heldLeft && !LeftPressed()) heldLeft = false;
        if (heldRight && !RightPressed()) heldRight = false;
	}

    private bool UpPressed()
    {
        return Input.GetAxis("Vertical") > BUTTON_THRESHOLD;
    }

    private bool DownPressed()
    {
        return Input.GetAxis("Vertical") < -BUTTON_THRESHOLD;
    }

    private bool LeftPressed(){
        return Input.GetAxis("Horizontal") < -BUTTON_THRESHOLD;
    }

    private bool RightPressed(){
        return Input.GetAxis("Horizontal") > BUTTON_THRESHOLD;
    }

    public bool Up() {
        if (UpPressed())
        {
            if (!heldUp)
            {
                heldUp = true;
                return true;
            }
        }
        return false;
    }

    public bool Down(){
        if (DownPressed())
        {
            if (!heldDown)
            {
                heldDown = true;
                return true;
            }
        }
        return false;
    }

    public bool Left(){
        if (LeftPressed())
        {
            if (!heldLeft)
            {
                heldLeft = true;
                return true;
            }
        }
        return false;
    }

    public bool Right(){
        if (RightPressed())
        {
            if (!heldRight)
            {
                heldRight = true;
                return true;
            }
        }
        return false;
    }

    public bool Pause(){ return Input.GetButtonDown("Pause"); }

    public bool Submit(){ return Input.GetButtonDown("Submit"); }

    public bool Cancel() { return Input.GetButtonDown("Cancel"); }
}
