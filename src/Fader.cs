using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Fader : MonoBehaviour 
{
    private int fadeState;
    public float fadeTime;
    public Image fadeImage;
    private float fadeSpeed;
    private Color tempColor;
    private Color unityLogoColor;

    void Start ()
    {
        fadeState = GC.NOT_FADING;
    }

    private void fadeIn(float time)
    {
        fadeImage.color = tempColor;
        tempColor.a = 1f;
        fadeState = GC.FADING_IN;
        fadeSpeed = 1f / time;
    }
    
    private void stopFade()
    {
        fadeState = GC.NOT_FADING;
    }

    private void fadeOut()
    {
        tempColor = Color.black;
        tempColor.a = 0f;
        fadeImage.color = tempColor;
        fadeState = GC.FADING_OUT;
        fadeSpeed = 1f / fadeTime;
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
