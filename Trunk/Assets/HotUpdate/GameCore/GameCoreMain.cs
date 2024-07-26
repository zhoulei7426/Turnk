using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniFramework;
using UniFramework.Singleton;
using UniFramework.Window;

public class GameCoreMain
{
    public static void Run()
    {
        Debug.Log("GameCoreMain.Run()");

        InitSingleton();
        
    }


    static void InitSingleton()
    {
        UniSingleton.CreateSingleton<UIMgr>();
    }
}
