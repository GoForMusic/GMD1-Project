using UnityEngine;

namespace Interfaces.Core
{
    public interface IFighter
    {
        void AttackBehavior(Animator animator, float attackInput);
        void SetEnemyTarger(GameObject target);
        string GetEnemyTag();
        GameObject GetEnemyTarget();
        void Hit(Vector3 position);
    }
}