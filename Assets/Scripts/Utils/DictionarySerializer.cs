using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DictionarySerializer 
{
    [SerializeField]
    string[] keys;
    [SerializeField]
    string[] values;

    public void parse(Dictionary<string, string> dict)
    {
        keys = new string[dict.Count];
        values = new string[dict.Count];

        int i = 0;
        foreach(KeyValuePair<string, string> kvp in dict)
        {
            values[i] = kvp.Value;
            keys[i] = kvp.Key;
            i++;
        }




    }


    public Dictionary<string, string> toDictionary()
    {
        Dictionary<string, string> dict = new Dictionary<string, string>();
        for(int i = 0; i < keys.Length; i++)
        {
            dict.Add(keys[i], values[i]);
        }

        return dict;
    }
}
