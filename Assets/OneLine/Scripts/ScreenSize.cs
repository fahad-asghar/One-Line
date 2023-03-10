using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenSize : MonoBehaviour
{
    private void Awake()
    {
        Camera.main.aspect = 1080f / 1920f;
    }
}
