using EEA.Object;
using EEA.UI;
using UnityEngine;
using UnityEngine.UI;

public class UIMonsterHealth : UIHudBase
{
    [SerializeField] private Slider _slider;

    private ObjectBase _target;
    private Camera _mainCam;
    private Transform _headPost;

    public void Init(ObjectBase target)
    {
        _target = target;
        _mainCam = Camera.main;

        _headPost = _target.transform.Find("_headPos");
    }

    private void LateUpdate()
    {
        if (_target == null || _mainCam == null)
            return;

        // 체력 바 갱신
        _slider.value = _target.Health / _target.MaxHealth;

        // RectTransform의 높이를 world offset으로 변환
        Vector3 screenPos = _mainCam.WorldToScreenPoint(_headPost.position);

        transform.position = screenPos;
    }
}
