using UnityEngine;

public class PlayerData
{
    public int night;
    public int stars;
    public bool unlockedCustom;

    public PlayerData GetDataFromJSON(string json) => JsonUtility.FromJson<PlayerData>(json);
    public string SaveToJSON() => JsonUtility.ToJson(this);
}
