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


        #region Healing & Damage
        public abstract float healthMax { get; set; }
        
        public abstract float health { get; set; }
        public virtual bool needsHealth { get; set; } = false;
        
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
            if (!canTakeDamage) return;
            health -= damage;
            if (health <= healthMax / 3) needsHealth = true;
            if (health <= 0)
                Destroy(this.gameObject);
            canTakeDamage = false;
            StartCoroutine(ResetDamageCooldown());
        }
        
        private IEnumerator ResetDamageCooldown()
        {
            yield return new WaitForSeconds(0.2f);
            canTakeDamage = true;
        }

        #endregion
        
        
        
    }
}
