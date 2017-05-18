//#define USE_QUATERNION
//USE_QUATERNION gives better precision, but is not available for all phones
#define USE_SYS_DRAW
//USE_SYS_DRAW provides faster streaming but uses System.Drawing.dll, it will require additional libs to use it in Mac/Linux (http://answers.unity3d.com/questions/53170/using-drawing-package-like-systemdrawing.html)

using System.IO;
using UnityEngine.UI;
using UnityEngine;
using System.Collections;
using corelib;
using corelib.util;
using System.Collections.Generic;
using System;
using System.Threading;
using UnityEngine.Events;

namespace trinus{
	/// <summary>
	/// Base class to manage Trinus connectivity and delivery.
	/// Uses TrinusNotificationManager component to send messages to UI
	/// </summary>
	public class TrinusProcessor : MonoBehaviour {

		public delegate void mainMsgCallback(String msg, String extra);
		public delegate void errorMsgCallback(String msg);
		public delegate void infoMsgCallback(String msg);
		
		static mainMsgCallback mmCallback = null;
		static errorMsgCallback emCallback = null;
		static infoMsgCallback imCallback = null;

		#region datastructs
		public class LensParams
		{
			public const string CUSTOM_PRESET = "Custom Preset";
			public float screenx  = 0.5f;
			public float screeny  = 0.5f;
			public float lensx  = 0.5f;
			public float lensy  = 0.5f;
			public float scalex  = 0.7f;
			public float scaley  = 0.7f;
			public float warpx  = 1.0f;
			public float warpy  = 0.5f;
			public float warpz  = 0.1f;
			public float warpw  = 0.5f;
			public float chroma  = 0f;

			public string currentPreset = null;//null = custom
			public static Dictionary<string, LensParams> presets = null;

			public void loadPreset(){
				//Debug.Log ("LOAD PRESET " + PlayerPrefs.GetString ("preset", "custom_preset"));
				selectPreset (PlayerPrefs.GetString ("preset", CUSTOM_PRESET));
			}
			static void loadAllPresets(){
				presets = new Dictionary<string, LensParams> ();

				TextAsset localStringsAsset = (TextAsset)Resources.Load ("lensPresets");

				if (localStringsAsset != null && !string.IsNullOrEmpty (localStringsAsset.text)) {
					System.IO.StringReader textStream = new System.IO.StringReader (localStringsAsset.text);
					string line = null;
					while ((line = textStream.ReadLine()) != null) {
						int eqIndex = line.IndexOf ("=");
						if (eqIndex > 0) {
							string presetName = line.Substring (0, eqIndex);
							LensParams lp = new LensParams();
							string[] vals = line.Substring (eqIndex + 1, line.Length - eqIndex - 1).Split(',');
							if (vals.Length != 11)
								Debug.LogWarning("Invalid number of values for preset " + presetName);
							else{
								lp.screenx = float.Parse(vals[0]);
								lp.screeny = float.Parse(vals[1]);
								lp.lensx = float.Parse(vals[2]);
								lp.lensy = float.Parse(vals[3]);
								lp.scalex = float.Parse(vals[4]);
								lp.scaley = float.Parse(vals[5]);
								lp.warpx = float.Parse(vals[6]);
								lp.warpy = float.Parse(vals[7]);
								lp.warpz = float.Parse(vals[8]);
								lp.warpw = float.Parse(vals[9]);
								lp.chroma = float.Parse(vals[10]);

								if (presets.ContainsKey(presetName))
									presets.Remove(presetName);
								presets.Add(presetName, lp);
							}
						}
					}
					
					textStream.Close ();
				}

				//selectPreset (currentPreset);
			}
			public static ICollection<string> getPresetNames(){
				if (presets == null)
					loadAllPresets ();
				return presets.Keys;
			}
			public void selectPreset(string preset){
				currentPreset = preset;
				if (presets == null)
					loadAllPresets ();
				if (CUSTOM_PRESET.Equals (currentPreset)) {
					//Debug.Log ("Setting custom lens preset");
					screenx = PlayerPrefs.GetFloat ("screenX", screenx);
					screeny = PlayerPrefs.GetFloat ("screenY", screeny);
					lensx = PlayerPrefs.GetFloat ("lensX", lensx);
					lensy = PlayerPrefs.GetFloat ("lensY", lensy);
					scalex = PlayerPrefs.GetFloat ("scalex", scalex);
					scaley = PlayerPrefs.GetFloat ("scaley", scaley);
					warpx = PlayerPrefs.GetFloat ("warpx", warpx);
					warpy = PlayerPrefs.GetFloat ("warpy", warpy);
					warpz = PlayerPrefs.GetFloat ("warpz", warpz);
					warpw = PlayerPrefs.GetFloat ("warpw", warpw);
					chroma = PlayerPrefs.GetFloat ("chroma", chroma);
				} else {
					if (presets.ContainsKey(preset)){
					
						LensParams lp = presets[preset];
                        Debug.Log("Setting lens preset: " + lp.warpx+" " + lp.warpy+" " + lp.warpz);
                        screenx = lp.screenx;
						screeny = lp.screeny;
						lensx = lp.lensx;
						lensy = lp.lensy;
						scalex = lp.scalex;
						scaley = lp.scaley;
						warpx = lp.warpx;
						warpy = lp.warpy;
						warpz = lp.warpz;
						warpw = lp.warpw;
						chroma = lp.chroma;
					}
				}
			}

			public void savePreset()
			{
				PlayerPrefs.SetString ("preset", currentPreset);
				//Debug.Log (currentPreset + " SAVE " + PlayerPrefs.GetString ("preset"));
				if (CUSTOM_PRESET.Equals (currentPreset)) {
					PlayerPrefs.SetFloat ("screenX", screenx);
					PlayerPrefs.SetFloat ("screenY", screeny);
					PlayerPrefs.SetFloat ("lensX", lensx);
					PlayerPrefs.SetFloat ("lensY", lensy);
					PlayerPrefs.SetFloat ("scalex", scalex);
					PlayerPrefs.SetFloat ("scaley", scaley);
					PlayerPrefs.SetFloat ("warpx", warpx);
					PlayerPrefs.SetFloat ("warpy", warpy);
					PlayerPrefs.SetFloat ("warpz", warpz);
					PlayerPrefs.SetFloat ("warpw", warpw);
					PlayerPrefs.SetFloat ("chroma", chroma);
				}
			}

			public LensParams(){
			}
			public LensParams(float screenX, float screenY, float lensX, float lensY, float scalex, float scaley, float warpx, float warpy, float warpz, float warpw, float chroma)
			{
				this.screenx = screenX;
				this.screeny = screenY;
				this.lensx = lensX;
				this.lensy = lensY;
				this.scalex = scalex;
				this.scaley = scaley;
				this.warpx = warpx;
				this.warpy = warpy;
				this.warpz = warpz;
				this.warpw = warpw;
				this.chroma = chroma;
			}

			public override String ToString()
			{
				return currentPreset + ": " + screenx + "," + screeny + "," + lensx + "," + lensy + "," + scalex + "," + scaley + "," + warpx + "," + warpy + "," + warpz + "," + warpw + "," + chroma;
			}
		}

		public class UserSettings{
			public bool motionBoost = true;
			public bool disableLens = false;
			public bool showFps = false;
			public int videoPort = 7777;
			public int sensorPort = 5555;
			public String forcedIP = null;
			public int yawScale = 2;
			public int compressionQuality = 75;
			public float ipd = 0.5f;
			public LensParams lensParams = new LensParams();

			public static UserSettings load(){
				UserSettings result = new UserSettings ();
				result.motionBoost = PlayerPrefs.GetInt ("motionBoost", result.motionBoost? 1 : 0) == 1;
				result.disableLens = PlayerPrefs.GetInt ("disableLens", result.disableLens? 1 : 0) == 1;
				result.showFps = PlayerPrefs.GetInt ("showFps", result.showFps? 1 : 0) == 1;
				result.videoPort = PlayerPrefs.GetInt ("videoPort", result.videoPort);
				result.sensorPort = PlayerPrefs.GetInt ("sensorPort", result.sensorPort);
				result.yawScale = PlayerPrefs.GetInt ("yawScale", result.yawScale);
				result.compressionQuality = PlayerPrefs.GetInt ("compressionQuality", result.compressionQuality);
				result.ipd = PlayerPrefs.GetFloat ("ipd", result.ipd);
				result.lensParams.loadPreset ();
				return result;
			}
			public static void save(UserSettings settings){
				PlayerPrefs.SetInt ("motionBoost", settings.motionBoost? 1 : 0);
				PlayerPrefs.SetInt ("disableLens", settings.disableLens? 1 : 0);
				PlayerPrefs.SetInt ("showFps", settings.showFps? 1 : 0);
				PlayerPrefs.SetInt ("videoPort", settings.videoPort);
				PlayerPrefs.SetInt ("sensorPort", settings.sensorPort);
				PlayerPrefs.SetInt ("yawScale", settings.yawScale);
				PlayerPrefs.SetInt ("compressionQuality", settings.compressionQuality);
				PlayerPrefs.SetFloat ("ipd", settings.ipd);
				settings.lensParams.savePreset ();
			}
		}
		#endregion
		const int REVIEW_TIME = 3;
		
		Manager trinusManager;
		static bool alreadyExists = false;
		
		byte[] bytes = null;
		Texture2D output = null;
		
		//float yawOffset;
//		bool resetViewFlag = false;
//		Vector3 offset;
		//float pitchOffset = 90;
		bool ignoreTracking = false;
		bool lastTriggerState = false;

		Rect screenRect = new Rect();
		
		WaitForEndOfFrame endOfFrame = new WaitForEndOfFrame();
		#if USE_SYS_DRAW
		System.Drawing.Bitmap bitmap = null;
		#endif
		System.Drawing.Rectangle bitsRect;
		//Transform playerHead;

		//int targetWidth;
		int targetHeight;

		float lastReviewTime;
		float displayScale = 1;
		//float compressionQuality = 0.9f;

		static bool paused = true;

		private static UserSettings settingsInstance;

		Quaternion cameraRotation = new Quaternion();
		Vector3 acceleration = new Vector3();

		public delegate void setSensorData(Quaternion rotation, Vector3 acceleration, bool trigger);
		public setSensorData sensorDataDelegate;

		[Tooltip("Invoke method when VR Trigger is pressed")]
		public UnityEvent triggerPressEvent;
		[Tooltip("Invoke method when VR Trigger is released (a common use case is to recenter the view)")]
		public UnityEvent triggerReleaseEvent;

	//	[Tooltip("Name of the Trinus Head object to receive the head tracking (contains player cameras)")]
	//	public String trinusHeadObjectName = "TrinusHead";
		[Tooltip("Prevent Trinus object from being destroyed on reload, avoiding reconnection. Trinus will reattach to the loaded Trinus Head")]
		public bool preserveGameObject = true;
	//	[Tooltip("Target frame rate to automatically adjust quality (set to 0 disable auto-adjust). Note that Motion boost increases frame rate when movement is detected (ie. improved speed when it is most needed)")]
	//	public int minFrameRate = 42;
	//	[Tooltip("Trinus will autodetect the client IP, but can be manually set")]
	//	public string forcedIp = null;
		[Tooltip("Restricting to windowed mode, which gives better control on quality (and performs slightly faster)")]
		public bool allowFullscreen = false;
        
		[Tooltip("This will set time scale to 0 until Trinus connection is established")]
		public bool pauseWhileConnecting = true;
		
		[Tooltip("Option to create Trinus hotspot for the phone to connect directly (i.e. no router). Won't work for all PCs and requires Windows + Admin rights")]
		public bool createHotspot = false;

//		[Tooltip("Have the headtracking reset option only affect the yaw (ignore pitch/roll position)")]
//		public bool resetYawOnly = true;

		[Tooltip("Reset head tracking to make current phone position the default, looking straight")]
		public KeyCode resetTrackingKey = KeyCode.R;
		[Tooltip("Enable/Disable head tracking")]
		public KeyCode ignoreViewKey = KeyCode.I;

		[Tooltip("Camera for Trinus head tracking, if not set TrinusProcessor will look for a 'TrinusCamera' GameObject. You can switch active camera with switchCamera()")]
		public TrinusCamera trinusCamera;

		[Tooltip("Trinus will activate head tracking for the currently active camera")]
		public bool autoSwitchCamera = true;

		[Tooltip("It is recommended to disable vsync for best performance (vsync is not relevant for VR streaming)")]
		public bool disableVsync = true;

		[Tooltip("You can develop and test your project without an Id, but you will need a Project Id when you release your project to the public. Email info@trinusvr.com to obtain one")]
		public string projectId;

	#region Unity implementation to manage connection and stream data
		/// <summary>
		/// Trinus lib is set to establish connection with client
		/// </summary>
		void Awake () {
			if (disableVsync) {
				QualitySettings.vSyncCount = 0;
				//Application.targetFrameRate = 65;
			}

			if (alreadyExists) {
				Destroy (this.gameObject);
				return;
			}
			if (!allowFullscreen && Screen.fullScreen)
				Screen.SetResolution (Screen.width, Screen.height, false);

			alreadyExists = true;
			if (preserveGameObject) {
				DontDestroyOnLoad (this.gameObject);
			}

			#if DEBUG
			Debug.Log ("Trinus is " + (paused? "paused" : "unpaused"));
			#endif
			if (trinusManager == null) {
				trinusManager = new Manager (projectId);
				if (!trinusManager.isIdValid())
					Debug.LogError ("TEST MODE ON: No Valid Project Id set in TrinusManager. Email project details to info@trinusvr.com to obtain one.");
			} else {
				return;
			}
           // Debug.Log(Screen.height + "+++++" + Screen.width);
            #if UNITY_STANDALONE_WIN
            if (createHotspot)//requires exec with admin rights!
			{
				try{
					String path = Application.dataPath;

					System.Diagnostics.Process hotspotTool = new System.Diagnostics.Process();
					hotspotTool.StartInfo.FileName = path + "/hotspotTool.exe";
                    Debug.Log(path);
					hotspotTool.Start();
				}catch(Exception e){
					Debug.Log(e);
				}
			}
			#endif

			getUserSettings ();//init settings

			findCamera ();
		}

		public static UserSettings getUserSettings(){
			if (settingsInstance == null)
				settingsInstance = UserSettings.load ();
			return settingsInstance;
		}

		public static void trinusPause(bool p){
			paused = p;
		}
		public void resetDisconnectStatus(){
			trinusManager.resetDisconnection ();
		}

		public static void setMessageCallbacks(mainMsgCallback mmc, errorMsgCallback emc, infoMsgCallback imc){
			mmCallback = mmc;
			emCallback = emc;
			imCallback = imc;
		}
		private void setMessage(String msg){
			setMessage (msg, null);
		}
		private void setMessage(String msg, String extra){
			if (mmCallback != null)
				mmCallback (msg, extra);
			else
				Debug.Log ("M: " + msg);
		}
		private void setError(String msg){
			if (emCallback != null)
				emCallback (msg);
			else
				Debug.Log ("E: " + msg);
		}
		private void setInfo(String msg){
			if (imCallback != null)
				imCallback (msg);
			else
				Debug.Log ("I: " + msg);
		}
		private void clearMessages(){
			setMessage (null);
			setError (null);
			setInfo (null);
		}

		public int getFov(){
			return trinusCamera.getFov ();
	//		if (leftCamera != null)
	//			return (int)leftCamera.fieldOfView;
	//		return PlayerPrefs.GetInt ("fov", 100);
		}
		public void setFov(int f){
			trinusCamera.setFov (f);
	//		if (leftCamera != null && rightCamera != null) {
	//			leftCamera.fieldOfView = f;
	//			rightCamera.fieldOfView = f;
	//			PlayerPrefs.SetInt ("fov", f);
	//		}
		}

		public DataStructs.STATUS getStatus(){
			if (trinusManager != null)
				return trinusManager.getStatus ();
			return DataStructs.STATUS.IDLE;
		}
		private TrinusCamera findCamera(){
			TrinusCamera foundCam = null;
			GameObject cam = GameObject.Find ("TrinusCamera");
			if (cam != null && cam.GetComponent<TrinusCamera> () != null)
				foundCam = cam.GetComponent<TrinusCamera> ();
			else {
				//search for the TrinusCamera script attached to a differently named object
				TrinusCamera[] cams = GameObject.FindObjectsOfType<TrinusCamera> ();
				if (cams.Length > 0)
					foundCam = cams [0];
				else
					Debug.LogWarning ("No TrinusCamera found for TrinusManager!");
			}
			if (foundCam != null) {
				decideResolution (PlayerPrefs.GetInt ("resolution", 720), getMonoscopic ());
				//yawOffset = trinusCamera.defaultViewAngle;
				switchCamera(foundCam);
				foundCam.setCameraEnabledDelegate (switchCamera);
			}
			return foundCam;
		}
		/// <summary>
		/// Taking care of keypresses, connection process and frame preparation
		/// </summary>
		void Update ()
		{	
			if (trinusCamera == null) {
				findCamera();
//				GameObject cam = GameObject.Find ("TrinusCamera");
//				if (cam != null && cam.GetComponent<TrinusCamera> () != null)
//					foundCam = cam.GetComponent<TrinusCamera> ();
//				else {
//					//search for the TrinusCamera script attached to a differently named object
//					TrinusCamera[] cams = GameObject.FindObjectsOfType<TrinusCamera> ();
//					if (cams.Length > 0)
//						foundCam = cams [0];
//					else
//						Debug.LogWarning ("No TrinusCamera found for TrinusManager!");
//				}

//				if (foundCam != null) {
//					decideResolution (PlayerPrefs.GetInt ("resolution", 720), getMonoscopic ());
//					//yawOffset = trinusCamera.defaultViewAngle;
//					switchCamera(foundCam);
//					foundCam.setCameraEnabledDelegate (switchCamera);
//				}
			}

			if (paused)
				return;

			if (Input.GetKeyUp (resetTrackingKey)) {
				resetTracking();
			}
			if (Input.GetKeyUp (ignoreViewKey)) {
				disableHeadTracking(!ignoreTracking);
			}

			#if DEBUG
			if (Input.GetKeyUp(KeyCode.L)){
				Debug.Log("Status:"+trinusManager.getStatus());
				Debug.Log ("Last result:" + trinusManager.getLastResult());
				Debug.Log ("Internal Log:" + trinusManager.getSimpleLog());
			}
			#endif
			switch (trinusManager.getStatus ()) {
			case DataStructs.STATUS.IDLE:
				#if DEBUG
				DataStructs.RESULT err = trinusManager.getLastResult ();
				if (err.code == DataStructs.RESULT.CODE.ERROR) {
					Debug.Log ("Error detected: " + err);
					Debug.Log (trinusManager.getSimpleLog ());
					setError (err.ToString ());
				}
				//Debug.Log("SIMPLE LOG: " + trinusManager.getSimpleLog());
				#endif

				DataStructs.RESULT r = connect();
				if (r.subCode == DataStructs.RESULT.SUBCODE.SUCCESS){
					setMessage("msgTrinusWait", settingsInstance.forcedIP == null? "(Auto)" : "("+settingsInstance.forcedIP+")");
				}
				else{
					Debug.LogError(r.ToString());
					Debug.LogError (trinusManager.getSimpleLog());
					setMessage("msgFailedConn");
					setError(r.ToString());
				}
				#if DEBUG
				if (r.subCode == DataStructs.RESULT.SUBCODE.SUCCESS)
					Debug.Log("Waiting for connection");
				else
					Debug.Log ("Unable to initialize connection " + r);
				#endif
				break;
			case DataStructs.STATUS.DISCONNECTED:
				DataStructs.RESULT disconnectResult = trinusManager.getLastResult ();
				Debug.Log ("Disconnected (Reason: " + disconnectResult + ")");
				if (disconnectResult.code == DataStructs.RESULT.CODE.ERROR){// && disconnectResult.subCode == DataStructs.RESULT.SUBCODE.ERR_ALREADY_USE) {
					setMessage("msgFailedConn");
					setError (disconnectResult.detail);
				}
//				else
					trinusManager.resetDisconnection ();
				break;
			case DataStructs.STATUS.CONNECTING:
				setMessage ("msgTrinusWait", (string.IsNullOrEmpty(trinusManager.getLastResult ().detail)? "" : (settingsInstance.forcedIP != null ? "IP " + settingsInstance.forcedIP : "") +
					"Port " + settingsInstance.videoPort + " STEP " + trinusManager.getLastResult ().detail));
				if (pauseWhileConnecting && Time.timeScale > 0) {
					Time.timeScale = 0;
				}
				break;
			case DataStructs.STATUS.CONNECTED:
				//setMessage("msgTrinusWait", "CB_" + trinusManager.getLastResult().detail);
				#if DEBUG
				Debug.Log ("Connected, starting streaming " + trinusManager.getLastResult ().detail);
				#endif
				clearMessages ();

				lastReviewTime = Time.realtimeSinceStartup;

				DataStructs.DEVICE deviceInfo = trinusManager.getDeviceInfo ();

				//targetWidth = Math.Min (deviceInfo.width, Screen.width);
				targetHeight = Math.Min (deviceInfo.height, Screen.height);
                  

                //screen resolution should be equal or lower than device resolution
                //use resolution to balance quality vs performance
                //note: fullscreen mode will have resolution options restricted
                decideResolution (PlayerPrefs.GetInt("resolution", Math.Min(720, targetHeight)), trinusCamera.getMode () == TrinusCamera.CAMERA_MODE.SINGLE);
                   // Debug.Log(Screen.height + "+++++" + Screen.width+ (trinusCamera.getMode() == TrinusCamera.CAMERA_MODE.SINGLE? 0:1));
                    trinusManager.startStreaming ();

				//resolution info is only sent to device while streaming...
				resolutionChange (Screen.width, Screen.height);

				applyLensParams ();

				StartCoroutine (prepareImage ());

				if (pauseWhileConnecting)
					Time.timeScale = 1;

				//trinusCamera.enable ();

				break;
				
			case DataStructs.STATUS.STREAMING:
				if (settingsInstance.showFps && lastReviewTime + REVIEW_TIME < Time.realtimeSinceStartup) {
					DataStructs.RESULT result = trinusManager.getLastResult ();
					if (result.detail != null)
						setInfo (result.detail);
					lastReviewTime = Time.realtimeSinceStartup;
				}

				DataStructs.SENSORS sensorData = trinusManager.getSensorData ();

				if (!lastTriggerState && sensorData.trigger && triggerPressEvent != null)
					triggerPressEvent.Invoke ();
				if (lastTriggerState && !sensorData.trigger && triggerReleaseEvent != null)
					triggerReleaseEvent.Invoke ();
					
				lastTriggerState = sensorData.trigger;
				#if !USE_QUATERNION
				if (!ignoreTracking && trinusCamera != null)
					trinusCamera.transform.localEulerAngles = new Vector3 (sensorData.pitch, sensorData.yaw * settingsInstance.yawScale, -sensorData.roll);
				#else
				cameraRotation.Set (sensorData.quatX, sensorData.quatY, sensorData.quatZ, sensorData.quatW);// * Quaternion.Inverse(test);
				if (!ignoreTracking && trinusCamera != null) {
					trinusCamera.transform.localRotation = cameraRotation;
				}
				#endif
				acceleration.Set (sensorData.accelX, sensorData.accelY, sensorData.accelZ);

				if (sensorDataDelegate != null) {
					sensorDataDelegate (cameraRotation, acceleration, sensorData.trigger);
				}
				break;
			}
		}
		/// <summary>
		///Used to set the view to the current yaw and pitch angle (i.e. wherever user is looking now it will be the main/start position)
		/// </summary>
		public void resetTracking(){
			if (trinusManager != null)
				trinusManager.resetSensor ();
		}

		void LateUpdate()
		{
			if (trinusManager != null && trinusManager.getStatus () == DataStructs.STATUS.STREAMING) {
				if (screenRect.width != Screen.width || screenRect.height != Screen.height) {
					resolutionChange (Screen.width, Screen.height);
				} 
			}
		}
		public static string validateIp(string ip){
			if (ip == null || ip.Trim ().Equals (""))
				return null;

			if (!ip.Contains ("."))
				return null;
			
			System.Net.IPAddress address;
			if (!System.Net.IPAddress.TryParse (ip, out address)) {
				return null;
			}
			return address.ToString();
		}
		public static int validatePort(string port){
			int vport = -1;
			if (port == null || port.Trim ().Equals (""))
				return vport;

			try{
				vport = int.Parse (port);
			}catch{
			}
			if (vport < 1025 || vport > 65535)
				return -1;
			
			return vport;
		}
		private DataStructs.RESULT connect(){
			DataStructs.SETTINGS settings = DataStructs.SETTINGS.defaultSettings();
			settings.fake3DMode = DataStructs.Fake3DMode.DISABLED;
			settings.videoPort = settingsInstance.videoPort;
			settings.sensorPort = settingsInstance.sensorPort;
			settings.motionBoost = settingsInstance.motionBoost;
			settings.clientNoLens = settingsInstance.disableLens;
			settings.ipAddress = validateIp(settingsInstance.forcedIP);
	#if USE_SYS_DRAW
			settings.convertImage = true;
	#endif
			trinusManager.setSettings(settings);

			DataStructs.RESULT r = trinusManager.connectAsync (3);

			return r;
		}


		IEnumerator prepareImage() {
			while (trinusManager.getStatus() == DataStructs.STATUS.STREAMING) {
				yield return endOfFrame;
				//Debug.Log ("Frame ready " + trinusManager.isReadyForFrame ());
				lock(this)
					if (output != null && trinusManager.isReadyForFrame()) {
					//						RenderTexture tex = GameObject.Find("TrinusCameraTest/SplitCamera").GetComponent<Camera>().targetTexture;
//						if (tex == null) {
//							tex = new RenderTexture (bitmap.Width, bitmap.Height, 24);
//							GameObject.Find ("TrinusCameraTest/SplitCamera").GetComponent<Camera> ().targetTexture = tex;
//						}
//						RenderTexture.active = tex;
					output.ReadPixels (screenRect, 0, 0, false);
					#if USE_SYS_DRAW
					bytes = output.GetRawTextureData();

					trinusManager.sendNextFrame(bytes, bitsRect.Width , bitsRect.Height, settingsInstance.compressionQuality);

					#else
						bytes = output.EncodeToJPG (settingsInstance.compressionQuality);
						trinusManager.sendNextFrame(bytes);
					#endif
				}
			}
			yield return null;
		}
			
		public void OnDestroy()
		{
			endStreaming ();
			UserSettings.save (getUserSettings());
		}
		public void endStreaming(){
			lock(this)
			{
				if (output != null)
					Destroy (output);
				Destroy (output);
				output = null;
				if (trinusManager != null) {
					#if DEBUG
					Debug.Log ("Trinus END");
					#endif
					trinusManager.disconnect ();
					//Debug.Log (trinusManager.getSimpleLog());
					if (createHotspot)
						trinusManager.setHotspot(false, null, null);
				}
			}
		}

	#endregion
		//<summary>
		//This method should be called whenever there's a resolution change (and trinus is streaming)
		//</summary>
		public void resolutionChange(int width, int height)
		{
			//resolutionChange (width, height, DataStructs.Fake3DMode.DISABLED);
			resolutionChange (width, height, (width > height && trinusCamera.getMode() != TrinusCamera.CAMERA_MODE.SINGLE? DataStructs.Fake3DMode.DISABLED : DataStructs.Fake3DMode.ENABLED));
		}
		//<summary>
		//This method should be called whenever there's a resolution change (and trinus is streaming)
		//</summary>
		public void resolutionChange(int width, int height, DataStructs.Fake3DMode fake3D)
		{
			if (trinusManager != null) {
				//Debug.Log ("FAKE?"+fake3D);
				trinusManager.resetView (width, height, fake3D);
				lock(this)
				{
					if (output != null)
						Destroy (output);
					output = null;
					
					screenRect.x = 0;
					screenRect.y = 0;
					screenRect.width = width;
					screenRect.height = height;

					#if USE_SYS_DRAW
					//adjusting width to avoid stride padding in bitmap
					while((float)(width) * 3 / 4 != (width) * 3 / 4)
					{
						width--;
					}

					if (bitmap != null){
						bitmap.Dispose();
					}

					bitmap = new System.Drawing.Bitmap (width, height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
					bitsRect = new System.Drawing.Rectangle (0, 0, width, height);
					#endif
					output = new Texture2D (width, height, TextureFormat.RGB24, false);
					output.filterMode = FilterMode.Trilinear;
					PlayerPrefs.SetInt ("resolution", height);
				}

				//Cursor.lockState = CursorLockMode.Confined;
			}
			#if DEBUG
			//Debug.Log ("Resolution set to " + Screen.width + "x" + Screen.height);
			#endif
		}
		public void switchCamera(TrinusCamera newCam){
			switchCamera (newCam, true);
		}
		public void switchCamera(TrinusCamera newCam, bool toggleActive){
			if (toggleActive) {
				newCam.gameObject.SetActive (true);
				if (trinusCamera != null)
					trinusCamera.gameObject.SetActive (false);
			}
			trinusCamera = newCam;
			resetTracking ();
		}
		public Camera getMainCamera(){
			if (trinusCamera != null)
				return trinusCamera.getMainCamera();
			return null;
		}
		public Camera getUICamera(){
			if (trinusCamera != null)
				return trinusCamera.getUICamera();
			return null;
		}
//		public void setSensorDataDelegate(setSensorData ssd){
//			this.sensorDataDelegate = ssd;
//		}
		public DataStructs.SENSORS getSensorData(){
			if (trinusManager != null)
				return trinusManager.getSensorData ();
			return new DataStructs.SENSORS();
		}
		public bool shouldScaleUI(){
			return trinusCamera.getMode () == TrinusCamera.CAMERA_MODE.DUAL || trinusCamera.getMode () == TrinusCamera.CAMERA_MODE.SINGLE;
		}
		/// <summary>
		///apply lens parameter changes done to lensParames object
		/// </summary>
		public void applyLensParams()
		{
//			if (settingsInstance.disableLens) {
//				trinusManager.setScale (1, 1);
//			} else {
				//		#if DEBUG
				//		Debug.Log ("Apply lens: " + settingsInstance.lensParams.ToString());
				//		#endif
				//trinusManager.setIpd (lensParams.screenX);
			trinusManager.setScreenCenter (settingsInstance.lensParams.screenx + (settingsInstance.ipd - 0.5f) / 2,
				settingsInstance.lensParams.screeny);
			trinusManager.setLensCenter (settingsInstance.lensParams.lensx, settingsInstance.lensParams.lensy);
			trinusManager.setScale (settingsInstance.lensParams.scalex, settingsInstance.lensParams.scaley);
			trinusManager.setWarp (settingsInstance.lensParams.warpx, settingsInstance.lensParams.warpy, settingsInstance.lensParams.warpz, settingsInstance.lensParams.warpw);
			trinusManager.setChroma (settingsInstance.lensParams.chroma);
//			}
		}
		public void setIpd(float x){
			if (trinusManager != null && trinusManager.getStatus () == DataStructs.STATUS.STREAMING)
				trinusManager.setScreenCenter (settingsInstance.lensParams.screenx + (x - 0.5f) / 2,
					settingsInstance.lensParams.screeny);
		}
		public void setScreenCenter(float x, float y){
			if (trinusManager != null && trinusManager.getStatus () == DataStructs.STATUS.STREAMING)
				trinusManager.setScreenCenter (x, y);
		}
		public void setLensCenter(float x, float y){
			if (trinusManager != null && trinusManager.getStatus () == DataStructs.STATUS.STREAMING)
				trinusManager.setLensCenter (x, y);
		}
		public void setScale(float x, float y){
			if (trinusManager != null && trinusManager.getStatus () == DataStructs.STATUS.STREAMING)
				trinusManager.setScale (x, y);
		}
		public void setWarp(float x, float y, float z){
			setWarp (x, y, z, 0);
		}
		public void setWarp(float x, float y, float z, float w){
			if (trinusManager != null && trinusManager.getStatus () == DataStructs.STATUS.STREAMING)
				trinusManager.setWarp (x, y, z, w);
		}
		public void setChroma(float c){
			if (trinusManager != null && trinusManager.getStatus () == DataStructs.STATUS.STREAMING)
				trinusManager.setChroma (c);
		}
		/// <summary>
		///Check if Trinus is connected and streaming
		/// </summary>
		public bool isStreaming(){
			return trinusManager != null ? trinusManager.getStatus () == DataStructs.STATUS.STREAMING : false;
		}
		public DataStructs.STATUS getTrinusStatus(){
			if (trinusManager != null)
				return trinusManager.getStatus ();
			return DataStructs.STATUS.IDLE;
		}
		/// <summary>
		///start/stop moving the camera through head tracking. Useful for cutscenes, pause, etc.
		///once reactivated, it might be good to call resetYaw to ensure the view stays on current direction
		/// </summary>
		public void disableHeadTracking(bool ignore)
		{
			ignoreTracking = ignore;
		}

		public void setMonoscopic(bool active){
			decideResolution (Screen.height, active);
		}
		public bool getMonoscopic(){
			return PlayerPrefs.GetInt ("mono", 0) == 1;
		}
		public void decideResolution(int h){
			decideResolution (h, getMonoscopic());
		}
		public void decideResolution(int h, bool monoscopic){
			int resHeight = (int)Math.Min(1080, h * displayScale);
	//		int resWidth = (int)Math.Min(1920, w * displayScale);
			int resWidth = (int) (resHeight * (16 / 9f));

	//		if (trinusCamera.getMode () == TrinusCamera.CAMERA_MODE.SINGLE)
	//			monoscopic = true;
			if (trinusCamera != null) {
				if (monoscopic) {
					resWidth = (int)(resHeight * (16 / 9f) / 2);
					trinusCamera.setMode (TrinusCamera.CAMERA_MODE.SINGLE);
				} else {
	//			resWidth = (int) (resHeight * (16 / 9f));
					trinusCamera.setDefaultMode ();
				}
				PlayerPrefs.SetInt ("mono", monoscopic? 1 : 0);
			}
			//trinusCamera.enable (!monoscopic);

			if (Screen.fullScreen) 
			{
				Resolution[] res = validResolutions();
				Resolution selected = new Resolution();
				int closest = 99999999;
				foreach(Resolution r in res)
				{
					if (Math.Abs(r.width * r.height - resWidth * resHeight) < Math.Abs(closest - resWidth * resHeight))
					{
						closest = r.width * r.height;
						selected = r;
					}
				}
				if (selected.width != 0)
				{
					resWidth = selected.width;
					resHeight = selected.height;
				}

			}

			if (Screen.width != resWidth || Screen.height != resHeight)
				Screen.SetResolution (resWidth, resHeight, Screen.fullScreen && allowFullscreen);

			resolutionChange (Screen.width, Screen.height, monoscopic? DataStructs.Fake3DMode.ENABLED : DataStructs.Fake3DMode.DISABLED);
		}
		public static Resolution[] validResolutions(){
			Resolution[] res = Screen.resolutions;
			LinkedList<Resolution> vRes = new LinkedList<Resolution> ();
			foreach(Resolution r in res)
			{
				if ((float)r.width / r.height > 1.6)//widescreen only
				{
					vRes.AddFirst(r);
				}
			}
			Resolution[] result = new Resolution[vRes.Count];
			vRes.CopyTo (result, 0);
			return result;
		}
	}
}
