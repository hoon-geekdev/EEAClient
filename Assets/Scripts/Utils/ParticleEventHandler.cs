using EEA.Object;
using System.Collections.Generic;
using UnityEngine;

namespace EEA.Utils
{
    public class ParticleEventHandler : MonoBehaviour
    {
        private void OnParticleCollision(GameObject other)
        {
            if (other.CompareTag("Enemy"))
            {
                // other.GetComponent<Enemy>().TakeDamage(10);
                Debug.Log("Particle Collision with Enemy");
            }
        }
    }
}
