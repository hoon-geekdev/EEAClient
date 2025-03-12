using EEA.Define;
using EEA.Manager;
using EEA.UI;
using System.Collections;
using System.Linq;
using TableData;
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

        public void Init(int tableId)
        {
            _id = tableId;
            ObjectTable table = TableManager.Instance.GetData<ObjectTable>(_id);

            _target = GameManager.Instance.Player.GetComponent<Rigidbody2D>();
            _collider.enabled = true;
            _rigid.simulated = true;
            _animator.SetBool("Dead", false);
            _spriteRenderer.sortingOrder = 25;

            base.Init(table.Health, table.Move_speed);
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

            // 플레이어와 가까이 붙으면 kinematic으로 변경
            if (dir.magnitude <= 1.5f)
                _rigid.bodyType = RigidbodyType2D.Kinematic;
            else
                _rigid.bodyType = RigidbodyType2D.Dynamic;

            if (dir.magnitude <= 0.5f)
                return;

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
                //StartCoroutine(KnockBack());
            }

            GameObject go = PoolManager.Instance.GetObject(AssetPathUI.UIDamageText);
            UIDamageText damageText = go.GetComponent<UIDamageText>();
            damageText.SetText(transform, damage);
        }

        private IEnumerator KnockBack()
        {
            yield return _waitForSec;
            Vector3 playerPos = GameManager.Instance.Player.transform.position;
            Vector3 dir = (transform.position - playerPos);

            _rigid.AddForce(dir.normalized * 1, ForceMode2D.Impulse);
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

            ItemDrop();

        }

        private void ItemDrop()
        {
            ObjectTable table = TableManager.Instance.GetData<ObjectTable>(_id);
            int sumWeight = table.Drop_rates.Sum();
            int random = Random.Range(0, sumWeight);
            int dropItemCode = 0;

            for (int i = 0; i < table.Drop_items.Length; i++)
            {
                random -= table.Drop_rates[i];
                if (random <= 0)
                {
                    dropItemCode = table.Drop_items[i];
                    break;
                }
            }

            if (dropItemCode != 0)
            {
                ItemTable itemTable = TableManager.Instance.GetData<ItemTable>(dropItemCode);
                GameObject dropGo = PoolManager.Instance.GetObject(itemTable.Asset_path);
                dropGo.transform.position = transform.position;

                DropItem dropItem = dropGo.GetComponent<DropItem>();
                dropItem.Init(dropItemCode);
            }
        }
    }
}
