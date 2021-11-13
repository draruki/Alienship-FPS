using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GUIManager : MonoBehaviour 
{
    // general
    public GameManager gameManager;
    public PlayerEntity player;

    // icons
    public Color weaponEquippedColor;
    public Color weaponSheathedColor;
    public Color weaponEquippedNoAmmoColor;
    public Color weaponSheathedNoAmmoColor;
    public Color lowHealthColor;
    private int lowHealthPercentage = 20;

    // GUI
    public Image healthSprite;
    public Text healthText;
    public Text ammoText;
    public Image shotgunSprite;
    public Image rifleSprite;
    public Image ammoSprite;
    public Image pistolSprite;
    public Image crosshairSprite;
    public Image hurtSprite;

    // fade
    private float fadeSpeed;
    public float normalFadeTime = 0.01f;
    private int fading;
    private Color fadeTempColor;
    public Image fadeBackground;

    // UI (menus)
    public GameObject gameMenu;
    public GameObject mainMenu;
    public GameObject optionsMenu;
    public Image mainMenuAlphaImage;
    public Image mainMenuDarkCorners;
    public RawImage mainMenuBackgroundPicture;
    public RectTransform[] screenImageTransforms;

    // audio
    private bool ignoreFirstMenuSound;
    private AudioSource audioSource;
    private int maxAudios = 3;

    // resolution
    private int resolutionIndex = 0;
    public Text resolutionText;
    public Toggle windowedToggle;
    public Toggle audioToggle;
    private int triesToGetReso = 50;

    // input
    public Text[] gameMenuTexts;
    public Text[] mainMenuTexts;
    public Text[] optionsTexts;
    public Color hoverColor;
    public Color normalColor;
    private bool confirmMenuQuit;
    private int inputIndex;
    private bool inputLocked;

    // death
    public Text deathText;
    public Image deathSprite;

    // hurt
    public float hurtTime = 0.1f;
    
    // sliders
    public Slider fovSlider;
    public Slider musicSlider;
    public Slider masterSlider;
    public Slider soundSlider;

    void Awake()
    {
        gameMenu.SetActive(false);
        mainMenu.SetActive(false);
        optionsMenu.SetActive(false);
        confirmMenuQuit = false;
        fading = GC.NOT_FADING;
        darkenAll();
        fadeSpeed = 0;
        inputLocked = false;
        inputIndex = 0;
        fadeBackground.enabled = false;
        fadeTempColor = new Color(0, 0, 0, 0f);
        fadeBackground.color = fadeTempColor;
    }

	void Start () 
    {
        masterSlider.onValueChanged.AddListener(optionsOnMasterVolume);
        musicSlider.onValueChanged.AddListener(optionsOnMusicVolume);
        soundSlider.onValueChanged.AddListener(optionsOnSoundVolume);
        fovSlider.onValueChanged.AddListener(optionsOnFov);
        masterSlider.value = AudioListener.volume;
        musicSlider.value = GC.MUSIC_VOLUME;
        audioSource = GetComponent<AudioSource>();
        soundSlider.value = GC.SOUND_VOLUME;
        windowedToggle.isOn = !Screen.fullScreen;
        hideDeathScreen();
        getResolution();
        hurtSprite.enabled = false;
        audioToggle.isOn = AudioListener.pause;
        scaleBackgroundsToScreen();
	}
	
	void Update () 
    {
        if (fading == GC.FADING_IN)
        {
            fadeTempColor.a -= fadeSpeed * Time.deltaTime;
            fadeBackground.color = fadeTempColor;
            if (fadeTempColor.a <= 0.05f)
                stopFade();
        }
        else if (fading == GC.FADING_OUT)
        {
            fadeTempColor.a += fadeSpeed * Time.deltaTime;
            fadeBackground.color = fadeTempColor;
            if (fadeTempColor.a >= 1f)
                stopFade();
        }

        if (GC.PAUSED)
            return;

        if (player != null)
        {
            healthText.text = player.health.ToString();
            ammoText.text = player.getAmmo().ToString();
        }

        if (gameManager.playerHasShotgun && !shotgunSprite.gameObject.activeInHierarchy)
            shotgunSprite.gameObject.SetActive(true);
        else if (!gameManager.playerHasShotgun && shotgunSprite.gameObject.activeInHierarchy)
            shotgunSprite.gameObject.SetActive(false);

        if (gameManager.playerHasRifle && !rifleSprite.gameObject.activeInHierarchy)
            rifleSprite.gameObject.SetActive(true);
        else if (!gameManager.playerHasRifle && rifleSprite.gameObject.activeInHierarchy)
            rifleSprite.gameObject.SetActive(false);

        if (player.getAmmo() <= 0)
        {
            ammoText.color = weaponEquippedNoAmmoColor;
            ammoSprite.color = weaponEquippedNoAmmoColor;
        }
        else
        {
            ammoText.color = weaponEquippedColor;
            ammoSprite.color = weaponEquippedColor;
        }

        if (player.health <= lowHealthPercentage)
        {
            healthText.color = lowHealthColor;
            healthSprite.color = lowHealthColor;
        }
        else
        {
            healthText.color = weaponEquippedColor;
            healthSprite.color = weaponEquippedColor;
        }

        if (player.singleAmmo > 0)
        {
            if (player.currentWeapon == WeaponManager.pistol)
                pistolSprite.color = weaponEquippedColor;
            else
                pistolSprite.color = weaponSheathedColor;
        }
        else
        {
            if (player.currentWeapon == WeaponManager.pistol)
                pistolSprite.color = weaponEquippedNoAmmoColor;
            else
                pistolSprite.color = weaponSheathedNoAmmoColor;
        }

        if (player.spreadAmmo > 0)
        {
            if (player.currentWeapon == WeaponManager.shotgun)
                shotgunSprite.color = weaponEquippedColor;
            else
                shotgunSprite.color = weaponSheathedColor;
        }
        else
        {
            if (player.currentWeapon == WeaponManager.shotgun)
                shotgunSprite.color = weaponEquippedNoAmmoColor;
            else
                shotgunSprite.color = weaponSheathedNoAmmoColor;
        }

        if (player.autoAmmo > 0)
        {
            if (player.currentWeapon == WeaponManager.rifle)
                rifleSprite.color = weaponEquippedColor;
            else
                rifleSprite.color = weaponSheathedColor;
        }
        else
        {
            if (player.currentWeapon == WeaponManager.rifle)
                rifleSprite.color = weaponEquippedNoAmmoColor;
            else
                rifleSprite.color = weaponSheathedNoAmmoColor;
        }
	}

    private void scaleBackgroundsToScreen()
    {
        /*
        print(Screen.width + "  " + Screen.height);

        for (int i = 0; i < screenImageTransforms.Length; ++i)
        {
            if (screenImageTransforms[i] != null)
                screenImageTransforms[i].sizeDelta = new Vector2(Screen.width, Screen.height);
        }
        */
    }

    private void getResolution()
    {
        for (int i = 0; i < triesToGetReso; ++i)
        {
            optionsOnNextResolution();

            if (Screen.resolutions[resolutionIndex].width == Screen.width)
            {
                return;
            }

        }
    }

    public void setWeapon(Weapon weapon)
    {
        if (weapon == WeaponManager.pistol)
        {
            crosshairSprite.sprite = Assets.instance.pistolCrosshairSprite;
            crosshairSprite.SetNativeSize();
            ammoSprite.sprite = Assets.instance.singleAmmoSprite;
        }
        else if (weapon == WeaponManager.shotgun)
        {
            crosshairSprite.sprite = Assets.instance.shotgunCrosshairSprite;
            crosshairSprite.SetNativeSize();
            ammoSprite.sprite = Assets.instance.spreadAmmoSprite;
        }
        else if (weapon == WeaponManager.rifle)
        {
            crosshairSprite.sprite = Assets.instance.rifleCrosshairSprite;
            crosshairSprite.SetNativeSize();
            ammoSprite.sprite = Assets.instance.autoAmmoSprite;
        }
    }

    public void fadeInTime(float time)
    {
        fadeIn();
        fadeSpeed = 1f / time;
    }
    public void fadeOutTime(float time)
    {
        fadeOut();
        fadeSpeed = 1f / time;
    }
    public void fadeIn()
    {
        fading = GC.FADING_IN;
        fadeBackground.enabled = true;
        fadeSpeed = 1f / normalFadeTime;
        fadeTempColor.a = 1f;
        fadeBackground.color = fadeTempColor;
    }
    public void fadeOut()
    {
        fading = GC.FADING_OUT;
        fadeBackground.enabled = true;
        fadeSpeed = 1f / normalFadeTime;
        fadeTempColor.a = 0f;
        fadeBackground.color = fadeTempColor;
    }
    public void stopFade()
    {
        if (fading == GC.FADING_IN)
            hideFade();
        
        fading = GC.NOT_FADING;
    }
    public void hideFade()
    {
        fadeBackground.enabled = false;
    }

    public void startHurt()
    {
        if (hurtSprite.enabled)
            CancelInvoke("stopHurt");
        
        hurtSprite.enabled = true;
        Invoke("stopHurt", hurtTime);
    }
    private void stopHurt()
    {
        hurtSprite.enabled = false;
    }

    public void showDeathScreen()
    {
        deathText.enabled = true;
        deathSprite.enabled = true;
    }
    public void hideDeathScreen()
    {
        deathText.enabled = false;
        deathSprite.enabled = false;
    }

    public void showBlackScreen()
    {
        deathSprite.enabled = true;
    }
    public void hideBlackScreen()
    {
        deathSprite.enabled = false;
    }

    public void hideGUI()
    {
        healthSprite.enabled = false;
        healthText.enabled = false;
        ammoText.enabled = false;
        shotgunSprite.enabled = false;
        crosshairSprite.enabled = false;
        pistolSprite.enabled = false;
        rifleSprite.enabled = false;
        ammoSprite.enabled = false;
    }

    public void showGUI()
    {
        healthSprite.enabled = true;
        healthText.enabled = true;
        ammoText.enabled = true;
        shotgunSprite.enabled = true;
        crosshairSprite.enabled = true;
        pistolSprite.enabled = true;
        rifleSprite.enabled = true;
        ammoSprite.enabled = true;
    }

    public void menuEnter()
    {
        if (inputLocked)
            return;
        
        if (ignoreFirstMenuSound)
        {
            ignoreFirstMenuSound = false;
            return;
        }

        gameMenuTexts[3].text = "Quit to Menu";
        confirmMenuQuit = false;

        /*
        if (false && audioSource.isPlaying)
        {
            maxAudios--;
            if (maxAudios != 0)
                return;
            else
                maxAudios = 20;
        }
        */

        audioSource.clip = Assets.instance.menuMoveSound;
        audioSource.volume = Assets.instance.menuMoveSoundVolume * GC.SOUND_VOLUME;
        audioSource.Play();
    }
    public void menuClick()
    {
        if (inputLocked)
            return;
        
        audioSource.clip = Assets.instance.menuSelectSound;
        audioSource.volume = Assets.instance.menuSelectSoundVolume * GC.SOUND_VOLUME;
        audioSource.Play();
    }
    public void setIndex(int i)
    {
        if (inputLocked)
            return;

        inputIndex = i;
    }

    private void darkenAll()
    {
        foreach (Text t in gameMenuTexts)
        {
            t.color = normalColor;
        }
        foreach (Text t in mainMenuTexts)
        {
            t.color = normalColor;
        }
        foreach (Text t in optionsTexts)
        {
            t.color = normalColor;
        }
    }
    public void inputUp()
    {
        if (inputLocked)
            return;
        
        inputIndex--;
        confirmMenuQuit = false;
        gameMenuTexts[3].text = "Quit to Menu";
        darkenAll();
        menuEnter();

        switch (GC.STATE)
        {
            case GC.GAMEMENU:
            {
                if (inputIndex < 0)
                {
                    inputIndex = gameMenuTexts.Length - 1;
                }
                gameMenuTexts[inputIndex].color = hoverColor;
                break;
            }
            case GC.MAINMENU:
            {
                if (inputIndex < 0)
                {
                    inputIndex = mainMenuTexts.Length - 1;
                }
                mainMenuTexts[inputIndex].color = hoverColor;
                break;
                }
            case GC.OPTIONSGAME: case GC.OPTIONSMENU:
            {
                if (inputIndex < 0)
                {
                    inputIndex = optionsTexts.Length - 1;
                }
                optionsTexts[inputIndex].color = hoverColor;
                break;
            }
        }

    }
    public void inputDown()
    {
        if (inputLocked)
            return;
        
        inputIndex++;
        confirmMenuQuit = false;
        gameMenuTexts[3].text = "Quit to Menu";
        darkenAll();
        menuEnter();

        switch (GC.STATE)
        {
            case GC.GAMEMENU:
            {
                if (inputIndex > gameMenuTexts.Length - 1)
                {
                    inputIndex = 0;
                }
                gameMenuTexts[inputIndex].color = hoverColor;
                break;
            }
            case GC.MAINMENU:
            {
                if (inputIndex > mainMenuTexts.Length - 1)
                {
                    inputIndex = 0;
                }
                mainMenuTexts[inputIndex].color = hoverColor;
                break;
                }
            case GC.OPTIONSGAME: case GC.OPTIONSMENU:
            {
                if (inputIndex > optionsTexts.Length - 1)
                {
                    inputIndex = 0;
                }
                optionsTexts[inputIndex].color = hoverColor;
                break;
            }
        }
    }
    public void inputEnter()
    {
        if (inputLocked)
            return;
        
        menuClick();
        darkenAll();

        switch (GC.STATE)
        {
            case GC.GAMEMENU:
                {
                    switch (inputIndex)
                    {
                        case 0: gameMenuOnResumeGame(); break;
                        case 1: gameMenuOnOptions(); break;
                        case 2: gameMenuOnRestartAtCheckpoint(); break;
                        case 3: gameMenuOnQuitToMenu(); break;
                    }
                    break;
                }
            case GC.MAINMENU:
                {
                    switch (inputIndex)
                    {
                        case 0: mainMenuOnNewGame(); break;
                        case 1: mainMenuOnOptions(); break;
                        case 2: mainMenuOnQuit(); break;
                    }
                    break;
                }
            case GC.OPTIONSMENU:
                {
                    switch (inputIndex)
                    {
                        case 0: optionsMenuOnBack(); break;
                    }
                    break;
                }
            case GC.OPTIONSGAME:
                {
                    switch (inputIndex)
                    {
                        case 0: optionsMenuOnBack(); break;
                    }
                    break;
                }
        }
    }
    public void inputHover(int i)
    {
        if (inputLocked)
            return;
        
        inputIndex = i;
        confirmMenuQuit = false;
        gameMenuTexts[3].text = "Quit to Menu";
        darkenAll();
        menuEnter();

        switch (GC.STATE)
        {
            case GC.GAMEMENU:
                {
                    gameMenuTexts[i].color = hoverColor;
                    break;
                }
            case GC.MAINMENU:
                {
                    mainMenuTexts[i].color = hoverColor;
                    break;
                }
            case GC.OPTIONSGAME: case GC.OPTIONSMENU:
                {
                    optionsTexts[i].color = hoverColor;
                    break;
                }
        }
    }
    public void optionsBackExitHover()
    {
        if (inputLocked)
            return;
        
        optionsTexts[0].color = normalColor;
        confirmMenuQuit = false;
        gameMenuTexts[3].text = "Quit to Menu";
    }
    public void lockInput()
    {
        inputLocked = true;
    }
    public void unlockInput()
    {
        inputLocked = false;
    }

    public void openMainMenu()
    {
        gameMenu.SetActive(false);
        mainMenu.SetActive(true);
        optionsMenu.SetActive(false);
        ignoreFirstMenuSound = true;
        mainMenuTexts[0].color = hoverColor;
        inputIndex = 0;
    }
    public void closeMainMenu()
    {
        mainMenu.SetActive(false);
    }

    public void mainMenuOnNewGame()
    {
        if (inputLocked)
            return;
        
        gameManager.onNewGame();
        inputIndex = 0;
    }
    public void mainMenuOnOptions()
    {
        if (inputLocked)
            return;
        
        gameManager.onOptions();
        optionsMenu.SetActive(true);
        optionsTexts[0].color = hoverColor;
        inputIndex = 0;
    }
    public void mainMenuOnQuit()
    {
        if (inputLocked)
            return;
        
        gameManager.onQuit();
    }

    public bool openGameMenu()
    {
        if (inputLocked)
            return false;
        
        if (GC.PAUSED)
            return true;
        
        gameMenu.SetActive(true);
        mainMenu.SetActive(false);
        optionsMenu.SetActive(false);
        darkenAll();
        gameMenuTexts[0].color = hoverColor;
        inputIndex = 0;

        return true;
    }
    public void closeGameMenu()
    {
        if (inputLocked)
            return;
        
        gameMenu.SetActive(false);
    }

    public void gameMenuOnResumeGame()
    {
        if (inputLocked)
            return;
        
        gameManager.onCloseGameMenu();
    }
    public void gameMenuOnOptions()
    {
        if (inputLocked)
            return;
        
        gameManager.onOptions();
        inputIndex = 0;
        optionsMenu.SetActive(true);
        optionsTexts[0].color = hoverColor;
    }
    public void gameMenuOnRestartAtCheckpoint()
    {
        if (inputLocked)
            return;
        
        gameManager.onRestartAtCheckpoint();
    }
    public void gameMenuOnQuitToMenu()
    {
        if (inputLocked)
            return;
        
        if (confirmMenuQuit == false)
        {
            confirmMenuQuit = true;
            gameMenuTexts[3].color = hoverColor;
            gameMenuTexts[3].text = "Are you sure?";
            inputIndex = 3;
        }
        else
        {
            gameManager.onQuitToMenu();
            inputIndex = 0;
            mainMenuTexts[0].color = hoverColor;
        }
    }

    public void optionsMenuOnBack()
    {
        gameManager.onOptionsBack();
        mainMenuTexts[0].color = hoverColor;
        optionsMenu.SetActive(false);
        inputIndex = 0;
        gameMenuTexts[0].color = hoverColor;
    }
    public void optionsOnAudioEnable(bool disable)
    {
        gameManager.onAudioEnable(disable);
    }
    public void optionsOnMasterVolume(float volume)
    {
        gameManager.onMasterVolume(volume);
    }
    public void optionsOnSoundVolume(float volume)
    {
        gameManager.onSoundVolume(volume);
    }
    public void optionsOnMusicVolume(float volume)
    {
        gameManager.onMusicVolume(volume);
    }
    public void optionsOnFov(float fov)
    {
        gameManager.onFov((int)fov);
    }
    public void optionsOnNextResolution()
    {
        resolutionIndex++;

        if (resolutionIndex >= Screen.resolutions.Length)
            resolutionIndex = 0;

        resolutionText.text = Screen.resolutions[resolutionIndex].width + "x" + Screen.resolutions[resolutionIndex].height;
    }
    public void optionsOnLastResolution()
    {
        resolutionIndex--;

        if (resolutionIndex <= 0)
            resolutionIndex = Screen.resolutions.Length - 1;

        resolutionText.text = Screen.resolutions[resolutionIndex].width + "x" + Screen.resolutions[resolutionIndex].height;
    }
    public void optionsOnApply()
    {
        Resolution r = Screen.resolutions[resolutionIndex];
        gameManager.onResolutionChange(r.width, r.height);

        scaleBackgroundsToScreen();
    }
    //from gamemanager
    public void setResolutionText(string text)
    {
        resolutionText.text = text;
    }
    public void optionsOnWindowed(bool windowed)
    {
        gameManager.onWindowed(windowed);
        
        
        scaleBackgroundsToScreen();
    }
}