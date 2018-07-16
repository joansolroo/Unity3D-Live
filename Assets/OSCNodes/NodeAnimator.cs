using System.Collections.Generic;

using UnityEngine;
using UnityEditor;

using UNEB;

using System.Collections;
using Pipes;

public abstract class NodeComponent : NodePipe
{
    public abstract bool Bind(Component component);
}

public class NodeAnimatorParameter : NodeComponent
{
    [SerializeField] public AnimatorParameter parameter;

    public override void Init()
    {

        var gobj = AddInput();
        gobj.name = "Animator";

        var value = AddInput();
        value.name = "Value";

        FitKnobs();

        bodyRect.height += 35f;
        bodyRect.width = 200f;
    }

    public override void OnBodyGUI()
    {

        this.name = "Animator:" + (gameObject != null ? gameObject.name : "");
        EditorGUI.BeginChangeCheck();

        if (parameter != null)
        {
            Animator animator = parameter.animator;
            if (animator != null)
            {
                //EditorGUILayout.TextField("name", animator != null ? "" + animator.runtimeAnimatorController : "");

                //EditorGUILayout.Toggle("Has animator", animator!=null);

                int h = 0;
                {
                    string[] options = new string[animator.parameterCount];
                    // animator.speed = EditorGUILayout.Slider(animator.speed, 0, 2);
                    for (int c = 0; c < animator.parameterCount; ++c)
                    {
                        options[c] = animator.parameters[c].name;
                    }

                    parameter.parameterIdx = EditorGUILayout.Popup("Label", parameter.parameterIdx, options);
                    //EditorGUILayout.LabelField("Type", "" + animator.parameters[parameter.parameterIdx].type);

                    AnimatorControllerParameter p = animator.parameters[parameter.parameterIdx];
                    // foreach (AnimatorControllerParameter p in animator.parameters)
                    {
                        switch (p.type)
                        {
                            case (AnimatorControllerParameterType.Int):
                                animator.SetInteger(p.nameHash, EditorGUILayout.IntField(p.name, animator.GetInteger(p.nameHash)));
                                break;
                            case (AnimatorControllerParameterType.Float):
                                animator.SetFloat(p.nameHash, EditorGUILayout.FloatField(p.name, animator.GetFloat(p.nameHash)));
                                break;
                            case (AnimatorControllerParameterType.Bool):
                                animator.SetBool(p.nameHash, EditorGUILayout.Toggle(p.name, animator.GetBool(p.nameHash)));
                                break;
                            case (AnimatorControllerParameterType.Trigger):
                                if (EditorGUILayout.Toggle(p.name, false))
                                {
                                    animator.SetTrigger(p.nameHash);
                                }
                                break;
                        }
                        h++;
                    }
                    bodyRect.height = kDefaultSize.y + h * 10;
                }
            }
        }
        else
        {
            if (GUILayout.Button("Bind"))
            {
                gameObject = Selection.activeGameObject;
                Bind();
            }

        }

        if (EditorGUI.EndChangeCheck())
        {
            
        }
    }
    void Bind()
    {
        if (gameObject != null)
        {
            parameter = gameObject.GetComponent<AnimatorParameter>();
        }
    }

    void CheckConnections()
    {
        var node = this;

        // Search neighbors forward
        foreach (var output in node.Outputs)
        {
            foreach (var input in output.Inputs)
            {
                //dfs.Push(input.ParentNode);
            }
        }
    }
    override public bool Bind(Component component)
    {
        Debug.Log("Binding " + component);
        if (component.GetType() == typeof(Animator))
        {
            Animator _animator = component as Animator;
            gameObject = _animator.gameObject;
            parameter.animator = _animator;

            return true;
        }
        else
        {
            return false;
        }
    }

    public override Pipe GetPipe()
    {
        return parameter;
    }
}

/*
public class NodeAnimator : NodeComponent
{
    [SerializeField] Animator animator;

    public override void Init()
    {

        var gobj = AddInput();
        gobj.name = "Animator";

        var speed = AddInput();
        speed.name = "speed";

        FitKnobs();
        
        bodyRect.height += 35f;
        bodyRect.width = 300f;
    }

    public override void OnBodyGUI()
    {
        this.name = "Animator:" + (go != null ? go.name : "");
        EditorGUI.BeginChangeCheck();

        EditorGUILayout.TextField("name", animator != null ? ""+animator.runtimeAnimatorController : "");
        //EditorGUILayout.Toggle("Has animator", animator!=null);
        
       
        int h = 0;
        if (animator != null)
        {
            
           
            bodyRect.height = kDefaultSize.y + 40;
        }

        if (EditorGUI.EndChangeCheck())
        {
            //updateOutputNodes();
        }
    }

    override public bool Bind(Component component)
    {
        Debug.Log("Binding " + component);
        if (component.GetType() == typeof(Animator))
        {
            Animator _animator = component as Animator;
            go = _animator.gameObject;
            this.animator = _animator;

            return true;
        }
        else
        {
            return false; 
        }
    }
}*/
