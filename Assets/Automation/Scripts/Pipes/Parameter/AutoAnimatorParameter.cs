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
    private void Update()
    {
        float value = (float)animatorParameter.GetValue();
        value = remap.Evaluate(value);
        //value = Mathf.MoveTowards(previous, value, Mathf.Abs(value - previous) * (1-smoothing));
       // previous = value;
        switch (parameterType)
        {
            case (AnimatorParameter.ParameterType.Int):
                animator.SetInteger(parameterName, (int)value);
                break;
            case (AnimatorParameter.ParameterType.Float):
                animator.SetFloat(parameterName, value);
                break;
            case (AnimatorParameter.ParameterType.Bool):
                animator.SetBool(parameterName, value > 0);
                break;
            case (AnimatorParameter.ParameterType.Trigger):
                if (value > 0)
                {
                    animator.SetTrigger(parameterName);
                }
                break;
        }
    }
}




[CustomEditor(typeof(AutoAnimatorParameter))]
public class AutoAnimatorParameterEditor : Editor
{
    public override void OnInspectorGUI()
    {
        
        AutoAnimatorParameter myTarget = (AutoAnimatorParameter)target;

        string[] options = new string[myTarget.animator.parameterCount];
        for (int p = 0; p < myTarget.animator.parameterCount; ++p)
        {
            options[p] = myTarget.animator.GetParameter(p).name;
        }
        myTarget.parameterIdx = EditorGUILayout.Popup("Parameter", myTarget.parameterIdx, options);
        myTarget.parameterName = options[myTarget.parameterIdx];
        myTarget.parameterType = AnimatorParameter.Cast(myTarget.animator.GetParameter(myTarget.parameterIdx).type);

        //EditorGUILayout.ObjectField("current pipe:", myTarget.animatorParameter, typeof(AnimatorParameter), true);

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel("Channel");
        myTarget.channelName = GUILayout.TextField(myTarget.channelName);
        EditorGUILayout.EndHorizontal();

        myTarget.remap =  EditorGUILayout.CurveField("Remap:",myTarget.remap);
        myTarget.smoothing = EditorGUILayout.Slider("Smoothing:", myTarget.smoothing, 0 ,1);

        myTarget.Setup();

    }
}
