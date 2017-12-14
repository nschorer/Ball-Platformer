using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using InControl;

public class PlayerController : MonoBehaviour, IMovingObject {

	public float acceleration = 1f;
	public float maxSpeed = 50f;
	public bool debugMode = false;
    public Material[] ballColors;

    public enum BallColor { Maroon, Blue, Gray, Green, Purple, Yellow, Forest, Desert, Canyon, Island, Space};

	private const float SD_TIME = 2f;
    private const float EXIT_TIME = 3f;

    private Rigidbody rb;
 	private Camera mainCamera;
 	private bool dying;
 	private bool exploded;
 	private float deathTime;
 	private bool beatLevel;
    private float lastBoosted;
    private bool climbing;
    private Vector3 climbNormal;
    private float climbSpeed;
    private bool paused;
    private Vector3 velocWhenPaused;
    private PauseBP pauseBP;
    private bool unpausedThisFrame;
    private float boostPP, bouncePP, climbPP, rewindPP;
    private Vector3 lastMovementVector;
    private bool holdAbButton;
    private bool usingClimbSkill; //Using Climb ability (may not be touching a surface)
    private float waitRechargeRewind;
    private float waitRechargeClimb;
    private bool padClimb; //Climbing on a Climb Pad
    private bool isFirstFrame;
    private bool consumeClimbPPThisFrame;
    private GameData gameData;
    private SoundFx soundfx;

    private float tempTimer;

    private const float MAX_BOOST = 5f;
    private const float MAX_BOUNCE = 8f;
    private const float MAX_CLIMB = 2f;
    private const float MAX_REWIND = 4.5f;
    private const float CLIMB_CHARGE_DELAY = 1f;
    private const float CLIMB_PENALTY = 0.25f;
    private const float REWIND_CHARGE_DELAY = 0.5f;

    private void Awake() {
        //Rewind.ClearMovingObjects();
    }

    //setup
    void Start (){
		rb = GetComponent<Rigidbody> ();
        isFirstFrame = true;

		// If debug mode is on, the player starts at its location in the Scene Editor
		// Otherwise, it starts on top of the start pad
		if (!debugMode) {

            bool foundAltStart = FindAlternateStart();

            if (!foundAltStart) {
                // There should only be one with object with tag Start per scene
                GameObject startPad = GameObject.FindGameObjectWithTag("Start");
                if (startPad == null)
                    Debug.LogError("Could not find start pad.");
                else
                    gameObject.transform.position = (startPad.transform.position + (Vector3.up * 0.3f));
            }
		}

        tempTimer = 1f;

        // Find exit text
        GameObject pauseObject = GameObject.FindGameObjectWithTag("Pause");
        if (pauseObject != null) pauseBP = pauseObject.GetComponent<PauseBP>();
        else { Debug.LogWarning("Pause Menu is not implemented in scene."); }
        gameData = GameObject.FindGameObjectWithTag("Game Data").GetComponent<GameData>();
        if (gameData != null) gameData.SetPlayerReference(this);
        soundfx = GameObject.FindGameObjectWithTag("Sound Fx").GetComponent<SoundFx>();


        // Set sd/exit timers and camera.
		mainCamera = Camera.main;
        lastBoosted = 0f; //While boosted, we can exceed the normal max speed for a short time.
        climbNormal = Vector3.up; //When climbNormal is UP, movement directions are same as normal.
        climbSpeed = 5f; //Movement speed is locked at 5f when climbing.
        lastMovementVector = Vector3.zero; //When we use player-boost, we need to know the last direction player was moving in.
        InitAbilities();
        //Rewind.GetMovingObjects();
    }

    // In the hub world, there are multiple places where you can spawn, depending on which world you just came from.
    // As such, when this method is called, we haven't updated SessionData.currentWorld (we update it at the end).
    bool FindAlternateStart() {
        if (!LevelManager.IsHub()) return false;

        GameObject startPad = null;

        if (SessionData.currentWorld == WorldEntrance.World.Forest) {
            startPad = GameObject.FindGameObjectWithTag("Forest");
        }else if (SessionData.currentWorld == WorldEntrance.World.Desert) {
            startPad = GameObject.FindGameObjectWithTag("Desert");
        }else if (SessionData.currentWorld == WorldEntrance.World.Canyon) {
            startPad = GameObject.FindGameObjectWithTag("Canyon");
        } else if (SessionData.currentWorld == WorldEntrance.World.Island) {
            startPad = GameObject.FindGameObjectWithTag("Island");
        } else if (SessionData.currentWorld == WorldEntrance.World.Space) {
            startPad = GameObject.FindGameObjectWithTag("Space");
        }

        SessionData.currentWorld = WorldEntrance.World.Hub;

        if (startPad == null) return false;
        else {
            gameObject.transform.position = (startPad.transform.position + (Vector3.up * 0.3f));
            return true;
        }
    }

    void InitAbilities() {
        boostPP = MAX_BOOST;
        bouncePP = MAX_BOUNCE;
        climbPP = MAX_CLIMB;
        rewindPP = MAX_REWIND;
    }

    private void Update (){
        tempTimer -= Time.deltaTime;
        if (tempTimer <= 0) {
            tempTimer = 1f;
            //print(rb.velocity.magnitude);
        }

        if (isFirstFrame) {
            SetBallColor(gameData.BallColor);
            isFirstFrame = false;
        }

		// Player is dead. Play death animation.
		if (dying) {
            HandleDeath();
		} else {

            if (!paused && !unpausedThisFrame) {
                CheckAbility();
                if (consumeClimbPPThisFrame) {
                    climbPP -= Time.deltaTime;
                    consumeClimbPPThisFrame = false;
                }
            }

            CheckPause();
		}
    }

    //Use physics to move the ball
    private void FixedUpdate (){

        if (!paused){
            PlayerMovement();
        }
    }

    public void PlayerMovement() {
        // Get player input
        InputDevice device = InputManager.ActiveDevice;
        //Joystick input
        float moveHorizontal = device.LeftStick.X *1.1f;
        float moveVertical = device.LeftStick.Y *1.1f;

        //Keyboard input
        if (moveHorizontal == 0f && moveVertical == 0f) {
            moveHorizontal = Input.GetAxis("Key Horizontal");
            moveVertical = Input.GetAxis("Key Vertical");
        }
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
        if (climbing && (Vector3.Distance(climbNormal, Vector3.down) >= 0.3f)) {
            Quaternion rotation = Quaternion.FromToRotation(Vector3.up, climbNormal);

            forward = rotation * forward;
            right = rotation * right;
        }


        movement = (moveHorizontal * right) + (moveVertical * forward);
        if (movement != Vector3.zero) lastMovementVector = movement.normalized;

        if (climbing) {
            if (movement.magnitude == 0f) {
                rb.velocity = Vector3.zero;
            } else {
                rb.velocity = (movement.normalized * climbSpeed);
                soundfx.Climb(transform.position);
            }
        } else if (!IsStillBoosted() && horVec.magnitude > maxSpeed) {
            float yVeloc = rb.velocity.y;
            Vector3 adjMovement = (horVec + 2*movement/maxSpeed).normalized * maxSpeed;
            rb.velocity = adjMovement + (yVeloc * Vector3.up);
        } else {
            rb.AddForce(movement * acceleration);
        }
    }

    void HandleDeath() {
        deathTime -= Time.deltaTime;

        if (deathTime > 1.5f) {
            Color playerColor = gameObject.GetComponent<Renderer>().material.color;
            playerColor.r = Mathf.Lerp(playerColor.r, 255f, 0.0001f);
            playerColor.g = Mathf.Lerp(playerColor.g, 255f, 0.0001f);
            playerColor.b = Mathf.Lerp(playerColor.b, 255f, 0.0001f);
            gameObject.GetComponent<Renderer>().material.color = playerColor;
        } else if (deathTime > 0f) {
            if (!exploded) {
                gameObject.GetComponent<ParticleSystem>().Play();

                // We turn the game object invisible (so that it actually looks like it exploded)
                // We can't destroy the object because the camera is still referencing it
                gameObject.GetComponent<MeshRenderer>().enabled = false;

                exploded = true;
            }
        } else {
            GameObject.FindObjectOfType<LevelManager>().ReloadLevel();
        }
    }

    void CheckPause() {
        if (!paused && !beatLevel) {
            InputDevice device = InputManager.ActiveDevice;

            if (!unpausedThisFrame && (Input.GetButtonDown("Pause") || device.MenuWasPressed)) {
                if (pauseBP != null) pauseBP.TogglePause(true);
            }
            unpausedThisFrame = false;
        }
    }



    void CheckAbility() {
        SessionData.Ability ab = SessionData.currentAbility;
        //Nothing
        if (ab == SessionData.Ability.None) {
            return;
        }

        InputDevice device = InputManager.ActiveDevice;
        InputControl control = device.GetControl(InputControlType.Action1);

        bool abPressed = control.WasPressed || Input.GetButtonDown("Ability");
        bool abHeld = control.IsPressed || Input.GetButton("Ability"); 

        //Boost
        if (ab == SessionData.Ability.Boost) {
            if (abPressed && !climbing && (lastMovementVector != Vector3.zero) && (boostPP == MAX_BOOST)) {
                Boost(lastMovementVector * 500f);
                boostPP = 0f;
            }else {
                RecoverBoostPP();
            }
        }
    }

    public void Boost(Vector3 boostVector){
        rb.AddForce(boostVector);
        lastBoosted = Time.time;
        soundfx.Boost(transform.position);
    }

    private bool IsStillBoosted(){
        return (Time.time - lastBoosted) < 4f;
    }

    void Bounce() {
        Vector3 bounceVector = Vector3.up;

        bounceVector *= 350f;
        if (rb.velocity.y < 0f) {
            bounceVector += rb.velocity.y * -40f * Vector3.up;
        }

        soundfx.Bounce(transform.position + (Vector3.down * .5f), 1);
        rb.AddForce(bounceVector);
    }

    // If isPlayer = false, player is on a climb pad. If true, then player is using ability.
    public void Climb(Vector3 point, GameObject climbedObject, bool isPlayer = false){
        climbNormal = (transform.position - point).normalized;
        transform.position = Vector3.MoveTowards(transform.position, point, 0.01f);

        if (!climbing){
            climbing = true;
            if (!isPlayer) padClimb = true; //Specifically, are we climbing via a pad, not via the skill
            rb.useGravity = false;
        }
    }

    public void EndClimb(bool isPlayer = false){
        climbing = false;
        rb.useGravity = true;
        if (!isPlayer) padClimb = false;
    }

    void RecoverBoostPP() {
        if (boostPP < MAX_BOOST) {
            boostPP += Time.deltaTime;
        } else if (boostPP > MAX_BOOST) {
            boostPP = MAX_BOOST;
        }
    }

    void RecoverBouncePP() {
        if (bouncePP < MAX_BOUNCE) {
            bouncePP += Time.deltaTime;
        }else if (bouncePP > MAX_BOUNCE) {
            bouncePP = MAX_BOUNCE;
        }
    }

    void RecoverClimbPP() {
        if (climbPP < MAX_CLIMB) {
            climbPP += (Time.deltaTime * 0.25f);
        } else if (climbPP > MAX_CLIMB) {
            climbPP = MAX_CLIMB;
        }
    }

    void RecoverRewindPP() {
        if (rewindPP < MAX_REWIND) {
            rewindPP += (Time.deltaTime * 0.33f);
        } else if (rewindPP > MAX_REWIND) {
            rewindPP = MAX_REWIND;
        }
    }

    void PunishClimbPP() {
        climbPP -= CLIMB_PENALTY;
        if (climbPP < 0f) climbPP = 0f;
    }

    public float GetAbilityPP(SessionData.Ability ab) {
        switch (ab) {
            case SessionData.Ability.Boost:
                return boostPP / MAX_BOOST;
            case SessionData.Ability.Bounce:
                return bouncePP / MAX_BOUNCE;
            case SessionData.Ability.Climb:
                return climbPP / MAX_CLIMB;
            case SessionData.Ability.Rewind:
                return rewindPP / MAX_REWIND;
            default:
                return 0;
        }
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

    public void RewindTime(bool rewindOn){
        // No implementation
        // The ball should have all the properties of IMovableObject, but it should not wind.
    }

    public void PauseRB(bool pauseOn)
    {
        paused = pauseOn;
        holdAbButton = false;

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

    public void SetBallColor(BallColor color) {
        Material ballMat = ballColors[(int)color];

        gameObject.GetComponent<MeshRenderer>().material = ballMat;
        gameObject.GetComponent<TrailRenderer>().material = ballMat;
    }

    void OnCollisionStay(Collision collision) {

        if (!paused) {

            if (usingClimbSkill) {
                Vector3 pointOfContact = collision.contacts[0].point;
                Climb(pointOfContact, collision.gameObject, true);
                consumeClimbPPThisFrame = true;
            }else {
                if (climbing && !padClimb) EndClimb(true);
            }

        }
    }

    void OnCollisionExit(Collision collision) {

        if (!paused) {
            EndClimb();
        }
    }
}
