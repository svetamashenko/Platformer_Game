using UnityEditor;
using UnityEngine;

namespace PixelCrew.Model.Definitions.Editor
{
    public class InventoryIdAttribute : PropertyAttribute
    {
        [MenuItem("Tools/MyTool/Do It in C#")]
        static void DoIt()
        {
            EditorUtility.DisplayDialog("MyTool", "Do It in C# !", "OK", "");
        }
    }
}