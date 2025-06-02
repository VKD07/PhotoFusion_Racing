using UnityEngine;

namespace Code
{
    public interface IMovementBehavior
    {
        public void MoveTo(Vector3 destination);
        public void Stop();
    }
}