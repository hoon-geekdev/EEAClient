using EEA.Manager;
using EEA.Object;
using System.Collections;
using UnityEngine;

namespace EEA.AbilitySystem
{
    public class ALaserUnit : MonoBehaviour
    {
        [SerializeField] private ParticleSystem _explosion;
        [SerializeField] private ParticleSystem _laser;

        private float _damage;
        private float _range;

        public void Init(float damage, float range)
        {
            _explosion.gameObject.SetActive(false);
            _laser.gameObject.SetActive(false);

            _damage = damage;
            _range = range;
            StartCoroutine(Fire());
        }

        private IEnumerator Fire()
        {
            _laser.gameObject.SetActive(true);
            _explosion.gameObject.SetActive(true);

            _laser.Play();
            _explosion.Play();
            yield return new WaitForSeconds(0.1f);

            // position보다 0.5만큼 위에 생성
            Vector3 pos = transform.position + Vector3.up * 0.5f;
            Collider2D[] colliders = Physics2D.OverlapCapsuleAll(pos, new Vector2(_range, 1f), CapsuleDirection2D.Horizontal, 0, LayerMask.GetMask("Enemy"));
            
            foreach (Collider2D collider in colliders)
            {
                ObjectBase target = collider.GetComponent<ObjectBase>();
                if (target == null)
                    continue;

                target.TakeDamage(_damage);
            }

            yield return new WaitForSeconds(1f);

            gameObject.SetActive(false);
        }

        //private void OnDrawGizmos()
        //{
        //    Gizmos.color = Color.red;

        //    Vector3 position = transform.position;
        //    float height = 1f;
        //    Vector3 size = new Vector2(_range, height);
        //    float radius = height / 2f;

        //    // 중심 사각형
        //    Vector2 boxCenter = position;
        //    Vector2 boxSize = new Vector2(_range - height, height);
        //    Gizmos.DrawWireCube(boxCenter, boxSize);

        //    // 양쪽 반원 (캡슐 끝)
        //    Vector3 left = position + Vector3.left * (_range / 2f - radius);
        //    Vector3 right = position + Vector3.right * (_range / 2f - radius);
        //    DrawWireCircle(left, radius);
        //    DrawWireCircle(right, radius);
        //}

        //// 원을 그리는 유틸 함수
        //void DrawWireCircle(Vector3 center, float radius, int segments = 20)
        //{
        //    float angleStep = 360f / segments;
        //    Vector3 prevPoint = center + new Vector3(Mathf.Cos(0), Mathf.Sin(0)) * radius;

        //    for (int i = 1; i <= segments; i++)
        //    {
        //        float rad = Mathf.Deg2Rad * angleStep * i;
        //        Vector3 nextPoint = center + new Vector3(Mathf.Cos(rad), Mathf.Sin(rad)) * radius;
        //        Gizmos.DrawLine(prevPoint, nextPoint);
        //        prevPoint = nextPoint;
        //    }
        //}
    }
}
