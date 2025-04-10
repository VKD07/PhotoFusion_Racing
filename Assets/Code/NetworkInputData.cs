using Fusion;
using UnityEngine;

namespace Code
{
    public struct NetworkInputData : INetworkInput
    {
        public Vector3 direction;
        public float mouseXRotation;
        public float mouseYRotation;
        public float currentSpeed;
        
        public Vector3 holdPosition;
        
        public float horizontal;
        public float vertical;
        public const byte SPACEBUTTON = 1;
        public const byte INTERACTBUTTON = 2;

        public NetworkButtons buttons;
    }
}