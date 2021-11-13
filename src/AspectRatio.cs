using UnityEngine;
using System.Collections;

public class AspectRatio
{
    // object
    public int a;
    public int b;
    public float orthoSize;
    public int renderResolutionX;
    public int renderResolutionY;
    public int fov;

    // statics
    public static AspectRatio AR_16_9;
    public static AspectRatio AR_16_10;
    public static AspectRatio AR_4_3;
    public static AspectRatio AR_5_4;
    public static AspectRatio AR_3_2;
    public static AspectRatio AR_5_3;
    public static AspectRatio AR_4_5;

    public static void initializeAspectRatios()
    {
        // 16:9
        AspectRatio.AR_16_9 = new AspectRatio();
        AR_16_9.a = 16;
        AR_16_9.b = 9;
        AR_16_9.orthoSize = 4.5f;
        AR_16_9.renderResolutionX = 640;
        AR_16_9.renderResolutionY = 480;
        AR_16_9.fov = 80;

        // 16:10
        AspectRatio.AR_16_10 = new AspectRatio();
        AR_16_10.a = 16;
        AR_16_10.b = 10;
        AR_16_10.orthoSize = 4.97f;
        AR_16_10.renderResolutionX = 640;
        AR_16_10.renderResolutionY = 480;
        AR_16_10.fov = 80;

        // 4:3
        AspectRatio.AR_4_3 = new AspectRatio();
        AR_4_3.a = 4;
        AR_4_3.b = 3;
        AR_4_3.orthoSize = 1.494f;
        AR_4_3.renderResolutionX = 640;
        AR_4_3.renderResolutionY = 480;
        AR_4_3.fov = 75;

        // 5:4
        AspectRatio.AR_5_4 = new AspectRatio();
        AR_5_4.a = 5;
        AR_5_4.b = 4;
        AR_5_4.orthoSize = 1.99f;
        AR_5_4.renderResolutionX = 640;
        AR_5_4.renderResolutionY = 480;
        AR_5_4.fov = 75;

        // 3:2
        AspectRatio.AR_3_2 = new AspectRatio();
        AR_3_2.a = 3;
        AR_3_2.b = 2;
        AR_3_2.orthoSize = 0.99f;
        AR_3_2.renderResolutionX = 640;
        AR_3_2.renderResolutionY = 480;
        AR_3_2.fov = 75;

        // 5:3
        AspectRatio.AR_5_3 = new AspectRatio();
        AR_5_3.a = 5;
        AR_5_3.b = 3;
        AR_5_3.orthoSize = 1.5f;
        AR_5_3.renderResolutionX = 640;
        AR_5_3.renderResolutionY = 480;
        AR_5_3.fov = 75;

        // 4:5
        AspectRatio.AR_4_5 = new AspectRatio();
        AR_4_5.a = 4;
        AR_4_5.b = 5;
        AR_4_5.orthoSize = 2.5f;
        AR_4_5.renderResolutionX = 640;
        AR_4_5.renderResolutionY = 480;
        AR_4_5.fov = 70;
    }

    public static AspectRatio getAspectRatio(int width, int height)
    {
        float aspectRatio = width*1f / height * 10;
        int aspectRatioi = (int)Mathf.Ceil(aspectRatio);

        switch (aspectRatioi)
        {
            case 18: return AR_16_9;
            case 16: return AR_16_10;
            case 14: return AR_4_3;
        }

        return AR_16_9;
    }
    
    public static AspectRatio getAspectRatio(Resolution resolution)
    {
        return getAspectRatio(resolution.width, resolution.height);
    }

    public override string ToString()
    {
        return a + ":" + b;
    }
}