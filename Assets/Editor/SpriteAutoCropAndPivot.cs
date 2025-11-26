using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class SpriteAutoCropAndPivot : EditorWindow
{
    private string folderPath = "Assets";
    private Vector2 pivot = new Vector2(0.5f, 0f); // Bottom Center

    [MenuItem("Window/Sprite Tools/Auto Crop & Pivot")]
    private static void ShowWindow()
    {
        GetWindow<SpriteAutoCropAndPivot>("Sprite Auto Crop & Pivot");
    }

    private void OnGUI()
    {
        GUILayout.Label("Sprite Auto Crop and Set Pivot", EditorStyles.boldLabel);

        EditorGUILayout.LabelField("Folder Path");
        folderPath = EditorGUILayout.TextField(folderPath);

        if (GUILayout.Button("Browse..."))
        {
            string path = EditorUtility.OpenFolderPanel("Select Sprite Folder", "Assets", "");
            if (!string.IsNullOrEmpty(path))
            {
                folderPath = path.Replace(Application.dataPath, "Assets");
            }
        }

        EditorGUILayout.LabelField("Pivot (0..1)");
        pivot = EditorGUILayout.Vector2Field("", pivot);

        if (GUILayout.Button("Process Sprites", GUILayout.Height(30)))
        {
            ProcessSprites();
        }
    }

    private void ProcessSprites()
    {
        string[] guids = AssetDatabase.FindAssets("t:Sprite", new[] { folderPath });

        if (guids.Length == 0)
        {
            Debug.LogWarning("No sprites found in: " + folderPath);
            return;
        }

        List<string> texturePaths = new List<string>();
        int processedCount = 0;

        // 1. Собираем пути и включаем Read/Write
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            texturePaths.Add(path);

            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer != null && !importer.isReadable)
            {
                importer.isReadable = true;
                AssetDatabase.ImportAsset(path);
                Debug.Log("✅ Enabled Read/Write for: " + path);
            }
        }

        // 2. Обрабатываем каждый спрайт по пути (без кэширования ссылок)
        foreach (string path in texturePaths)
        {
            Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);
            if (sprite == null)
            {
                Debug.LogError("Failed to load sprite: " + path);
                continue;
            }

            Texture2D texture = sprite.texture;
            Rect rect = sprite.textureRect;

            Rect croppedRect = GetCroppedRect(texture, rect);
            if (croppedRect != rect)
            {
                SetSpriteRect(sprite, croppedRect);
                processedCount++;
            }

            // Установка Pivot
            SerializedObject serializedObject = new SerializedObject(sprite);
            SerializedProperty pivotProp = serializedObject.FindProperty("m_Pivot");
            pivotProp.vector2Value = pivot;
            serializedObject.ApplyModifiedProperties();
        }

        // 3. Отключаем Read/Write
        foreach (string path in texturePaths)
        {
            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer != null && importer.isReadable)
            {
                importer.isReadable = false;
                AssetDatabase.ImportAsset(path);
                Debug.Log("❌ Disabled Read/Write for: " + path);
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"✅ FINISHED: Processed {processedCount} sprites out of {texturePaths.Count}.");
        Debug.Log($"📍 Pivot set to: ({pivot.x}, {pivot.y})");
    }


    private Rect GetCroppedRect(Texture2D texture, Rect sourceRect)
    {
        Color32[] pixels = texture.GetPixels32();
        int texWidth = texture.width;
        int texHeight = texture.height;

        int srcX = (int)sourceRect.x;
        int srcY = (int)sourceRect.y;
        int width = (int)sourceRect.width;
        int height = (int)sourceRect.height;

        int left = width, right = 0, top = height, bottom = 0;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int texX = srcX + x;
                int texY = srcY + y;
                if (texX < texWidth && texY < texHeight)
                {
                    Color32 pixel = pixels[texY * texWidth + texX];
                    if (pixel.a > 0)
                    {
                        if (x < left) left = x;
                        if (x > right) right = x;
                        if (y < top) top = y;
                        if (y > bottom) bottom = y;
                    }
                }
            }
        }

        if (left == width) return sourceRect; // Все пиксели прозрачные

        return new Rect(
            sourceRect.x + left,
            sourceRect.y + top,
            right - left + 1,
            bottom - top + 1
        );
    }

    private void SetSpriteRect(Sprite sprite, Rect rect)
    {
        SerializedObject serializedObject = new SerializedObject(sprite);
        SerializedProperty rectProp = serializedObject.FindProperty("m_Rect");
        rectProp.vector4Value = new Vector4(rect.x, rect.y, rect.width, rect.height);
        serializedObject.ApplyModifiedProperties();
    }
}
