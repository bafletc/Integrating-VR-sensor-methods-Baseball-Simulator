using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace LightBuzz.Vitruvius
{
    [CustomEditor(typeof(LineRenderer))]
    [CanEditMultipleObjects]
    public class UILineRendererEditor : Editor
    {
        new LineRenderer[] targets = null;

        string[] layerNames = null;
        string[] layerNamesMultiEdit = null;
        int layerNameSelectedIndex = 0;
        int layerNameSelectedIndexMultiEdit = 0;

        int layerOrder = 0;
        string layerOrderMultiEdit = "—";

        bool multiEditLayerNames = false;
        bool multiEditLayerOrders = false;

        void OnEnable()
        {
            Object[] objectTargets = base.targets;

            targets = new LineRenderer[objectTargets.Length];

            for (int i = 0; i < objectTargets.Length; i++)
            {
                targets[i] = (LineRenderer)objectTargets[i];

                if (i > 0)
                {
                    if (targets[i].sortingLayerName != targets[i - 1].sortingLayerName)
                        multiEditLayerNames = true;

                    if (targets[i].sortingOrder != targets[i - 1].sortingOrder)
                        multiEditLayerOrders = true;
                }
            }

            layerNames = GetSortingLayerNames();

            if (multiEditLayerNames)
            {
                List<string> tempSortingLayerNames = new List<string>(layerNames);
                tempSortingLayerNames.Insert(0, "—");
                layerNamesMultiEdit = tempSortingLayerNames.ToArray();
            }

            layerNameSelectedIndex = layerNames.TakeWhile(x => x != targets[0].sortingLayerName).Count();

            layerOrder = targets[0].sortingOrder;
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            Undo.RecordObjects(targets, "Undo LineSortingLayer");

            if (multiEditLayerNames)
            {
                int prevLayerNameSelectedIndex = layerNameSelectedIndexMultiEdit;

                layerNameSelectedIndexMultiEdit = EditorGUILayout.Popup("Sorting Layer", layerNameSelectedIndexMultiEdit, layerNamesMultiEdit);

                if (layerNameSelectedIndexMultiEdit != prevLayerNameSelectedIndex)
                {
                    for (int i = 0; i < targets.Length; i++)
                    {
                        targets[i].sortingLayerName = layerNames[layerNameSelectedIndexMultiEdit - 1];
                        EditorUtility.SetDirty(targets[i]);
                    }
                }
            }
            else
            {
                int prevLayerNameSelectedIndex = layerNameSelectedIndex;

                layerNameSelectedIndex = EditorGUILayout.Popup("Sorting Layer", layerNameSelectedIndex, layerNames);

                if (layerNameSelectedIndex != prevLayerNameSelectedIndex)
                {
                    for (int i = 0; i < targets.Length; i++)
                    {
                        targets[i].sortingLayerName = layerNames[layerNameSelectedIndex];
                        EditorUtility.SetDirty(targets[i]);
                    }

                    multiEditLayerNames = false;
                }
            }

            if (multiEditLayerOrders)
            {
                string prevLayerOrderMultiEdit = layerOrderMultiEdit;

                layerOrderMultiEdit = EditorGUILayout.TextField("Sorting Order", layerOrderMultiEdit);
                layerOrderMultiEdit = new string(layerOrderMultiEdit.Where(MultiSelectionNumber).ToArray());

                if (layerOrderMultiEdit != prevLayerOrderMultiEdit)
                {
                    if (!string.IsNullOrEmpty(layerOrderMultiEdit))
                    {
                        layerOrder = int.Parse(layerOrderMultiEdit);

                        for (int i = 0; i < targets.Length; i++)
                        {
                            targets[i].sortingOrder = layerOrder;
                            EditorUtility.SetDirty(targets[i]);
                        }

                        multiEditLayerOrders = false;
                    }
                    else
                    {
                        layerOrderMultiEdit = "—";
                    }
                }
            }
            else
            {
                int prevLayerOrder = layerOrder;

                layerOrder = EditorGUILayout.IntField("Sorting Order", layerOrder);

                if (layerOrder != prevLayerOrder)
                {
                    for (int i = 0; i < targets.Length; i++)
                    {
                        targets[i].sortingOrder = layerOrder;
                        EditorUtility.SetDirty(targets[i]);
                    }
                }
            }

            if (GUI.changed)
                for (int i = 0; i < targets.Length; i++)
                    EditorUtility.SetDirty(targets[i]);

            Undo.RecordObjects(targets, "Undo LineSortingLayer");
        }

        bool MultiSelectionNumber(char c)
        {
            return c == '—' || char.IsDigit(c);
        }

        public string[] GetSortingLayerNames()
        {
            System.Type internalEditorUtilityType = typeof(UnityEditorInternal.InternalEditorUtility);
            PropertyInfo sortingLayersProperty = internalEditorUtilityType.GetProperty("sortingLayerNames", BindingFlags.Static | BindingFlags.NonPublic);
            return (string[])sortingLayersProperty.GetValue(null, new object[0]);
        }
    }
}