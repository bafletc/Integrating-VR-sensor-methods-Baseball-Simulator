using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Reflection;

namespace trinus{
	public class TrinusCamera : MonoBehaviour
	{
		public enum CAMERA_MODE{
			DISABLED,
			SINGLE,
			DUAL,
			UNITY_VR
		}
		Camera leftCamera;
		Camera rightCamera;
		Camera splitCamera;

		Camera uiCameraMain;
		Camera uiCameraSub;

		Vector3 offset;

		Vector3 defaultRotation;

		public delegate void cameraEnabled(TrinusCamera camera, bool toggle);
		cameraEnabled cameraEnabledDelegate;

		CAMERA_MODE activeMode = CAMERA_MODE.DISABLED;

		[Tooltip("The Camera mode determines if the view should be monoscopic or stereosopic (using Unity VR mode or a two camera rig). Refer to the manual for details")]
		public CAMERA_MODE defaultMode = CAMERA_MODE.UNITY_VR;

		[Tooltip("Camera offset to better simulate neck rotation")]
		public float neckHeight = 0.15f;

//		[Tooltip("When using the auto camera switch in TrinusProcessor, the newly selected camera will be offset to look on this angle. This avoids having to call TrinusProcessor.resetView(float)")]
//		public float defaultViewAngle = 0;

		void Awake(){
			defaultRotation = new Vector3(transform.localEulerAngles.x , transform.localEulerAngles.y, transform.localEulerAngles.z);
		//	    transform.position = new Vector3 (transform.position.x, transform.position.y - neckHeight, transform.position.z);

			Transform t = transform.FindChild ("LeftCamera");
			if (t != null) {
				leftCamera = (Camera)t.GetComponent<Camera> ();
				t.localPosition = new Vector3 (t.localPosition.x, neckHeight, t.localPosition.z);
			}
			t = transform.FindChild ("RightCamera");
			if (t != null){
				rightCamera = (Camera)t.GetComponent<Camera> ();
				t.localPosition = new Vector3 (t.localPosition.x, neckHeight, t.localPosition.z);
			}
			t = transform.FindChild ("SplitCamera");
			if (t != null) {
				splitCamera = (Camera)t.GetComponent<Camera> ();
				t.localPosition = new Vector3 (t.localPosition.x, neckHeight, t.localPosition.z);
			}

			if (splitCamera == null && transform.GetComponent<Camera>() != null) {
			//TrinusCamera attached to a camera, ignore children and use that one as main
				splitCamera = transform.GetComponent<Camera>();
				//TODO spawn new left/right cams
				if (leftCamera == null)
					leftCamera = splitCamera;
				if (rightCamera == null)
					rightCamera = splitCamera;
				defaultMode = CAMERA_MODE.SINGLE;
				Debug.Log("TrinusCamera using external camera (" + splitCamera + "). Switching to single camera mode");
			}

			uiCameraMain = GameObject.Find ("TrinusUICamera").GetComponent<Camera>();
			uiCameraSub = uiCameraMain.transform.FindChild ("Camera").GetComponent<Camera> ();

			//int savedMode = PlayerPrefs.GetInt ("cameraMode", (int)defaultMode);
			//setMode ((CAMERA_MODE)savedMode);
			//setFov (getFov ());
			setMode(defaultMode);
		}

		public void setFov(int f){
			if (activeMode == CAMERA_MODE.DUAL) {
				leftCamera.fieldOfView = f;
				rightCamera.fieldOfView = f;
			} else
				splitCamera.fieldOfView = f;

			PlayerPrefs.SetInt ("fov", f);
		}
		public Vector3 getDefaultRotation(){
			return defaultRotation;
		}
		public Vector3 getOffset(){
			return offset;
		}
		public void setOffset(Vector3 offset){
			this.offset = offset;
		}

		public int getFov(){
			return PlayerPrefs.GetInt ("fov", 100);
		}
		public void setConvergence(float c){
		}
		public float getConvergence(){
			return 0;
		}
		public void setSeparation(float s){
		}
		public float getSeparation(){
			return 0;
		}
		public Camera getMainCamera(){
			if (activeMode == CAMERA_MODE.DUAL)
				return leftCamera;
			return splitCamera;
		}
		public Camera[] getMainDualCamera(){
			return new Camera[]{ leftCamera, rightCamera };
		}
		public Camera getUICamera(){
			return uiCameraMain;
		}

		public CAMERA_MODE getMode(){
			return activeMode;
		}
		public void setDefaultMode(){
			setMode (defaultMode);
		}
		public void setMode(CAMERA_MODE mode){
			if (mode == activeMode)
				return;
			activeMode = mode;
	//		if (activeMode != CAMERA_MODE.DISABLED)
	//			PlayerPrefs.SetInt ("cameraMode", (int)activeMode);
			switch (mode) {
			case CAMERA_MODE.DISABLED:
				leftCamera.gameObject.SetActive(false);
				rightCamera.gameObject.SetActive(false);
				splitCamera.gameObject.SetActive(false);
				break;
			case CAMERA_MODE.SINGLE:
				leftCamera.gameObject.SetActive(false);
				rightCamera.gameObject.SetActive(false);
				splitCamera.gameObject.SetActive(true);
				UnityEngine.VR.VRSettings.enabled = false;
				//UnityEngine.VR.VRSettings.loadedDevice = UnityEngine.VR.VRDeviceType.None;

				uiCameraSub.gameObject.SetActive (false);
				uiCameraMain.rect = new Rect (0, 0, 1, 1);
				break;
			case CAMERA_MODE.UNITY_VR:
				leftCamera.gameObject.SetActive (false);
				rightCamera.gameObject.SetActive (false);
				splitCamera.gameObject.SetActive (true);
                    UnityEngine.VR.VRSettings.LoadDeviceByName(UnityEngine.VR.VRSettings.supportedDevices.ToString());
				UnityEngine.VR.VRSettings.enabled = true;

				uiCameraSub.gameObject.SetActive (false);
				uiCameraMain.rect = new Rect (0, 0, 1, 1);
				break;
			case CAMERA_MODE.DUAL:
				leftCamera.gameObject.SetActive (true);
				rightCamera.gameObject.SetActive (true);
				if (splitCamera != leftCamera)//TrinusCamera attached to external camera case
					splitCamera.gameObject.SetActive (false);
				//UnityEngine.VR.VRSettings.enabled = false;
				//UnityEngine.VR.VRSettings.loadedDevice = UnityEngine.VR.VRDeviceType.None;

				uiCameraSub.gameObject.SetActive (true);
				uiCameraSub.gameObject.SetActive (true);
				uiCameraSub.rect = new Rect (0.5f, 0, 0.5f, 1);
				uiCameraMain.rect = new Rect (0, 0, 0.5f, 1);
				break;
			}
			setFov (getFov ());
		}
		public void setCameraEnabledDelegate(cameraEnabled ce){
			cameraEnabledDelegate = ce;
		}
		public void OnEnable(){
			enabledCamera ();
		}
		public void enabledCamera(){
			if (cameraEnabledDelegate != null)
				cameraEnabledDelegate (this, false);
//			if (trinusProcessor != null && trinusProcessor.autoSwitchCamera) {
//				trinusProcessor.switchCamera (this, false);
////				trinusProcessor.resetView (-defaultViewAngle);
//			}
		}
		public void syncCameras(){
			if (leftCamera != null & rightCamera != null) {
				syncComponents (leftCamera.gameObject, rightCamera.gameObject);
				syncComponents (rightCamera.gameObject, leftCamera.gameObject);
			}
		}
		private void syncComponents(GameObject source, GameObject dest){
			List<Component> list = new List<Component>();
			source.GetComponents (list);
			foreach(Component component in list){
				copyComponent(dest, component);
			}
		}
		private static void copyComponent<T>(GameObject gameObject, T other) where T : Component
		{
			Type type = other.GetType();
			//		Debug.Log (gameObject + " has " + other + "? " + gameObject.GetComponent (type));
			if (gameObject.GetComponent(type) != null)
				return;//already exists
			Component newComponent = gameObject.AddComponent (type);

			if (other is Behaviour)
				((Behaviour)newComponent).enabled = (other as Behaviour).enabled;

			BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Default | BindingFlags.DeclaredOnly;
			PropertyInfo[] pinfos = type.GetProperties(flags);
			foreach (PropertyInfo pinfo in pinfos) {
				if (pinfo.CanWrite) {
					try {
						pinfo.SetValue(newComponent, pinfo.GetValue(other, null), null);
					}
					catch { }
				}
			}
			FieldInfo[] finfos = type.GetFields(flags);
			foreach (FieldInfo finfo in finfos) {
				finfo.SetValue(newComponent, finfo.GetValue(other));
			}

		}
	}
}
