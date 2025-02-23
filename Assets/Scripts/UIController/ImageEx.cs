using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace EEA.UI.Controller
{
    public class ImageEx : Image
    {
        [SerializeField] private List<Sprite> _sprites = new List<Sprite>();

        public void ChangeSprite(int index)
        {
            if (_sprites == null || _sprites.Count == 0)
            {
                Debug.LogError("Sprites list is empty.");
                return;
            }
            if (index >= 0 && index < _sprites.Count)
            {
                sprite = _sprites[index];
            }
            else
            {
                Debug.LogError("Index out of range.");
            }
        }

        public void ChangeSprite(int index, float width = -1, float height = -1)
        {
            Sprite newSprite = null;

            if (_sprites == null || _sprites.Count == 0)
            {
                Debug.LogError("Sprites list is empty.");
                return;
            }
            if (index >= 0 && index < _sprites.Count)
            {
                newSprite = _sprites[index];
            }
            else
            {
                Debug.LogError("Index out of range.");
                return;
            }

            sprite = newSprite;

            RectTransform rectTransform = GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                if (width > 0 && height > 0)
                {
                    rectTransform.sizeDelta = new Vector2(width, height);
                }
                else if (newSprite != null)
                {
                    rectTransform.sizeDelta = new Vector2(newSprite.rect.width, newSprite.rect.height);
                }
            }
        }

        public void ChangeSpriteWhitSetNativeSize(int index)
        {
            Sprite newSprite = null;

            if (_sprites == null || _sprites.Count == 0)
            {
                Debug.LogError("Sprites list is empty.");
                return;
            }
            if (index >= 0 && index < _sprites.Count)
            {
                newSprite = _sprites[index];
            }
            else
            {
                Debug.LogError("Index out of range.");
                return;
            }

            sprite = newSprite;

            if (newSprite != null)
                SetNativeSize();
        }

        public void ClearImg()
        {
            sprite = null;
        }
    }
}

