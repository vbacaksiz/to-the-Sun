using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenuController : MonoBehaviour
{
    private static bool gamePaused = false;

    public GameObject PauseMenu;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (gamePaused)
                Resume();
            else
                Pause();
        }
    }

    public void Resume()
    {
        Time.timeScale = 1f;
        PauseMenu.SetActive(false);
        gamePaused = false;
    }

    public void Pause()
    {
        Time.timeScale = 0f;
        PauseMenu.SetActive(true);
        gamePaused = true;
    }

    public void Restart()
    {
        Time.timeScale = 1f;
        PauseMenu.SetActive(false);
        gamePaused = false;
        int savedLevel = PlayerPrefs.GetInt("save");
        SceneManager.LoadScene(savedLevel);
    }

    public void toMenu()
    {
        Time.timeScale = 1f;
        PauseMenu.SetActive(false);
        gamePaused = false;
        int menuSceneIndex = 0;
        SceneManager.LoadScene(menuSceneIndex);
    }
}
