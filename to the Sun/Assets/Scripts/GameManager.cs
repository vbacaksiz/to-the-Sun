using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    int buildIndex = 0;
    PlayerControl player;
    // Start is called before the first frame update
    void Start()
    {
        buildIndex = SceneManager.GetActiveScene().buildIndex;
        PlayerPrefs.SetInt("save", SceneManager.GetActiveScene().buildIndex);
        Debug.Log(buildIndex);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
