using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;

[RequireComponent(typeof(Animator))]
public class AutoAnimatorParameter : MonoBehaviour
{

    [SerializeField] [HideInInspector] public AnimatorParameter animatorParameter;
    [SerializeField] [HideInInspector] public Animator animator;
    [SerializeField] [HideInInspector] public int parameterIdx = 0;
    [SerializeField] [HideInInspector] public string parameterName;
    [SerializeField] [HideInInspector] public AnimatorParameter.ParameterType parameterType;
    [SerializeField] [HideInInspector] public string channelName;
    [SerializeField] [HideInInspector] public AnimationCurve remap;

    public AutoAnimatorParameterEditor inspector;

    private void OnValidate()
    {
        animator = this.GetComponent<Animator>();
        Setup();
    }

    public void Setup()
    {
        if (remap == null || remap.keys.Length <2)
        {
            remap.keys = new Keyframe[]{ new Keyframe(0,0),new Keyframe(1,1)};
        }
        AnimatorParameter previous = animatorParameter;
        animatorParameter = null;
        for (int c = 0; c < transform.childCount; ++c)
        {
            AnimatorParameter ap = transform.GetChild(c).gameObject.GetComponent<AnimatorParameter>();
            if (ap != null && ap.ParameterName == parameterName)
            {
                animatorParameter = ap;
                break;
            }
        }
        if (animatorParameter == null)
        {
           // Debug.LogWarning("not found, creating new");
            GameObject apgo = new GameObject();
            AnimatorParameter ap = apgo.AddComponent<AnimatorParameter>();
            apgo.transform.parent = this.transform;

            ap.ParameterName = parameterName;
            animatorParameter = ap;
            animatorParameter.parameterType = parameterType;
           
        }
        animatorParameter.Animator = animator;

        GameObject nxgo;
        if (animatorParameter.transform.childCount == 0)
        {
            nxgo = new GameObject();
            nxgo.transform.parent = animatorParameter.transform;
        }
        else
        {
            nxgo = animatorParameter.transform.GetChild(0).gameObject;
        }
        NexusOut nx = nxgo.GetComponent<NexusOut>();
        if (nx == null)
        {
            nx = nxgo.AddComponent<NexusOut>();
        }
        nx.Address = channelName; 

        if(previous!=null && previous != animatorParameter)
        {
            StartCoroutine(DestroyWhenPossible(previous.gameObject));
        }
    }

    IEnumerator DestroyWhenPossible(GameObject go)
    {
        yield return new WaitForSecondsRealtime(0.01f);
        DestroyImmediate(go);
    }

    float previous = 0;
    [SerializeField]public float smoothing = 0;
    public float inValue =0;
    public float outValue = 0;

    bool initialized = false;
    private void Update()
    {
        float previous = inValue;
        inValue = (float)animatorParameter.GetValue();
        inValue = remap.Evaluate(inValue);
        inValue = Mathf.MoveTowards(previous, inValue, Mathf.Abs(inValue - previous) * (1 - smoothing));
        if (!initialized || previous != inValue)
        {
            previous = inValue;
            switch (parameterType)
            {
                case (AnimatorParameter.ParameterType.Int):
                    outValue = (int)(inValue * 127);
                    animator.SetInteger(parameterName, (int)outValue);
                    break;
                case (AnimatorParameter.ParameterType.Float):
                    outValue = inValue;
                    animator.SetFloat(parameterName, outValue);
                    break;
                case (AnimatorParameter.ParameterType.Bool):
                    outValue = inValue > 0.5f ? 1 : 0;
                    animator.SetBool(parameterName, outValue > 0.5f);
                    break;
                case (AnimatorParameter.ParameterType.Trigger):
                    outValue = inValue > 0 ? 1 : 0;
                    if (outValue > 0)
                    {
                        animator.SetTrigger(parameterName);
                    }
                    break;
            }
           // Debug.Log(parameterName + ":" + outValue);
            initialized = true;

           // Debug.Log("Upddate" + ", From channel:" + inValue + ", To animator:" + outValue);
           
        }
    }
}




[CustomEditor(typeof(AutoAnimatorParameter))]
public class AutoAnimatorParameterEditor : Editor
{
    AutoAnimatorParameter myTarget;
    void Awake()
    {
        myTarget = (AutoAnimatorParameter)target;
        myTarget.inspector = this;
    }
    public override void OnInspectorGUI()
    {
        string[] options = new string[myTarget.animator.parameterCount];
        for (int p = 0; p < myTarget.animator.parameterCount; ++p)
        {
            options[p] = myTarget.animator.GetParameter(p).name;
        }
        myTarget.parameterIdx = EditorGUILayout.Popup("Parameter", myTarget.parameterIdx, options);
        if (myTarget.parameterIdx < options.Length)
        {
            myTarget.parameterName = options[myTarget.parameterIdx];
            myTarget.parameterType = AnimatorParameter.Cast(myTarget.animator.GetParameter(myTarget.parameterIdx).type);

            //EditorGUILayout.ObjectField("current pipe:", myTarget.animatorParameter, typeof(AnimatorParameter), true);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("Channel");
            myTarget.channelName = GUILayout.TextField(myTarget.channelName);
            EditorGUILayout.EndHorizontal();

            myTarget.remap = EditorGUILayout.CurveField("Remap:", myTarget.remap);
            myTarget.smoothing = EditorGUILayout.Slider("Smoothing:", myTarget.smoothing, 0, 1);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("From channel:");
            GUILayout.TextField(myTarget.inValue.ToString());
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("To animator:");
            GUILayout.TextField(myTarget.outValue.ToString());
            EditorGUILayout.EndHorizontal();
            //Debug.Log("OninspectorGUI" + ", From channel:" + myTarget.inValue + ", To animator:" + myTarget.outValue);

            myTarget.Setup();
        }
        EditorUtility.SetDirty(myTarget);
    }
}
