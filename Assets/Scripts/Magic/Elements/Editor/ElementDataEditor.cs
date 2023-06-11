using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

[CustomEditor(typeof(ElementData))]
public class ElementDataEditor : Editor
{
    private static List<Type> dataCompTypes = new List<Type>();

    private ElementData elementData;
    private bool showButtons;

    private void OnEnable()
    {
        elementData = target as ElementData;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        showButtons = EditorGUILayout.Foldout(showButtons, "Add Components");

        if (showButtons)
        {
            for (int i = 0; i < dataCompTypes.Count; i++)
            {
                if (GUILayout.Button(dataCompTypes[i].Name))
                {
                    var comp = Activator.CreateInstance(dataCompTypes[i]) as ElementEffect;

                    if (comp == null)
                        return;

                    elementData.AddEffect(comp);
                }
            }
        }
    }

    [DidReloadScripts]
    private static void OnRecompile()
    {
        System.Reflection.Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
        List<Type> types = new List<Type>();
        foreach (System.Reflection.Assembly item in assemblies)
        {
            foreach (Type type in item.GetTypes())
            {
                types.Add(type);
            }
        }
        dataCompTypes = new List<Type>();
        for (int i = 0; i < types.Count; i++) 
        {
            if (types[i].IsSubclassOf(typeof(ElementEffect)) && !types[i].ContainsGenericParameters && types[i].IsClass)
            {
                dataCompTypes.Add(types[i]);
            }
        }
    }
}

