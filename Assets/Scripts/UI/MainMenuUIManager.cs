using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuUIManager : MonoBehaviour
{
    [Obsolete("Obsolete")]
    private void Awake()
    {
        Cursor.visible = false;
        Time.timeScale = 1;
    }

    public void CloseGame()
    {
        Application.Quit();
    }
}
