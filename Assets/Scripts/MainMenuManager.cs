using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class MainMenuManager : MonoBehaviour
{
    public void PlayGame(AudioMixerSnapshot snapshot)
    {
        Debug.Log("Play button clicked! Loading Game Scene...");
        snapshot.TransitionTo(1);
        SceneManager.LoadScene("GameScene");
    }
}
