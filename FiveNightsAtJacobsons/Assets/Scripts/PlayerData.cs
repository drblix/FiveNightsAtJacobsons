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
    public static bool UnlockedSixth { get; private set; } = false;
    public static bool UnlockedChallenges { get; private set; } = false;
    public static bool[] CompletedChallenges { get; private set; } = new bool[5];

    private static readonly string dataPath = Application.streamingAssetsPath + "/Player_Data/Data.json";

    public static SaveData GetPlayerData()
    {
        if (File.Exists(dataPath))
        {
            // Decoding data, then converting from base 64 to a legible json
            try
            {
                string decodedJson = DecodeBase64(File.ReadAllText(dataPath));
                SaveData data = JsonUtility.FromJson<SaveData>(decodedJson);

                if (data.completedChallenges == null)
                {
                    Debug.LogError("ERR! Failed to fetch existing data. Wiping!");
                    WipeData();
                    string convertedJson = DecodeBase64(File.ReadAllText(dataPath));
                    SaveData data2 = JsonUtility.FromJson<SaveData>(convertedJson);
                    return data2;
                }

                Night = data.night;
                Stars = data.stars;
                UnlockedCustom = data.unlockedCustom;
                UnlockedSixth = data.unlockedSixth;
                UnlockedChallenges = data.unlockedChallenges;
                CompletedChallenges = data.completedChallenges;
                return data;
            }
            catch (System.Exception)
            {
                Debug.LogError("ERR! Failed to fetch existing data. Wiping!");
                // error handling if player mishandled data or if it's just corrupted
                WipeData();
                string convertedJson = DecodeBase64(File.ReadAllText(dataPath));
                SaveData data = JsonUtility.FromJson<SaveData>(convertedJson);
                return data;
            }
        }
        else
        {
            WipeData();
            string convertedJson = DecodeBase64(File.ReadAllText(dataPath));
            SaveData data = JsonUtility.FromJson<SaveData>(convertedJson);
            return data;
        }
    }

    private static void CreateSaveData()
    {
        // Creating json file w/ public fields from encoded base64
        string json = JsonUtility.ToJson(new SaveData(Night, Stars, UnlockedCustom, UnlockedSixth, UnlockedChallenges, CompletedChallenges));
        string encodedData = EncodeBase64(json);

        // Creating directory for data
        Directory.CreateDirectory(Application.streamingAssetsPath + "/Player_Data/");

        // Writing to file
        File.WriteAllText(dataPath, encodedData);
    }

    public static void WipeData()
    {
        // Resets all data then creates a file for it
        Night = 1;
        Stars = 0;
        UnlockedCustom = false;
        UnlockedSixth = false;
        UnlockedChallenges = false;
        CompletedChallenges = new bool[5];
        CreateSaveData();
    }

    public static void SetNight(int n)
    {
        Night = Mathf.Clamp(n, 1, 7);
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

    public static void SetSixth(bool s)
    {
        UnlockedSixth = s;
        CreateSaveData();
    }

    public static void SetChallenges(bool c)
    {
        UnlockedChallenges = c;
        CreateSaveData();
    }

    public static void SetChallenges(bool[] c)
    {
        CompletedChallenges = c;
        CreateSaveData();
    }

    private static string EncodeBase64(string text)
    {
        byte[] textBytes = System.Text.Encoding.UTF8.GetBytes(text);
        return System.Convert.ToBase64String(textBytes);
    }

    private static string DecodeBase64(string encodedText)
    {
        byte[] textBytes = System.Convert.FromBase64String(encodedText);
        return System.Text.Encoding.UTF8.GetString(textBytes);
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
    public bool unlockedSixth;
    public bool unlockedChallenges;
    public bool[] completedChallenges;

    public SaveData(int n, int s, bool u, bool u_s, bool u_c, bool[] c_c)
    {
        night = n;
        stars = s;
        unlockedCustom = u;
        unlockedSixth = u_s;
        unlockedChallenges = u_c;
        completedChallenges = c_c;
    }
}
