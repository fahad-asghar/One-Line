using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class UIControllerForGame : MonoBehaviour
{
    public Text hintText;
    public Text stageText;
    public Text packageName;

    [SerializeField] GameObject pauseScene;
    [SerializeField] GameObject puzzleCompeletePopUp;
    [SerializeField] GameObject puzzleCompletedWithReward;
    [SerializeField] GameObject packCompletedPopUp;
    [SerializeField] GameObject rateUsPopUp;
    [SerializeField] GameObject rewardRateUsPopUp;

    public static int solvedPuzzlesCounter = 0;

    public GameObject dotAnim;

    void Start()
    {
        UpdateHint();
        Invoke("ShowRateUs", 1);
    }

    public void PlayButtonSound()
    {
        Sound.instance.PlayButton();
    }

    public void UpdateHint()
    {
        if (PlayerData.instance.NumberOfHints == 0)
        {
            hintText.fontSize = 111;
            hintText.text = "+";
        }
        else
        {
            hintText.fontSize = 60;
            hintText.text = "" + PlayerData.instance.NumberOfHints;
        }

        int level = LevelData.levelSelected;
        int world = LevelData.worldSelected;

        stageText.text = level + "/" + LevelData.totalLevelsPerWorld;
        packageName.text = LevelData.worldNames[world - 1];
    }

    public void ShowPauseScene()
    {
        pauseScene.SetActive(true);
    }

    public void ResumeGame()
    {
        pauseScene.SetActive(false);
    }

    public void OpenStageMode()
    {
        UIController.mode = UIController.UIMODE.OPENLEVELSCREEN;
        SceneManager.LoadScene(0);
    }
    public void OpenHomeMode()
    {
        UIController.mode = UIController.UIMODE.OPENPLAYSCREEN;
        SceneManager.LoadScene(0);
    }

    private void ShowRateUs()
    {
        if (solvedPuzzlesCounter > Constants.regularRateUsPuzzle
            && PlayerPrefs.GetInt("RegularRateUsAppeared") == 0)
        {
            PlayerPrefs.SetInt("RegularRateUsAppeared", 1);
            rateUsPopUp.SetActive(true);
            return;
        }

        if (solvedPuzzlesCounter >= Constants.rewardRateUsPuzzle
            && solvedPuzzlesCounter % Constants.rewardRateUsPuzzle == 0
            && PlayerPrefs.GetInt("IsRated") == 0)
        {
            rewardRateUsPopUp.SetActive(true);
            return;
        }
    }

    public void ShowWinUi()
    { 
        int world = LevelData.worldSelected;
        int stage = LevelData.levelSelected;

        Sound.instance.Play(Sound.Others.Win);
        PlayerData.instance.SetLevelCrossed(LevelData.worldSelected, stage);
        solvedPuzzlesCounter++;

        if (PlayerData.instance.LevelCrossedForOneWorld(world) ==
            LevelData.totalLevelsPerWorld && PlayerPrefs.GetInt("pack" + world) == 0)
        {
            PlayerPrefs.SetInt("pack" + world, 1);
            packCompletedPopUp.SetActive(true);
        }
        else if (solvedPuzzlesCounter % Constants.popUpBonusFrequency == 0)
            puzzleCompletedWithReward.SetActive(true);

        else
            puzzleCompeletePopUp.SetActive(true);
    }

    public void CollectHintsOnPuzzleCompleted(int hints)
    {
        Sound.instance.PlayButton();
        int stage = LevelData.levelSelected;

        PlayerData.instance.NumberOfHints += hints;
        PlayerData.instance.SaveData();

        if (stage == LevelData.totalLevelsPerWorld)
        {
            UIController.mode = UIController.UIMODE.OPENWORLDSCREEN;
            SceneManager.LoadScene(0);
            return;
        }

        LevelData.levelSelected++;
        SceneManager.LoadScene(1);
    }

    public void CollectHintsOnPackCompleted(int hints)
    {
        Sound.instance.PlayButton();
        int stage = LevelData.levelSelected;

        PlayerData.instance.NumberOfHints += hints;
        PlayerData.instance.SaveData();

        UIController.mode = UIController.UIMODE.OPENWORLDSCREEN;
        SceneManager.LoadScene(0);
    }

    public void LoadNextLevel()
    {
        Sound.instance.PlayButton();
        int stage = LevelData.levelSelected;

        if (stage == LevelData.totalLevelsPerWorld)
        {
            UIController.mode = UIController.UIMODE.OPENWORLDSCREEN;
            SceneManager.LoadScene(0);
            return;
        }

        LevelData.levelSelected++;
        SceneManager.LoadScene(1);
    }

    public void RateUs()
    {
        Sound.instance.PlayButton();
        PlayerPrefs.SetInt("IsRated", 1);
        rateUsPopUp.SetActive(false);
        Application.OpenURL("market://details?id=" + Application.identifier);
    }

    public void Later()
    {
        Sound.instance.PlayButton();
        rateUsPopUp.SetActive(false);
    }

    public void RewardedRateUs(int hints)
    {
        Sound.instance.PlayButton();

        PlayerPrefs.SetInt("IsRated", 1);
        PlayerData.instance.NumberOfHints += hints;
        PlayerData.instance.SaveData();
        UpdateHint();
        rewardRateUsPopUp.SetActive(false);
        Application.OpenURL("market://details?id=" + Application.identifier);
    }

    public void NoThanks()
    {
        Sound.instance.PlayButton();
        rewardRateUsPopUp.SetActive(false);
    }

    public void ShowAnimationOnAllNodes()
    {
        GameObject.FindObjectOfType<DotAnimation>().gameObject.SetActive(false);
        WaysUI[] allUis = GameObject.FindObjectsOfType<WaysUI>();
        List<Vector3> dotAnimations = new List<Vector3>();

        foreach (WaysUI wayUi in allUis)
        {
            for (int i = 0; i < 2; i++)
            {
                Vector3 pos = wayUi.childPos(i);

                if (!dotAnimations.Contains(pos))
                {
                    GameObject an = Instantiate(dotAnim) as GameObject;
                    an.GetComponent<DotAnimation>().setTargetScale(2.5f);
                    an.GetComponent<DotAnimation>().setEnableAtPosition(true, pos);
                    an.GetComponent<DotAnimation>().scalingSpeed =  2.5f;
                    dotAnimations.Add(pos);
                }
            }
        }

        Invoke("ShowWinUi", 1.3f);
    }
}
