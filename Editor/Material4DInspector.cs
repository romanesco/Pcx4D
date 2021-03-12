// 4D extension of Pcx by Hiroyuki Inou:
// Pcx - Point cloud importer & renderer for Unity
// https://github.com/keijiro/Pcx

using UnityEngine;
using UnityEditor;

namespace Pcx4D
{
    class Point4DMaterialInspector : ShaderGUI
    {
        public override void OnGUI(MaterialEditor editor, MaterialProperty[] props)
        {
            editor.ShaderProperty(FindProperty("_Tint", props), "Tint");
            editor.ShaderProperty(FindProperty("_PointSize", props), "Point Size");
            editor.ShaderProperty(FindProperty("_Distance", props), "Apply Distance");
            editor.ShaderProperty(FindProperty("_Translation4D", props), "4D Translation");
            editor.ShaderProperty(FindProperty("_Chiral", props), "Chirality (Invert w Coordinate)");

            EditorGUILayout.HelpBox(
                "Only some platform support these point size properties.",
                MessageType.None
            );
        }
    }

    class Disk4DMaterialInspector : ShaderGUI
    {
        public override void OnGUI(MaterialEditor editor, MaterialProperty[] props)
        {
            editor.ShaderProperty(FindProperty("_Tint", props), "Tint");
            editor.ShaderProperty(FindProperty("_PointSize", props), "Point Size");
            editor.ShaderProperty(FindProperty("_Translation4D", props), "4D Translation");
        }
    }
}
