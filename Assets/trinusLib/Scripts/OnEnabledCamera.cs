using UnityEngine;
using System.Collections;
using trinus;

namespace trinus{
	public class OnEnabledCamera : MonoBehaviour {

		public void OnEnable(){
			TrinusCamera cam = transform.parent.GetComponent<TrinusCamera> ();
			if (cam != null)
				cam.enabledCamera ();
		}
	}
}