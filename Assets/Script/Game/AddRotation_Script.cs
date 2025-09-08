using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddRotation_Script : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public float speed;
    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.left,speed,Space.Self);
    }
}
