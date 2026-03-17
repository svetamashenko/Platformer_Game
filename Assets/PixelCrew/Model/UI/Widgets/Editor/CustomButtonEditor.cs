using UnityEditor.UI;
using UnityEditor;
using PixelCrew.Components.UI.Widgets;

namespace Assets.PixelCrew.Components.UI.Widgets.Editor
{
    [CustomEditor(typeof(CustomButon), true)]
    [CanEditMultipleObjects]
    public class CustomButtonEditor : ButtonEditor
    {
        public override void OnInspectorGUI()
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_normal"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_pressed"));
            serializedObject.ApplyModifiedProperties();

            base.OnInspectorGUI(); 
        }
    }
}