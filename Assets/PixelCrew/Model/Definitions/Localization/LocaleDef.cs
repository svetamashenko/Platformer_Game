using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;

namespace Assets.PixelCrew.Model.Definitions.Localization
{
    [CreateAssetMenu(menuName = "Defs/LocaleDef", fileName = "LocaleDef")]
    public class LocaleDef : ScriptableObject
    {
        public enum LoadSource
        {
            File,
            URL,
            Manual
        }

        [SerializeField] private LoadSource _source = LoadSource.Manual;
        [SerializeField] private string _url;
        [SerializeField] private string _fileName;
        [SerializeField] private List<LocaleItem> _localeitems;

        private UnityWebRequest _request;

        public Dictionary<string, string> GetData()
        {
            var dictionary = new Dictionary<string, string>();
            foreach (var localeItem in _localeitems)
            {
                dictionary.Add(localeItem.Key, localeItem.Value);
            }
            return dictionary;
        }

        [ContextMenu("UpdateLocale")]
        public void UpdateLocale()
        {
            switch (_source)
            {
                case LoadSource.File:
                    LoadFromFile();
                    break;
                case LoadSource.URL:
                    LoadFromUrl();
                    break;
                case LoadSource.Manual:
                    Debug.Log("Manual localization - no auto-update");
                    break;
            }
        }

        private void LoadFromFile()
        {
            if (string.IsNullOrEmpty(_fileName))
            {
                Debug.LogError("No file name specified");
                return;
            }

            try
            {
                var textAsset = Resources.Load<TextAsset>($"Locales/{_fileName}");

                if (textAsset == null)
                {
                    string streamingPath = Path.Combine(Application.streamingAssetsPath, "Locales", _fileName + ".txt");
                    if (File.Exists(streamingPath))
                    {
                        var content = File.ReadAllText(streamingPath);
                        ParseContent(content);
                        Debug.Log($"Localization loaded from StreamingAssets: {_fileName} ({_localeitems.Count} entries)");
                        return;
                    }

                    Debug.LogError($"File not found: {_fileName} (tried Resources/Locales/ and StreamingAssets/Locales/)");
                    return;
                }

                ParseContent(textAsset.text);
                Debug.Log($"Localization loaded from Resources: {_fileName} ({_localeitems.Count} entries)");
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to load localization from file: {e.Message}");
            }
        }

        private void ParseContent(string content)
        {
            var rows = content.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            _localeitems.Clear();

            foreach (var row in rows)
            {
                var trimmedRow = row.Trim();
                if (!string.IsNullOrEmpty(trimmedRow) && !trimmedRow.StartsWith("#"))
                {
                    AddLocaleItem(trimmedRow);
                }
            }
        }

        private void LoadFromUrl()
        {
            if (string.IsNullOrEmpty(_url))
            {
                Debug.LogError("No URL specified for localization");
                return;
            }

            if (_request != null) return;

            _request = UnityWebRequest.Get(_url);
            _request.SendWebRequest().completed += OnDataLoaded;
        }

        private void OnDataLoaded(AsyncOperation operation)
        {
            if (operation.isDone)
            {
                try
                {
                    var content = _request.downloadHandler.text;
                    ParseContent(content);
                    Debug.Log($"Localization loaded from URL: {_url} ({_localeitems.Count} entries)");
                }
                catch (Exception e)
                {
                    Debug.LogError($"Failed to parse localization from URL: {e.Message}");
                }
                finally
                {
                    _request = null;
                }
            }
        }

        private void AddLocaleItem(string row)
        {
            try
            {
                var parts = row.Split('\t');
                if (parts.Length >= 2)
                {
                    var key = parts[0].Trim();
                    var value = parts[1].Trim();

                    if (parts.Length > 2)
                    {
                        for (int i = 2; i < parts.Length; i++)
                        {
                            value += "\t" + parts[i].Trim();
                        }
                    }

                    _localeitems.Add(new LocaleItem { Key = key, Value = value });
                }
                else
                {
                    Debug.LogWarning($"Skipping invalid row (need at least 2 columns): {row}");
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Can't parse row: {row}.\n {e}");
            }
        }

        [Serializable]
        private class LocaleItem
        {
            public string Key;
            public string Value;
        }
    }
}