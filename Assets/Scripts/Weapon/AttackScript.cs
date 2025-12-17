using System;
using Factory;
using UnityEngine;

namespace Weapon
{
    public class AttackScript : MonoBehaviour
    {
        private WeaponFactory gun;

        private void Awake()
        {
            gun = GetComponentInChildren<WeaponFactory>();
            if(gun == null) Debug.LogWarning($"Got no gun: {name}");
            else Debug.Log($"Got a gun: {name}");
        }

        private void Update()
        {
            Shoot();
        }


        private void Shoot()
        {
            if (gun == null) return;
            gun.Shoot();
        }
    }
}

