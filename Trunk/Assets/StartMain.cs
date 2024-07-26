using System.Collections;
using UniFramework.Event;
using UniFramework.Singleton;
using Unity.VisualScripting;
using UnityEngine;
using YooAsset;

public class StartMain : MonoBehaviour
{
    /// <summary>
    /// ��Դϵͳ����ģʽ
    /// </summary>
    public EPlayMode PlayMode = EPlayMode.EditorSimulateMode;

    void Awake()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        Debug.Log($"��Դϵͳ����ģʽ��{PlayMode}");
#if !UNITY_EDITOR
		if(PlayMode!= EPlayMode.HostPlayMode)
        {
			PlayMode = EPlayMode.HostPlayMode;
			Debug.Log($"��⵽�������,���л�����ģʽ����{PlayMode}");
		}
#endif
        Application.targetFrameRate = 60;
        Application.runInBackground = true;
    }


    void Start()
    {
        GameManager.Instance.Behaviour = this;
        // ��ʼ���¼�ϵͳ
        UniEvent.Initalize();

        // ��ʼ������ϵͳ
        UniSingleton.Initialize();

        // ��ʼ����Դϵͳ
        YooAssets.Initialize();
        YooAssets.SetOperationSystemMaxTimeSlice(30);


        UniSingleton.CreateSingleton<PatchOperation>();
        UniSingleton.GetSingleton<PatchOperation>().Run(PlayMode);

        // ����Ĭ�ϵ���Դ��
        var gamePackage = YooAssets.GetPackage("Res");
        YooAssets.SetDefaultPackage(gamePackage);

        StartCoroutine(LoadDllMgr.DownLoadAssets());
    }

}
