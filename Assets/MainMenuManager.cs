using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public void PlayGame()
    {
        Debug.Log("Play button clicked! Loading Game Scene...");
        SceneManager.LoadScene("GameScene"); // Passe den Namen der Spielszene an
    }
}
