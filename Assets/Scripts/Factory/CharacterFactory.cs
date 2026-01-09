using System;
using System.Collections;
using Common;
using Common.Interfaces;
using UnityEngine;
using Weapon;

namespace Factory
{
    public abstract class CharacterFactory : MonoBehaviour, IHealth
    {
        public abstract float viewingDistance { get; set; }
        public virtual float speed { get; set; }
        
        public abstract LayerMask layerMask { get; set; }

        internal Quaternion _rotateGoal;


        #region Healing & Damage
        public abstract float healthMax { get; set; }
        
        public abstract float health { get; set; }
        public virtual bool needsHealth { get; set; } = false;
        internal bool seperationOverride = false;
        
        [HideInInspector]public bool canTakeHeal = true;
        [HideInInspector]public bool canTakeDamage = true;

        public virtual void TakeHeal(float healing)
        {
            if (!canTakeHeal) return;
            health += healing;
            if (health >= healthMax)
            {
                health = healthMax;
                needsHealth = false;
                seperationOverride = true;
                
            }
            Debug.Log(health);

            canTakeHeal = false;
            StartCoroutine(ResetHealCooldown());
        }

        private IEnumerator ResetHealCooldown()
        {
            yield return new WaitForSeconds(0.2f);
            canTakeHeal = true;
        }

        public virtual void TakeDamage(float damage)
        {
            health -= damage;
            Debug.Log($"{name} Took Damage; Health: {health}");
            if (health <= healthMax / 3) needsHealth = true;
            if (health <= 0)
                Destroy(this.gameObject);
        }
        

        #endregion
        
        internal void RotateTowards()
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, _rotateGoal, speed*Time.deltaTime);
        }

        /// <summary>
        /// Set a rotation goal 
        /// </summary>
        /// <param name="targetPosition">the targets Vector3</param>
        public void SetVectorRotateTarget(Vector3 targetPosition)
        {
            Vector3 directionToTarget = targetPosition - transform.position;
            _rotateGoal = Quaternion.LookRotation(directionToTarget.normalized, Vector3.up);
        }

        public void SetQuaternionRotation(Quaternion rotation)
        {
            _rotateGoal = rotation;
        }
        
        
        
        /// <summary>
        /// Retuns an angle from original pos to target
        /// </summary>
        /// <param name="orginPos">Send in forward</param>
        /// <param name="targetPosition">Send in a normalized</param>
        /// <returns>Angle towards target</returns>
        public float AngleToTarget(Vector3 orginPos, Vector3 targetPosition)
        {
            return Vector3.Dot(orginPos, targetPosition);
        }
    }
}
