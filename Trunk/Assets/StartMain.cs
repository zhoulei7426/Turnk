using System.Collections;
using UniFramework.Event;
using UniFramework.Singleton;
using Unity.VisualScripting;
using UnityEngine;
using YooAsset;

public class StartMain : MonoBehaviour
{
    /// <summary>
    /// 资源系统运行模式
    /// </summary>
    public EPlayMode PlayMode = EPlayMode.EditorSimulateMode;

    void Awake()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        Debug.Log($"资源系统运行模式：{PlayMode}");
#if !UNITY_EDITOR
		if(PlayMode!= EPlayMode.HostPlayMode)
        {
			PlayMode = EPlayMode.HostPlayMode;
			Debug.Log($"检测到真机运行,已切换运行模式至：{PlayMode}");
		}
#endif
        Application.targetFrameRate = 60;
        Application.runInBackground = true;
    }


    void Start()
    {
        GameManager.Instance.Behaviour = this;
        // 初始化事件系统
        UniEvent.Initalize();

        // 初始化单例系统
        UniSingleton.Initialize();

        // 初始化资源系统
        YooAssets.Initialize();
        YooAssets.SetOperationSystemMaxTimeSlice(30);


        UniSingleton.CreateSingleton<PatchOperation>();
        UniSingleton.GetSingleton<PatchOperation>().Run(PlayMode);

        // 设置默认的资源包
        var gamePackage = YooAssets.GetPackage("Res");
        YooAssets.SetDefaultPackage(gamePackage);

        StartCoroutine(LoadDllMgr.DownLoadAssets());
    }

}
