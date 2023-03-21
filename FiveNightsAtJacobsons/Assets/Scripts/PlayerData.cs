using UnityEngine;
using System.IO;

/// <summary>
/// Collection of various functions for obtaining and saving the player's data
/// </summary>
public static class PlayerData
{   
    public static int Night { get; private set; } = 1;
    public static int Stars { get; private set; } = 0;
    public static bool UnlockedCustom { get; private set; } = false;

    private static readonly string dataPath = Application.streamingAssetsPath + "/Player_Data/Data.json";

    public static SaveData GetPlayerData() 
    {   
        if (File.Exists(dataPath))
        {
            // Fetching data from json file and returning it
            SaveData data = JsonUtility.FromJson<SaveData>(File.ReadAllText(dataPath));
            Night = data.night;
            Stars = data.stars;
            UnlockedCustom = data.unlockedCustom;
            return data;
        }
        else
        {
            WipeData();

            SaveData data = JsonUtility.FromJson<SaveData>(File.ReadAllText(dataPath));
            return data;
        }
    }

    private static void CreateSaveData() 
    {
        // Creating json file w/ public fields
        string json = JsonUtility.ToJson(new SaveData(Night, Stars, UnlockedCustom));

        // Creating directory for data
        Directory.CreateDirectory(Application.streamingAssetsPath + "/Player_Data/");

        // Writing to file
        File.WriteAllText(dataPath, json);
    }

    public static void WipeData()
    {
        // Resets all data then creates a file for it
        Night = 1;
        Stars = 0;
        UnlockedCustom = false;
        CreateSaveData();
    }

    public static void SetNight(int n)
    {
        Night = Mathf.Clamp(n, 1, 6);
        CreateSaveData();
    }

    public static void SetStars(int s)
    {
        Stars = Mathf.Clamp(s, 0, 3);
        CreateSaveData();
    }

    public static void SetCustom(bool c)
    {
        UnlockedCustom = c;
        CreateSaveData();
    }
    
}


/// <summary>
/// Workaround struct for holding data that is then transformed into a JSON file. A type of work-around since Unity's JSON utility doesn't accept public static fields.
/// </summary>
public struct SaveData
{
    public int night;
    public int stars;
    public bool unlockedCustom;

    public SaveData(int n, int s, bool u)
    {
        night = n;
        stars = s;
        unlockedCustom = u;
    }
}
