using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestObj : MonoBehaviour {

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
    void Start () {
        Debug.Log(C);//使用持久化数据
        C++;
        Debug.Log(Loc.ToString());
        if(Loc!=default(Vector3))
        {
            transform.position = Loc;
        }
        Debug.Log(myName);
        myName = myName + C.ToString();
        Debug.Log(ll);
        ll += 50.0f;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnApplicationQuit()
    {
        Loc = transform.position;//持久化数据准备入库
        PrefsManager.Instance.SaveIntoPerfs(this);
    }
}
