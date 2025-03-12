using DG.Tweening;
using EEA.Manager;
using System.Collections;
using TableData;
using UnityEngine;

namespace EEA.Object
{
    public class DropItem : MonoBehaviour
    {
        private SpriteRenderer _spriteRenderer;
        private ItemTable _dropItem;

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            GetComponent<BoxCollider2D>().isTrigger = true;
        }

        public void Init(int tableCode)
        {
            transform.localScale = new Vector3(1, 1, 1);
            _dropItem = TableManager.Instance.GetData<ItemTable>(tableCode);
            Sprite icon = ResourceManager.Instance.LoadAsset<Sprite>(_dropItem.Drop_icon_path);

            _spriteRenderer.sprite = icon;

            PlayDropEffect();
        }

        void PlayDropEffect()
        {
            Vector3 startPos = transform.position; // 현재 드랍된 위치 저장
            transform.DOMoveY(startPos.y + 1f, 0.2f)
            .SetEase(Ease.OutQuad)
            .OnComplete(() => transform.DOMoveY(startPos.y, 0.2f).SetEase(Ease.InBounce));
        }

        public void RootingItem()
        {
            transform.DOKill(); // 기존 DOTween 애니메이션 정지

            // 플레이어 위치로 이동 & 점점 작아짐
            transform.DOMove(GameManager.Instance.Player.transform.position, 0.3f);

            transform.DOScale(Vector3.zero, 0.3f) // 0.3초 동안 크기가 점점 작아짐
                .OnComplete(() => {
                    gameObject.SetActive(false);
                    if (_dropItem != null)
                    {
                        GameManager.Instance.AddExp((int)_dropItem.Ability);
                    }
                });
        }
    }
}
