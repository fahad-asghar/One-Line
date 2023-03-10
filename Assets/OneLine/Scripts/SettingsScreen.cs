using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class SettingsScreen : MonoBehaviour
{
    [SerializeField] Button musicButton;
    [SerializeField] Transform musicDot;
    [SerializeField] GameObject musicOnText;
    [SerializeField] GameObject musicOffText;

    [SerializeField] Button soundButton;
    [SerializeField] Transform soundDot;
    [SerializeField] GameObject soundOnText;
    [SerializeField] GameObject soundOffText;

    [SerializeField] GameObject resetConfirmationPopUp;

    float switchPositionOn = 54;
    float switchPositionOff = -54;

    private void Start()
    {
        ChangeMusic(Music.instance.IsEnabled());
        ChangeSound(Sound.instance.IsEnabled());
    }

    public void ToggleSound()
    {
        bool isEnabled = !Sound.instance.IsEnabled();
        Sound.instance.SetEnabled(isEnabled);
        ChangeSound(isEnabled);
    }

    void ChangeSound(bool isEnabled)
    {
        if (isEnabled)
        {
            soundButton.interactable = false;
            soundOnText.SetActive(true);
            soundOffText.SetActive(false);
            soundDot.DOLocalMoveX(switchPositionOn, 0.05f, false).OnComplete(delegate ()
            {
                soundButton.interactable = true;
            });
        }
        else
        {
            soundButton.interactable = false;
            soundOnText.SetActive(false);
            soundOffText.SetActive(true);
            soundDot.DOLocalMoveX(switchPositionOff, 0.05f, false).OnComplete(delegate ()
            {
                soundButton.interactable = true;
            });
        }
    }


    public void ToggleMusic()
    {
        bool isEnabled = !Music.instance.IsEnabled();
        Music.instance.SetEnabled(isEnabled, true);
        ChangeMusic(isEnabled);
    }

    void ChangeMusic(bool isEnabled)
    {
        if (isEnabled)
        {
            musicButton.interactable = false;
            musicOnText.SetActive(true);
            musicOffText.SetActive(false);
            musicDot.DOLocalMoveX(switchPositionOn, 0.05f, false).OnComplete(delegate ()
            {
                musicButton.interactable = true;
            });
        }
        else
        {
            musicButton.interactable = false;
            musicOnText.SetActive(false);
            musicOffText.SetActive(true);
            musicDot.DOLocalMoveX(switchPositionOff, 0.05f, false).OnComplete(delegate ()
            {
                musicButton.interactable = true;
            });
        }
    }

    public void PrivacyPolicy()
    {
        Application.OpenURL("http://playmania.io/privacy-policy");
    }

    public void ResetGame()
    {
        resetConfirmationPopUp.SetActive(true);
    }

    public void Reset()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        File.Delete(Application.persistentDataPath + "/userInfo3.dat");

        UIControllerForGame.solvedPuzzlesCounter = 0;
        Music.instance.SetEnabled(true, true);
        Sound.instance.SetEnabled(true);

        PlayerData.instance.LoadData();
        UIController.mode = UIController.UIMODE.OPENWORLDSCREEN;
        SceneManager.LoadScene(0);
    }

    public void Cancel()
    {
        resetConfirmationPopUp.SetActive(false);
    }
}
