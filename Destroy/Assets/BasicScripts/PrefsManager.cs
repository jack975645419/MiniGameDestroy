using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

[AttributeUsage(AttributeTargets.Field)]
public class AutosaveAttribute : Attribute
{
    public AutosaveAttribute() { }
}

public class PrefsManager : Singleton<PrefsManager> {

    /* 想象这样一个场景，NPC实例在一次退出游戏时将属性和相关数据储存（包括位置），以便在下一次开启时加载位置，这就需要用到持久化的功能
     * 本工具实现：
     * 1. 根据键储存裸数据
     * 2. 为实例Object提供其专有的数据库
     */

    private BindingFlags AUTOSAVE_ATTRIBUTES_FLAG = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance;
    public string m_UserName = "";

    #region 持久化的基本接口
    public int GetInt(string key, int defaultValue = 0)
    {
        return PlayerPrefs.GetInt(key, defaultValue);
    }
    public float GetFloat(string key, float defaultValue = 0)
    {
        return PlayerPrefs.GetFloat(key, defaultValue);
    }
    public string GetString(string key, string defaultValue = "")
    {
        return PlayerPrefs.GetString(key, defaultValue);
    }
    public Vector3 GetVector3(string key, Vector3 defaultValue = default(Vector3))
    {
        var vAsStr = PlayerPrefs.GetString(key, "");
        if(vAsStr.Equals(""))
        {
            return defaultValue;
        }
        else
        {
            return BasicTools.ConvertToVector3(vAsStr);
        }
    }

    public void Set<T>(string key, T v)
    {
        SetWithType(key, default(T).GetType(), v);    
    }

    //尝试以T的类型去转换v，并储存到PlayerPrefs中
    public void SetWithType(string key, Type T, object v)
    {
        if (T == typeof(int))
        {
            PlayerPrefs.SetInt(key, (int)v);
        }
        else if (T == typeof(float))
        {
            PlayerPrefs.SetFloat(key, (float)v);
        }
        else if (T == typeof(string))
        {
            PlayerPrefs.SetString(key, (string)v);
        }
        else if (T == typeof(Vector3))
        {
            PlayerPrefs.SetString(key, ((Vector3)(v)).ToString());
        }
        else
        {
            Debug.LogError("暂不支持---没有这种类型的储存机制");
        }
    }

    public bool HasKey(string key)
    {
        return PlayerPrefs.HasKey(key);
    }
    #endregion


    #region 游戏持久化的接口
    /// <summary>
    /// 键构造器
    /// </summary>
    /// <param name="obj">游戏中的物体或物体的某脚本的实例</param>
    /// <param name="fieldName">obj的某属性的名称</param>
    /// <param name="userName">可选 用户名，用于做不同用户的区别</param>
    /// <returns></returns>
    private string MakeFieldUID(object obj, string fieldName, string userName = "")
    {
        return obj.GetHashCode() + "_" + fieldName + "_" + userName;
    }

    /// <summary>
    /// 对外接口，载入游戏时使用
    /// </summary>
    /// <param name="obj"></param>
    public void ReloadFromPerfs(object obj)
    {
        var type = obj.GetType();
        var fields = type.GetFields(AUTOSAVE_ATTRIBUTES_FLAG);
        for (int i = 0; i < fields.Length; i++)
        {
            var attrs = fields[i].GetCustomAttributes(true);
            for (int j = 0; j < attrs.Length; j++)
            {
                //如果该字段存在Autosave特性，则获得持久化字段值并应用
                if (attrs[j] is AutosaveAttribute)
                {
                    var fieldValue = GetFieldValueForAutoSaved(obj, fields[i]);
                    if (fieldValue != null)
                    {
                        fields[i].SetValue(obj, fieldValue);
                    }
                    continue;
                }
            }
        }
    }

    /// <summary>
    /// 对外接口，退出游戏时使用
    /// </summary>
    /// <param name="obj"></param>
    public void SaveIntoPerfs(object obj)
    {
        var type = obj.GetType();
        var fields = type.GetFields(AUTOSAVE_ATTRIBUTES_FLAG);
        for(int i = 0; i<fields.Length; i++)
        { 
            var attrs = fields[i].GetCustomAttributes(true);
            for (int j = 0; j < attrs.Length; j++)
            {
                //如果该字段存在Autosave特性，则储存该字段
                if (attrs[j] is AutosaveAttribute)
                {
                    SaveFieldValueForAutoSaved(obj, fields[i]);
                    continue;
                }
            }
        }
    }

    //储存某实例的某属性
    private void SaveFieldValueForAutoSaved(object obj, FieldInfo fi)
    {
        var type = fi.FieldType;
        SetWithType(MakeFieldUID(obj, fi.Name, m_UserName), type, fi.GetValue(obj));
    }

    //获取某实例的某属性
    private object GetFieldValueForAutoSaved(object obj, FieldInfo fi)
    {
        var type = fi.FieldType;
        if (!HasKey(MakeFieldUID( obj, fi.Name, m_UserName)))
        {
            return null;
        }
        if(type == typeof(int))
        {
            return GetInt(MakeFieldUID(obj, fi.Name, m_UserName));
        }
        else if(type == typeof(float))
        {
            return GetFloat(MakeFieldUID(obj, fi.Name, m_UserName));
        }
        else if(type == typeof(string))
        {
            return GetString(MakeFieldUID(obj, fi.Name, m_UserName));
        }
        else if(type == typeof(Vector3))
        {
            return GetVector3(MakeFieldUID(obj, fi.Name, m_UserName));
        }
        Debug.LogError("不支持该类型");
        return null;
    }
    #endregion

}


#region 物体持久化的使用示例
/*
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestObj : MonoBehaviour
{

    [Autosave()]
    public int C;
    [Autosave()]
    private Vector3 Loc = default(Vector3);
    [Autosave()]
    string myName = "";
    [Autosave()]
    float ll = 0;

    private void Awake()
    {
        PrefsManager.Instance.ReloadFromPerfs(this);//获取持久化数据

    }
    // Use this for initialization
    void Start()
    {
        Debug.Log(C);//使用持久化数据
        C++;
        Debug.Log(Loc.ToString());
        if (Loc != default(Vector3))
        {
            transform.position = Loc;
        }
        Debug.Log(myName);
        myName = myName + C.ToString();
        Debug.Log(ll);
        ll += 50.0f;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnApplicationQuit()
    {
        Loc = transform.position;//持久化数据准备入库
        PrefsManager.Instance.SaveIntoPerfs(this);
    }
}
*/
#endregion