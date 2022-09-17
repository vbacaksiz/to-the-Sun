using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    int maximumHealth = 6;
    private static UIManager _instance;
    public static UIManager instance
    {
        get
        {
            if(_instance == null)
            {
                Debug.LogError("UIManager is null");
            }

            return _instance;
        }
    }

    private void Awake()
    {
        _instance = this;
    }

    public Image[] lifeBars;

    public void showCurrentLifeBars(int currentHealth)
    {
        for(int i = 0; i < maximumHealth; i++)
        {
            if (i < currentHealth)
                lifeBars[i].enabled = true;
            else
                lifeBars[i].enabled = false;
        }
    }

    public void UpdateLifeBars(int remainingLife)
    {
        for(int i = 0; i <= remainingLife; i++)
        {
            if(i == remainingLife)
            {
                lifeBars[i].enabled = false;
            }
        }
    }
}
