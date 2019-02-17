using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;
using OSC;

public class AutoOSCOut : MonoBehaviour
{

    [SerializeField] public int channels = 4;
    [SerializeField] public List<string> OSCChannelOut = new List<string>();
    [SerializeField] public List<string> LocalName = new List<string>();
    /*
    public void OnValidate()
    {
        if (OSCChannelOut == null || OSCChannelOut.Count == 0)
        {
            OSCChannelOut = new List<string>();
            LocalName = new List<string>();
            for (int c = 0; c < channels; ++c)
            {
                OSCChannelOut.Add("/OSC/??");
                LocalName.Add("local_" + c);
            }
        }
    }*/
    private void Awake()
    {
        for (int c = 0; c < OSCChannelOut.Count; ++c)
        {
            GameObject chgo = new GameObject();
            chgo.transform.parent = this.transform;
            ChannelOut chout = chgo.AddComponent<ChannelOut>();
            chout.address = OSCChannelOut[c];

            GameObject nxgo = new GameObject();
            NexusOut nx = nxgo.AddComponent<NexusOut>();
            nx.Address = LocalName[c];
            nx.transform.parent = chgo.transform;
        }
    }

}



[CustomEditor(typeof(AutoOSCOut))]
public class AutoOSCOutEditor : Editor
{
    public override void OnInspectorGUI()
    {

        AutoOSCOut myTarget = (AutoOSCOut)target;

        myTarget.channels = EditorGUILayout.IntField("Channel count", myTarget.channels);

        // Keeps the right amount of channels
        while (myTarget.OSCChannelOut.Count > myTarget.channels)
        {
            myTarget.OSCChannelOut.RemoveAt(myTarget.channels);
            myTarget.LocalName.RemoveAt(myTarget.channels);
        }
        while (myTarget.OSCChannelOut.Count < myTarget.channels)
        {
            myTarget.OSCChannelOut.Add("/OSC/??");
            myTarget.LocalName.Add("local_" + (myTarget.OSCChannelOut.Count));
        }

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel("Channel");
        EditorGUILayout.LabelField("Local Channel", "OSC channel");
        EditorGUILayout.EndHorizontal();
        for (int idx = 0; idx < myTarget.channels; ++idx)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("C" + idx);
            //selection = EditorGUILayout.Popup("Input", selection, options);
            myTarget.LocalName[idx] = GUILayout.TextField(myTarget.LocalName[idx]);
            myTarget.OSCChannelOut[idx] = GUILayout.TextField(myTarget.OSCChannelOut[idx]);
           
            EditorGUILayout.EndHorizontal();
        }

        EditorUtility.SetDirty(myTarget);
    }
}


