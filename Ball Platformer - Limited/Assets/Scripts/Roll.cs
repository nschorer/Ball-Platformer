using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Roll : MonoBehaviour, IMovingObject {

    public enum RollType { wobble, fullRotation};
    public enum Axis { x, y, z};

    public RollType rollType;
    public Axis rollAxis;
    public float rollSpeed;

    private Vector3 startRot;
    private Vector3 rotVector;
    private bool rewinding;
    private bool paused;
    private float fakeTime;

	// Use this for initialization
	void Start () {
        startRot = transform.rotation.eulerAngles;
        GetRotationVector();
        rewinding = false;
        fakeTime = 0f;
	}
	
	// FixedUpdate is called once per frame
	void FixedUpdate () {
        if (!paused){
            if (rollType == RollType.fullRotation)
            {
                FullRotation();
            }
            else if (rollType == RollType.wobble)
            {
                Wobble();
            }
        }
	}

    void FullRotation(){
        float mult = 1;
        if (rewinding) mult = -1;

        transform.eulerAngles += (rotVector * rollSpeed * mult);
    }

    void Wobble(){
        float timeDiff = Time.deltaTime;
        if (rewinding) timeDiff *= -1;
        fakeTime += timeDiff;

        transform.eulerAngles = startRot + 30 * Mathf.Sin(fakeTime * rollSpeed) * rotVector;
    }

    void GetRotationVector(){
        switch (rollAxis) {
            case Axis.x:
                rotVector = Vector3.right;
                break;
            case Axis.y:
                rotVector = Vector3.up;
                break;
            case Axis.z:
                rotVector = Vector3.forward;
                break;
            default:
                rotVector = Vector3.zero;
                break;
                }
    }

    public void Rewind()
    {
        rewinding = true;
    }

    public void Unrewind()
    {
        rewinding = false;
    }

    public void PauseRB(bool pauseOn)
    {
        paused = pauseOn;
        RigidbodyConstraints rbConstraints = pauseOn ? RigidbodyConstraints.FreezeAll : RigidbodyConstraints.None;
        transform.GetComponent<Rigidbody>().constraints = rbConstraints;
    }
}
