using UnityEngine;
using System.Collections;

namespace trinus{
	public class TrinusTheme : MonoBehaviour {

		public Font font;
		public float fontSizeRatio = 1f;
		public Color fontColor = Color.white;

		public Sprite cursorSprite;

		public Color panelColor = Color.grey;
		public Sprite panelSprite;

		public Color buttonColor = Color.blue;
		public Sprite buttonDefaultSprite;
		public Sprite buttonHighlightSprite;
		public Sprite buttonPressedSprite;
		public Sprite buttonDisabledSprite;

		public Color inputFieldColor = Color.black;
		public Color inputFieldHighlightColor = Color.blue;
		public Color inputFieldPressedColor = Color.white;
		public Color inputFieldDisabledColor = Color.grey;
	}
}