using UnityEngine;

namespace EEA.AbilitySystem
{
    public class Melee : MonoBehaviour
    {
        private float _damage;

        public void Init(float damage)
        {
            _damage = damage;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Enemy"))
            {
                collision.GetComponent<Object.ObjectBase>().TakeDamage(_damage);
            }
        }
    }
}
