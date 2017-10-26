﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

	public float turnSpeed = 3f;

    private static Vector3 START_OFFSET = new Vector3(0f, 9.5f, -10f);
    private const float START_TURN_COEFFICIENT = 0.05f;

    private GameObject player;
    private Vector3 offset;
    private float hCoefficient, vCoefficient;

	// Use this for initialization
	void Start () {
		// Camera stays in line with the player
		player = GameObject.FindGameObjectWithTag("Player");
        offset = START_OFFSET;
        hCoefficient = ResetTurnCoefficient();
        vCoefficient = ResetTurnCoefficient();
    }
	
	// Things that need to happen after update/right before the camera renders.
	// In this case, this ensures that the camera moves AFTER the ball, so it is always centered.
	void LateUpdate ()
	{

		// [Used code from answers.unity3d.com/questions/8444/moving-player-relative-to-camera.html]
		// When we rotate up or down, we are rotating around the axis that is perpendicular to the
		// camera's foward direction.
		Vector3 right = gameObject.transform.TransformDirection (Vector3.right);
		right.y = 0f;
		right = right.normalized;

		// Detect whether the camera is looking straight up or straight down
		Vector3 direction = (player.transform.position - gameObject.transform.position).normalized;

        float moveHorizontal = Input.GetAxis("Camera Horizontal");
        float moveVertical = Input.GetAxis("Camera Vertical");

        //rotate horizontal
        offset = Quaternion.AngleAxis(moveHorizontal * turnSpeed, Vector3.up) * offset;

        //rotate vertical
        bool okayToMoveVertical = true;
        if (moveVertical > 0 && Vector3.Distance(direction, Vector3.down) < 0.08) okayToMoveVertical = false;
        if (moveVertical < 0 && Vector3.Distance(direction, Vector3.up) < 0.08) okayToMoveVertical = false;
        if (okayToMoveVertical) offset = Quaternion.AngleAxis(moveVertical * 0.5f * turnSpeed, right) * offset;

        // The camera always stays a fixed distance from the ball
        transform.position = player.transform.position + offset;
        transform.LookAt(player.transform.position);


        // If all they are doing is holding directions that cancel each other out, then they aren't actually holding a direction
        bool heldHDirection = moveHorizontal != 0 ? true : false;
        bool heldVDirection = moveVertical != 0 ? true : false;

        if (!heldHDirection) hCoefficient = ResetTurnCoefficient();
        else hCoefficient = IncrementTurnCoefficient(hCoefficient);

        if (!heldVDirection) vCoefficient = ResetTurnCoefficient();
        else vCoefficient = IncrementTurnCoefficient(vCoefficient);
    }

    float ResetTurnCoefficient()
    {
        return START_TURN_COEFFICIENT;
    }

    float IncrementTurnCoefficient(float turnCoefficient)
    {
        if (turnCoefficient < 1f) turnCoefficient += START_TURN_COEFFICIENT * Mathf.Lerp(0f, 1f, 1 - (turnCoefficient/1f));
        if (turnCoefficient > 1f) turnCoefficient = 1f;
        return turnCoefficient;
    }
}

