using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

namespace trinus{
	public class Localization : MonoBehaviour {

		static Dictionary<string, string> texts;
		static Dictionary<string, string> defaultTexts;

		static void loadTexts(){
			string language = System.Globalization.CultureInfo.CurrentCulture.Name;
			int dash = language.IndexOf ("-");
			if (dash > 0)
				language = language.Substring (0, dash);

			TextAsset localStringsAsset = (TextAsset)Resources.Load ("text_" + language);
			if (localStringsAsset == null) {//using english default
				localStringsAsset = (TextAsset)Resources.Load ("text_en");
				//Debug.Log ("text_" + language + " not found, using default locale (en)");
			} else {
				//Debug.Log ("Using locale " + language);
			}

			if (localStringsAsset != null && !string.IsNullOrEmpty (localStringsAsset.text)) {
				texts = populateTexts (localStringsAsset.text);
			}
			localStringsAsset = (TextAsset)Resources.Load ("text_en");
			if (localStringsAsset != null && !string.IsNullOrEmpty (localStringsAsset.text)) {
				defaultTexts = populateTexts (localStringsAsset.text);
			}
		}
		static Dictionary<string, string> populateTexts(string source){
			Dictionary<string, string> texts = new Dictionary<string, string> ();
			System.IO.StringReader textStream = new System.IO.StringReader (source);
			string line = null;
			while ((line = textStream.ReadLine()) != null) {
				int eqIndex = line.IndexOf ("=");
				if (eqIndex > 0) {
					string textId = line.Substring (0, eqIndex);
					if (texts.ContainsKey(textId))
						texts.Remove(textId);
					texts.Add (textId, line.Substring (eqIndex + 1, line.Length - eqIndex - 1));
				}
			}

			textStream.Close ();
			return texts;
		}
		public static string getText(string textId){
			return getText (textId, false);
		}
		public static string getText(string textId, bool useDefault){
			if (texts == null)
				loadTexts ();
			if (textId != null) {
				if (texts != null && texts.ContainsKey (textId))
					return texts [textId].Replace ("\\n", "\n");
				else
					if (defaultTexts != null && defaultTexts.ContainsKey (textId)) 
						return defaultTexts [textId].Replace ("\\n", "\n");
			}
			return textId;
		}
		public static void setLocalization(Transform parent){
			Text[] allTexts = parent.GetComponentsInChildren<Text> (true);
			foreach (Text t in allTexts) {
				if (t.name.StartsWith("label")){
					string text = getText(t.name, true);
					if (text != null){
						t.text = text;
					}
				}
			}
			Dropdown[] allDropdowns = parent.GetComponentsInChildren<Dropdown> (true);
			foreach (Dropdown d in allDropdowns) {
				if (d.captionText.text.StartsWith ("label")) {
					string t = getText (d.captionText.text, true);
					if (t != null) {
						d.captionText.text = t;
					}
				}
				foreach(Dropdown.OptionData o in d.options){
					if (o.text.StartsWith ("label")) {
						string text = getText(o.text, true);
						if (text != null){
							o.text = text;
						}
					}
				}
			}
		}

	}
}