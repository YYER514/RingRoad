using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hit_Script : MonoBehaviour
{
    private void OnEnable()
    {
        Invoke("DestroySelf", 2.0f);
    }
    // Start is called before the first frame update
    void Start()
    {
      
    }
    public string effectName;
    // Update is called once per frame
    void Update()
    {
        
    }


    void DestroySelf()
    {
        gameObject.SetActive(false);
        ObjectPool.Instance.Return(effectName, this.gameObject);
    }

}
