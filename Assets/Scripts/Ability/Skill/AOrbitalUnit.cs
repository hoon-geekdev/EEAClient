using EEA.Define;
using UnityEngine;

namespace EEA.AbilitySystem
{
    public class AOrbitalUnit : MonoBehaviour
    {
        DamageEvent _damageEvent;
        public void Init(DamageEvent evt)
        {
            _damageEvent = evt;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Enemy"))
            {
                collision.GetComponent<Object.ObjectBase>().TakeDamage(_damageEvent);
            }
        }
    }
}
