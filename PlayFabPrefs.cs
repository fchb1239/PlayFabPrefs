using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using PlayFab;
using PlayFab.ClientModels;

/*
MIT License

Copyright (c) 2023 fchb1239

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

public class PlayFabPrefs
{
    public static bool ready = false;
    private static Dictionary<string, string> saveData = new Dictionary<string, string>();

    public static bool CheckOverlap(string key, string value)
    {
        if (saveData.ContainsKey(key))
            return saveData[key] == value;
        else
            return false;
    }

    public static async Task Get()
    {
        Debug.Log("Getting saved data");

        var result = await PlayFabAsyncClientAPI.GetUserDataAsync(new GetUserDataRequest
        {
            Keys = null,
            PlayFabId = LoginManager.playFabId
        });

        if (result.IsError)
        {
            Debug.LogError("Failed to get user data");
            return;
        }

        foreach (KeyValuePair<string, UserDataRecord> kv in result.Result.Data)
        {
            string str = kv.Value.Value;

            //Debug.Log($"{kv.Key} : {str}");

            saveData[kv.Key] = str;
        }

        ready = true;
    }

    public static void DeleteAll()
    {
        Debug.LogError("You can't delete all user data for obvious reasons");
    }

    public static void DeleteKey(string key)
    {
        Debug.Log("Deleting key");

        PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest()
        {
            KeysToRemove = new List<string>() { key },
            Permission = UserDataPermission.Public
        }, msg => { Debug.Log("Deleted key"); }, error => { Debug.LogError("Failed to upload pdata"); });

        PlayerPrefs.DeleteKey(key);
    }

    public static float GetFloat(string key, float defaultValue)
    {
        if (!ready)
            return PlayerPrefs.GetFloat(key, defaultValue);

        if (saveData.ContainsKey(key))
        {
            if (float.TryParse(saveData[key], out float result))
                return result;
        }

        return defaultValue;
    }

    public static float GetFloat(string key)
    {
        if (!ready)
            return PlayerPrefs.GetFloat(key);

        if (saveData.ContainsKey(key))
        {
            if (float.TryParse(saveData[key], out float result))
                return result;
        }

        return 0;
    }

    public static int GetInt(string key, int defaultValue)
    {
        if (!ready)
            return PlayerPrefs.GetInt(key, defaultValue);

        if (saveData.ContainsKey(key))
        {
            if (int.TryParse(saveData[key], out int result))
                return result;
        }

        return defaultValue;
    }

    public static int GetInt(string key)
    {
        if (!ready)
            return PlayerPrefs.GetInt(key);

        if (saveData.ContainsKey(key))
        {
            if (int.TryParse(saveData[key], out int result))
                return result;
        }

        return 0;
    }

    public static string GetString(string key, string defaultValue)
    {
        if (!ready)
            return PlayerPrefs.GetString(key, defaultValue);

        if (saveData.ContainsKey(key))
            return saveData[key];

        return defaultValue;
    }

    public static string GetString(string key)
    {
        if (!ready)
            return PlayerPrefs.GetString(key);

        if (saveData.ContainsKey(key))
            return saveData[key];

        return "";
    }

    public static bool HasKey(string key)
    {
        if (!ready)
            return PlayerPrefs.HasKey(key);

        return saveData.ContainsKey(key);
    }

    public static void Save()
    {
        Debug.Log("Saving data");

        PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest()
        {
            Data = saveData,
            Permission = UserDataPermission.Public
        }, msg => { Debug.Log("Saved data"); }, error => { Debug.LogError("Failed to upload data"); });

        PlayerPrefs.Save();
    }

    public static void SetFloat(string key, float value)
    {
        if (!CheckOverlap(key, value.ToString()))
        {
            saveData[key] = value.ToString();

            Debug.Log("Setting float");

            PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest()
            {
                Data = new Dictionary<string, string>()
                {
                    { key, value.ToString() }
                },
                Permission = UserDataPermission.Public
            }, msg => { Debug.Log("Saved float"); }, error => { Debug.LogError("Failed to upload data"); });

            UnityEngine.PlayerPrefs.SetFloat(key, value);
        }
        else
        {
            Debug.Log("No reason to upload as data is the same");
        }
    }

    public static void SetInt(string key, int value)
    {
        if (!CheckOverlap(key, value.ToString()))
        {
            saveData[key] = value.ToString();

            Debug.Log("Setting int");

            PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest()
            {
                Data = new Dictionary<string, string>()
                {
                    { key, value.ToString() }
                },
                Permission = UserDataPermission.Public
            }, msg => { Debug.Log("Saved int"); }, error => { Debug.LogError("Failed to upload data"); });

            UnityEngine.PlayerPrefs.SetInt(key, value);
        }
        else
        {
            Debug.Log("No reason to upload as data is the same");
        }
    }

    public static void SetString(string key, string value)
    {
        if (!CheckOverlap(key, value))
        {
            saveData[key] = value;

            Debug.Log("Setting string");

            PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest()
            {
                Data = new Dictionary<string, string>()
                {
                    { key, value }
                },
                Permission = UserDataPermission.Public
            }, msg => { Debug.Log("Saved string"); }, error => { Debug.LogError("Failed to upload data"); });

            UnityEngine.PlayerPrefs.SetString(key, value);
        }
        else
        {
            Debug.Log("No reason to upload as data is the same");
        }
    }

    private static void Save(string key)
    {
        if (HasKey(key))
        {
            Debug.Log("Saving key");

            PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest()
            {
                Data = new Dictionary<string, string>()
                {
                    { key, saveData[key] }
                },
                Permission = UserDataPermission.Public
            }, msg => { Debug.Log("Saved key"); }, error => { Debug.LogError("Failed to upload data"); });

            UnityEngine.PlayerPrefs.Save();
        }
    }
}
