// 4D extension of Pcx by Hiroyuki Inou:
// Pcx - Point cloud importer & renderer for Unity
// https://github.com/keijiro/Pcx

using UnityEngine;
using UnityEditor;

namespace Pcx
{
    [CustomEditor(typeof(PointCloudData4D))]
    public sealed class PointCloudData4DInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            var count = ((PointCloudData4D)target).pointCount;
            EditorGUILayout.LabelField("Point Count", count.ToString("N0"));
        }
    }
}
