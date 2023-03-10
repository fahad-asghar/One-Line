using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
#if EASY_MOBILE
using EasyMobile;
#endif

public class UIController : MonoBehaviour
{

    public static int totalLevel = LevelData.totalLevelsPerWorld * LevelData.worldNames.Length;
    public static int totalLevelInWorld = LevelData.totalLevelsPerWorld;

    public Transform packagesContent, levelsContent;

    public enum UIMODE : int
    {
        OPENPLAYSCREEN,
        OPENLEVELSCREEN,
        OPENWORLDSCREEN
    }

    public static UIMODE mode = UIMODE.OPENWORLDSCREEN;

    public GameObject playScreen;

    public GameObject levelScreen;

    public GameObject worldScene;

    [SerializeField] GameObject settingsScreen;


    public Sprite disableSprite;
    public Sprite enableSprite;

    public GameObject soundButton;
    public Sprite soundOn;
    public Sprite soundOf;

    public GameObject musicButton;
    public Sprite musicOn;
    public Sprite musicOff;

    public Image shopImage;
    public Sprite rateSprite;


    // Use this for initialization
    void Start()
    {
        if (mode == UIMODE.OPENPLAYSCREEN)
        {
            EnablePlayScreen();
        }
        else if (mode == UIMODE.OPENWORLDSCREEN)
        {
            EnableWorldScreen();
        }
        else if (mode == UIMODE.OPENLEVELSCREEN)
        {
            EnableStageScreen(LevelData.worldSelected);
        }

        CUtils.ChangeGameMusic();
    }

    public void PlayButtonSound()
    {
        Sound.instance.PlayButton();
    }

    public void EnablePlayScreen()
    {
        playScreen.SetActive(true);

        levelScreen.SetActive(false);
        worldScene.SetActive(false);
    }

    public void EnableSettingsScreen()
    {
        settingsScreen.SetActive(true);
        levelScreen.SetActive(false);
        worldScene.SetActive(false);
    }

    public void EnableWorldScreen()
    {
        PrepareWorldScreen();
        playScreen.SetActive(false);
        settingsScreen.SetActive(false);
        levelScreen.SetActive(false);
        worldScene.SetActive(true);
    }

    public void EnableStageScreen()
    {
        playScreen.SetActive(false);

        levelScreen.SetActive(true);
        worldScene.SetActive(false);
    }

    // data for worlds
    private void PrepareWorldScreen()
    {
        int cCount = packagesContent.childCount;
        UpdateWorldTitle(worldScene.transform.Find("Title"));

        for (int i = 0; i < (cCount - 1); i++)
        {
            UpdateWorld(packagesContent.GetChild(i), i + 1);
        }
    }

    private void UpdateWorldTitle(Transform title)
    {
        Text levels = title.GetComponentInChildren<Text>();
        levels.text = "" + PlayerData.instance.GetTotalLevelCrossed() + " / " + totalLevel;
    }

    private void UpdateWorld(Transform world, int index)
    {
        int isUnlocked = PlayerData.instance.LEVELUNLOCKED[index];

        var starObj = world.Find("Button/Star").gameObject;
        var progressTextObj = world.Find("Button/ProgressText").gameObject;
        var lockedTextObj = world.Find("Button/Locked").gameObject;
        var packageName = world.Find("PackageName").GetComponent<Text>();
        packageName.text = LevelData.worldNames[index - 1];

        if (index > 1 && isUnlocked == 0)
        {
            int prvLevelCrossed = PlayerData.instance.LevelCrossedForOneWorld(index - 1);

            if (prvLevelCrossed < LevelData.prvLevelToCrossToUnLock)
            {
                starObj.SetActive(false);
                progressTextObj.SetActive(false);
                lockedTextObj.SetActive(true);
                return;
            }
        }

        starObj.SetActive(true);
        progressTextObj.SetActive(true);
        lockedTextObj.SetActive(false);

        int levelCrossed = PlayerData.instance.LevelCrossedForOneWorld(index);

        Text levels = world.GetComponentInChildren<Text>();
        levels.text = "" + levelCrossed + " / " + totalLevelInWorld;

        if (levelCrossed >= LevelData.totalLevelsPerWorld)
        {
            levels.gameObject.SetActive(false);
            world.Find("Button/Tick").gameObject.SetActive(true);
        }
        else
        {
            levels.gameObject.SetActive(true);
            world.Find("Button/Tick").gameObject.SetActive(false);
        }

    }


    //data for level
    public void EnableStageScreen(int indexWorld)
    {
        if (indexWorld == 1)
        {
            LevelSetup(indexWorld);
            EnableStageScreen();
        }
        else
        {
            if (PlayerData.instance.LevelCrossedForOneWorld(indexWorld - 1) == LevelData.totalLevelsPerWorld)
            {
                LevelSetup(indexWorld);
                EnableStageScreen();
            }

            else
            {
                //IAP PopUp
            }
            //CUSTOM LOGIC FOR IN-APP

            /*LevelData.pressedWorld = indexWorld;
            int isUnLockedByInApp = PlayerData.instance.LEVELUNLOCKED[indexWorld];

            if (isUnLockedByInApp == 0)
            {
                int prvLevelCrossed = PlayerData.instance.LevelCrossedForOneWorld(indexWorld - 1);

                if (prvLevelCrossed >= LevelData.prvLevelToCrossToUnLock)
                {
                    // play level
                    LevelSetup(indexWorld);
                    EnableStageScreen();
                }
                else
                {
                    //unlockPackageDialog.Show(LevelData.worldNames, indexWorld);
                }
            }
            else
            {
                //play level
                LevelSetup(indexWorld);
                EnableStageScreen();
            }*/
        }
    }

    private void LevelSetup(int indexWorld)
    {
        LevelData.worldSelected = indexWorld;
        LevelScreenReader(indexWorld);
    }

    void LevelScreenReader(int indexWorld)
    {
        Transform top = levelScreen.transform.GetChild(1);
        top.Find("title").GetComponent<Text>().text = LevelData.worldNames[indexWorld - 1];
        top.Find("Score").GetComponent<Text>().text = PlayerData.instance.LevelCrossedForOneWorld(indexWorld) + "/" + totalLevelInWorld;

        // list of all levelssssss
        int largetLevel = PlayerData.instance.GetLargestLevel(indexWorld);
        for (int i = 0; i < LevelData.totalLevelsPerWorld; i++)
        {
            bool isShownHint = true;
            Transform child = levelsContent.GetChild(i);
            child.GetComponentInChildren<Text>().text = "" + (i + 1);
            Transform locked = child.Find("Locked");
            Transform unlocked = child.Find("Unlocked");

            //if (i < largetLevel + 3)
            //{
            locked.gameObject.SetActive(false);
            unlocked.gameObject.SetActive(true);

            if (PlayerData.instance.IsLevelCrossed(indexWorld, i + 1))
            {
                isShownHint = false;
                unlocked.Find("Star1").gameObject.SetActive(true);
                unlocked.Find("Star2").gameObject.SetActive(false);
            }
            else
            {
                unlocked.Find("Star1").gameObject.SetActive(false);
                unlocked.Find("Star2").gameObject.SetActive(true);
            }

            child.GetComponent<Button>().interactable = true;
            //}

            /*else
              {
                  locked.gameObject.SetActive(true);
                  unlocked.gameObject.SetActive(false);
                  child.GetComponent<Button>().interactable = false;
              }*/

            /*if (isShownHint && LevelData.isLevelIsHintLevel(indexWorld, i + 1))
            {
                child.Find("Hint").gameObject.SetActive(true);
            }
            else
            {
                child.Find("Hint").gameObject.SetActive(false);
            }*/
        }
    }

    public void Share()
    {
#if UNITY_EDITOR
#if EASY_MOBILE
        Toast.instance.ShowMessage("This feature only works in Android and iOS");
#else
        Toast.instance.ShowMessage("See Console for more information");

        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        sb.Append("You need to import Easy Mobile Lite (free) to use this function. This is how to import:(Click this log to see full instruction)").AppendLine();
        sb.Append("1. Open the asset store: Window -> General -> Asset Store or Window -> Asset Store").AppendLine();
        sb.Append("2. Search: Easy Mobile Lite").AppendLine();
        sb.Append("3. Download and import it");

        Debug.LogWarning(sb.ToString());
#endif
#elif (UNITY_ANDROID || UNITY_IPHONE) && EASY_MOBILE
        StartCoroutine(DoShare());
#endif
    }

    private IEnumerator DoShare()
    {
        yield return new WaitForEndOfFrame();
#if EASY_MOBILE
        Sharing.ShareScreenshot("screenshot", "");
#endif
    }

    public void LoadLevel(int levelSelected)
    {
        PlayButtonSound();
        LevelData.levelSelected = levelSelected;
        SceneManager.LoadScene(1);
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
            musicButton.GetComponent<Image>().sprite = musicOn;
        }
        else
        {
            musicButton.GetComponent<Image>().sprite = musicOff;
        }
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
            soundButton.GetComponent<Image>().sprite = soundOn;
        }
        else
        {
            soundButton.GetComponent<Image>().sprite = soundOf;
        }
    }

}
