using System;
using Factory;
using UnityEngine;

namespace Weapon
{
    public class Pistol : WeaponFactory
    {
        [SerializeField, Header("Time before new bullet")] private float shotCooldown = 2f;
        [SerializeField, Header("Bullet Settings")] private GameObject bulletPrefab;
        [SerializeField] private Transform barrelEnd;
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
                BulletSpawn(bulletPrefab, barrelEnd.position,  barrelEnd.rotation);
                
                // TODO: Add in WeaponFactory a function that spawns in new bullets and fly's towards destination
                
                Debug.Log("Shoot from pistol");
                canShoot = false;
            }

        }
    }
}
