using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class Splash : MonoBehaviour 
{
    public RawImage psxOpeningImage;
    public RawImage studioGhibliImage;
    public RawImage darkloreImage;
    public Image fadeImage;
    public RawImage justMakeIMage;

    public float pauseTime;
    public float splashTime;
    public float fadeTime;
    public float initialWait;

    private float fadeSpeed;
    private int fadeState;
    private Color unityLogoColor;
    private Color tempColor;

	void Start () 
    {
        psxOpeningImage.enabled = false;
        studioGhibliImage.enabled = false;
        darkloreImage.enabled = false;
        justMakeIMage.enabled = false;
        fadeState = GC.NOT_FADING;
        unityLogoColor = new Color(34 / 255f, 44 / 255f, 55 / 255f, 1f);

        if (GC.SPLASH)
        {
            begin();
        }
        else
        {
            loadGamescene();
        }
	}

    private void begin()
    {
        fadeImage.color = tempColor;
        fadeIn(fadeTime / 2);
        Invoke("psxOpening", fadeTime + initialWait);
        tempColor = unityLogoColor;
    }

    private void next(string name, bool last)
    {
        Invoke("fadeOut", splashTime - fadeTime - pauseTime);
        fadeIn(fadeTime);
        if (last)
            Invoke(name, splashTime - pauseTime);
        else
            Invoke(name, splashTime);
    }

    private void psxOpening()
    {
        psxOpeningImage.enabled = true;
        next("justLikeMakeGame", false);
        tempColor = Color.black;
    }
    private void justLikeMakeGame()
    {
        psxOpeningImage.enabled = false;
        next("darklore", false);
        justMakeIMage.enabled = true;
    }
    private void darklore()
    {
        justMakeIMage.enabled = false;
        next("loadGamescene", true);
        darkloreImage.enabled = true;
    }
    private void loadGamescene()
    {
        SceneManager.LoadScene(GC.GAME_SCENE);
    }

    private void fadeIn(float time)
    {
        fadeState = GC.FADING_IN;
        fadeSpeed = 1f / time;
        fadeImage.color = tempColor;
        tempColor.a = 1f;
    }
    private void fadeOut()
    {
        fadeState = GC.FADING_OUT;
        fadeSpeed = 1f / fadeTime;
        fadeImage.color = tempColor;
        tempColor.a = 0f;
        tempColor = Color.black;
    }
    private void stopFade()
    {
        fadeState = GC.NOT_FADING;
    }
	
	void Update () 
    {
        if (fadeState == GC.FADING_IN)
        {
            tempColor.a -= Time.deltaTime * fadeSpeed;
            fadeImage.color = tempColor;
            if (fadeImage.color.a <= 0f)
            {
                stopFade();
            }
        }
        else if (fadeState == GC.FADING_OUT)
        {
            tempColor.a += Time.deltaTime * fadeSpeed;
            fadeImage.color = tempColor;
            if (fadeImage.color.a >= 1f)
            {
                stopFade();
            }
        }
	}
}