using System;
using Assets.TemplateLibrary.UnityDesignPatterns.Singleton;
using UnityEngine;

namespace Assets.TemplateLibrary.Helpers
{
    public class Vibration : NonMonoSingleton<Vibration>
    {
        public bool IsAvailable = false;

        public Vibration()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            using (AndroidJavaObject activity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").
                GetStatic<AndroidJavaObject>("currentActivity"))
            {
                var mWindowManager = activity.Call<AndroidJavaObject>("getSystemService", "sensor");
                IsAvailable = mWindowManager.Call<bool>("hasVibrator");
            }

            Debug.LogWarning("Is Vibro available (JavaClass): " + IsAvailable);
#elif UNITY_IOS && !UNITY_EDITOR

            if (UnityEngine.iOS.Device.generation.ToString().IndexOf("iPad") > -1 ||
                UnityEngine.iOS.Device.generation.ToString().IndexOf("iPod") > -1)
            {
                IsAvailable = false;
            }
            else
            {
                IsAvailable = true;
            }

            Debug.LogWarning("Is Vibro available (iOS generation parsing:) " + IsAvailable);

#else 
            IsAvailable = SystemInfo.supportsVibration;
#endif
            Debug.LogWarning("Is Vibro available (unity SystemInfo): " + SystemInfo.supportsVibration);
        }

    }
}