using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bat : MonoBehaviour {

    GameObject hand;
    public int x, y, z;
	// Use this for initialization
	void Start () {
        hand = GameObject.Find("female_02/PreBase/SpineBase/SpineMid/SpineShoulder/ShoulderRight/ElbowRight/WristRight/HandRight");
	}
	
	// Update is called once per frame
	void Update () {
        this.transform.parent = hand.transform;
        this.transform.localPosition = new Vector3(0.18f, 0.48f, -0.05f);
        this.transform.localRotation = Quaternion.identity;
        this.transform.localRotation = Quaternion.Euler(x, y, z);
	}
}
