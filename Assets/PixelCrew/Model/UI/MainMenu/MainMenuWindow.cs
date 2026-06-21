using Assets.PixelCrew.Utils;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Assets.PixelCrew.Components.UI.MainMenu
{
    public class MainMenuWindow : AnimatedWindow
    {
        private Action _closeAction;
        public void OnShowSettings()
        {
            var window = Resources.Load<GameObject>("UI/SettingsWindow");
            var canvas = FindObjectOfType<Canvas>();
            Instantiate(window, canvas.transform);
        }
        public void OnStartGame()
        {
            _closeAction = () => { SceneManager.LoadScene("Level1"); };
            Close();
        }

        public void OnRestartLevel()
        {
            var scene = SceneManager.GetActiveScene();
            _closeAction = () => { SceneManager.LoadScene(scene.name); };
            Close();
        }

        public void OnExit()
        {
            _closeAction = () => {

                Application.Quit();

#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#endif
            };
            Close();
        }

        public void OnLanguages()
        {
            WindowUtils.CreateWindow("UI/LocalizationWindow");
        }

        public void OpenMenu()
        {
            GameObject newCanvasGO = new GameObject("NewCanvas");
            Canvas newCanvas = newCanvasGO.AddComponent<Canvas>();
            newCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            newCanvas.pixelPerfect = true;

            CanvasScaler scaler = newCanvasGO.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(320f, 180f);
            scaler.referencePixelsPerUnit = 32;

            newCanvasGO.AddComponent<GraphicRaycaster>();

            GameObject settingsWindowPrefab = Resources.Load<GameObject>("UI/GameMenuWindow Variant");
            GameObject instance = Instantiate(settingsWindowPrefab, newCanvas.transform);

            newCanvasGO.transform.SetAsLastSibling();
        }

        public override void OnCloseAnimetionCompleted()
        {
            _closeAction?.Invoke();
            base.OnCloseAnimetionCompleted();
        }
    }
}