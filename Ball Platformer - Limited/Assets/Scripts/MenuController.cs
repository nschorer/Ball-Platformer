using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;

public class MenuController : MonoBehaviour {

    private bool heldUp, heldDown, heldLeft, heldRight;
    private bool justPressedSubmit, justPressedCancel, justPressedPause;

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
        if (justPressedPause && !PausePressed()) justPressedPause = false;
        if (justPressedSubmit && !SubmitPressed()) justPressedSubmit = false;
        if (justPressedCancel && !CancelPressed()) justPressedCancel = false;
	}

    private bool UpPressed()
    {
        if (InputManager.ActiveDevice.LeftStick.Y > BUTTON_THRESHOLD) return true;
        if (InputManager.ActiveDevice.DPadUp) return true;

        return Input.GetAxis("Key Vertical") > BUTTON_THRESHOLD/5f;
    }

    private bool DownPressed()
    {
        if (InputManager.ActiveDevice.LeftStick.Y < -BUTTON_THRESHOLD) return true;
        if (InputManager.ActiveDevice.DPadDown) return true;

        return Input.GetAxis("Key Vertical") < -BUTTON_THRESHOLD/5f;
    }

    private bool LeftPressed(){
        if (InputManager.ActiveDevice.LeftStick.X < -BUTTON_THRESHOLD) return true;
        if (InputManager.ActiveDevice.DPadLeft) return true;

        return Input.GetAxis("Key Horizontal") < -BUTTON_THRESHOLD/5f;
    }

    private bool RightPressed(){
        if (InputManager.ActiveDevice.LeftStick.X > BUTTON_THRESHOLD) return true;
        if (InputManager.ActiveDevice.DPadRight) return true;

        return Input.GetAxis("Key Horizontal") > BUTTON_THRESHOLD/5f;
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

    public bool PausePressed() {
        if (InputManager.ActiveDevice.MenuWasPressed) return true;
        return Input.GetButtonDown("Pause");
    }

    public bool SubmitPressed() {
        if (InputManager.ActiveDevice.GetControl(InputControlType.Action1)) return true;
        return Input.GetButtonDown("Key Submit");
    }

    public bool CancelPressed() {
        if (InputManager.ActiveDevice.GetControl(InputControlType.Action2)) return true;
        return Input.GetButtonDown("Key Cancel");
    }

    public bool Pause(){
        if (PausePressed()) {
            if (!justPressedPause) {
                justPressedPause = true;
                return true;
            }
        }
        return false;
    }

    public bool Submit(){
        if (SubmitPressed()) {
            if (!justPressedSubmit) {
                justPressedSubmit = true;
                return true;
            }
        }
        return false;
    }

    public bool Cancel() {
        if (CancelPressed()) {
            if (!justPressedCancel) {
                justPressedCancel = true;
                return true;
            }
        }
        return false;
    }

}
