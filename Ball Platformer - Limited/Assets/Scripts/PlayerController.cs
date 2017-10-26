using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour, IMovingObject {

	public float acceleration = 1f;
	public float maxSpeed = 50f;
	public bool debugMode = false;

	private const float SD_TIME = 2f;
    private const float EXIT_TIME = 3f;

    private Rigidbody rb;
 	private Camera mainCamera;
 	private bool dying;
 	private bool exploded;
 	private float deathTime;
 	private bool beatLevel;
 	private float sdTimer;
    private float exitTimer;
    private Text exitText;
    private float lastBoosted;
    private bool climbing;
    private Vector3 climbNormal;
    private float climbSpeed;
    private bool rewinding;
    private bool paused;
    private Vector3 velocWhenPaused;
    private PauseBP pauseBP;
    private bool unpausedThisFrame;

    //setup
    private void Start ()
	{
		rb = GetComponent<Rigidbody> ();

		// If debug mode is on, the player starts at its location in the Scene Editor
		// Otherwise, it starts on top of the start pad
		if (!debugMode) {
			// There should only be one with object with tag Start per scene
			GameObject startPad = GameObject.FindGameObjectWithTag ("Start");
			if (startPad == null)
				Debug.LogError ("Could not find start pad.");
			else
				gameObject.transform.position = (startPad.transform.position + (Vector3.up * 0.3f));
		}

        // Find exit text
        GameObject exitTextObject = GameObject.FindGameObjectWithTag("Exit Text");
        if (exitTextObject != null) exitText = exitTextObject.GetComponent<Text>();
        GameObject pauseObject = GameObject.FindGameObjectWithTag("Pause");
        if (pauseObject != null) pauseBP = pauseObject.GetComponent<PauseBP>();
        else { Debug.LogWarning("Pause Menu is not implemented in scene."); }

        // Set sd/exit timers and camera.
		sdTimer = SD_TIME;
        exitTimer = EXIT_TIME;
		mainCamera = Camera.main;
        lastBoosted = 0f;
        climbNormal = Vector3.up;
        climbSpeed = 5f;
    }

    private void Update ()
	{
		// Player is dead. Play death animation.
		if (dying) {

			deathTime -= Time.deltaTime;

			if (deathTime > 1.5f) {
				Color playerColor = gameObject.GetComponent<Renderer> ().material.color;
				playerColor.r = Mathf.Lerp (playerColor.r, 255f, 0.0001f);
				playerColor.g = Mathf.Lerp (playerColor.g, 255f, 0.0001f);
				playerColor.b = Mathf.Lerp (playerColor.b, 255f, 0.0001f);
				gameObject.GetComponent<Renderer> ().material.color = playerColor;
			} else if (deathTime > 0f) {
				if (!exploded) {
					gameObject.GetComponent<ParticleSystem> ().Play ();

					// We turn the game object invisible (so that it actually looks like it exploded)
					// We can't destroy the object because the camera is still referencing it
					gameObject.GetComponent<MeshRenderer> ().enabled = false;

					exploded = true;
				}
			} else {
				GameObject.FindObjectOfType<LevelManager> ().ReloadLevel ();
			}
		} else {

            if (!paused && !beatLevel)
            {

                if (!unpausedThisFrame && Input.GetButtonDown("Pause"))
                {
                    if (pauseBP != null) pauseBP.TogglePause(true);
                }
                unpausedThisFrame = false;
            }
		}
    }

    //Use physics to move the ball
    private void FixedUpdate (){

        if (!paused)
        {

            // Get player input
            float moveHorizontal = Input.GetAxis("Horizontal");
            float moveVertical = Input.GetAxis("Vertical");
            Vector3 movement;

            // Move ball relative to camera direction
            // Used code from answers.unity3d.com/questions/8444/moving-player-relative-to-camera.html
            Vector3 forward = mainCamera.transform.TransformDirection(Vector3.forward);
            forward.y = 0f;
            forward = forward.normalized;
            Vector3 right = new Vector3(forward.z, 0.0f, -forward.x);
            // Limit the ball to a max horizontal speed
            Vector3 horVec = (rb.velocity.x * Vector3.right) + (rb.velocity.z * Vector3.forward);

            // just in case
            if (!climbing && !rb.useGravity) rb.useGravity = true;

            // If we are climbing, the plane of movement is not the XZ plane,
            //    it is the plane of the surface we are rolling on.
            if (climbing && (Vector3.Distance(climbNormal, Vector3.down) >= 0.3f))
            {
                Quaternion rotation = Quaternion.FromToRotation(Vector3.up, climbNormal);

                forward = rotation * forward;
                right = rotation * right;
            }


            movement = (moveHorizontal * right) + (moveVertical * forward);

            if (climbing)
            {
                if (movement.magnitude == 0f)
                {
                    rb.velocity = Vector3.zero;
                }
                else
                {
                    rb.velocity = (movement.normalized * climbSpeed);
                }
            }
            else if (!IsStillBoosted() && horVec.magnitude > maxSpeed)
            {
                float yVeloc = rb.velocity.y;
                rb.velocity = (horVec.normalized * maxSpeed) + (yVeloc * Vector3.up);
            }
            else
            {
                rb.AddForce(movement * acceleration);
            }

        }
    }

    public void Boost(Vector3 boostVector){
        rb.AddForce(boostVector);
        lastBoosted = Time.time;
    }

    private bool IsStillBoosted(){
        return (Time.time - lastBoosted) < 4f;
    }

    public void Climb(Vector3 point, Vector3 displacementVec){
        climbNormal = (transform.position - point).normalized;
        transform.position = Vector3.MoveTowards(transform.position, point, 0.01f);
        //transform.position += displacementVec;

        if (!climbing){
            climbing = true;
            rb.useGravity = false;
        }
    }

    public void Climb(Vector3 point){
        Climb(point, Vector3.zero);
    }

    public void EndClimb(){
        climbing = false;
        rb.useGravity = true;
    }

    public void BeatLevel (){
    	beatLevel = true;
	}

    public void Die (){
		if (!beatLevel){
			rb.isKinematic = true;
			dying = true;
			deathTime = 2f;
		}
    }

    public bool IsDying(){
        return dying;
    }

    public void Rewind()
    {
        
    }

    public void Unrewind()
    {
        
    }

    public void PauseRB(bool pauseOn)
    {
        paused = pauseOn;

        if (pauseOn){
            velocWhenPaused = rb.velocity;
        }

        RigidbodyConstraints rbConstraints = pauseOn ? RigidbodyConstraints.FreezeAll : RigidbodyConstraints.None;
        transform.GetComponent<Rigidbody>().constraints = rbConstraints;

        if (!pauseOn){
            rb.velocity = velocWhenPaused;
            unpausedThisFrame = true;
        }
    }
}
