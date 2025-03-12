using EEA.Manager;
using EEA.Object;
using UnityEngine;

namespace EEA.UI
{
    public class UIJoystick : UIHudBase
    {
        [SerializeField] private Joystick _joystickMove;
        [SerializeField] private Joystick _joystickLook;

        private Player _player;

        private void Awake()
        {
            _player = GameManager.Instance.Player;
            _joystickMove.OnHandleUpdate += OnInputMove;
            _joystickLook.OnHandleUpdate += OnInputLook;
        }

        private void OnInputMove(Vector2 input)
        {
            _player.MoveDir(input);
        }

        private void OnInputLook(Vector2 input)
        {
            _player.LookDir(_joystickLook.Direction);
        }
    }
}
