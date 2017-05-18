using UnityEngine;
using System.Collections;
using trinus;

public class SampleIntegratedUI : MonoBehaviour {
	enum STATE {
		INIT, GAME, PAUSE, TRINUS_SETTINGS
	}
	STATE state = STATE.INIT;

	Canvas canvas;
	TrinusProcessor trinusProcessor;
	public void Awake(){
		trinusProcessor = GameObject.Find ("TrinusManager").GetComponent<TrinusProcessor> ();
		GameObject tc = GameObject.Find ("TrinusUI");
		if (tc != null) {
			TrinusUI tui = tc.GetComponent<TrinusUI> ();
			//these can be set from the Editor, just putting them here for a quick set up
			tui.connectedEvent.AddListener(() => this.trinusEventConnected());//we can display the game UI once Trinus is ready
			tui.disconnectedEvent.AddListener(() => this.trinusEventDisconnected());//on disconnect, let's go back to main menu
			tui.finishedSettingsEvent.AddListener(() => this.trinusEventSettingsFinished());//show the game pause settings UI when closing the Trinus settings UI
		}
		canvas = GetComponent<Canvas> ();

		transform.Find ("InitialUI").gameObject.SetActive (true);
	}
	public void Update(){
		if (canvas.worldCamera == null) {
			TrinusUI.assignUICamera (canvas, trinusProcessor.getUICamera ());
			if (trinusProcessor.shouldScaleUI())
				transform.localScale = new Vector3 (transform.localScale.x / 2, transform.localScale.y, transform.localScale.z);

		}

		if (Input.GetButtonDown ("Cancel")) {//in game mode, pause by pressing ESC
			switch (state) {
			case STATE.GAME:
				pause (true);
				break;
			case STATE.PAUSE:
				pause (false);
				break;
			}
		}
		//when Trinus is running in VR mode, standard cursor won't work so we enable and update Trinus cursor
		if (state == STATE.PAUSE)
			TrinusUI.updateTrinusCursor ();
	}

	//done after initial game presentation/menus and ready to start playing
	public void beginTrinus(){
		transform.Find ("InitialUI").gameObject.SetActive (false);
		GameObject.Find ("TrinusUI").GetComponent<TrinusUI> ().openIntro ();
		//showGameUI ();
	}
	//option to cancel current established or attempted connection
	public void restartTrinus(){
		state = STATE.INIT;
		transform.Find ("PauseUI").gameObject.SetActive (false);
		GameObject.Find ("TrinusUI").GetComponent<TrinusUI> ().restartConnection ();
	}

	//your standard game pause
	public void pause(bool pause){
		if (pause) {
			Time.timeScale = 0;
			transform.Find ("PauseUI").gameObject.SetActive (true);
			state = STATE.PAUSE;
			Cursor.lockState = CursorLockMode.None;
		} else {
			TrinusUI.setTrinusCursor (false);
			Time.timeScale = 1;
			transform.Find ("PauseUI").gameObject.SetActive (false);
			state = STATE.GAME;
			Cursor.lockState = CursorLockMode.Confined;
		}
	}

	//withing our game pause settings we want to show the option to go to Trinus settings
	public void showTrinusSettings(){
		state = STATE.TRINUS_SETTINGS;
		transform.Find ("PauseUI").gameObject.SetActive (false);
		GameObject.Find ("TrinusUI").GetComponent<TrinusUI> ().openSettings ();
	}
	public void trinusEventSettingsFinished(){
		state = STATE.PAUSE;
		Debug.Log ("Trinus Settings finished");
		transform.Find ("PauseUI").gameObject.SetActive (true);
	}
	public void trinusEventConnected(){
		Debug.Log ("Trinus Event Connected");
		showGameUI ();
	}
	public void trinusEventDisconnected(){
		Debug.Log ("Trinus Event Disconnected");
		state = STATE.INIT;
		transform.Find ("InitialUI").gameObject.SetActive (false);
	}

	public void quit(){
		#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
		#else
		//Application.Quit();//this seems to crash badly, Unity bug?

		System.Diagnostics.Process.GetCurrentProcess().Kill();
		#endif
	}
	public void showGameUI(){
		state = STATE.GAME;
		transform.Find ("InGameUI").gameObject.SetActive (true);
	}

}
