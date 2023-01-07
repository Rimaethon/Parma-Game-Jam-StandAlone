using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class InputVibrationController : MonoBehaviour
{
    public int DeviceIndex = 0;
    [Range(0, 1)] public float Power = 1;
    public InputHubController Hub;

    public void Vibrate(float lowFrequency, float highFrequency)
    {
        switch (Hub.inputMode)
        {
            #if ENABLE_INPUT_SYSTEM
            case InputHubController.Mode.Gamepad:
                Gamepad.all[DeviceIndex].SetMotorSpeeds(lowFrequency * Power, highFrequency * Power);
                break;
            #endif
            case InputHubController.Mode.Touchscreen:
                if ((lowFrequency > 0 || highFrequency > 0) && Power > 0) AndroidVibration.Vibrate((int)(1000*Time.deltaTime)); //Handheld.Vibrate();
                break;
        }
    }

    public static class AndroidVibration
    {

#if UNITY_ANDROID && !UNITY_EDITOR
    public static AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
    public static AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
    public static AndroidJavaObject vibrator = currentActivity.Call<AndroidJavaObject>("getSystemService", "vibrator");
#else
        public static AndroidJavaClass unityPlayer;
        public static AndroidJavaObject currentActivity;
        public static AndroidJavaObject vibrator;
#endif

        public static void Vibrate()
        {
            if (isAndroid())
                vibrator.Call("vibrate");
            else
                Handheld.Vibrate();
        }


        public static void Vibrate(long milliseconds)
        {
            if (isAndroid())
                vibrator.Call("vibrate", milliseconds);
            else
                Handheld.Vibrate();
        }

        public static void Vibrate(long[] pattern, int repeat)
        {
            if (isAndroid())
                vibrator.Call("vibrate", pattern, repeat);
            else
                Handheld.Vibrate();
        }

        public static bool HasVibrator()
        {
            return isAndroid();
        }

        public static void Cancel()
        {
            if (isAndroid())
                vibrator.Call("cancel");
        }

        private static bool isAndroid()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
	return true;
#else
            return false;
#endif
        }
    }
}
