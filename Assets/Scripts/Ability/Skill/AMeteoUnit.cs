using EEA.Manager;
using EEA.Object;
using System.Collections;
using UnityEngine;

namespace EEA.AbilitySystem
{
    public class AMeteoUnit : MonoBehaviour
    {
        [SerializeField] private ParticleSystem _explosion;
        [SerializeField] private ParticleSystem _meteo;

        private float _damage;
        private float _range;

        public void Init(float damage, float range)
        {
            _explosion.gameObject.SetActive(false);
            _meteo.gameObject.SetActive(false);

            _damage = damage;
            _range = range;
            StartCoroutine(Fire());
        }

        private IEnumerator Fire()
        {
            _meteo.gameObject.SetActive(true);
            _explosion.gameObject.SetActive(true);
            _meteo.Play();
            _explosion.Play();
            yield return new WaitForSeconds(1f);

            // position보다 0.5만큼 위에 생성
            Vector3 pos = transform.position + Vector3.up * 0.5f;
            Collider2D[] colliders = Physics2D.OverlapCircleAll(pos, _range, LayerMask.GetMask("Enemy"));
            foreach (Collider2D collider in colliders)
            {
                ObjectBase target = collider.GetComponent<ObjectBase>();
                if (target == null)
                    continue;

                target.TakeDamage(_damage);
            }

            GameManager.Instance.ShakeCamera();

            yield return new WaitForSeconds(1f);

            gameObject.SetActive(false);
        }
    }
}
