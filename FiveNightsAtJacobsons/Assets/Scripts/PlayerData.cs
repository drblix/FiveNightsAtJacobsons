using UnityEngine;
using System.IO;

public static class PlayerData
{
    public static int night { get; private set; } = 1;
    public static int stars { get; private set; } = 0;
    public static bool unlockedCustom { get; private set; } = false;

    private static readonly string dataPath = Application.streamingAssetsPath + "/Player_Data/Data.json";

    public static SaveData GetPlayerData() 
    {   
        if (File.Exists(dataPath))
        {
            // Fetching data from json file and returning it
            SaveData data = JsonUtility.FromJson<SaveData>(File.ReadAllText(dataPath));
            night = data.night;
            stars = data.stars;
            unlockedCustom = data.unlockedCustom;
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
        string json = JsonUtility.ToJson(new SaveData(night, stars, unlockedCustom));

        // Creating directory for data
        Directory.CreateDirectory(Application.streamingAssetsPath + "/Player_Data/");

        // Writing to file
        File.WriteAllText(dataPath, json);
    }

    public static void WipeData()
    {
        // Resets all data then creates a file for it
        night = 1;
        stars = 0;
        unlockedCustom = false;
        CreateSaveData();
    }

    public static void SetNight(int n)
    {
        night = Mathf.Clamp(n, 1, 6);
        CreateSaveData();
    }

    public static void SetStars(int s)
    {
        stars = Mathf.Clamp(s, 0, 3);
        CreateSaveData();
    }

    public static void SetCustom(bool c)
    {
        unlockedCustom = c;
        CreateSaveData();
    }
}

// Struct for holding data in JSON format since static class fields aren't accepted in Unity's JSON utility
// Kind of a work-around(?)
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
