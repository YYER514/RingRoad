using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpereLook_Scipt : SingletonMono<SpereLook_Scipt>
{
    protected override void Awake()
    {
        base.Awake();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }


    public bool LookAt;
    void LookAtCamera(Transform targetObject, Transform cameraTransform)
    {
        // 计算摄像机朝向
        Vector3 cameraForward = cameraTransform.forward;

        // 如果需要忽略 Y 轴（只看向水平方向）

        cameraForward.Normalize(); // 重新归一化方向

        // 设置物体看向摄像机的方向
        targetObject.rotation = Quaternion.LookRotation(cameraForward);
    }

        // Update is called once per frame
        void Update()
    {
        if (LookAt)
        {

           LookAtCamera(gameObject.transform, Camera.main.transform);

        }
        else
        {

            if (GameManager.Instance.arrow.activeInHierarchy)
            {
                if (ReturnDistance(gameObject.transform, GameManager.Instance.arrow.transform) < 1.0f)
                {

                    GameManager.Instance.arrow.SetActive(false);

                }
                else
                {

                    gameObject.transform.GetChild(0).gameObject.SetActive(true);

                    RotateTowardsTarget(gameObject.transform, GameManager.Instance.arrow.transform);

                }

            }
            else
            {

                gameObject.transform.GetChild(0).gameObject.SetActive(false);

            }


        }
       
    }

    bool firstTip;
    bool secondTip;

    public void SetFirstTip()
    {
        if (!firstTip)
        {
            firstTip = true;
            GameManager.Instance.arrow.transform.position = new Vector3(-0.42f,2.542f,0.03f);
            GameManager.Instance.arrow.SetActive(true);
        }

    
    }

    public void SetThirdTip()
    {
        if (!secondTip)
        {

            secondTip = true;
            GameManager.Instance.arrow.transform.position = new Vector3(2.43f, 1.85f, 6.29f);
            GameManager.Instance.arrow.SetActive(true);
        }

    }

    public float ReturnDistance(Transform current,Transform target)
    {

        float distance = 0;

        distance = Vector3.Distance(new Vector3(current.position.x,0, current.position.z),new Vector3(target.position.x, 0, target.position.z));

        return distance;

    }

    void RotateTowardsTarget(Transform target, Transform current)
    {

        // 计算目标方向，忽略 Y 轴高度差
        Vector3 direction = new Vector3(target.position.x - current.position.x, 0, target.position.z - current.position.z);

        // 如果方向为零向量，忽略旋转
        if (direction == Vector3.zero) return;

        // 计算旋转角度（Y轴），使用 X 和 Z 平面的方向
        float angle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;

        // 设置当前物体的旋转角度，仅调整 Z 轴
        target.eulerAngles = new Vector3(-90, 0, angle);

    }

}
