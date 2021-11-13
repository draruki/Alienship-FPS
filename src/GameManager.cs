using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // gamescene variables
    public Camera screenCamera;
    public Camera gameCamera;
    public GUIManager guiManager;
    public float deathTime = 2f;
    public Camera weaponCamera;
    public AudioSource musicPlayer;
    public PlayerEntity player;
    public Transform renderQuad;

    // music
    private float musicVolume = 0.5f;

    // player checkpoint
    private int checkpointSingleAmmo;
    private int checkpointAutoAmmo;
    private int checkpointSpreadAmmo;
    private int checkpointHealth;

    // weapons
    public bool playerHasShotgun = false;
    public bool playerHasRifle = false;

    // level
    public Level[] levelList;
    private Level currentLevel;
    public GameObject debugLevel;
    public GameObject debugLevel2;
    public GameObject debugLevel3;

    // main menu
    private float fadeOutTime = 2f;
    public float endGameFadeTime;

    // debug variables
    public bool infiniteHp;
    public bool infiniteAmmo;
    [Range(1, 3)] public int startLevel = 1;
    [Range(1, 7)] public int startCheckpoint = 1;
    public bool spawnAtCheckpoint;
    public bool startAtMainMenu;
    public bool drawText;
    public bool splash;
    public bool release;
    public bool anime;
    private static bool booted = false;

    void Awake () 
    {
        if (SceneManager.GetActiveScene().buildIndex != GC.BOOT_SCENE && !GameManager.booted)
        {
            SceneManager.LoadScene(GC.BOOT_SCENE);
            return;
        }

        if (GameManager.booted && getCurrentScene() == GC.GAME_SCENE)
        {
            GameScene();
            return;
        }

        if (GameManager.booted && getCurrentScene() == GC.SPLASH_SCENE)
        {
            SplashScene();
            return;
        }

        BootScene();
    }

    void BootScene()
    {
        GameManager.booted = true;
        Screen.fullScreen = true;
        Cursor.visible = false;
        AspectRatio.initializeAspectRatios();
        WeaponManager.initialize();

        if (release)
        {
            splash = true;
            drawText = false;
        }

        GC.SPLASH = splash;
        AudioListener.volume = 0.8f;
        GC.MUSIC_VOLUME = 0.8f;
        GC.SOUND_VOLUME = 0.8f;
        SceneManager.LoadScene(GC.SPLASH_SCENE);
    }

    void SplashScene()
    {
        
    }

    void GameScene()
    {
        if (release)
        {
            infiniteHp = false;
            infiniteAmmo = false;
            spawnAtCheckpoint = true;
            startLevel = 1;
            startCheckpoint = 1;
            playerHasRifle = false;
            playerHasShotgun = false;
            startAtMainMenu = true;
            drawText = false;
        }

        GC.INFINITE_HP = infiniteHp;
        GC.INFINITE_AMMO = infiniteAmmo;
        GC.SHOTGUN = playerHasShotgun;
        GC.RIFLE = playerHasRifle;
        GC.RELEASE = release;

        if (release)
            screenCamera.backgroundColor = Color.black;

        setAspectRatio(AspectRatio.getAspectRatio(Screen.currentResolution));
        GC.gameCamera = gameCamera;
        startLevel--;
        startCheckpoint--;

        if (debugLevel != null)
            Destroy(debugLevel);
        if (debugLevel2 != null)
            Destroy(debugLevel2);
        if (debugLevel3 != null)
            Destroy(debugLevel3);

        GC.ANIME = anime;

        if (startAtMainMenu)
        {
            player.gameObject.SetActive(false);
            GC.PAUSED = true;
            guiManager.openMainMenu();
            GC.STATE = GC.MAINMENU;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            musicPlayer.clip = Assets.instance.mainMenuMusic;
            musicPlayer.volume = Assets.instance.mainMenuMusicVolume * GC.MUSIC_VOLUME;
            musicVolume = Assets.instance.mainMenuMusicVolume;
            musicPlayer.loop = false;
            musicPlayer.Play();
            guiManager.fadeInTime(1.5f);
        }
        else
        {
            onNewGame();
        }
    }

    private void setAspectRatio(AspectRatio ar)
    {
        Vector3 newScale = new Vector3(ar.a, ar.b, 1);
        renderQuad.localScale = newScale;
        screenCamera.orthographicSize = ar.orthoSize;
        gameCamera.fieldOfView = ar.fov;
        weaponCamera.fieldOfView = ar.fov;
    }

    public void setLevel(int levelIndex, int checkpointIndex, bool setExpectedAmmo)
    {
        if (levelIndex >= levelList.Length)
            levelIndex = 0;

        if (currentLevel != null)
            Destroy(currentLevel.gameObject);

        currentLevel = Instantiate(levelList[levelIndex].gameObject).GetComponent<Level>();

        for (int i = 0; i < currentLevel.enemiesList.Count; ++i)
            currentLevel.enemiesList[i].kill();

        if (checkpointIndex >= currentLevel.checkpointsList.Count)
            checkpointIndex = 0;

        for (int i = 0; i < currentLevel.checkpointsList.Count; ++i)
        {
            currentLevel.checkpointsList[i].setGameManager(this);

            if (i <= checkpointIndex)
            {
                currentLevel.checkpointsList[i].setReached();
            }
            else
            {
                currentLevel.checkpointsList[i].spawn();
            }
        }

        currentLevel.nextLevelTrigger.setGameManager(this);

        if (spawnAtCheckpoint)
        {
            movePlayerToCheckpoint(currentLevel.checkpointsList[checkpointIndex]);
        }

        if (setExpectedAmmo)
        {
            player.singleAmmo = currentLevel.checkpointsList[checkpointIndex].expectedSingleAmmo;
            player.spreadAmmo = currentLevel.checkpointsList[checkpointIndex].expectedSpreadAmmo;
            player.autoAmmo = currentLevel.checkpointsList[checkpointIndex].expectedAutoAmmo;
            playerHasShotgun = currentLevel.checkpointsList[checkpointIndex].shotgun || GC.SHOTGUN || playerHasShotgun;
            playerHasRifle = currentLevel.checkpointsList[checkpointIndex].rifle || GC.RIFLE || playerHasRifle;
        }

        storePlayerAmmo();
        ParticleManager.clearDecals();

        if (currentLevel.endGameTrigger != null)
        {
            currentLevel.endGameTrigger.setGameManager(this);
        }
    }

    public void onPlayerDeath(bool youDied)
    {
        player.resetHealth();
        resetPlayerAmmo();

        for (int i = 0; i < currentLevel.enemiesList.Count; ++i)
        {
            currentLevel.enemiesList[i].kill();
        }

        Checkpoint latestCheckpoint = currentLevel.getStartCheckpoint();
        for (int i = 0; i < currentLevel.checkpointsList.Count; ++i)
        {
            currentLevel.checkpointsList[i].spawn();

            if (currentLevel.checkpointsList[i].playerReached == true)
            {
                latestCheckpoint = currentLevel.checkpointsList[i];
            }
        }

        movePlayerToCheckpoint(latestCheckpoint);
        latestCheckpoint.activateDebasers();
        pauseGameAndThenFadeIn(deathTime);
        if (youDied)
            guiManager.showDeathScreen();
    }

    private void pauseGameAndThenFadeIn(float pauseTime)
    {
        GC.PAUSED = true;
        guiManager.showBlackScreen();
        Invoke("resumeGame", pauseTime);
    }
    private void resumeGame()
    {
        guiManager.hideDeathScreen();
        GC.PAUSED = false;
        guiManager.fadeInTime(1.3f);
    }

    private void storePlayerAmmo()
    {
        checkpointSingleAmmo = player.singleAmmo;
        checkpointSpreadAmmo = player.spreadAmmo;
        checkpointAutoAmmo = player.autoAmmo;
        checkpointHealth = player.health;
    }
    private void resetPlayerAmmo()
    {
        player.singleAmmo = checkpointSingleAmmo;
        player.spreadAmmo = checkpointSpreadAmmo;
        player.autoAmmo = checkpointAutoAmmo;
    }

    public void onPlayerDamaged()
    {
        guiManager.startHurt();
    }

    public void onReachedCheckpoint(Checkpoint c)
    {
        storePlayerAmmo();
    }

    public void onNewWeapon(WeaponManager.WEAPON type)
    {
        if (type == WeaponManager.WEAPON.Shotgun)
        {
            playerHasShotgun = true;
        }
        else if (type == WeaponManager.WEAPON.Rifle)
        {
            playerHasRifle = true;
        }
    }

    public void onAnime()
    {
        GC.ANIME = true;
    }

    public void onChangedWeapon(Weapon weapon)
    {
        guiManager.setWeapon(weapon);
    }

    public void onLevelTrigger(int level)
    {
        level--;
        setLevel(level, 0, false);
        pauseGameAndThenFadeIn(1f);
    }

    public void onNewGame()
    {
        GC.PAUSED = true;
        guiManager.fadeOutTime(1f);
        guiManager.lockInput();
        Invoke("newGame", 1f + fadeOutTime);
    }
    private void newGame()
    {
        guiManager.unlockInput();
        player.gameObject.SetActive(true);
        musicPlayer.GetComponent<AudioListener>().enabled = false;
        GC.PAUSED = false;
        guiManager.closeMainMenu();
        GC.STATE = GC.INGAME;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        setLevel(startLevel, startCheckpoint, true);
        musicPlayer.clip = Assets.instance.gameMusic;
        musicPlayer.volume = Assets.instance.gameMusicVolume * GC.MUSIC_VOLUME;
        musicVolume = Assets.instance.gameMusicVolume;
        musicPlayer.loop = true;
        musicPlayer.Play();
        guiManager.fadeInTime(3f);
    }

    public void onQuit()
    {
        Application.Quit();
    }

    public void onAudioEnable(bool enable)
    {
        AudioListener.pause = enable;
    }
    public void onMasterVolume(float volume)
    {
        AudioListener.volume = volume;
    }
    public void onSoundVolume(float volume)
    {
        GC.SOUND_VOLUME = volume;
    }
    public void onMusicVolume(float volume)
    {
        GC.MUSIC_VOLUME = volume;
        musicPlayer.volume = musicVolume * volume;
    }
    public void onFov(int fov)
    {

    }
    public void onResolutionChange(int width, int height)
    {
        Screen.SetResolution(width, height, Screen.fullScreen, 0);
        setAspectRatio(AspectRatio.getAspectRatio(width, height));
    }
    public void onWindowed(bool windowed)
    {
        Screen.fullScreen = !windowed;
    }

    public void onOptions()
    {
        if (GC.STATE == GC.GAMEMENU)
        {
            GC.STATE = GC.OPTIONSGAME;
        }
        else if (GC.STATE == GC.MAINMENU)
        {
            GC.STATE = GC.OPTIONSMENU;
        }
    }
    public void onOptionsBack()
    {
        if (GC.STATE == GC.OPTIONSGAME)
        {
            GC.STATE = GC.GAMEMENU;
        }
        else if (GC.STATE == GC.OPTIONSMENU)
        {
            GC.STATE = GC.MAINMENU;
        }
    }

    public void onOpenGameMenu()
    {
        if (GC.STATE == GC.INGAME && !GC.PAUSED)
        {
            if (guiManager.openGameMenu() == false)
                return;

            GC.PAUSED = true;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            GC.STATE = GC.GAMEMENU;
        }
    }
    public void onCloseGameMenu()
    {
        if (GC.STATE == GC.GAMEMENU)
        {
            GC.PAUSED = false;
            guiManager.closeGameMenu();
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            GC.STATE = GC.INGAME;
        }
        else if (GC.STATE == GC.OPTIONSGAME || GC.STATE == GC.OPTIONSMENU)
        {
            guiManager.optionsMenuOnBack();
        }
    }
    public void onRestartAtCheckpoint()
    {
        onCloseGameMenu();
        onPlayerDeath(false);
    }
    public void onQuitToMenu()
    {
        GC.PAUSED = true;
        guiManager.fadeOutTime(1f);
        guiManager.lockInput();
        Invoke("quitToMenu", 1f + fadeOutTime);
    }
    private void quitToMenu()
    {
        guiManager.unlockInput();
        guiManager.showGUI();

        if (currentLevel != null)
            Destroy(currentLevel.gameObject);

        resetPlayerAmmo();
        player.resetHealth();
        playerHasShotgun = false;
        playerHasRifle = false;
        player.gameObject.SetActive(false);
        musicPlayer.GetComponent<AudioListener>().enabled = true;
        guiManager.closeGameMenu();
        guiManager.openMainMenu();
        GC.STATE = GC.MAINMENU;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        musicPlayer.clip = Assets.instance.mainMenuMusic;
        musicPlayer.volume = Assets.instance.mainMenuMusicVolume * GC.MUSIC_VOLUME;
        musicVolume = Assets.instance.mainMenuMusicVolume;
        musicPlayer.loop = false;
        musicPlayer.Play();
        guiManager.fadeInTime(1.5f);
    }

    public void onGameEnd()
    {
        onCloseGameMenu();
        guiManager.lockInput();
        guiManager.hideGUI();
        guiManager.fadeOutTime(endGameFadeTime);
        Invoke("quitToMenu", endGameFadeTime + 0.7f);
    }

    private void movePlayerToCheckpoint(Checkpoint checkpoint)
    {
        player.transform.position = new Vector3(checkpoint.transform.position.x, checkpoint.transform.position.y, checkpoint.transform.position.z);
        player.transform.rotation =  Quaternion.Euler(0, checkpoint.transform.rotation.eulerAngles.y, 0);
        gameCamera.transform.rotation = Quaternion.Euler(checkpoint.transform.rotation.eulerAngles);
    }

    private int getCurrentScene()
    {
        return SceneManager.GetActiveScene().buildIndex;
    }

    void Update()
    {
        if(Input.GetKeyUp(KeyCode.Tab) && !GC.RELEASE)
        {
            Cursor.visible = !Cursor.visible;
        }
        if (GC.STATE == GC.INGAME && GC.RELEASE)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    void OnGUI()
    {
        if (drawText)
        {
            float aspectRatio = Screen.currentResolution.width*1f / Screen.currentResolution.height * 10;
            aspectRatio = Mathf.Ceil(aspectRatio);
            GUI.Label(new Rect(10, 10, 200, 20), "resolution: " + Screen.currentResolution.ToString());
            GUI.Label(new Rect(10, 30, 200, 20), "aspect: " + Screen.currentResolution.width*1f / Screen.currentResolution.height);
            GUI.Label(new Rect(10, 50, 200, 20), "aspect ceiled: " + aspectRatio);
            GUI.Label(new Rect(10, 70, 200, 20), "ar: " + AspectRatio.getAspectRatio(Screen.currentResolution).ToString());
        }
    }
}