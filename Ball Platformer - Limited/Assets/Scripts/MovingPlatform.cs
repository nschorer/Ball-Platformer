using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour, IMovingObject {

	public Vector3[] flightPath;
    public int startAtIndex = 0;
	public float speed;
	public float waitTime;
    public bool noLoop;

    enum Progress { AtStart, InProgress, AtEnd, Looping };

    private Transform tf;
	private int index;
	private bool isMoving;
	private float timeUntilMoving;
	private int numSegments;
	private Vector3 start;
	private Vector3 destination;
	private Vector3 currentTrajectory;
    private bool rewinding;
    private Progress flightProgress;
    private float waitTimer;
    private bool paused;

	void Start(){
		tf = gameObject.transform;
		if (speed == 0f) speed = 1f;
		//timeUntilMoving = waitTime;
		// By setting the index to -1, we can ensure that the moving platform starts off 'waiting',
		// but will also start moving based on the first instruction in the array.
		// This is the only time the index should be less than 0.
		numSegments = flightPath.GetLength(0);
        if (startAtIndex > numSegments - 1) startAtIndex = numSegments - 1;
        else if (startAtIndex < 0) startAtIndex = 0;
        index = startAtIndex - 1;

        rewinding = false;
        if (noLoop) flightProgress = Progress.AtStart;
        else flightProgress = Progress.Looping;
	}

	// FixedUpdate is called once per frame
	void FixedUpdate (){
        if (!paused)
        {
            if (flightProgress == Progress.Looping ||
                !rewinding && flightProgress != Progress.AtEnd ||
                rewinding && flightProgress != Progress.AtStart)
            {

                MovePlatform();
            }
        }
	}

    void MovePlatform(){
        // Figure out when it should be moving/waiting
        if (numSegments > 0)
        {
            if (isMoving)
            {
                if (flightProgress == Progress.AtStart ||
                    flightProgress == Progress.AtEnd)
                {

                    flightProgress = Progress.InProgress;
                }

                tf.position += (currentTrajectory * speed * Time.deltaTime);
                if (HasReachedDestination())
                {
                    SwitchState(false);
                }
            }
            else
            {
                if (!rewinding){
                    waitTimer += Time.deltaTime;
                    if (waitTimer >= waitTime) SwitchState(true);
                }else{
                    waitTimer -= Time.deltaTime;
                    if (waitTimer <= 0f) SwitchState(true);
                }
            }
        }
    }

	// Switch between moving and waiting.
	void SwitchState (bool nowMoving){
		// Start moving to calculated destination
		if (nowMoving) {

            // If we've reached the end of the flight instructions, do we stop or start over?
            if (!rewinding){
                index++;
                // We should ONLY hit this segment if noLoop = false
                if (index >= numSegments)
                {
                    index = 0;
                    if (noLoop) Debug.LogError("Index above upper limit on noLoop MovingPlatform");
                }

                // Determine where we started and where we are going.
                start = tf.position;
                destination = start + flightPath[index];
                currentTrajectory = flightPath[index].normalized;

            }
            else{
                if (index <= -1)
                {
                    if (noLoop)
                    {
                        flightProgress = Progress.AtStart;
                        index = -1;
                        return;
                    }
                    else
                    {
                        index = numSegments-1;
                    }
                }

                // Determine where we started and where we are going.
                start = tf.position;
                destination = start - flightPath[index];
                currentTrajectory = -flightPath[index].normalized;
            }

            isMoving = true;

		// Stop moving and wait
		} else {

            // NOTE: we end up setting this to true if this is the last leg in a noLoop flight
            isMoving = false;

            if (!rewinding)
            {
                waitTimer = 0f;
                if (noLoop && (index == numSegments - 1))
                {
                    // The platform starts in a the 'waiting' state and ends in the 'moving' state
                    flightProgress = Progress.AtEnd;
                    isMoving = true;
                }
            }
            else
            {
                waitTimer = waitTime;
                index--;
            }
		}
	}

	// Check if the platform is at the destination segment OR if it has passed it.
	bool HasReachedDestination (){
		// If the platform arrived, or if it overshot
		if (IsPlatAtDestination ()) {
			tf.position = destination;
			return true;
		} else if (DidPlatPassDestination ()) {
			tf.position = destination;
			return true;
		}

		return false;
	}

	bool IsPlatAtDestination(){
		return Vector3.Distance(tf.position, destination) <= 0.1;
	}

	// This assumes that the platform is traveling on a straight line between the two points
	bool DidPlatPassDestination (){
		float startX, startY, startZ;
		float destX, destY, destZ;
		float curX, curY, curZ;

		startX = start.x;
		startY = start.y;
		startZ = start.z;

		destX = destination.x;
		destY = destination.y;
		destZ = destination.z;

		curX = tf.position.x;
		curY = tf.position.y;
		curZ = tf.position.z;

		// Overpassed on the x-axis
		if ((startX >= destX) && (curX < destX)) return true;
		else if ((destX >= startX) && (curX > destX)) return true;

		// Overpassed on the y-axis
		if ((startY >= destY) && (curY < destY)) return true;
		else if ((destY >= startY) && (curY > destY)) return true;

		// Overpassed on the z-axis
		if ((startZ >= destZ) && (curZ < destZ)) return true;
		else if ((destZ >= startZ) && (curZ > destZ)) return true;

		return false;
	}

    private void ReverseCourse(){
        currentTrajectory = -currentTrajectory;
        Vector3 temp, temp2;

        temp = destination;
        temp2 = start;
        destination = temp2;
        start = temp;
    }

    public void Rewind(){
        ReverseCourse();
        rewinding = true;
    }

    public void Unrewind(){
        ReverseCourse();
        rewinding = false;
    }

    public void PauseRB(bool pauseOn){
        paused = pauseOn;
        RigidbodyConstraints rbConstraints = pauseOn ? RigidbodyConstraints.FreezeAll : RigidbodyConstraints.None;
        transform.GetComponent<Rigidbody>().constraints = rbConstraints;
    }
}
