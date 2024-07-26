using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniFramework.Machine;
using UniFramework.Event;
using YooAsset;
using UniFramework.Singleton;

public class PatchOperation : SingletonInstance<PatchOperation>, ISingleton
{
    private EventGroup _eventGroup = new EventGroup();
    private StateMachine _machine;

    public void OnCreate(object createParam)
    {
    }

    void ISingleton.OnUpdate()
    {
        if (_machine != null)
            _machine.Update();
    }

    public void OnDestroy()
    {
        _eventGroup.RemoveAllListener();
    }


    public void Run(EPlayMode playMode)
    {
        // 注册监听事件
        _eventGroup.AddListener<UserEventDefine.UserTryInitialize>(OnHandleEventMessage);
        _eventGroup.AddListener<UserEventDefine.UserBeginDownloadWebFiles>(OnHandleEventMessage);
        _eventGroup.AddListener<UserEventDefine.UserTryUpdatePackageVersion>(OnHandleEventMessage);
        _eventGroup.AddListener<UserEventDefine.UserTryUpdatePatchManifest>(OnHandleEventMessage);
        _eventGroup.AddListener<UserEventDefine.UserTryDownloadWebFiles>(OnHandleEventMessage);

        // 创建状态机
        _machine = new StateMachine(this);
        _machine.AddNode<FsmInitializePackage>();
        _machine.AddNode<FsmUpdatePackageVersion>();
        _machine.AddNode<FsmUpdatePackageManifest>();
        _machine.AddNode<FsmCreatePackageDownloader>();
        _machine.AddNode<FsmDownloadPackageFiles>();
        _machine.AddNode<FsmDownloadPackageOver>();
        _machine.AddNode<FsmClearPackageCache>();
        _machine.AddNode<FsmUpdaterDone>();

    }

    /// <summary>
    /// 接收事件
    /// </summary>
    private void OnHandleEventMessage(IEventMessage message)
    {
        if (message is UserEventDefine.UserTryInitialize)
        {
            _machine.ChangeState<FsmInitializePackage>();
        }
        else if (message is UserEventDefine.UserBeginDownloadWebFiles)
        {
            _machine.ChangeState<FsmDownloadPackageFiles>();
        }
        else if (message is UserEventDefine.UserTryUpdatePackageVersion)
        {
            _machine.ChangeState<FsmUpdatePackageVersion>();
        }
        else if (message is UserEventDefine.UserTryUpdatePatchManifest)
        {
            _machine.ChangeState<FsmUpdatePackageManifest>();
        }
        else if (message is UserEventDefine.UserTryDownloadWebFiles)
        {
            _machine.ChangeState<FsmCreatePackageDownloader>();
        }
        else
        {
            throw new System.NotImplementedException($"{message.GetType()}");
        }
    }
}