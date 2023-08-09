using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(StarGenerator))]
public class StarGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        StarGenerator generator = (StarGenerator) target;
        DrawDefaultInspector();
        if (GUILayout.Button("Generate"))
        {
            generator.GenerateStars();
        }
        if (GUILayout.Button("Reset"))
        {
            generator.ResetStars();
        }
    }
}
