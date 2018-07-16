using UnityEngine;
using UnityEditor;
using System.Reflection;

using System;

[AttributeUsage(AttributeTargets.Class)]
public class EditableUsingNodes : Attribute { }

[AttributeUsage(AttributeTargets.Field)]
public class NodeInput : Attribute { }
[AttributeUsage(AttributeTargets.Field)]
public class NodeOutput : Attribute { }


public class AutoNode : MonoBehaviour
{
    private void Start()
    {
        MonoBehaviour[] sceneActive = FindObjectsOfType<MonoBehaviour>();

        foreach (MonoBehaviour mono in sceneActive)
        {
            FieldInfo[] objectFields = mono.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public);
            for (int i = 0; i < objectFields.Length; i++)
            {
                NodeInput attribute = System.Attribute.GetCustomAttribute(objectFields[i], typeof(NodeInput)) as NodeInput;
                if (attribute != null)
                    Debug.Log(objectFields[i].Name); // The name of the flagged variable.
            }
        }
    }
}
