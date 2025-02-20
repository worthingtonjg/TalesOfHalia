using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelStart : MonoBehaviour
{
    public string levelName;

    public void StartLevel()
    {
        SceneManager.LoadScene(levelName);
    }
}
