using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using trinus;

public class SampleNoUI : MonoBehaviour {

	[Tooltip("If not set, Trinus will attempt to autodetect client")]
	public string IPAddress;
	[Tooltip("Yaw rotation ratio, useful while sitting to set 90 degree real world turn into 180 (yawScale = 2)")]
	public int yawScale = 1;
	[Tooltip("Connection port, should be the same as set on Trinus phone. Usually no need to change unless there is some network restriction/other activity on the port")]
	public int videoPort = 7777;
	[Tooltip("Sensor data port. Usually no need to change unless there is some network restriction/other activity on the port")]
	public int sensorPort = 5555;

	TrinusProcessor trinusProcessor;
	Text trinusMessage;
	Canvas canvas;
	// Use this for initialization
	void Awake () {
		trinusProcessor = GameObject.Find ("TrinusManager").GetComponent<TrinusProcessor> ();
		trinusMessage = transform.FindChild ("TrinusMessage").GetComponent<Text> ();

		GameObject trinusUI = GameObject.Find ("TrinusUI");
		if (trinusUI != null)
			trinusUI.SetActive (false);
	
		TrinusProcessor.setMessageCallbacks (setMessage, setHint, setHint);

		canvas = GetComponent<Canvas> ();
	}

	void Start(){
		TrinusProcessor.UserSettings settings = TrinusProcessor.getUserSettings ();

		settings.forcedIP = IPAddress;
		settings.yawScale = yawScale;
		settings.videoPort = videoPort;
		settings.sensorPort = sensorPort;
		IEnumerator<string> e = TrinusProcessor.LensParams.getPresetNames ().GetEnumerator ();
		e.MoveNext ();
		settings.lensParams.selectPreset (e.Current);
		trinusProcessor.applyLensParams ();

		startConnection ();

	}
	// Update is called once per frame
	void Update () {
		if (canvas.worldCamera == null) {
			TrinusUI.assignUICamera (canvas, trinusProcessor.getUICamera ());
		}
		if (Input.GetButtonDown ("Cancel")) {//in game mode, pause by pressing ESC
			quit();
		}
	}
	public void startConnection(){
		trinusProcessor.resetDisconnectStatus ();
		TrinusProcessor.trinusPause (false);
	}

	public void quit(){
		#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
		#else
		//Application.Quit();//this seems to crash badly, Unity bug?

		trinusProcessor.endStreaming();
		System.Diagnostics.Process.GetCurrentProcess().Kill();
		#endif
	}

	public void setMessage(){
		setMessage (null, null);
	}
	public void setMessage(string textId){
		setMessage (textId, null);
	}
	public void setMessage(string textId, string extra){
		if (textId == null)
			trinusMessage.gameObject.SetActive (false);
		else {
			trinusMessage.gameObject.SetActive (true);
			trinusMessage.text = string.Format(Localization.getText(textId), extra);
		}
	}
	public void setHint(){
		setHint (null, null);
	}
	public void setHint(string textId){
		setHint (textId, null);
	}
	public void setHint(string textId, string extras){
//		if (textId != null)
//			Debug.Log(string.Format(Localization.getText(textId), extras));
	}
}
