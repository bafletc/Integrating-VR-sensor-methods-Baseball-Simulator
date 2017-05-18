using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Text;
using System;
using WiimoteApi;

public class WiimoteDemo : MonoBehaviour {
    /*
    public WiimoteModel model;

    private Quaternion initial_rotation;

    private Wiimote wiimote;

    private Vector2 scrollPosition;

    private Vector3 wmpOffset = Vector3.zero;

	private int pitched = 0;
    */
	public Vector3 spawnposition;
	public Transform ballprefab;
	public Vector3 pitchvelocity;
//	MotionPlusData data;
    
	void Start() {/*
        initial_rotation = model.rot.localRotation;
		WiimoteManager.FindWiimotes();
		wiimote = WiimoteManager.Wiimotes [0];
		wiimote.SendPlayerLED(true,false,false,false);
		wiimote.SendDataReportMode(InputDataType.REPORT_BUTTONS_ACCEL_EXT16);
		wiimote.RequestIdentifyWiiMotionPlus();
		wiimote.ActivateWiiMotionPlus();
		data = wiimote.MotionPlus;
        */
	}

	void Update () {
        /*
		if (!WiimoteManager.HasWiimote ()) {
			return;
		}

		wiimote = WiimoteManager.Wiimotes [0];

		if (pitched == 0 && wiimote.Button.a) {
				pitched = 1;
		} else if (pitched == 1 && !wiimote.Button.a) {
				Pitch ();
				pitched = 0;
		}

		if(wiimote.Button.one)
			wiimote.Accel.CalibrateAccel(0);

		if (wiimote.Button.two) {
			data.SetZeroValues ();
			model.rot.rotation = Quaternion.FromToRotation (model.rot.rotation * GetAccelVector (), Vector3.down) * model.rot.rotation;
			model.rot.rotation = Quaternion.FromToRotation (model.rot.forward, Vector3.forward) * model.rot.rotation;
		}

		if(wiimote.Button.one && wiimote.Button.two)
			wmpOffset = Vector3.zero;

        int ret;
        do
        {
            ret = wiimote.ReadWiimoteData();

            if (ret > 0 && wiimote.current_ext == ExtensionController.MOTIONPLUS) {
                Vector3 offset = new Vector3(  wiimote.MotionPlus.PitchSpeed,
                                                wiimote.MotionPlus.RollSpeed,
                                                wiimote.MotionPlus.YawSpeed) / 95f; // Divide by 95Hz (average updates per second from wiimote)
                wmpOffset += offset;

                model.rot.Rotate(offset, Space.Self);
            }
        } while (ret > 0);
			
        if (wiimote.current_ext != ExtensionController.MOTIONPLUS)
            model.rot.localRotation = initial_rotation;
            */
        if (Input.GetKeyUp(KeyCode.P))
        {
            Pitch();
        }




    }
		

        
	/*
	void OnGUI()
    {
        GUI.Box(new Rect(0,0,320,Screen.height), "");

        GUILayout.BeginVertical(GUILayout.Width(300));
        GUILayout.Label("Wiimote Found: " + WiimoteManager.HasWiimote());
        if (GUILayout.Button("Find Wiimote"))
            WiimoteManager.FindWiimotes();

        if (GUILayout.Button("Cleanup"))
        {
            WiimoteManager.Cleanup(wiimote);
            wiimote = null;
        }

        if (wiimote == null)
            return;

        GUILayout.Label("Extension: " + wiimote.current_ext.ToString());

        GUILayout.Label("LED Test:");
        GUILayout.BeginHorizontal();
        for (int x = 0; x < 4;x++ )
            if (GUILayout.Button(""+x, GUILayout.Width(300/4)))
                wiimote.SendPlayerLED(x == 0, x == 1, x == 2, x == 3);
        GUILayout.EndHorizontal();

        GUILayout.Label("Set Report:");
        GUILayout.BeginHorizontal();
        if(GUILayout.Button("But/Acc", GUILayout.Width(300/4)))
            wiimote.SendDataReportMode(InputDataType.REPORT_BUTTONS_ACCEL);
        if(GUILayout.Button("But/Ext8", GUILayout.Width(300/4)))
            wiimote.SendDataReportMode(InputDataType.REPORT_BUTTONS_EXT8);
        if(GUILayout.Button("B/A/Ext16", GUILayout.Width(300/4)))
            wiimote.SendDataReportMode(InputDataType.REPORT_BUTTONS_ACCEL_EXT16);
        if(GUILayout.Button("Ext21", GUILayout.Width(300/4)))
            wiimote.SendDataReportMode(InputDataType.REPORT_EXT21);
        GUILayout.EndHorizontal();

        if (GUILayout.Button("Request Status Report"))
            wiimote.SendStatusInfoRequest();

        GUILayout.Label("IR Setup Sequence:");
        GUILayout.BeginHorizontal();
        if(GUILayout.Button("Basic", GUILayout.Width(100)))
            wiimote.SetupIRCamera(IRDataType.BASIC);
        if(GUILayout.Button("Extended", GUILayout.Width(100)))
            wiimote.SetupIRCamera(IRDataType.EXTENDED);
        if(GUILayout.Button("Full", GUILayout.Width(100)))
            wiimote.SetupIRCamera(IRDataType.FULL);
        GUILayout.EndHorizontal();

        GUILayout.Label("WMP Attached: " + wiimote.wmp_attached);
        if (GUILayout.Button("Request Identify WMP"))
            wiimote.RequestIdentifyWiiMotionPlus();
        if ((wiimote.wmp_attached) && GUILayout.Button("Activate WMP"))
            wiimote.ActivateWiiMotionPlus();
		if (wiimote.current_ext == ExtensionController.MOTIONPLUS && GUILayout.Button("Deactivate WMP"))
			wiimote.DeactivateWiiMotionPlus ();

        GUILayout.Label("Calibrate Accelerometer");
        GUILayout.BeginHorizontal();
        for (int x = 0; x < 3; x++)
        {
            AccelCalibrationStep step = (AccelCalibrationStep)x;
            if (GUILayout.Button(step.ToString(), GUILayout.Width(100)))
                wiimote.Accel.CalibrateAccel(step);
        }
        GUILayout.EndHorizontal();

        if (GUILayout.Button("Print Calibration Data"))
        {
            StringBuilder str = new StringBuilder();
            for (int x = 0; x < 3; x++)
            {
                for (int y = 0; y < 3; y++)
                {
                    str.Append(wiimote.Accel.accel_calib[y, x]).Append(" ");
                }
                str.Append("\n");
            }
            Debug.Log(str.ToString());
        }

        if (wiimote != null && wiimote.current_ext != ExtensionController.NONE)
        {
            scrollPosition = GUILayout.BeginScrollView(scrollPosition);
            GUIStyle bold = new GUIStyle(GUI.skin.button);
            bold.fontStyle = FontStyle.Bold;
            if (wiimote.current_ext == ExtensionController.MOTIONPLUS)
            {
                GUILayout.Label("Wii Motion Plus:", bold);
                MotionPlusData data = wiimote.MotionPlus;
                GUILayout.Label("Pitch Speed: " + data.PitchSpeed);
                GUILayout.Label("Yaw Speed: " + data.YawSpeed);
                GUILayout.Label("Roll Speed: " + data.RollSpeed);
                GUILayout.Label("Pitch Slow: " + data.PitchSlow);
                GUILayout.Label("Yaw Slow: " + data.YawSlow);
                GUILayout.Label("Roll Slow: " + data.RollSlow);
                if (GUILayout.Button("Zero Out WMP"))
                {
                    data.SetZeroValues();
                    model.rot.rotation = Quaternion.FromToRotation(model.rot.rotation*GetAccelVector(), Vector3.down) * model.rot.rotation;
                    model.rot.rotation = Quaternion.FromToRotation(model.rot.forward, Vector3.forward) * model.rot.rotation;
                }
                if(GUILayout.Button("Reset Offset"))
                    wmpOffset = Vector3.zero;
                GUILayout.Label("Offset: " + wmpOffset.ToString());
            }
            GUILayout.EndScrollView();
        } else {
            scrollPosition = Vector2.zero;
        }
        GUILayout.EndVertical();
    }*/
    /*
    private Vector3 GetAccelVector()
    {
        float accel_x;
        float accel_y;
        float accel_z;

        float[] accel = wiimote.Accel.GetCalibratedAccelData();
        accel_x = accel[0];
        accel_y = -accel[2];
        accel_z = -accel[1];

        return new Vector3(accel_x, accel_y, accel_z).normalized;
    }

    [System.Serializable]
    public class WiimoteModel
    {
        public Transform rot;
    }


	void OnApplicationQuit() {
		if (wiimote != null) {
			WiimoteManager.Cleanup(wiimote);
	        wiimote = null;
		}
	}
    */
	void Pitch(){
		var ball = Instantiate(ballprefab,spawnposition, Quaternion.identity);
		//move ball
		ball.GetComponent<Rigidbody>().AddForce(pitchvelocity);
	}


}
