using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class SpriteBatchProcessor : EditorWindow
{
    [MenuItem("Window/Sprite Batch Processor")]
    public static void ShowWindow()
    {
        GetWindow<SpriteBatchProcessor>("Sprite Batch Processor");
    }

    void OnGUI()
    {
        if (GUILayout.Button("Process Sprites"))
        {
            ProcessSprites();
        }
    }

    private void ProcessSprites()
    {
        string[] guids = AssetDatabase.FindAssets("t:Sprite", new[] { "Assets" });
        int processedCount = 0;
        int skippedCount = 0;

        Debug.Log($"🔍 Found {guids.Length} sprites. Starting processing...");

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);

            if (sprite == null)
            {
                Debug.LogWarning($"❔ Skipped (sprite load failed): {path}");
                skippedCount++;
                continue;
            }

            Texture2D texture = sprite.texture;
            if (texture == null)
            {
                Debug.LogWarning($"❔ Skipped (texture is null): {sprite.name} | Path: {path}");
                skippedCount++;
                continue;
            }

            // Проверяем Read/Write Enabled
            if (!texture.isReadable)
            {
                Debug.LogWarning(
                    $"❔ Skipped (texture not readable): {texture.name}\n" +
                    $"   Path: {AssetDatabase.GetAssetPath(texture)}\n" +
                    $"   Solution: Enable 'Read/Write Enabled' in Texture Import Settings.");
                skippedCount++;
                continue;
            }

            try
            {
                // Обрезаем пустые края
                Rect croppedRect = GetCroppedRect(texture, sprite.rect);
                if (croppedRect == sprite.rect)
                {
                    Debug.Log($"→ {sprite.name}: No cropping needed.");
                    continue;
                }

                // Применяем новый rect
                SetSpriteRect(sprite, croppedRect);

                // Настраиваем pivot (центр обрезанной области)
                SetPivotToCenter(sprite, croppedRect);

                processedCount++;
                Debug.Log($"✅ Processed: {sprite.name} | Old rect: {sprite.rect} → New rect: {croppedRect}");
            }
            catch (System.Exception e)
            {
                Debug.LogError(
                    $"❌ Error processing {sprite.name}:\n" +
                    $"   Path: {path}\n" +
                    $"   Texture: {texture.name} ({texture.width}x{texture.height})\n" +
                    $"   Exception: {e.Message}\n" +
                    $"   StackTrace: {e.StackTrace}");
                skippedCount++;
            }
        }

        // Сохраняем изменения
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"\n📊 Summary:\n" +
                  $"   Processed: {processedCount}\n" +
                  $"   Skipped: {skippedCount}\n" +
                  $"   Total: {guids.Length}");

        if (skippedCount > 0)
        {
            Debug.LogWarning("⚠️ Some sprites were skipped. Check warnings/errors above for details.");
        }
        else
        {
            Debug.Log("✅ All sprites processed successfully!");
        }
    }

    // Включаем Read/Write для текстуры (если нужно)
    private void EnableTextureReadWrite(Texture2D texture)
    {
        string texturePath = AssetDatabase.GetAssetPath(texture);
        TextureImporter importer = AssetImporter.GetAtPath(texturePath) as TextureImporter;
        if (importer != null && !importer.isReadable)
        {
            importer.isReadable = true;
            AssetDatabase.ImportAsset(texturePath, ImportAssetOptions.ForceUpdate);
            Debug.Log($"✅ Enabled Read/Write for: {texturePath}");
        }
    }

    // Обрезаем пустые края
    private Rect GetCroppedRect(Texture2D texture, Rect sourceRect)
    {
        Color32[] pixels = texture.GetPixels32();
        int texWidth = texture.width;
        int texHeight = texture.height;

        // Абсолютные координаты области спрайта в текстуре
        int xMin = Mathf.FloorToInt(sourceRect.xMin);
        int yMin = Mathf.FloorToInt(sourceRect.yMin);
        int xMax = Mathf.FloorToInt(sourceRect.xMax);
        int yMax = Mathf.FloorToInt(sourceRect.yMax);

        Debug.Log($"🔍 Analyzing {texture.name}");
        Debug.Log($"   Source rect: {sourceRect}");
        Debug.Log($"   Texture size: {texWidth}x{texHeight}");
        Debug.Log($"   Pixel region: ({xMin},{yMin}) → ({xMax},{yMax})");

        // Ищем минимальные/максимальные координаты непрозрачных пикселей
        int left = xMax, right = xMin;
        int top = yMax, bottom = yMin;

        for (int y = yMin; y < yMax; y++)
        {
            for (int x = xMin; x < xMax; x++)
            {
                // Проверка границ текстуры
                if (x >= texWidth || y >= texHeight) continue;

                Color32 pixel = pixels[y * texWidth + x];

                // Порог прозрачности: 10/255 ≈ 4%
                if (pixel.a >= 10)
                {
                    if (x < left) left = x;
                    if (x > right) right = x;
                    if (y < top) top = y;
                    if (y > bottom) bottom = y;
                }
            }
        }

        // Если не нашли непрозрачных пикселей
        if (left == xMax)
        {
            Debug.LogWarning($"⚠️ No opaque pixels in {texture.name}. Using original rect.");
            return sourceRect;
        }

        // Новый rect в координатах текстуры
        Rect result = new Rect(left, top, right - left + 1, bottom - top + 1);

        Debug.Log($"✅ Found bounds:");
        Debug.Log($"   left={left}, right={right}, top={top}, bottom={bottom}");
        Debug.Log($"   New rect: {result}");

        return result;
    }



    // Применяем новый rect к спрайту
    private void SetSpriteRect(Sprite sprite, Rect rect)
    {
        SerializedObject serializedObject = new SerializedObject(sprite);
        SerializedProperty rectProp = serializedObject.FindProperty("m_Rect");

        if (rectProp != null)
        {
            rectProp.rectValue = rect;
            serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(sprite);
        }
        else
        {
            Debug.LogError($"❌ Failed to find 'm_Rect' property for {sprite.name}");
        }
    }

    // Устанавливаем pivot в центр обрезанной области
    private void SetPivotToCenter(Sprite sprite, Rect croppedRect)
    {
        Vector2 pivot = new Vector2(
            (croppedRect.xMin + croppedRect.xMax) / 2f,
            (croppedRect.yMin + croppedRect.yMax) / 2f
        );

        SerializedObject serializedObject = new SerializedObject(sprite);
        SerializedProperty pivotProp = serializedObject.FindProperty("m_Pivot");

        if (pivotProp != null)
        {
            pivotProp.vector2Value = pivot;
            serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(sprite);
        }
        else
        {
            Debug.LogError($"❌ Failed to find 'm_Pivot' property for {sprite.name}");
        }
    }
}
