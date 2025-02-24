using EEA.Object;
using System.Collections;
using UnityEngine;

namespace EEA.AbilitySystem
{
    public class ASlashSkill : Ability
    {
        [SerializeField] private ASlashHitBoxUnit _unitComponent;
        private ParticleSystem _particle;
        private Player _player;

        protected override void OnRefreshData()
        {
        }

        protected override void OnStart()
        {
            _player = _owner as Player;
            _delay = 1f;
            _duration = 0.2f;
            _damage = 30f;

            _particle = GetComponentInChildren<ParticleSystem>();

            StartCoroutine(Effect());
        }

        // 이펙트의 범위를 표시하기 위한 기즈모
        private void OnDrawGizmos()
        {
            if (_player == null)
                return;
        }

        private IEnumerator Effect()
        {
            while (true)
            {
                yield return new WaitForSeconds(_delay - _duration);

                // _owner가 바라보고 있는 방향 으로 이펙트 회전을 조정
                transform.rotation = Quaternion.Euler(0, 0, _player.LookAngle());
                _particle.gameObject.SetActive(true);
                _unitComponent.Fire(_damage, _player.LookAngle(), _duration, 180f);

                yield return new WaitForSeconds(_duration);
                _particle.gameObject.SetActive(false);
                _unitComponent.Finish();
            }
        }
    }
}
