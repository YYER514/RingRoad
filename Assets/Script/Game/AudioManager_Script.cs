using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager_Script :SingletonMono<AudioManager_Script>
{
    protected override void Awake()
    {
        base.Awake();
    }
    public AudioSource bossAtack;
    public AudioSource playerAttack;
    public AudioSource goin;
    public AudioSource boosHit;
    public AudioSource childEnemyHit;
    public AudioSource chidEnemyAttack;
    public AudioSource bgm;

    public List<AudioClip> audioClips;
    public List<AudioSource> audioSources;

    // Start is called before the first frame update
    void Start()
    {
        AudioListener.volume = 0;
        for (int i = 0; i < audioSources.Count; i++)
        {
            audioSources[i].playOnAwake = false;

            audioSources[i].clip= audioClips[i];
            if (audioSources[i].GetComponent<AudioDete>()!=null)
            {
                audioSources[i].GetComponent<AudioDete>().enabled = true;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0)&& !bgm.isPlaying)
        {
            AudioListener.volume = 1;
            bgm.Play();
        }
    }

    public void BoosAttackF()
    {
        bossAtack.Play();
    }
    public void playerAttackF()
    {
        playerAttack.Play();
    }
    bool goinJudge;
    public void GoinF()
    {
        if (!goinJudge)
        {
            goinJudge = true;
            var data = Instantiate(goin).gameObject;
            data.GetComponent<AudioSource>().Play();
            if (!goins[0].isPlaying)
            {
                for (int i = 0; i < goins.Count; i++)
                {
                    goins[i].Play();
                }
            }
           
            Invoke("CloseGoin",0.3f);
        }   
    }
    public List<ParticleSystem> goins;

    private void CloseGoin()
    {
        goinJudge = false;
    }

    public void boosHitF()
    {
     
        var data = Instantiate(boosHit).gameObject;
        data.GetComponent<AudioSource>().Play();
    }
    public void childEnemyHitF()
    {
       
        var data = Instantiate(childEnemyHit).gameObject;
        data.GetComponent<AudioSource>().Play();
    }

    public void chidEnemyAttackF()
    {
      var data=  Instantiate(chidEnemyAttack).gameObject;
        data.GetComponent<AudioSource>().Play();
    }

}
