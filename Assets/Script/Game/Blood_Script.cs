using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Blood_Script : SingletonMono<Blood_Script>
{

    protected override void Awake()
    {
        base.Awake();
    }
    // Start is called before the first frame update
    void Start()
    {
        maxbloodNumber = bloodNumber;
        oldBlood = maxbloodNumber;
    }

    private int oldBlood;

    // Update is called once per frame
    void Update()
    {
        if (oldBlood != bloodNumber)
        {
            oldBlood--;
            fildAmount.fillAmount = oldBlood / (float)maxbloodNumber;
        }
    }

    public Animator hitAni;

    public void SubHP(int hp)
    {
        bloodNumber -= hp;
        if (bloodNumber<=0)
        {
            bloodNumber = 0;
            GameManager.Instance.gameType = GameType.GameEnd;
            RockerController.Instance.gameObject.SetActive(false);
            GameManager.Instance.lose.SetActive(true);
            GameManager.Instance.OpenEndJump();
        }
        hitAni.Play("HitHouse", 0, 0);
       
       
    }

    public int bloodNumber;
    private int maxbloodNumber;


    public Image fildAmount;

}
