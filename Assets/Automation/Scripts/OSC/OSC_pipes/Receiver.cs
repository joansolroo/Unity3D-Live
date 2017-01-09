using UnityEngine;
using System.Collections;

using System;
using System.Collections.Generic;

namespace OSC
{
    public class Receiver : Communicator
    {

        // This method sets the communication (in this case, just receiving messages)
        // and setting the default values per channel
        override protected void InitCommunicator()
        {
            handler.SetAllMessageHandler(AllMessageHandler);
        }

        public void AllMessageHandler(OscMessage oscMessage)
        {

            var msgAddress = oscMessage.Address; //the message parameters
            object msgValue = null;
            if (oscMessage.Values != null && oscMessage.Values.Count > 0)
            {
                msgValue = oscMessage.Values[0]; //the message value
            }
            //var msgString = Osc.OscMessageToString(oscMessage); //the message and value combined
            //Debug.Log(msgString); //log the message and values coming from OSC

            values[msgAddress] = msgValue == null ? 0 : msgValue;
        }

        public void AddChannel(string _address, object defaultValue)
        {
            values[_address]= defaultValue;
        }
        /*
        void OnValidate()
        {
            gameObject.name = "Receiver p:" + gameObject.GetComponent<Connection>().ListenerPort;

        }*/
    }

}