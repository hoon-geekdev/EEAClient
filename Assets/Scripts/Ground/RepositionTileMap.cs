using EEA.Manager;
using EEA.Object;
using UnityEngine;

namespace EEA.Ground
{
    public class RepositionTileMap : MonoBehaviour
    {
        private Collider2D _colider;

        private void Awake()
        {
            _colider = GetComponent<Collider2D>();
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.CompareTag("Area") == false)
                return;

            Player player = GameManager.Instance.Player;
            Vector3 playerPos = player.transform.position;
            Vector3 tilePos = transform.position;

            float diffX = playerPos.x - tilePos.x;
            float diffY = playerPos.y - tilePos.y;

            float dirX = diffX < 0 ? -1 : 1;
            float dirY = diffY < 0 ? -1 : 1;

            diffX = Mathf.Abs(diffX);
            diffY = Mathf.Abs(diffY);

            if (diffX > diffY)
                transform.Translate(Vector3.right * dirX * 40);
            else if (diffX < diffY)
                transform.Translate(Vector3.up * dirY * 40);
        }
    }
}
