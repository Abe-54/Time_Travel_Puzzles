using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TimeSwapManager))]
public class GameTools : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector(); // Draws the default inspector

        TimeSwapManager timeSwapManager = (TimeSwapManager)target; // Casts the target to your specific component type

        // Add a horizontal line to separate the default inspector from the custom inspector
        GUILayout.Space(10);

        GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(1));

        // Add a title to the custom inspector
        GUILayout.Label("Time Swap Inspector Tools", EditorStyles.boldLabel);
        GUILayout.Space(10); // Add some space between the title and the button
        // Have the Label for the button be next to the button
        GUILayout.BeginHorizontal();

        // Show the current time period, I onlty want currentTimePeriod from timeSwapManager to be bold
        GUILayout.Label("Current Time Period: ", EditorStyles.label);
        GUILayout.Label(timeSwapManager.currentTimePeriod.ToString(), EditorStyles.boldLabel);


        // Button for swapping the environment
        if (GUILayout.Button("Swap Environment (Editor Only)"))
        {
            // Call the method to swap the environment here without playing audio
            // Ensure your TimeSwapManager component has a method that can be called here to swap without playing audio
            timeSwapManager.SwapTimePeriodEditorOnly();
        }

        GUILayout.EndHorizontal();


    }
}
