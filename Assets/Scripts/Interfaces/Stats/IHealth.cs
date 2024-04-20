using UnityEngine;

namespace Interfaces.Stats
{
    public interface IHealth
    {
        public bool IsDead();
        void DealDamage(float damage,ref string gameObjectTag, GameObject gameObject);
        void Revive(ref string gameObjectTag);
    }
}