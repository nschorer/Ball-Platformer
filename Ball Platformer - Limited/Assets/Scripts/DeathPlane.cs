using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathPlane : MonoBehaviour {

	void OnTriggerEnter (Collider collider){
		if (collider.tag == "Player") {
			PlayerController player = collider.GetComponent<PlayerController>();
			player.Die();
		}
	}
}
