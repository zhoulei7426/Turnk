using UniFramework.Singleton;
using UnityEngine;

public class UIMgr : SingletonInstance<UIMgr>, ISingleton
{
    public void OnCreate(object createParam)
    {
        Debug.LogError("UIMgr.OnCreate()");
    }

    public void OnDestroy()
    {
        Debug.LogError("UIMgr.OnDestroy()");
    }

    public void OnUpdate()
    {
    }
}
