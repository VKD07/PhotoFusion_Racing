using UnityEngine;

namespace Code
{
    public interface IPullable
    {
        public void PullTowards(Vector3 pullPoint);
    }
}
