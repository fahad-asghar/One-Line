using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class Theme
{
    public Color dotColor;
    public Color drawingLineColor;
    public Color lineColor;
}

public class ThemeChanger : MonoBehaviour {

    public Theme[] themes;
    public static Theme current;
    public static Theme current1;
    public static ThemeChanger instance;

    private void Awake()
    {
        instance = this;

        if(SceneManager.GetActiveScene().name != "GameScene")
            current = themes[Random.Range(0, themes.Length)];
    }

    public void RandomColor()
    {
        current = themes[Random.Range(0, themes.Length)];
    }
}
