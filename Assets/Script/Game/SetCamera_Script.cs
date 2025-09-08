using System.Collections;
using System.Collections.Generic;
using System.Drawing;

using UnityEngine;

public class SetCamera_Script : SingletonMono<SetCamera_Script>
{
    protected override void Awake()
    {
        base.Awake();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }


    public Camera selfCamera;
   
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            WinFuntcion();
        }
        if (overCamera)
        {
            Vector3 newPos21 = Screen.width > Screen.height ? new Vector3(-13.24f, 25.62f, -23.33f) : new Vector3(-11.27f, 25.62f, -24.43f);

            transform.position =newPos21;

            transform.rotation = Quaternion.Euler(40, 29.2f, 0);
            float endFov1 = Screen.width > Screen.height ? 15 : 30;
            float endFovOld1 = Screen.width > Screen.height ? 33 : 60;

            Camera.main.fieldOfView = Mathf.Lerp(endFovOld1, endFov1, 1);

        }
        if (win)
        {

            return;
        }
        if (Screen.width > Screen.height)
        {
            selfCamera.fieldOfView = 33.0f;
        }
        else
        {
            selfCamera.fieldOfView = 60.0f;

        }
    
        
      
    }
    bool win;
    public void WinFuntcion()
    {
     
        StartCoroutine(WinIE());
    }
    public List<GameObject> house;
    IEnumerator WinIE()
    {
        float elapsed = 0f;
        float duration = 1.0f;
        transform.SetParent(null);
        Vector3 oldPos = transform.position;
        Quaternion oldRot= transform.rotation;
     
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            Vector3 newPos = new Vector3(-2.57f, 8.43f, -5.71f) ;
            transform.position = Vector3.Lerp(oldPos,newPos,t);
            yield return null; // 每帧继续
        }
        Vector3 newPos1 = new Vector3(-2.57f, 8.43f, -5.71f);
        transform.position = Vector3.Lerp(oldPos, newPos1, 1);
        for (int i = 0; i < GameManager.Instance.winObjs.Count; i++)
        {
            GameManager.Instance.winObjs[i].SetActive(true);
        }
        yield return new WaitForSeconds(0.5f);
        GameManager.Instance.OpenEnd();
        Vector3 oldPos1 = transform.position;
        elapsed = 0;
        duration = 3.0f;
        win = true;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            Vector3 newPos2 = Screen.width>Screen.height?new Vector3(-13.24f,25.62f,-23.33f): new Vector3(-11.27f, 25.62f, -24.43f);
            float t = Mathf.Clamp01(elapsed / duration);
            transform.position = Vector3.Lerp(oldPos1, newPos2, t);
            float endFov = Screen.width > Screen.height ? 15 : 30;
            float endFovOld= Screen.width > Screen.height ? 33 : 60;
            transform.rotation = Quaternion.Lerp(oldRot, Quaternion.Euler(40, 29.2f, 0), t);
            Camera.main.fieldOfView = Mathf.Lerp(endFovOld, endFov,t);

            yield return null; // 每帧继续
        }
        Vector3 newPos21 = Screen.width > Screen.height ? new Vector3(-13.24f, 25.62f, -23.33f) : new Vector3(-11.27f, 25.62f, -24.43f);
        
        transform.position = Vector3.Lerp(oldPos1, newPos21, 1);

        transform.rotation = Quaternion.Lerp(oldRot, Quaternion.Euler(40, 29.2f, 0), 1);
        float endFov1 = Screen.width > Screen.height ? 15 : 30;
        float endFovOld1 = Screen.width > Screen.height ? 33 : 60;
    
        Camera.main.fieldOfView = Mathf.Lerp(endFovOld1, endFov1, 1);
        overCamera = true;
    }
    bool overCamera = false;

}
