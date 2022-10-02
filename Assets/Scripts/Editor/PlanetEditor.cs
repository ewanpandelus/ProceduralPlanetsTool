using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Planet))]
public class PlanetEditor : Editor 
{
    Planet planet;
    Editor shapeEditor;
    Editor biomeEditor;
    public override void OnInspectorGUI()
    {
        using (var check = new EditorGUI.ChangeCheckScope()) 
        {
            base.OnInspectorGUI();
            if (check.changed)
            {
                planet.GeneratePlanet(false);
            }
        }
        if(GUILayout.Button("Generate Planet")) 
        {
            planet.GeneratePlanet(true);
        }
        if (GUILayout.Button("Generate Biomes"))
        {
            planet.GenerateColours();
        }
        DrawSettingsEditor(planet.shapeSettings, planet.OnShapeSettingsUpdated, ref planet.shapeSettingsFoldout, ref shapeEditor);
        DrawSettingsEditor(planet.biomeSettings, planet.OnColourSettingsUpdated, ref planet.biomeSettingsFoldout, ref biomeEditor);
    }
    void DrawSettingsEditor(Object settings, System.Action onSettingsUpdated, ref bool foldout, ref Editor editor)
    {
        if (settings != null)
        {
            foldout = EditorGUILayout.InspectorTitlebar(foldout, settings);
            using (var check = new EditorGUI.ChangeCheckScope())
            {
                if (foldout)
                {
                    CreateCachedEditor(settings, null, ref editor);
                    editor.OnInspectorGUI();
                    if (check.changed)
                    {
                        if (onSettingsUpdated != null)
                        {
                            onSettingsUpdated();
                        }
                    }
                }

            }
        }
    

      
    }
    private void OnEnable()
    {
        planet = (Planet)target;
    }
}
