using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    int savedLevel;
    private int startLevel = 3;
    public Button continueButton;

    // Start is called before the first frame update
    void Start()
    {
        savedLevel = PlayerPrefs.GetInt("save");
        Debug.Log(savedLevel);
        continueButton = continueButton.GetComponent<Button>();
        if (savedLevel == 3 || savedLevel == 0)
        {
            continueButton.enabled = false;
            ColorBlock colorBlock = continueButton.colors;
            colorBlock.normalColor = Color.grey;
            continueButton.colors = colorBlock;
        }
    }

    public void continueGame()
    {
        savedLevel = PlayerPrefs.GetInt("save");
        SceneManager.LoadScene(savedLevel);
    }

    public void newGame()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.SetInt("life", 6);
        SceneManager.LoadScene(startLevel);
    }

    public void settings()
    {
        Debug.Log("Settings OK!");
    }

    public void quit()
    {
        Debug.Log("Quit");
        Application.Quit();
    }
}
