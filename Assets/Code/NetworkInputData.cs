using Fusion;
using UnityEngine;

namespace Code
{
    public struct NetworkInputData : INetworkInput
    {
        public Vector3 direction;
        public float horizontal;
        public float vertical;
        public const byte SPACEBUTTON = 1;

        public NetworkButtons buttons;
    }
}