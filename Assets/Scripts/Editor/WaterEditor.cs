using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(WaterBody))]
public class WaterEditor : Editor 
{
    WaterBody body;

    public override void OnInspectorGUI()
    {
        using (var check = new EditorGUI.ChangeCheckScope()) 
        {
            base.OnInspectorGUI();
            if (check.changed)
            {
                body.GeneratePlanet(true);
            }
        }
     
    }

    private void OnEnable()
    {
        body = (WaterBody)target;
    }
}
