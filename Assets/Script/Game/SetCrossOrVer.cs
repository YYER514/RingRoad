using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetCrossOrVer : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public GameObject cross;
    public GameObject ver;

    // Update is called once per frame
    void Update()
    {
        if (Screen.width>Screen.height)
        {
            ver.SetActive(false);

            cross.SetActive(true);
        }
        else
        {
            ver.SetActive(true);

            cross.SetActive(false);
        }
    }
}
