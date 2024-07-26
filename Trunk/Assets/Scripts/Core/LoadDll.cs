using HybridCLR;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;

public static class LoadDllMgr
{

    static List<string> strings = new List<string>()
    {
        "GameCore",
        "MainLobby",
    };


    public static Dictionary<string, Assembly> AssemblyMap = new Dictionary<string, Assembly>();

    private static Dictionary<string, byte[]> s_assetDatas = new Dictionary<string, byte[]>();

    public static byte[] ReadBytesFromStreamingAssets(string dllName)
    {
        return s_assetDatas[dllName];
    }

    public static IEnumerator DownLoadAssets()
    {
        var assets = new List<string>
        {
            "GameCore.dll.bytes",
            "MainLobby.dll.bytes",
        }.Concat(AOTMetaAssemblyFiles);

        foreach (var asset in assets)
        {
            string dllPath = GetBundlePath(asset);
            Debug.Log($"start download asset:{dllPath}");
            UnityWebRequest www = UnityWebRequest.Get(dllPath);
            yield return www.SendWebRequest();

#if UNITY_2020_1_OR_NEWER
            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
#else
            if (www.isHttpError || www.isNetworkError)
            {
                Debug.Log(www.error);
            }
#endif
            else
            {
                // Or retrieve results as binary data
                byte[] assetData = www.downloadHandler.data;
                Debug.Log($"dll:{asset}  size:{assetData.Length}");
                s_assetDatas[asset] = assetData;
            }
        }

        Init();
    }


    public static void Init()
    {
        LoadMetadataForAOTAssemblies();
        foreach (string s in strings) {
            // Editor�����£�HotUpdate.dll.bytes�Ѿ����Զ����أ�����Ҫ���أ��ظ����ط���������⡣
#if  !UNITY_EDITOR
        Assembly hotUpdateAss = Assembly.Load(ReadBytesFromStreamingAssets($"{s}.dll.bytes"));
#else
            // Editor��������أ�ֱ�Ӳ��һ��HotUpdate����
            Assembly hotUpdateAss = System.AppDomain.CurrentDomain.GetAssemblies().First(a => a.GetName().Name == s);
#endif
            Type type = hotUpdateAss.GetType($"{s}Main");
            type.GetMethod("Run").Invoke(null, null);
            AssemblyMap.Add(s, hotUpdateAss);
        }
    }


    public static Assembly GetAssembly(string name)
    {
        Assembly assembly = null;
        AssemblyMap.TryGetValue(name, out assembly);
        return assembly;
    }


    private static List<string> AOTMetaAssemblyFiles { get; } = new List<string>()
    {
        "mscorlib.dll.bytes",
        "System.dll.bytes",
        "System.Core.dll.bytes",
    };


    /// <summary>
    /// Ϊaot assembly����ԭʼmetadata�� ��������aot�����ȸ��¶��С�
    /// һ�����غ����AOT���ͺ�����Ӧnativeʵ�ֲ����ڣ����Զ��滻Ϊ����ģʽִ��
    /// </summary>
    private static void LoadMetadataForAOTAssemblies()
    {
        /// ע�⣬����Ԫ�����Ǹ�AOT dll����Ԫ���ݣ������Ǹ��ȸ���dll����Ԫ���ݡ�
        /// �ȸ���dll��ȱԪ���ݣ�����Ҫ���䣬�������LoadMetadataForAOTAssembly�᷵�ش���
        /// 
        HomologousImageMode mode = HomologousImageMode.SuperSet;
        foreach (var aotDllName in AOTMetaAssemblyFiles)
        {
            byte[] dllBytes = ReadBytesFromStreamingAssets(aotDllName);
            // ����assembly��Ӧ��dll�����Զ�Ϊ��hook��һ��aot���ͺ�����native���������ڣ��ý������汾����
            LoadImageErrorCode err = RuntimeApi.LoadMetadataForAOTAssembly(dllBytes, mode);
            Debug.Log($"LoadMetadataForAOTAssembly:{aotDllName}. mode:{mode} ret:{err}");
        }
    }


    private static string GetBundlePath(string aotDllName)
    {
        var path = $"{Application.streamingAssetsPath}/{aotDllName}";
#if UNITY_EDITOR
        return path;
#elif UNITY_ANDROID
         if (path.StartsWith("jar:file://"))
            return path;
        else
            return string.Format("jar:file://{0}", path);
#endif
    }
}
