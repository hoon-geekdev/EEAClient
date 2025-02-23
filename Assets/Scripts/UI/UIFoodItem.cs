using EEA.UI.Controller;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EEA.UI
{
    public class UIFoodItem : MonoBehaviour
    {
        [SerializeField] private ImageEx _img;

        public int FoodType => _foodType;
        private int _foodType;

        public void SetFoodType(int type)
        {
            _foodType = type;
            _img.ChangeSprite(type);
            _img.color = type == 0 ? Color.white : Color.red;
        }
    }
}