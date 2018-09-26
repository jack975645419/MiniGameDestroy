using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Commander : Singleton<Commander> {

    /*// Use this for initialization
	void Start () {
		
	}*/

    // Update is called once per frame
    void Update()
    {

    }

    public void Exec(string s)
    {
        s = s.ToLower();
        Debug.Log("执行命令：" + s);
        string[] cmd = s.Split(' ');
        switch(cmd[0])
        {
            case "print":
                {
                    if (cmd.Length > 1)
                    {
                        Debug.Log(cmd[1]);
                    }
                    break;
                }
            case "checkpref":
                {
                    Vector3 a = PrefsManager.Instance.GetVector3("MyLocTes");
                    PrefsManager.Instance.Set<Vector3>("MyLocTes", a + new Vector3(1, 1, 1));
                    Debug.Log(a.ToString());
                    break;
                }
        }
    }
}
