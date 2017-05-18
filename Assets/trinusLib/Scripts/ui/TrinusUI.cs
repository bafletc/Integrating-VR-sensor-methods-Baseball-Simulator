using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.EventSystems;

namespace trinus{
	public class TrinusUI : MonoBehaviour {

		public enum UI_PAGE{
			NONE, INTRO, GAME, SETTINGS, CONNECTION_WAIT, LENS
		}
		UI_PAGE currentPage = UI_PAGE.NONE;
		GameObject currentPageObject;

		//string[] introSteps = new string[]{"IntroStep1", "IntroStep2", "IntroStep3", "IntroStep4"};
		int currentIntroStage = 0;

		static Transform trinusCursor;
		static TrinusProcessor trinusProcessor;
		Text trinusMessage;
		Text trinusHint;
		Canvas canvas;
		private CanvasRenderer customUI;
		float trinusUIDefaultScale = 0.01f;

		[Tooltip("Event to trigger when Trinus is connected and streaming")]
		public UnityEngine.Events.UnityEvent connectedEvent;

		[Tooltip("Event to trigger when Trinus is disconnected")]
		public UnityEngine.Events.UnityEvent disconnectedEvent;

		[Tooltip("Event to trigger when Trinus settings page is closed")]
		public UnityEngine.Events.UnityEvent finishedSettingsEvent;

		[Tooltip("UI Page to open on finished settings, will open Trinus UI Game Page if not set. You can set it to go back to your own previous settings/pause Page")]
		public CanvasRenderer finishedSettingsPage;

		[Tooltip("Event to trigger when a Trinus Exit button is pressed (these buttons are hidden by default). If not set, Trinus will close the app")]
		public UnityEngine.Events.UnityEvent exitEvent;

		[Tooltip("The environment will stop moving while in Trinus settings")]
		public bool disableTrackingWhenPaused = true;

		[Tooltip("List of introduction pages before starting the connection. These show the user how to download the client and establish the connection")]
		public CanvasRenderer[] introPages;

		[Tooltip("Page displayed while connecting. This can be set to none. Default 'Connecting Page' gives option to manually enter IP Address in case Trinus autodetection fails. It is recommended to provide this option")]
		public CanvasRenderer connectingPage;

		[Tooltip("Page displayed while in game. This can be set to none, and redirected to your own separate UI system via connectedEvent")]
		public CanvasRenderer gamePage;

		[Tooltip("Main Trinus settings. You may redirect to your own UI page, or integrate to your existing UI as shown in SampleIntegratedUI")]
		public CanvasRenderer mainSettingsPage;

		[Tooltip("Trinus Lens custom settings. If set to none, mainSettingsPage will hide the Customize button")]
		public CanvasRenderer lensSettingsPage;

		[Tooltip("A theme to change the Trinus UI look")]
		public TrinusTheme trinusTheme;

		// Use this for initialization
		void Awake () {
			trinusCursor = transform.FindChild("TrinusCursor").transform;
			trinusProcessor = GameObject.Find ("TrinusManager").GetComponent<TrinusProcessor> ();
			trinusMessage = transform.FindChild ("TrinusMessage").GetComponent<Text> ();
			trinusHint = transform.FindChild ("TrinusHint").GetComponent<Text> ();

			setMessage ();
			setHint ();

			trinusUIDefaultScale = transform.localScale.x;

			canvas = GetComponent<Canvas> ();

			TrinusProcessor.setMessageCallbacks (setMessage, setHint, setHint);
			setShowFps (TrinusProcessor.getUserSettings ().showFps);

			Localization.setLocalization (transform);
			setTheme (trinusTheme);

			trinusCursor.gameObject.SetActive (false);
		}
		// Update is called once per frame
		void Update () {
			if (canvas.worldCamera == null) {
				assignUICamera (canvas, trinusProcessor.getUICamera ());
			}

			if (trinusCursor != null && trinusCursor.gameObject.activeSelf) {//hack to correctly place the cursor in stereoscopic mode
				updateTrinusCursor();
			}
			if (currentPage == UI_PAGE.CONNECTION_WAIT && trinusProcessor.isStreaming ()) {
				if (connectedEvent != null)
					connectedEvent.Invoke ();
				openGame ();
			} else 
				if (currentPage != UI_PAGE.NONE && trinusProcessor.getStatus () == corelib.util.DataStructs.STATUS.DISCONNECTED) {
					if (disconnectedEvent != null) {
						currentPage = UI_PAGE.NONE;
						disconnectedEvent.Invoke ();
					} else
						startConnection ();
				}
		}
		public static void updateTrinusCursor(){
			if (trinusCursor != null) {
				if (!trinusCursor.gameObject.activeSelf)
					setTrinusCursor (true);
	//			trinusCursor.localPosition = new Vector3 (Input.mousePosition.x - Screen.width / 2, Input.mousePosition.y - Screen.height / 2, trinusCursor.localPosition.z);
				Vector3 mp = Input.mousePosition;
				mp.z = 1.0f;
				if (trinusProcessor.getUICamera () != null) {
					Vector3 wp = trinusProcessor.getUICamera ().ScreenToWorldPoint (mp);
					trinusCursor.position = new Vector3 (wp.x, wp.y, trinusCursor.position.z);
				}
			}
		}
		public void hideUIPages(){
			if (introPages != null)
				foreach (CanvasRenderer c in introPages)
					c.gameObject.SetActive (false);
			if (connectingPage != null)
				connectingPage.gameObject.SetActive (false);
			if (gamePage != null)
				gamePage.gameObject.SetActive(false);
			if (mainSettingsPage != null)
				mainSettingsPage.gameObject.SetActive (false);
			if (lensSettingsPage != null)
				lensSettingsPage.gameObject.SetActive (false);
		}

		public void startConnection(){
			trinusProcessor.resetDisconnectStatus ();
			currentIntroStage = -1;
			trinusCursor.gameObject.SetActive (false);
			TrinusProcessor.trinusPause (false);
			setCurrentPage(UI_PAGE.CONNECTION_WAIT);
		}
		public void restartConnection(){
			restartConnection (true);
		}
		public void restartConnection(bool doIntro){
			TrinusProcessor.trinusPause (true);
			trinusProcessor.endStreaming();
			setMessage ();
			setHint ();
			if (doIntro)
				openIntro (true);
			else
				startConnection ();
		}
		public void quit(){
			if (exitEvent != null)
				exitEvent.Invoke ();
			else {
				#if UNITY_EDITOR
				UnityEditor.EditorApplication.isPlaying = false;
				#else
				//Application.Quit();//this seems to crash badly, Unity bug?

				trinusProcessor.endStreaming();
				System.Diagnostics.Process.GetCurrentProcess().Kill();
				#endif
			}
		}
		public void showQuitButton(bool active){
			if (introPages != null && introPages.Length > 0)
				introPages [introPages.Length - 1].transform.FindChild ("QuitButton").gameObject.SetActive (active);
			if (mainSettingsPage != null)
				mainSettingsPage.transform.FindChild ("QuitButton").gameObject.SetActive (active);
		}
		public static void assignUICamera(Canvas canvas, Camera uiCamera){
			canvas.renderMode = RenderMode.WorldSpace;
			canvas.worldCamera = uiCamera;
			canvas.planeDistance = 0.1f;
			canvas.sortingOrder = 100;
			canvas.pixelPerfect = true;
		}
		public static void setTrinusCursor(bool active){
			setTrinusCursor (active, null);
		}
		public static void setTrinusCursor(bool active, Sprite sprite){
			Cursor.visible = false;
			if (!active)
				Cursor.lockState = CursorLockMode.Confined;
			trinusCursor.gameObject.SetActive (active);
			if (sprite != null)
				trinusCursor.GetComponent<Image> ().sprite = sprite;
		}

		#region Page navigation
		public void setCurrentPage(UI_PAGE page){
			hideUIPages ();
			currentPage = page;
			currentPageObject = null;
			setTrinusCursor (false);

			Camera mainCam = trinusProcessor.getMainCamera ();
			if (mainCam != null) {
				Rect camRect = mainCam.rect;
				transform.localScale = new Vector3 (trinusUIDefaultScale * camRect.width / 1f, transform.localScale.y, transform.localScale.z);
			}

			if (customUI != null) {
				customUI.gameObject.SetActive (false);
			}

			switch (page) {
			case UI_PAGE.INTRO:
				if (introPages != null && currentIntroStage < introPages.Length) {
					introPages [currentIntroStage].gameObject.SetActive (true);
					currentPageObject = introPages [currentIntroStage].gameObject;
					if (EventSystem.current != null && getCurrentPageChild("ContinueButton"))
						EventSystem.current.SetSelectedGameObject (getCurrentPageChild("ContinueButton").gameObject, null);
					setTrinusCursor (true);
				}
				break;
			case UI_PAGE.CONNECTION_WAIT:
				if (connectingPage != null) {
					connectingPage.gameObject.SetActive (true);
					currentPageObject = connectingPage.gameObject;
					setTrinusCursor (true);
				}
				break;
			case UI_PAGE.GAME:
				if (gamePage != null) {
					gamePage.gameObject.SetActive (true);
					currentPageObject = gamePage.gameObject;
				}
				break;
			case UI_PAGE.SETTINGS:
				if (mainSettingsPage != null) {
					mainSettingsPage.gameObject.SetActive (true);
					currentPageObject = mainSettingsPage.gameObject;

					if (EventSystem.current != null && getCurrentPageChild("FpsToggle"))
						EventSystem.current.SetSelectedGameObject (getCurrentPageChild("FpsToggle").gameObject, null);
					setTrinusCursor (true);
				}
				break;
			case UI_PAGE.LENS:
				if (lensSettingsPage != null) {
					lensSettingsPage.gameObject.SetActive (true);
					currentPageObject = lensSettingsPage.gameObject;

					if (EventSystem.current != null && getCurrentPageChild("SeparationSlider"))
						EventSystem.current.SetSelectedGameObject (getCurrentPageChild("SeparationSlider").gameObject, null);
					setTrinusCursor (true);
				}
				break;
			}
		}
		public UI_PAGE getCurrentPage(){
			return currentPage;
		}
		public GameObject getCurrentPageObject(){
			return currentPageObject;
		}
		public Transform getCurrentPageChild(string childName){
			GameObject current = getCurrentPageObject ();
			if (current == null)
				return null;

			Transform child = current.transform.FindChild (childName);
			if (child == null)
				return null;

			return child;
		}
		public T getCurrentPageChildComponent<T>(string childName){
			Transform child = getCurrentPageChild (childName);
			if (child == null)
				return default(T);
			
			return child.GetComponent<T>();
		}
		public void introNext(){
			currentIntroStage++;
			setCurrentPage (UI_PAGE.INTRO);
		}
		public void openNone(){
			if (disableTrackingWhenPaused)
				trinusProcessor.disableHeadTracking (true);

			setCurrentPage (UI_PAGE.NONE);
		}
		public void openCustom(CanvasRenderer customUI){
			if (disableTrackingWhenPaused)
				trinusProcessor.disableHeadTracking (true);

			setCurrentPage (UI_PAGE.NONE);
			setTrinusCursor (true);
			this.customUI = customUI;
			customUI.gameObject.SetActive (true);
		}
		public void openSettings(){
			if (disableTrackingWhenPaused)
				trinusProcessor.disableHeadTracking (true);

			setCurrentPage (UI_PAGE.SETTINGS);

			populateLensCorrectionDropdown ();

			getCurrentPageChildComponent<Toggle> ("FpsToggle").isOn = TrinusProcessor.getUserSettings ().showFps;
			getCurrentPageChildComponent<Dropdown> ("HeadTrackingDropdown").value = TrinusProcessor.getUserSettings ().yawScale - 1;
			updateSlider (getCurrentPageChild ("FovSlider"), trinusProcessor.getFov ());
			updateSlider (getCurrentPageChild ("IpdSlider"), (int)(TrinusProcessor.getUserSettings ().ipd * 100));
			//		getCurrentPageChildComponent<Dropdown>("QualityDropdown").GetComponent<Dropdown> ().value = Screen.currentResolution.height == 900? 1 :
			//			(Screen.currentResolution.height < 900? 0 : 2);
			int resolutionIndex = 0;
			if (Screen.height > 500)
				resolutionIndex++;
			if (Screen.height > 800)
				resolutionIndex++;
			if (Screen.height > 1000)
				resolutionIndex++;
			//Debug.Log ("RI " + resolutionIndex + " " + Screen.currentResolution.height + " " + Screen.height);
			getCurrentPageChildComponent<Dropdown> ("QualityDropdown").GetComponent<Dropdown> ().value = resolutionIndex;

			getCurrentPageChild ("CustomButton").gameObject.SetActive (lensSettingsPage != null);

			getCurrentPageChildComponent<Toggle> ("MonoscopicToggle").isOn = trinusProcessor.getMonoscopic ();
		}
		public void openGame(){
			setCurrentPage (UI_PAGE.GAME);
			if (disableTrackingWhenPaused)
				trinusProcessor.disableHeadTracking (false);
		}
		public void openCustomLensSetup(){
			setCurrentPage (UI_PAGE.LENS);

			TrinusProcessor.getUserSettings().lensParams.selectPreset(TrinusProcessor.LensParams.CUSTOM_PRESET);
			trinusProcessor.applyLensParams();

			updateSlider (getCurrentPageChild("SeparationSlider"), (int) (TrinusProcessor.getUserSettings().lensParams.screenx * 100));
			updateSlider (getCurrentPageChild("WidthSlider"), (int) (TrinusProcessor.getUserSettings().lensParams.scalex * 100));
			updateSlider (getCurrentPageChild("HeightSlider"), (int) (TrinusProcessor.getUserSettings().lensParams.scaley * 100));
			updateSlider (getCurrentPageChild("LensAngleXSlider"), (int) (TrinusProcessor.getUserSettings().lensParams.lensx * 100));
			updateSlider (getCurrentPageChild("WarpXSlider"), (int) (TrinusProcessor.getUserSettings().lensParams.warpx * 100));
			updateSlider (getCurrentPageChild("WarpYSlider"), (int) (TrinusProcessor.getUserSettings().lensParams.warpy * 100));
			updateSlider (getCurrentPageChild("WarpZSlider"), (int) (TrinusProcessor.getUserSettings().lensParams.warpz * 100));
			updateSlider (getCurrentPageChild("ChromaSlider"), (int) (TrinusProcessor.getUserSettings().lensParams.chroma * 100));
		}
		public void finishCustomLensSetup(bool save){
			openSettings ();
			Dropdown dropdown = getCurrentPageChild("LensCorrectionDropdown").GetComponent<Dropdown> ();			
			if (save) {
				dropdown.captionText.text = TrinusProcessor.getUserSettings ().lensParams.currentPreset;
				TrinusProcessor.getUserSettings().lensParams.savePreset();
			} else {
				TrinusProcessor.getUserSettings ().lensParams.selectPreset (dropdown.captionText.text);//TrinusProcessor.LensParams.CUSTOM_PRESET);
				trinusProcessor.applyLensParams();
			}
		}
		public void finishSettings(){
			if (finishedSettingsPage != null) {
				setCurrentPage (UI_PAGE.NONE);
				finishedSettingsPage.gameObject.SetActive (true);
			} else {
				openGame ();
			}

			if (finishedSettingsEvent != null)
				finishedSettingsEvent.Invoke ();
		}
		public void openIntro(){
			openIntro (false);
		}
		public void openIntro(bool skipBasics){
			//currentIntroStage = skipBasics? 2 : 0;
			currentIntroStage = 0;
			setCurrentPage (UI_PAGE.INTRO);
		}
		#endregion
		#region UI Events
		public void ipEnteredOnConnection(string ip){
			string vip = TrinusProcessor.validateIp (ip);

			InputField ipField = getCurrentPageChildComponent<InputField> ("IpInputField");
			if (ipField != null)
				ipField.text = vip == null? "" : vip;

			if (vip != null && !vip.Trim ().Equals ("")) {
				TrinusProcessor.getUserSettings ().forcedIP = vip;
				restartConnection (false);
			} else
				TrinusProcessor.getUserSettings ().forcedIP = null;

		}
		public void portEnteredOnConnection(string port){
			int vport = TrinusProcessor.validatePort(port);

			InputField portField = getCurrentPageChildComponent<InputField> ("PortInputField");
			if (portField != null)
				portField.text = vport == -1? "" : "" + vport;

			Debug.Log ("port set " + vport + " " + port);
			if (vport != -1) {
				TrinusProcessor.getUserSettings ().videoPort = vport;
				restartConnection (false);
			}
		}
		public void ipEntered(string ip){
			string vip = TrinusProcessor.validateIp (ip);
			TrinusProcessor.getUserSettings ().forcedIP = vip;

			InputField ipField = getCurrentPageChildComponent<InputField> ("IpInputField");
			if (ipField != null)
				ipField.text = vip == null? "" : vip;
		}
		public void videoPortEntered(string port){
			int portInt = 7777;
			if (!int.TryParse (port, out portInt) || (portInt <= 1024 || portInt >= 49151)) {
				portInt = 7777;

				InputField portField = getCurrentPageChildComponent<InputField> ("VideoPortInputField");
				if (portField != null)
					portField.text = portInt.ToString();
			}
			TrinusProcessor.getUserSettings ().videoPort = portInt;
		}
		public void sensorPortEntered(string port){
			int portInt = 5555;
			if (!int.TryParse (port, out portInt) || (portInt <= 1024 || portInt >= 49151)) {
				portInt = 5555;

				InputField portField = getCurrentPageChildComponent<InputField> ("SensorPortInputField");
				if (portField != null)
					portField.text = portInt.ToString();
			}
			TrinusProcessor.getUserSettings ().sensorPort = portInt;
		}
		public void setTracking(int value){
			TrinusProcessor.getUserSettings ().yawScale = value == 0? 1 : value * 2;
		}
		public void setFov(System.Single v){
			trinusProcessor.setFov((int)v);
		}
		public void setShowFps(bool on){
			TrinusProcessor.getUserSettings().showFps = on;
			if (!on)
				setHint ();
		}
		public void setMonoscopic(bool on){
			trinusProcessor.setMonoscopic (on);
			Rect camRect = trinusProcessor.getMainCamera ().rect;
			transform.localScale = new Vector3 (trinusUIDefaultScale * camRect.width / 1f, transform.localScale.y, transform.localScale.z);
		}
		public void setResolution(int l){
			int h = 480;
			switch (l) {
			case 1: h=720;break;
			case 2: h=900;break;
			case 3: h=1080;break;
			}
			trinusProcessor.decideResolution (h);
		}
		public void setLensCorrection(int value){
			List<string> presets = new List<string>(TrinusProcessor.LensParams.getPresetNames());
			if (value == 0)
				TrinusProcessor.getUserSettings ().lensParams.selectPreset(TrinusProcessor.LensParams.CUSTOM_PRESET);
			else
				TrinusProcessor.getUserSettings ().lensParams.selectPreset(presets[value - 1]);
			trinusProcessor.applyLensParams();
		}
		public void setIpd(System.Single v){
			TrinusProcessor.getUserSettings ().ipd = (v / 100f);
			trinusProcessor.setIpd(TrinusProcessor.getUserSettings ().ipd);
		}
		public void setScreenX(System.Single v){
			TrinusProcessor.getUserSettings ().lensParams.screenx = (v / 100f);
			trinusProcessor.setScreenCenter(TrinusProcessor.getUserSettings ().lensParams.screenx, TrinusProcessor.getUserSettings ().lensParams.screeny);
		}
		public void setScaleX(System.Single v){
			TrinusProcessor.getUserSettings ().lensParams.scalex = (v / 100f);
			trinusProcessor.setScale(TrinusProcessor.getUserSettings ().lensParams.scalex, TrinusProcessor.getUserSettings ().lensParams.scaley);
		}
		public void setScaleY(System.Single v){
			TrinusProcessor.getUserSettings ().lensParams.scaley = (v / 100f);
			trinusProcessor.setScale(TrinusProcessor.getUserSettings ().lensParams.scalex, TrinusProcessor.getUserSettings ().lensParams.scaley);
		}
		public void setLensX(System.Single v){
			TrinusProcessor.getUserSettings ().lensParams.lensx = (v / 100f);
			trinusProcessor.setLensCenter(TrinusProcessor.getUserSettings ().lensParams.lensx, TrinusProcessor.getUserSettings ().lensParams.lensy);
		}
		public void setWarpX(System.Single v){
			TrinusProcessor.getUserSettings ().lensParams.warpx = (v / 100f);
			trinusProcessor.setWarp(TrinusProcessor.getUserSettings ().lensParams.warpx, TrinusProcessor.getUserSettings ().lensParams.warpy, TrinusProcessor.getUserSettings ().lensParams.warpz);
		}
		public void setWarpY(System.Single v){
			TrinusProcessor.getUserSettings ().lensParams.warpy = (v / 100f);
			trinusProcessor.setWarp(TrinusProcessor.getUserSettings ().lensParams.warpx, TrinusProcessor.getUserSettings ().lensParams.warpy, TrinusProcessor.getUserSettings ().lensParams.warpz);
		}
		public void setWarpZ(System.Single v){
			TrinusProcessor.getUserSettings ().lensParams.warpz = (v / 100f);
			trinusProcessor.setWarp(TrinusProcessor.getUserSettings ().lensParams.warpx, TrinusProcessor.getUserSettings ().lensParams.warpy, TrinusProcessor.getUserSettings ().lensParams.warpz);
		}
		public void setChroma(System.Single v){
			TrinusProcessor.getUserSettings ().lensParams.chroma = (v / 100f);
			trinusProcessor.setChroma(TrinusProcessor.getUserSettings ().lensParams.chroma);
		}
		void updateSlider(Transform t, int val){
			t.GetComponent<Slider> ().value = val;
			t.GetComponent<SliderChange> ().valueChange(val);
		}
		void populateLensCorrectionDropdown(){
			Dropdown dropdown = getCurrentPageChild("LensCorrectionDropdown").GetComponent<Dropdown> ();
			List<string> presets = new List<string>(TrinusProcessor.LensParams.getPresetNames());
			presets.Insert (0, TrinusProcessor.LensParams.CUSTOM_PRESET);
			dropdown.options.Clear ();
			foreach(string optionStr in presets)
				dropdown.options.Add(new Dropdown.OptionData(optionStr));
//			dropdown.options.AddRange
//			dropdown.AddOptions (null);
//			dropdown.ClearOptions ();
//			dropdown.AddOptions (presets);
			dropdown.captionText.text = TrinusProcessor.getUserSettings ().lensParams.currentPreset;
		}
		#endregion
		#region Trinus messaging
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
				if (!trinusMessage.gameObject.activeSelf)
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
			if (textId == null)
				trinusHint.gameObject.SetActive (false);
			else {
				if (!trinusHint.gameObject.activeSelf)
					trinusHint.gameObject.SetActive (true);
				trinusHint.text = string.Format(Localization.getText(textId), extras);
			}
		}
		#endregion
		public void setTheme(TrinusTheme theme){
			if (theme != null) {
				if (theme.cursorSprite != null) {
					trinusCursor.GetComponent<Image> ().sprite = theme.cursorSprite;
				}
				Text[] allTexts = transform.GetComponentsInChildren<Text> (true);
				foreach (Text t in allTexts) {
					if (theme.font != null)
						t.font = theme.font;
					t.fontSize = (int) (t.fontSize * theme.fontSizeRatio);
					t.color = theme.fontColor;
				}
				Image[] allImages = transform.GetComponentsInChildren<Image> (true);
				foreach (Image i in allImages) {
					if (i.sprite != null) {
						if (i.sprite.name.StartsWith ("UIPanel")) {
							i.color = theme.panelColor;
							if (theme.panelSprite != null)
								i.sprite = theme.panelSprite;
						}
						if (i.sprite.name.StartsWith ("UIButton")) {
							i.color = theme.buttonColor;
							if (theme.buttonDefaultSprite != null)
								i.sprite = theme.buttonDefaultSprite;
							Button b = i.gameObject.GetComponent<Button> ();
							if (b != null) {
								SpriteState state = b.spriteState;
								if (theme.buttonHighlightSprite != null)
									state.highlightedSprite = theme.buttonHighlightSprite;
								else
									state.highlightedSprite = b.spriteState.highlightedSprite;
								if (theme.buttonPressedSprite != null)
									state.pressedSprite = theme.buttonPressedSprite;
								else
									state.pressedSprite = b.spriteState.pressedSprite;
								if (theme.buttonDisabledSprite != null)
									state.disabledSprite = theme.buttonDisabledSprite;
								else
									state.disabledSprite = b.spriteState.disabledSprite;
								b.spriteState = state;
							}
						}
					}
				}
				InputField[] allInputFields = transform.GetComponentsInChildren<InputField> (true);
				foreach (InputField i in allInputFields) {
					ColorBlock block = i.colors;
					block.normalColor = theme.inputFieldColor;
					block.highlightedColor = theme.inputFieldHighlightColor;
					block.pressedColor = theme.inputFieldPressedColor;
					block.disabledColor = theme.inputFieldDisabledColor;
					i.colors = block;
				}
				Slider[] allSliders = transform.GetComponentsInChildren<Slider> (true);
				foreach (Slider s in allSliders) {
					ColorBlock block = s.colors;
					block.normalColor = theme.inputFieldColor;
					block.highlightedColor = theme.inputFieldHighlightColor;
					block.pressedColor = theme.inputFieldPressedColor;
					block.disabledColor = theme.inputFieldDisabledColor;
					s.colors = block;
				}
				Dropdown[] allDropdowns = transform.GetComponentsInChildren<Dropdown> (true);
				foreach (Dropdown d in allDropdowns) {
					ColorBlock block = d.colors;
					block.normalColor = theme.inputFieldColor;
					block.highlightedColor = theme.inputFieldHighlightColor;
					block.pressedColor = theme.inputFieldPressedColor;
					block.disabledColor = theme.inputFieldDisabledColor;
					d.colors = block;
				}
				Toggle[] allToggles = transform.GetComponentsInChildren<Toggle> (true);
				foreach (Toggle t in allToggles) {
					ColorBlock block = t.colors;
					block.normalColor = theme.inputFieldColor;
					block.highlightedColor = theme.inputFieldHighlightColor;
					block.pressedColor = theme.inputFieldPressedColor;
					block.disabledColor = theme.inputFieldDisabledColor;
					t.colors = block;
				}
			}
		}
	}
}
