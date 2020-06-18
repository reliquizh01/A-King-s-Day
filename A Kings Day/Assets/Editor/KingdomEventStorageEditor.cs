using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using KingEvents;

[CustomEditor(typeof(KingdomEventStorage))]
public class KingdomEventStorageEditor : Editor
{
    static List<string> choices;
    [SerializeField]static int curChoice;
    KingdomEventStorage myStorage;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        this.serializedObject.Update();
        var someClass = target as KingdomEventStorage;
        if(choices == null)
        {
            choices = new List<string>();
        }
        if(choices.Count < someClass.storyArcEvents.Count || choices.Count > someClass.storyArcEvents.Count)
        {
            choices.Clear();
            for (int i = 0; i < someClass.storyArcEvents.Count; i++)
            {
                choices.Add(someClass.storyArcEvents[i].storyTitle);
            }

            if(curChoice < 0)
            {
                curChoice = 0;
            }
        }

        curChoice = someClass.storyArcEvents.FindIndex(x => x.storyTitle == someClass.arcForced);
        curChoice = EditorGUILayout.Popup("Story Arc",curChoice, choices.ToArray());
        someClass.arcForced = choices[curChoice];

        if (someClass.forceDifficulty || someClass.forceArc)
        {
            someClass.EnableDebugging = true;
        }
        else if (!someClass.forceDifficulty && !someClass.forceArc)
        {
            someClass.EnableDebugging = false;
            someClass.Difficultyforced = (DifficultyType)0;

            if (someClass.storyArcEvents != null && someClass.storyArcEvents.Count > 0)
            {
                someClass.arcForced = someClass.storyArcEvents[0].storyTitle;
            }
        }
        if(!someClass.forceDifficulty)
        {
            someClass.Difficultyforced = (DifficultyType)0;
        }
        if(!someClass.forceArc)
        {
            if(someClass.storyArcEvents != null && someClass.storyArcEvents.Count > 0)
            {
                someClass.arcForced = someClass.storyArcEvents[0].storyTitle;
            }
            someClass.initialEventOnly = false;
        }


        EditorUtility.SetDirty(someClass);
    }

}
