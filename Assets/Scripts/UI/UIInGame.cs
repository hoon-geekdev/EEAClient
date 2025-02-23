using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;

namespace EEA.UI
{
    public class UIInGame : MonoBehaviour
    {
        [SerializeField] private GameObject _character;
        [SerializeField] private UIFoodItem _foodItemPref;
        [SerializeField] private Transform _foodItemParent;

        private const int MAX_FOOD = 10;
        private List<UIFoodItem> _foodItems = new List<UIFoodItem>();
        private int _currentIndex = 0;
        private Animator _animator;
        private Vector3 _startPos;

        private bool _isEat = true;

        private void Start()
        {
            _animator = _character.GetComponent<Animator>();
            CreateItem();
        }

        public void CreateItem()
        {
            _startPos = Camera.main.WorldToScreenPoint(_character.transform.position);

            for (int i = 0; i < MAX_FOOD; i++)
            {
                UIFoodItem foodItem = Instantiate(_foodItemPref, _foodItemParent);
                // 2d game object��ǥ�� ui��ǥ�� ��ȯ
                Vector3 pos = _startPos;
                pos.x += (i * 200) + 50;
                pos.y = 0;
                pos.z = 0;
                foodItem.transform.position = pos;
                foodItem.transform.localPosition = new Vector3(foodItem.transform.localPosition.x, 0, 0);
                foodItem.name = $"FoodItem_{i}";

                _foodItems.Add(foodItem);

                if (i < 3)
                {
                    foodItem.SetFoodType(0);
                }
                else
                {
                    int rnd = Random.Range(0, 100);
                    foodItem.SetFoodType(rnd < 70 ? 0 : 1);
                }
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Slash))
            {
                _isEat = true;
                _character.GetComponent<SpriteRenderer>().color = _isEat == true ? Color.white : Color.red;
                EatAndThrow();
            }

            if (Input.GetKeyDown(KeyCode.Z))
            {
                _isEat = false;
                _character.GetComponent<SpriteRenderer>().color = _isEat == true ? Color.white : Color.red;
                EatAndThrow();
            }
        }

        private void EatAndThrow()
        {
            // ���� �ε����� ������ �� ������ ���������� �̵� �ϰ� �ε����� ����. 
            int calcIndex = _currentIndex % _foodItems.Count;
            UIFoodItem curItem = _foodItems[calcIndex];
            int foodType = _isEat ? 0 : 1;
            if (curItem.FoodType != foodType)
            {
                Debug.Log("�ҷ� ������ �Ծ����ϴ�.");
                return;
            }

            Vector3 pos = _startPos;
            curItem.transform.position = new Vector3(pos.x + (MAX_FOOD * 200) + 50, curItem.transform.localPosition.y, curItem.transform.localPosition.z);
            curItem.transform.localPosition = new Vector3(curItem.transform.localPosition.x, 0, 0);
            _currentIndex++;
            // ������ �����ϰ� ����
            int rnd = Random.Range(0, 100);
            _foodItems[calcIndex].SetFoodType(rnd < 70 ? 0 : 1);
            // ���ĵ��� �������� ��ĭ�� ������ �̵�
            for (int i = 0; i < _foodItems.Count; i++)
            {
                Vector3 tempPos = _foodItems[i].transform.position;
                tempPos.x -= 200;
                _foodItems[i].transform.position = tempPos;
            }
            if (_animator)
                _animator.SetTrigger("Eat");
        }
    }
}
