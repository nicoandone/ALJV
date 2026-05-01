using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenu : MonoBehaviour
{
    public void StartEasy()
    {
        GameSettings.selectedDifficulty = GameDifficulty.Easy;
        SceneManager.LoadScene(0);
    }

    public void StartHard()
    {
        GameSettings.selectedDifficulty = GameDifficulty.Hard;
        SceneManager.LoadScene(0);
    }
}