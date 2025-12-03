using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitLevelComponent : MonoBehaviour
{
    [SerializeField] private string _sceneName;
    public void Exit()
    {
        SceneManager.LoadScene(_sceneName);
    }
}
