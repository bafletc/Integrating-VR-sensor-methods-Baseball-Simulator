using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace trinus{
	public class SliderChange : MonoBehaviour {

		Text val;

		public void valueChange(System.Single v){
			if (val == null)
				val = transform.FindChild("ValueText").GetComponent<Text> ();
			val.text = v.ToString();
		}
	}
}