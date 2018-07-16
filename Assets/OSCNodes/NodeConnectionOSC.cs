using System.Collections.Generic;

using UnityEngine;
using UnityEditor;

using UNEB;

using OSC;
using System.Collections;

public abstract class NodePipe : Node
{
    public abstract Pipes.Pipe GetPipe();

    public override void OnInputConnectionRemoved(NodeInput removedInput)
    {
        var ipipe = removedInput.ParentNode as NodePipe;
        if (ipipe != null && ipipe.GetPipe().HasInput(this.GetPipe()))
        {
            Debug.Log(" Unplug " + ipipe.GetPipe().name + " from " + this.GetPipe().name);
            ipipe.GetPipe().UnplugInput(this.GetPipe());
        }
        else
        {
            Debug.Log("something else got unplugged");
        }
    }

    public override void OnNewInputConnection(NodeInput addedInput)
    {
        var ipipe = addedInput.ParentNode as NodePipe;
        if (ipipe != null && !ipipe.GetPipe().HasInput(this.GetPipe()))
        {
            Debug.Log(" Plug " + ipipe.GetPipe().name + " from " + this.GetPipe().name);
            ipipe.GetPipe().PlugInput(this.GetPipe());
        }
        else
        {
            Debug.Log("something else got plugged");
        }
    }

}
public class NodeChannelInOSC : NodePipe
{
    //[SerializeField] GameObject gameObject;

    [SerializeField] Connection connection;
    [SerializeField] ChannelIn channel;

    NodeInput IReceiver;
    NodeOutput OValue;

    private void Awake()
    {
        Bind();
    }
    public override void Init()
    {

        IReceiver = AddInput();
        IReceiver.name = "Receiver";
        IReceiver.getValue = () => { return connection.receiver; };

        OValue = AddOutput();
        OValue.name = "Value";
        OValue.getValue = () => { return channel.value; };

        FitKnobs();

        bodyRect.height += 35f;
        bodyRect.width = 200f;
    }
    public void Update()
    {
        OnBodyGUI();
    }
    public override void OnBodyGUI()
    {
        EditorGUI.BeginChangeCheck();

        if (gameObject != null && channel == null)
        {
            Bind();
        }
        if (channel != null)
        {
            channel.address = EditorGUILayout.TextField("Channel", channel.address);
            EditorGUILayout.TextField("Value", "" + channel.value);
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
            // updateOutputNodes();
        }
    }

    void Bind()
    {
        if (gameObject != null)
        {
            channel = gameObject.GetComponent<ChannelIn>();
        }
    }
    public override Pipes.Pipe GetPipe()
    {
        return channel;
    }

}
public class NodeConnectionOSC : Node
{
    //[SerializeField] GameObject gameObject;
    [SerializeField] Connection connection;

    private void Awake()
    {
        Bind();
    }
    public override void Init()
    {
        var sender = AddInput();
        sender.name = "Sender";
        sender.getValue = () => { return connection.sender; };

        var Receiver = AddOutput();
        Receiver.name = "Receiver";
        Receiver.getValue = () => { return connection.receiver; };

        var isConnectedOut = AddOutput();
        isConnectedOut.name = "Connected?";
        isConnectedOut.getValue = () => { return connection.IsConnected(); };

        FitKnobs();

        bodyRect.height += 95f;
        bodyRect.width = 200f;
    }

    public override void OnBodyGUI()
    {
        EditorGUI.BeginChangeCheck();

        if (gameObject != null && connection == null)
        {
            Bind();
        }
        if (connection != null)
        {
            connection.RemoteIP = EditorGUILayout.TextField("IP", connection.RemoteIP);
            connection.SendToPort = EditorGUILayout.IntField("Target Port", connection.SendToPort);
            connection.ListenerPort = EditorGUILayout.IntField("Listener Port", connection.ListenerPort);
            EditorGUILayout.Toggle("Connected", connection.IsConnected());

            if (EditorApplication.isPlaying)
            {
                if (!connection.IsConnected())
                {
                    if (GUILayout.Button("Connect"))
                    {
                        connection.Connect();
                    }
                }
                else
                {
                    if (GUILayout.Button("Disconnect"))
                    {
                        connection.Disconnect();
                    }
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
            updateOutputNodes();
        }
    }
    void Bind()
    {
        if (gameObject != null)
        {
            Connection con = gameObject.GetComponent<Connection>();
            if (con != null)
                connection = con;
        }

    }
    // Uses a simple DFS traversal to find the connected outputs.
    // Assumes a tree-like structure following output to input.
    // Does not handle cycles.
    private void updateOutputNodes()
    {
        // Temp solution.
        //var dfs = new Stack<Node>();

        //dfs.Push(this);

        //while (dfs.Count != 0)
        //{

        var node = this;

        // Search neighbors
        foreach (var output in node.Outputs)
        {
            foreach (var input in output.Inputs)
            {
                //dfs.Push(input.ParentNode);
            }
        }
        // }
    }
}