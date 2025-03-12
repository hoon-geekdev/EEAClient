using System.Collections;
using TMPro;
using UnityEngine;

namespace EEA.UI
{
    public class UIDamageText : MonoBehaviour
    {
        public TextMeshPro _textMesh;
        public float _destroyTime = 1f;
        public Vector3 _floatOffset = new Vector3(0, 1f, 0);

        public void SetText(Transform target, float damage)
        {
            Renderer targetRenderer = target.GetComponent<Renderer>();
            float targetHeight = targetRenderer.bounds.size.y; // Renderer의 높이

            Vector3 pos = target.position;
            pos.y += targetHeight / 2;
            transform.position = pos;

            _textMesh.text = damage.ToString("F0"); // 정수로 표시
            gameObject.SetActive(true);

            StartCoroutine(DisableText());
        }

        private void Update()
        {
            transform.position += _floatOffset * Time.deltaTime; // 위로 떠오름 효과
        }

        private IEnumerator DisableText()
        {
            yield return new WaitForSeconds(_destroyTime);
            gameObject.SetActive(false);
        }
    }
}
