using UnityEngine;
using System.Collections;

public static class VitruviusUI
{
    const float UI_WIDTH = 150;
    const float SMALL_BUTTON_WIDTH = 55;
    const float SPACING = 4;

    public static GUILayoutOption UIWidth
    {
        get
        {
            return GUILayout.Width(UI_WIDTH);
        }
    }

    public static GUILayoutOption SmallButtonWidth
    {
        get
        {
            return GUILayout.Width(SMALL_BUTTON_WIDTH);
        }
    }

    public static GUILayoutOption ButtonWidth
    {
        get
        {
            return GUILayout.Width(SMALL_BUTTON_WIDTH * 2 + SPACING);
        }
    }
}