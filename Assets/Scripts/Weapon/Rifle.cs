using Factory;
using UnityEngine;



namespace Weapon
{
    public class Rifle : WeaponFactory
    {
        
        [SerializeField, Header("Time before new bullet")] private float shotCooldown = 2f;
        [SerializeField, Header("Bullet Settings")] private GameObject bulletPrefab;
        
        public override float timeUntilNextShot { get; set; }
        public override bool canShoot { get; set; }
        
        
        private void Awake()
        {
            timeUntilNextShot = shotCooldown;
            canShoot =  true;
        }
        

        public override void Shoot()
        {
            if (canShoot)
            {
                //BulletSpawn(bulletPrefab, transform.position,  transform.rotation);
                
                // TODO: Add in WeaponFactory a function that spawns in new bullets and fly's towards destination
                Debug.LogWarning($"Shoot from {name}");
                canShoot = false;
            }

        }

    }
}