using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WiimoteApi;

public class CollisionAudio : MonoBehaviour {

	void Start(){
		
	}
	void OnCollisionEnter(){
		GetComponent<AudioSource> ().Play ();
	}
	void OnCollisionExit(){

	}
	// Update is called once per frame
	void Update () {

	}
}
