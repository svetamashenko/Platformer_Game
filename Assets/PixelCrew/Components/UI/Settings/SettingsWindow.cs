using UnityEditor;

namespace Assets.PixelCrew.Components.UI.Settings
{
    public class SettingsWindow : AnimatedWindow
    {
        [MenuItem("Tools/MyTool/Do It in C#")]
        static void DoIt()
        {
            EditorUtility.DisplayDialog("MyTool", "Do It in C# !", "OK", "");
        }
    }
}