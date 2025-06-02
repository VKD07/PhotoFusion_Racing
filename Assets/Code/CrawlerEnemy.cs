using UnityEngine;

namespace Code
{
    public class CrawlerEnemy : EnemyBase
    {
        public override void Spawned()
        {
            base.Spawned();
            Vector3 targetPos = transform.position + transform.forward * 50f;
            SetDestination(targetPos);
        }
    }
}