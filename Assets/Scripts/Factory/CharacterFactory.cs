using Common;
using Common.Interfaces;
using UnityEngine;
using Weapon;

namespace Factory
{
    public abstract class CharacterFactory : MonoBehaviour, IHealth
    {
        public abstract float health { get; set; }
        public abstract float viewingDistance { get; set; }
        public abstract float healthMax { get; set; }
        public virtual float speed { get; set; }
        
        public abstract LayerMask layerMask { get; set; }


        public virtual void TakeHeal(float healing)
        {
            health += healing;
            if(health > healthMax)
                health = healthMax;
        }

        public virtual void TakeDamage(float damage)
        {
            health -= damage;
            if (health <= 0)
                Destroy(this.gameObject);
        }
        
        
    }
}
