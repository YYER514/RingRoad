using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ani_Script : MonoBehaviour
{
    public Player_Script player;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayerAttackStart()
    {
        player.Shoot();
        AudioManager_Script.Instance.playerAttackF();
    }

    public void PlayerAttackOver()
    {
        player.isAttack = false;
        player.selfAni.Play("Null", 1, 0);
    }

}
