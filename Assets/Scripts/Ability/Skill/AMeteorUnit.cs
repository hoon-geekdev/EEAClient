using EEA.Define;
using EEA.Manager;
using EEA.Object;
using System.Collections;
using UnityEngine;

namespace EEA.AbilitySystem
{
    public class AMeteorUnit : MonoBehaviour
    {
        [SerializeField] private ParticleSystem _explosion;
        [SerializeField] private ParticleSystem _meteor;

        DamageEvent _damageEvent;
        public void Init(DamageEvent evt)
        {
            _explosion.gameObject.SetActive(false);
            _meteor.gameObject.SetActive(false);

            _damageEvent = evt;
            StartCoroutine(Fire());
        }

        private IEnumerator Fire()
        {
            _meteor.gameObject.SetActive(true);
            _explosion.gameObject.SetActive(true);
            _meteor.Play();
            _explosion.Play();
            yield return new WaitForSeconds(1f);

            // position보다 0.5만큼 위에 생성
            Vector3 pos = transform.position + Vector3.up * 0.5f;
            Collider2D[] colliders = Physics2D.OverlapCircleAll(pos, _damageEvent._range, LayerMask.GetMask("Enemy"));
            foreach (Collider2D collider in colliders)
            {
                ObjectBase target = collider.GetComponent<ObjectBase>();
                if (target == null)
                    continue;

                target.TakeDamage(_damageEvent);
            }

            GameManager.Instance.ShakeCamera();

            yield return new WaitForSeconds(1f);

            gameObject.SetActive(false);
        }
    }
}
