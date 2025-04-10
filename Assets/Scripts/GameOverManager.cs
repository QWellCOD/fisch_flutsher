using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    /// <summary>
    /// This method restarts the game.
    /// It resumes the game by setting the time scale to 1, resets the game speed, and reloads the current scene.
    /// </summary>
    public void RestartGame(AudioMixerSnapshot snapshot)
    {
        Debug.Log("Restart Game button clicked! Loading Game Scene...");
        snapshot.TransitionTo(0);
        Time.timeScale = 1;
        //GameManager.Instance.ResetSpeed();
        SceneManager.LoadScene("GameScene");
    }

    /// <summary>
    /// This method returns the player to the main menu.
    /// It resumes the game by setting the time scale to 1, resets the game speed, and loads the home scene.
    /// </summary>
    public void ReturnToMainMenu(AudioMixerSnapshot snapshot)
    {
        Debug.Log("Main Menu button clicked! Loading Home Scene...");
        snapshot.TransitionTo(0);
        Time.timeScale = 1;
        //GameManager.Instance.ResetSpeed();
        SceneManager.LoadScene("HomeScene");
    }
}
