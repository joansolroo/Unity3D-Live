using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;
using OSC;

public class AutoOSCIn : MonoBehaviour
{
    [SerializeField] public int channels = 4;
    [SerializeField] public List<string> OSCChannelIn = new List<string>();
    [SerializeField] public List<string> LocalName = new List<string>();

    public void OnValidate()
    {
        Debug.Log("OnValidate:" + channels);

        /*
        if (OSCChannelIn == null || OSCChannelIn.Count == 0)
        {
            OSCChannelIn = new List<string>();
            LocalName = new List<string>();
            for (int c = 0; c < channels; ++c)
            {
                OSCChannelIn.Add("/OSC/??");
                LocalName.Add("local_" + c);
            }
        }*/
    }
    private void Awake()
    {
        for (int c = 0; c < OSCChannelIn.Count; ++c)
        {
            GameObject chgo = new GameObject();
            chgo.transform.parent = this.transform;
            ChannelIn chin = chgo.AddComponent<ChannelIn>();
            chin.address = OSCChannelIn[c];

            GameObject nxgo = new GameObject();
            NexusIn nx = nxgo.AddComponent<NexusIn>();
            nx.Address = LocalName[c];
            nx.transform.parent = chgo.transform;
        }
    }

}



[CustomEditor(typeof(AutoOSCIn))]
public class AutoOSCInEditor : Editor
{
    AutoOSCIn myTarget;
    SerializedProperty pChannels;

    private void Awake()
    {
        myTarget = (AutoOSCIn)target;
        pChannels = serializedObject.FindProperty("channels");
    }
    public override void OnInspectorGUI()
    {
       
        
        if (serializedObject != null)
        {
            myTarget.channels = EditorGUILayout.IntField("Channel count", myTarget.channels);

            // Keeps the right amount of channels
            while (myTarget.OSCChannelIn.Count > myTarget.channels)
            {
                myTarget.OSCChannelIn.RemoveAt(myTarget.channels);
                myTarget.LocalName.RemoveAt(myTarget.channels);
            }
            while (myTarget.OSCChannelIn.Count < myTarget.channels)
            {
                myTarget.OSCChannelIn.Add("/OSC/??");
                myTarget.LocalName.Add("local_" + (myTarget.OSCChannelIn.Count));
            }

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("Channel");
            EditorGUILayout.LabelField("OSC Channel", "Local channel");
            EditorGUILayout.EndHorizontal();
            for (int idx = 0; idx < myTarget.channels; ++idx)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PrefixLabel("C" + idx);
                //selection = EditorGUILayout.Popup("Input", selection, options);
                myTarget.OSCChannelIn[idx] = GUILayout.TextField(myTarget.OSCChannelIn[idx]);
                myTarget.LocalName[idx] = GUILayout.TextField(myTarget.LocalName[idx]);
                EditorGUILayout.EndHorizontal();
            }
            EditorUtility.SetDirty(myTarget);
        }
    }
}

