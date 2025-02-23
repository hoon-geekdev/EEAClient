using EEA.UI.Controller;
using UnityEditor;
using UnityEditor.UI;
using UnityEngine;

namespace EEA.UI.Editor
{
    [CustomEditor(typeof(ImageEx))]
    [CanEditMultipleObjects]
    public class ImageExCustomEditor : ImageEditor
    {
        private SerializedProperty _sprites;

        protected override void OnEnable()
        {
            base.OnEnable();

            // ImageEx의 추가 속성
            _sprites = serializedObject.FindProperty("_sprites");
        }

        [MenuItem("GameObject/UI/ImageEx", false, 2000)]
        public static void AddImageEx(MenuCommand menuCommand)
        {
            GameObject imageExObject = new GameObject("ImageEx", typeof(ImageEx));
            RectTransform rectTransform = imageExObject.GetComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(100, 100);

            GameObject parent = menuCommand.context as GameObject;
            if (parent != null)
            {
                GameObjectUtility.SetParentAndAlign(imageExObject, parent);
            }

            Undo.RegisterCreatedObjectUndo(imageExObject, "Create ImageEx");
            Selection.activeObject = imageExObject;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            base.OnInspectorGUI();

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("ImageEx Settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(_sprites, new GUIContent("Sprites"), true);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
