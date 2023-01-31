using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flowers : MonoBehaviour
{
    [SerializeField] [Range(0, 20)]
    private int activity = 20;

    private int phase = 1;

    private float phaseDelay = 0f;

    private float moveTimer = 0f;

    private void Awake()
    {
        CalcPhaseDelay();
    }

    private void Update() 
    {
        moveTimer += Time.deltaTime;
    }


    private void CalcPhaseDelay()
    {
        switch (activity)
        {
            case 0:
                enabled = false;
                break;
            case 1:
                phaseDelay = 100f;
                break;
            case 2:
                phaseDelay = 95f;
                break;
            case 3:
                phaseDelay = 80f;
                break;
            case 4:
                phaseDelay = 75f;
                break;
            case 5:
                phaseDelay = 70f;
                break;
            case 6:
                phaseDelay = 70f;
                break;
            case 7:
                phaseDelay = 60f;
                break;
            case 8:
                phaseDelay = 55f;
                break;
            case 9:
                phaseDelay = 45f;
                break;
            case 10:
                phaseDelay = 40f;
                break;
            case 11:
                phaseDelay = 35f;
                break;
            case 12:
                phaseDelay = 28f;
                break;
            case 13:
                phaseDelay = 22f;
                break;
            default:
                phaseDelay = 15f;
                break;
        }
    }

    public void SetActivity(int a) {
        activity = a;
        moveTimer = 0f;
        CalcPhaseDelay();
    }
}
