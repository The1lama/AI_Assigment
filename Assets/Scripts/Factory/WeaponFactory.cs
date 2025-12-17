using UnityEngine;

namespace Factory
{
   // interface IWeapon
   // {
   //     public float TimeUntilNextShot { get; set; }
   //     public bool canShoot { get; set; }
   // }
    
    
    public abstract class WeaponFactory : MonoBehaviour
    {
        private float cooldownTimer = 0f;
        
        public abstract float timeUntilNextShot { get; set; }
        public abstract bool canShoot { get; set; }
        public abstract void Shoot();

        public void Update()
        {
            cooldownTimer -= Time.deltaTime;
            if (!(cooldownTimer <= 0)) return;
            
            
            canShoot = true;
            cooldownTimer = timeUntilNextShot;
        }

        
        
        /// <summary>
        /// Spawns bullet from the weapon that shoot it
        /// </summary>
        /// <param name="bulletPrefab">GameObject to instansiate</param>
        /// <param name="position">Position where to spawn the bullet</param>
        /// <param name="rotation">What rotation should it have</param>
        protected virtual void BulletSpawn(GameObject bulletPrefab, Vector3 position, Quaternion rotation)
        {
            //TODO: Add bullet force and Instantiate that shit
            
            //GameObject bullet = Instantiate(bulletPrefab, position, rotation);
            
            Debug.DrawRay(position, position, Color.red, 0.5f);
            
            
        }
        
        
    }
}
