using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EventManager))]
[RequireComponent(typeof(PrefsManager))]
[RequireComponent(typeof(Logger))]
[RequireComponent(typeof(Commander))]
[RequireComponent(typeof(TimerManager))]
public class BasicTools
{

    /// <summary>
    /// 此函数的v的z的含义是希望对应的游戏世界平面的z值
    /// </summary>
    /// <param name="v"></param>
    /// <returns></returns>
    public static Vector3 ScreenToWorld(Vector3 v)
    {
        v.z = Camera.main.WorldToScreenPoint(v).z;
        return Camera.main.ScreenToWorldPoint(v);
    }
    public static Vector3 NormalizedToScreen(Vector3 v)
    {
        v.x *= (float)Screen.width;
        v.y *= (float)Screen.height;
        return v;
    }
    public static Vector3 NormalizedToWorld(Vector3 v)
    {
        v = NormalizedToScreen(v);
        v = ScreenToWorld(v);
        return v;
    }

    public static void Assert(bool b)
    {
        if (!b) Debug.LogError("error happens");
    }

    /// <summary>
    /// 格式必须是(1,2,3)
    /// </summary>
    public static Vector3 ConvertToVector3(string v)
    {
        v = v.Substring(1, v.Length - 2);
        var t = v.Split(',');
        Assert(t.Length == 3);

        var x = (float)System.Convert.ToDouble(t[0]);
        var y = (float)System.Convert.ToDouble(t[1]);
        var z = (float)System.Convert.ToDouble(t[2]);
        return new Vector3(x, y, z);
    }

    /// <summary>
    /// 
    /// C#的模是考虑负数的模如 -10 % 3 = -1
    /// </summary>
    /// <param name="a"></param>
    public static float NormalizeAngleBetween_n180top180(float a)
    {
        if (a <= 180 && a >= -180) return a;
        else return a >= 0 ? (a + 180) % 360 - 180 : (a - 180) % 360 + 180;
    }

    


}

public class GameManager : Singleton<GameManager> {
    public GameObject Player = null;
    public GameObject Ground = null;
    public GameObject[] GOs = new GameObject[10];

    public override void Start()
    {
        base.Start();

        Player = GameObject.Find("Player");
        Ground = GameObject.Find("Ground");
    }

    // Update is called once per frame
    void Update () {
		
	}
}
