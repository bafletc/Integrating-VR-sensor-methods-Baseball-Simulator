using UnityEngine;
using System.Collections;
using trinus;

public class SampleTrinusUIOnly : MonoBehaviour
{
	TrinusUI trinusUI;
	Canvas canvas;
	TrinusProcessor trinusProcessor;

	void Awake(){
		trinusUI = GameObject.Find("TrinusUI").GetComponent<TrinusUI>();
		trinusUI.finishedSettingsEvent.AddListener(() => this.trinusEventSettingsFinished());//show the game pause settings UI when closing the Trinus settings UI
		trinusUI.exitEvent.AddListener(() => this.quit());//show the game pause settings UI when closing the Trinus settings UI
	}
	// Use this for initialization
	void Start ()
	{
		trinusUI.showQuitButton (true);
		trinusProcessor = GameObject.Find ("TrinusManager").GetComponent<TrinusProcessor> ();
		if (trinusProcessor == null || !trinusProcessor.isStreaming ()) {
			Time.timeScale = 0;
			trinusUI.openIntro ();
		} else
			trinusUI.openGame ();
		//Cursor.visible = false;		
		canvas = trinusUI.GetComponent<Canvas> ();
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (canvas != null && canvas.worldCamera == null) {
			TrinusUI.assignUICamera (canvas, trinusProcessor.getUICamera ());
		}

		if (Input.GetButtonDown ("Cancel")) {//in game mode, pause by pressing ESC
			//if (trinusUI.getCurrentPage() == TrinusUI.UI_PAGE.GAME)
			switch (trinusUI.getCurrentPage ()) {
			case TrinusUI.UI_PAGE.GAME:
				Time.timeScale = 0;
				trinusUI.openSettings ();
				break;
			case TrinusUI.UI_PAGE.SETTINGS:
				trinusEventSettingsFinished ();
				break;
			case TrinusUI.UI_PAGE.CONNECTION_WAIT:
				trinusUI.restartConnection ();
				break;
			}
		}
	}
	public void trinusEventSettingsFinished(){
		Time.timeScale = 1;
		trinusUI.openGame ();
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

}

