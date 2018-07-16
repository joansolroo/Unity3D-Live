using UnityEngine;
using System.Collections;

using UnityEditor;

using Pipes;

public class AnimatorParameter : Pipe
{
    [System.Serializable]
    public enum ParameterType
    {
        Int, Float, Bool, Trigger
    }
    [Header("Parameter Value")]
    [SerializeField]
    float value = 0;
    float _previousValue = 0;

    [SerializeField]
    bool changed = false;
    public Animator animator;
    public AnimatorControllerParameter parameter;
    public int parameterIdx = 0;

    public override Type type
    {
        get
        {
            return Type.IN;
        }
    }

    protected override bool Evaluate()
    {
        object obj = GetFirstInput();
        if (obj == null)
        {
            return false;
        }
        try
        {
            value = (float)obj;
        }
        catch (System.Exception e)
        {
            return false;
        }
        if (value == _previousValue)
        {
            changed = false;
        }
        else
        {
            changed = true;
            if(parameter == null)
            {
                parameter = animator.parameters[parameterIdx];
            }
            switch (parameter.type)
            {
                case (AnimatorControllerParameterType.Int):
                    animator.SetInteger(parameter.nameHash, (int)value);
                    break;
                case (AnimatorControllerParameterType.Float):
                    animator.SetFloat(parameter.nameHash, value);
                    break;
                case (AnimatorControllerParameterType.Bool):
                    animator.SetBool(parameter.nameHash, value > 0);
                    break;
                case (AnimatorControllerParameterType.Trigger):
                    if (value > 0)
                    {
                        animator.SetTrigger(parameter.nameHash);
                    }
                    break;
            }
            _previousValue = value;
        }
        return changed;
    }

    public override object GetValue()
    {
        return value;
    }
}
