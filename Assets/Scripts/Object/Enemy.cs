using EEA.Manager;
using System.Collections;
using UnityEngine;

namespace EEA.Object
{
    public class Enemy : ObjectBase
    {
        private Rigidbody2D _target;
        private WaitForSeconds _waitForSec = new WaitForSeconds(0);

        public void Init(float hp, float speed, int type)
        {
            base.Init(hp, speed);

            _target = GameManager.Instance.Player.GetComponent<Rigidbody2D>();

            _collider.enabled = true;
            _rigid.simulated = true;
            _animator.SetBool("Dead", false);
            _spriteRenderer.sortingOrder = 25;
        }

        private void OnEnable()
        {
            StartCoroutine(CheckDistance());
        }

        private void OnDisable()
        {
            StopAllCoroutines();
        }

        protected override void OnFixedUpdate()
        {
            if (IsDead == true || _target == null)
                return;

            if (_animator.GetCurrentAnimatorStateInfo(0).IsName("Hit"))
                return;

            Vector2 dir = _target.position - _rigid.position;
            Vector2 moveVec = dir.normalized * MoveSpeed * Time.fixedDeltaTime;

            _rigid.MovePosition(_rigid.position + moveVec);
            _rigid.linearVelocity = Vector2.zero;
        }

        protected override void OnLateUpdate()
        {
            if (IsDead == true || _target == null)
                return;

            _spriteRenderer.flipX = _target.position.x < _rigid.position.x;
        }

        private IEnumerator CheckDistance()
        {
            while (true)
            {
                Vector3 playerPos = GameManager.Instance.Player.transform.position;
                Vector3 dist = playerPos - transform.position;
                if (dist.magnitude >= 20)
                {
                    Vector3 randomPos = new Vector3(Random.Range(5f, 15f), Random.Range(5f, 15f), 0f);
                    transform.Translate(randomPos + dist);
                }

                yield return new WaitForSeconds(0.5f);
            }
        }

        protected override void OnTakeDamage(float damage)
        {
            if (IsDead == true)
                return;

            _health -= damage;

            if (IsDead == true)
            {
                //gameObject.SetActive(false);
                StartCoroutine(Dead());
            }
            else
            {
                _animator.SetTrigger("Hit");
                StartCoroutine(KnockBack());
            }
        }

        private IEnumerator KnockBack()
        {
            yield return _waitForSec;
            Vector3 playerPos = GameManager.Instance.Player.transform.position;
            Vector3 dir = (transform.position - playerPos);

            _rigid.AddForce(dir.normalized * 3, ForceMode2D.Impulse);
        }

        private IEnumerator Dead()
        {
            _collider.enabled = false;
            _rigid.simulated = false;
            _animator.SetBool("Dead", true);
            _spriteRenderer.sortingOrder = 0;

            GameManager.Instance.AddKillCount();
            yield return new WaitForSeconds(0.1f);
            
            PoolManager.Instance.GetEffect(0).transform.position = transform.position;
            yield return new WaitForSeconds(0.8f);
            gameObject.SetActive(false);

        }
    }
}
