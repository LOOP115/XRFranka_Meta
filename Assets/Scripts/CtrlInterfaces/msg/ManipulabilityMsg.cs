//Do not edit! This file was generated by Unity-ROS MessageGeneration.
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Unity.Robotics.ROSTCPConnector.MessageGeneration;

namespace RosMessageTypes.CtrlInterfaces
{
    [Serializable]
    public class ManipulabilityMsg : Message
    {
        public const string k_RosMessageName = "ctrl_interfaces/Manipulability";
        public override string RosMessageName => k_RosMessageName;

        public double value;

        public ManipulabilityMsg()
        {
            this.value = 0.0;
        }

        public ManipulabilityMsg(double value)
        {
            this.value = value;
        }

        public static ManipulabilityMsg Deserialize(MessageDeserializer deserializer) => new ManipulabilityMsg(deserializer);

        private ManipulabilityMsg(MessageDeserializer deserializer)
        {
            deserializer.Read(out this.value);
        }

        public override void SerializeTo(MessageSerializer serializer)
        {
            serializer.Write(this.value);
        }

        public override string ToString()
        {
            return "ManipulabilityMsg: " +
            "\nvalue: " + value.ToString();
        }

#if UNITY_EDITOR
        [UnityEditor.InitializeOnLoadMethod]
#else
        [UnityEngine.RuntimeInitializeOnLoadMethod]
#endif
        public static void Register()
        {
            MessageRegistry.Register(k_RosMessageName, Deserialize);
        }
    }
}
