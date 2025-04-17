using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    public void RestartGame(AudioMixerSnapshot snapshot)
    {
        Debug.Log("Restart Game button clicked! Loading Game Scene...");
        snapshot.TransitionTo(0);
        Time.timeScale = 1;
        SceneManager.LoadScene("GameScene");
    }
    public void ReturnToMainMenu(AudioMixerSnapshot snapshot)
    {
        Debug.Log("Main Menu button clicked! Loading Home Scene...");
        snapshot.TransitionTo(0);
        Time.timeScale = 1;
        SceneManager.LoadScene("HomeScene");
    }
}
