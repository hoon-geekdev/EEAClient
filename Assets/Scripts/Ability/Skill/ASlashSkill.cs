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
            _particle = GetComponentInChildren<ParticleSystem>();
            _particle.gameObject.SetActive(false);
            _unitComponent.Finish();
            StartCoroutine(Effect());
        }

        private IEnumerator Effect()
        {
            while (true)
            {
                yield return new WaitForSeconds(_delay);
                _particle.gameObject.SetActive(false);
                _particle.gameObject.SetActive(true);
                //var main = _particle.main;
                //main.simulationSpeed = main.duration / _delay; // 속도 계산

                //yield return new WaitForSeconds(0.4f);
                //_unitComponent.Fire(_damage, _player.LookAngle(), _duration, 180f);

                //yield return new WaitForSeconds(_duration);
                //_unitComponent.Finish();
            }
        }

        private void LateUpdate()
        {
            // _owner가 바라보고 있는 방향 으로 이펙트 회전을 조정
            if (_player != null)
                transform.rotation = Quaternion.Euler(0, 0, _player.LookAngle());
        }

        private void OnDrawGizmos()
        {
            // rect로 그리기


        }
    }
}
