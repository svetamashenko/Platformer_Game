using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

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

        public override void OnCloseAnimetionCompleted()
        {
            _closeAction?.Invoke();
            base.OnCloseAnimetionCompleted();
        }
    }
}