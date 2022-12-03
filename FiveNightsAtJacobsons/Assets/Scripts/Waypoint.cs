using UnityEngine;

public class Waypoint : MonoBehaviour
{
    private int pointNum;

    private void Awake() 
    {
        pointNum = int.Parse(transform.name.Split('t')[1]);
    }
}
