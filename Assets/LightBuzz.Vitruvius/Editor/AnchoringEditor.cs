using UnityEngine;
using UnityEditor;

namespace LightBuzz.Vitruvius
{
    [CustomEditor(typeof(Anchoring))]
    public class AnchoringEditor : Editor
    {
        Anchoring aTarget;

        void OnEnable()
        {
            aTarget = (Anchoring)base.target;
        }

        public override void OnInspectorGUI()
        {
            Undo.RecordObject(aTarget, "UIAnchoring Undo");

            aTarget.AnchoringBody = (Camera)EditorGUILayout.ObjectField("Anchoring Body", aTarget.AnchoringBody, typeof(Camera), true);

            aTarget.Anchor = (AnchorAlignment)EditorGUILayout.EnumPopup("Anchor", aTarget.Anchor);

            EditorGUI.indentLevel = 1;

            aTarget.AnchorOffset = (AnchorOffset)EditorGUILayout.EnumPopup("Anchor offset", aTarget.AnchorOffset);

            if (aTarget.AnchorOffset == AnchorOffset.Unit)
            {
                aTarget.UnitOffset = EditorGUILayout.Vector2Field(" ", aTarget.UnitOffset);
            }
            else
            {
                aTarget.PercentageOffset = EditorGUILayout.Vector2Field(" ", aTarget.PercentageOffset);
            }

            EditorGUI.indentLevel = 0;

            aTarget.Pivot = (SpriteAlignment)EditorGUILayout.EnumPopup("Pivot", aTarget.Pivot);

            if (aTarget.Pivot == SpriteAlignment.Custom)
            {
                aTarget.PivotPoint = EditorGUILayout.Vector2Field(" ", aTarget.PivotPoint);
            }
            else
            {
                GUI.enabled = false;
                EditorGUILayout.Vector2Field(" ", aTarget.PivotPoint);
                GUI.enabled = true;
            }

            if (GUI.changed)
            {
                Undo.RecordObject(aTarget, "UIAnchoring Undo");
                EditorUtility.SetDirty(aTarget);
            }
        }
    }
}