using Common.Interfaces;
using Factory;
using UnityEngine;

namespace Weapon
{
    public class BulletScript : MonoBehaviour
    {

        public float speed = 20f;
        public float lifeTime = 3f;
        public int damage = 10;

        private void Start()
        {
            Destroy(gameObject, lifeTime);
        }

        private void Update()
        {
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
        }
        
        private void OnCollisionEnter(Collision collision)
        {
            var health = collision.gameObject.GetComponent<IHealth>();
            if (health != null && !collision.gameObject.CompareTag(tag))
            {
                collision.gameObject.GetComponent<CharacterFactory>().TakeDamage(damage);
            }
            if(collision.gameObject.layer == LayerMask.NameToLayer("Obstacel"))
                Destroy(gameObject);
        }
    }
}
