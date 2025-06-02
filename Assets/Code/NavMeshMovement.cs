using System;
using UnityEngine;
using UnityEngine.AI;

namespace Code
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class NavMeshMovement : MonoBehaviour, IMovementBehavior
    {
        private NavMeshAgent _navAgent;

        private void Awake()
        {
            _navAgent = GetComponent<NavMeshAgent>();
        }

        public void MoveTo(Vector3 destination)
        {
            _navAgent.SetDestination(destination);
        }

        public void Stop()
        {
            _navAgent.ResetPath();
        }
    }
}