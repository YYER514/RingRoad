using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioDete : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }
  public  AudioSource selfAudio;
    bool detection;
    // Update is called once per frame
    void Update()
    {
        if (selfAudio.isPlaying)
        {
            detection = true;
        }
        if (detection&&!selfAudio.isPlaying)
        {
            Destroy(gameObject);
        }
    }
}
