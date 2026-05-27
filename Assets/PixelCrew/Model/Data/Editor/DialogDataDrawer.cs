using UnityEditor;
using UnityEngine;
using Assets.PixelCrew.Model.Data;

namespace Assets.PixelCrew.Model.Data.Editor
{
    [CustomPropertyDrawer(typeof(DialogData))]
    public class DialogDataDrawer : PropertyDrawer
    {
        private const float RowSpacing = 2f;
        private SerializedProperty _sentencesProperty;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            _sentencesProperty = property.FindPropertyRelative("_sentences");
            float height = EditorGUIUtility.singleLineHeight * 2;
            if (_sentencesProperty != null && _sentencesProperty.arraySize > 0)
                height += _sentencesProperty.arraySize * (EditorGUIUtility.singleLineHeight + RowSpacing) - RowSpacing;
            return height;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            _sentencesProperty = property.FindPropertyRelative("_sentences");
            if (_sentencesProperty == null) return;

            EditorGUI.BeginProperty(position, label, property);

            Rect labelRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(labelRect, label);

            float arrayStartY = position.y + EditorGUIUtility.singleLineHeight;
            float arrayHeight = position.height - EditorGUIUtility.singleLineHeight * 2;
            Rect arrayRect = new Rect(position.x, arrayStartY, position.width, arrayHeight);
            DrawSentenceList(arrayRect);

            Rect buttonRect = new Rect(position.x, position.y + position.height - EditorGUIUtility.singleLineHeight, 100f, EditorGUIUtility.singleLineHeight);
            if (GUI.Button(buttonRect, "Add Sentence"))
            {
                _sentencesProperty.arraySize++;
                var newElement = _sentencesProperty.GetArrayElementAtIndex(_sentencesProperty.arraySize - 1);
                newElement.FindPropertyRelative("Text").stringValue = "";
                newElement.FindPropertyRelative("IsPlayer").boolValue = false;
            }

            EditorGUI.EndProperty();
            property.serializedObject.ApplyModifiedProperties();
        }

        private void DrawSentenceList(Rect rect)
        {
            float rowHeight = EditorGUIUtility.singleLineHeight + RowSpacing;
            for (int i = 0; i < _sentencesProperty.arraySize; i++)
            {
                Rect rowRect = new Rect(rect.x, rect.y + i * rowHeight, rect.width, EditorGUIUtility.singleLineHeight);
                DrawSentenceRow(rowRect, i);
            }
        }

        private void DrawSentenceRow(Rect rect, int index)
        {
            var element = _sentencesProperty.GetArrayElementAtIndex(index);
            var textProp = element.FindPropertyRelative("Text");
            var isPlayerProp = element.FindPropertyRelative("IsPlayer");

            float toggleWidth = 20f;
            float removeWidth = 25f;

            Rect toggleRect = new Rect(rect.x, rect.y, toggleWidth, rect.height);
            Rect textRect = new Rect(rect.x + toggleWidth, rect.y, rect.width - toggleWidth - removeWidth, rect.height);
            Rect removeRect = new Rect(rect.x + rect.width - removeWidth, rect.y, removeWidth, rect.height);

            isPlayerProp.boolValue = EditorGUI.Toggle(toggleRect, isPlayerProp.boolValue);
            textProp.stringValue = EditorGUI.TextField(textRect, textProp.stringValue);

            if (GUI.Button(removeRect, "X"))
            {
                _sentencesProperty.DeleteArrayElementAtIndex(index);
            }
        }
    }
}