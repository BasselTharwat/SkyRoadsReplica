using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagerScript : MonoBehaviour
{
    public void loadScene(string sceneName)
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(sceneName);
    }
    public void QuitGame()
    {

        if( UnityEditor.EditorApplication.isPlaying == true)
        {
            // If running in the Unity editor, this will stop the play mode
            UnityEditor.EditorApplication.isPlaying = false;
        }
        else
        {
            // If running in a build, this will close the game
            Application.Quit();
        }
    }
}
