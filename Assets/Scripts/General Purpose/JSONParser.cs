//Author: Efe Ayan
using UnityEngine;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine.TextCore.Text;
using Newtonsoft.Json;
using UnityTextAsset = UnityEngine.TextAsset;

public class JSONParser
{
    public static JToken LoadAsJToken(string resourceDataName)
    {
        UnityTextAsset jsonText = Resources.Load<UnityTextAsset>(resourceDataName);
        if (jsonText == null)
        {
            Debug.LogError($"JSONParser: Failed to load JSON file at path: {resourceDataName}");
            return null;
        }

        try
        {
            return JToken.Parse(jsonText.text);
        }
        catch
        {
            Debug.LogError($"JSONParser: Failed to parse JSON file at path: {resourceDataName}");
            return null;
        }
    }

    public static List<T> LoadAsList<T>(string resourceDataName)
    {
        UnityTextAsset jsonText = Resources.Load<UnityTextAsset>(resourceDataName);
        if (jsonText == null)
        {
            Debug.LogError($"JSONParser: Failed to load JSON file at path: {resourceDataName}");
            return null;
        }

        try
        {
            return JsonConvert.DeserializeObject<List<T>>(jsonText.text);
        }
        catch
        {
            Debug.LogError($"JSONParser: Failed to deserialize JSON into list at path: {resourceDataName}");
            return null;
        }
    }

    public static Dictionary<KeyType, T> LoadAsDictionary<KeyType, T>(string resourceDataName)
    {
        UnityTextAsset jsonText = Resources.Load<UnityTextAsset>(resourceDataName);
        if (jsonText == null)
        {
            Debug.LogError($"JSONParser: Failed to load JSON file at path: {resourceDataName}");
            return null;
        }

        return JsonConvert.DeserializeObject<Dictionary<KeyType, T>>(jsonText.text);
        
    }
}

   