using UnityEngine;

namespace EEA.Object
{
    public class ItemRooting : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("DropItem") == false)
                return;

            DropItem dropitem = collision.GetComponent<DropItem>();
            dropitem.RootingItem();
        }
    }
}
