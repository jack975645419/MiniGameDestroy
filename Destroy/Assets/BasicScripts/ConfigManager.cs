using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;



public class ConfigManager : Singleton<ConfigManager> {


	public static string LoadTextFile(string filePath)
    {
        string text = string.Empty;
        TextAsset ta = Resources.Load<TextAsset>(filePath);
        if(ta!=null)
        {
            text = ta.text;
        }
        else
        {
            FileInfo fi = new FileInfo(filePath);
            if(fi.Exists)
            {
                StreamReader sr = null;
                try
                {
                    sr = fi.OpenText();
                    text = sr.ReadToEnd();
                    //另外一些好用的函数有sr.ReadBlock() sr.ReadLine()
                    sr.Close();
                    sr.Dispose();
                }
                catch(System.Exception e)
                {
                    if(sr!=null)
                    {
                        sr.Close();
                        sr.Dispose();
                    }
                }
            }
        }
        return text;
    }

}
