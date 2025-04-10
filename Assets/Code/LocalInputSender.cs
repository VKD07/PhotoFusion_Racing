using Fusion;
using UnityEngine;

namespace Code
{
    public class LocalInputSender : NetworkBehaviour
    {
        [SerializeField] private Transform _holdPosition;

        public Vector3 HoldPosition => _holdPosition.position;

        public static LocalInputSender Local; // Static ref for the local player

        public override void Spawned()
        {
            if (Object.HasInputAuthority)
            {
                Local = this;
            }
        }
    }
}