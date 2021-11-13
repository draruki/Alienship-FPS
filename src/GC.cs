using UnityEngine;

public class GC
{
    //states
    public static int STATE;
    public const int MAINMENU = 1;
    public const int INGAME = 2;
    public const int GAMEMENU = 3;
    public const int OPTIONSGAME = 4;
    public const int OPTIONSMENU = 5;

    //scenes
    public static int BOOT_SCENE = 0;
    public static int SPLASH_SCENE = 1;
    public static int GAME_SCENE = 2;

    //volumes
    public static float MUSIC_VOLUME;
    public static float SOUND_VOLUME;
    public static bool PAUSED = false;
    public static Camera gameCamera;

    // debug variables
    public static bool INFINITE_HP = false;
    public static bool RELEASE = false;
    public static bool INFINITE_AMMO = false;
    public static bool SHOTGUN = false;
    public static bool ANIME;
    public static bool RIFLE = false;
    public static bool SPLASH = false;

    //fade
    public static int NOT_FADING = 0;
    public static int FADING_IN = 1;
    public static int FADING_OUT = 2;
}