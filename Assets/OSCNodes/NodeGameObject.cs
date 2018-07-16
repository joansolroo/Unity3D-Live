using System.Collections.Generic;

using UnityEngine;
using UnityEditor;

using UNEB;

using System.Collections;

public class NodeGameObject : Node
{
    [SerializeField] Component[] components;

    public override void Init()
    {
        base.Init();
        Bind();
    }

    public override void OnBodyGUI()
    {
        
        this.name = "GO:"+(gameObject != null ? gameObject.name : "");
        EditorGUI.BeginChangeCheck();

       // EditorGUILayout.TextField("name", go!=null?go.name:"");
        EditorGUILayout.TextField("object", gameObject!=null?GetGameObjectPath(gameObject):"null");
    
        if (GUILayout.Button(Selection.activeGameObject!=null?"Bind":"Unbind"))
        {
            Bind();
            updateOutputNodes();
        }

        if (EditorGUI.EndChangeCheck())
        {
            updateOutputNodes();
        }
        
    }

    override public void OnNewInputConnection(NodeInput addedInput) {
        updateOutputNodes();
    }

    private void Bind()
    {
        gameObject = Selection.activeGameObject;
        if (gameObject != null)
        {
            components = gameObject.GetComponents<Component>();
        }
        else
        {
            components = null;
        }

        RemoveOutputs();

        if (components != null)
        {
            foreach (Component c in components)
            {
                var comp = AddOutput();
                comp.name = c.GetType().Name;
                comp.getValue = () => { return comp; };

            }
        }
        /*var gobj = AddOutput();
        gobj.name = "GameObject";
        gobj.getValue = () => { return go; };
        */
        FitKnobs();

        bodyRect.height += 35f;
        bodyRect.width = 200f;

      

    }
    public static string GetGameObjectPath(GameObject obj)
    {
        string path = "/" + obj.name;
        while (obj.transform.parent != null)
        {
            obj = obj.transform.parent.gameObject;
            path = "/" + obj.name + path;
        }
        return path;
    }

    // Uses a simple DFS traversal to find the connected outputs.
    // Assumes a tree-like structure following output to input.
    // Does not handle cycles.
    private void updateOutputNodes()
    {
        // Temp solution.
        // var dfs = new Stack<Node>();
        //dfs.Push(this);
        Debug.Log("Pushing " + this.name);
        //while (dfs.Count != 0)
        {

            var node = this; //dfs.Pop();

            int c = 0;
            // Search neighbors
            foreach (NodeOutput outputFrom in node.Outputs)
            {
                foreach (NodeInput inputTo in outputFrom.Inputs)
                {
                    //dfs.Push(input.ParentNode);
                    if (inputTo.name == outputFrom.name)
                    {
                        var targetNode = inputTo.ParentNode as NodeComponent;
                        if (targetNode != null)
                        {
                            targetNode.Bind(components[c]);
                        }
                    }
                }
                ++c;
            }
            
        }
    }
}