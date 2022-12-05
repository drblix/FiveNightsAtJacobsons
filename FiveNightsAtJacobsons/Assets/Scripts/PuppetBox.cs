using UnityEngine;
using UnityEngine.UI;

public class PuppetBox : MonoBehaviour
{
    private const float MAX_ATTACK_TIME = 90f;

    [SerializeField]
    private Image puppetWheel;

    [SerializeField]
    private int activity = 0;

    [SerializeField]
    private float windRate = 2.5f;

    private bool beingWound = false;

    private float attackTime;

    private float timer = 0f;


    private void Awake()
    {
        attackTime = MAX_ATTACK_TIME - (activity * 2f);
        enabled = !(activity == 0);
    }

    private void Update()
    {
        if (timer > attackTime)
        {
            Debug.Log("Puppet is out");
        }

        if (beingWound)
        {
            timer -= Time.deltaTime * windRate;
        }
        else
        {
            timer += Time.deltaTime;
        }

        timer = Mathf.Clamp(timer, 0f, MAX_ATTACK_TIME);

        float fill = 1f - timer / attackTime;
        puppetWheel.fillAmount = fill;
    }


    public void SetActivity(int a) => activity = a;
    public void SetWound(bool state) => beingWound = state;
}