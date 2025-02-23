using EEA.Manager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EEA.Object
{
    public class FollowPlayer : MonoBehaviour
    {
        RectTransform _rt;
        Camera _camera;

        private void Awake()
        {
            _rt = GetComponent<RectTransform>();
            _camera = Camera.main;
        }

        private void FixedUpdate()
        {
            Player player = GameManager.Instance.Player;
            if (player == null)
                return;

            Vector3 playerPos = player.transform.position;
            Vector3 screenPos = _camera.WorldToScreenPoint(playerPos);
            _rt.position = screenPos;
        }
    }
}
