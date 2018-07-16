﻿using UnityEngine;
using System.Collections;
using System;

using UnityEditor;
using Pipes;

namespace OSC
{
    public class ChannelIn : Pipes.Pipe
    {

        [Header("Setup")]
        public OSC.Receiver receiver;
        public string address;

        public bool isTrigger = false; //Handle this with types

        [Header("Value")]
        public float defaultValue = 0;
        public float value = 0;

        void LateUpdate()
        {
            if (isTrigger)
            {
                value = defaultValue;
            }
        }

        public float GetOutput()
        {
            if (receiver != null)
            {
                object msgValue = receiver.GetValue(address);
                if (msgValue != null)
                {
                    if (msgValue.GetType() == typeof(int))
                    {
                        return (int)msgValue;
                    }
                    else if (msgValue.GetType() == typeof(float))
                    {
                        return (float)msgValue;
                    }
                }
            }
            return defaultValue;
        }
        protected void Start()
        {
            //parent = transform.parent.gameObject.GetComponent<Connector>();
            InitializeConnections();
            //AutoPlug();
        }
        /*
        protected override void InitializePipe()
        {
            _direction = Pipes.Direction.DOWNWARDS;
            _type = Pipes.Type.OUT;

        }
        */
        protected void InitializeConnections()
        {
            //receiver = transform.parent.gameObject.GetComponent<Receiver>();
            receiver.AddChannel(address, defaultValue);
        }
        

        float prev = 0;

        public override Pipes.Type type
        {
            get
            {
                return Pipes.Type.OUT;
            }
        }

        protected override bool Evaluate()
        {
            object obj = receiver.GetValue(address);
            if (obj == null)
            {
                return false;
            }
            value = (float)obj;

            bool change = prev != value;
            if (change)
            {
                prev = value;
            }
            return change;
        }

        public override object GetValue()
        {
            return value;
        }
    }

}