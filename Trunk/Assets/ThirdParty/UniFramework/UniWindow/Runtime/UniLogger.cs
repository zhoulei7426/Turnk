﻿using System.Diagnostics;

namespace UniFramework.Window
{
    internal static class UniLogger
    {
        [Conditional("DEBUG")]
        public static void Log(string info)
        {
            UnityEngine.Debug.Log(info);
        }
        public static void Warning(string info)
        {
            UnityEngine.Debug.LogWarning(info);
        }
        public static void Error(string info)
        {
            UnityEngine.Debug.LogError(info);
        }
    }
}