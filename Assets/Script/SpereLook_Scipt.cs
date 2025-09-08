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
        // �������������
        Vector3 cameraForward = cameraTransform.forward;

        // �����Ҫ���� Y �ᣨֻ����ˮƽ����

        cameraForward.Normalize(); // ���¹�һ������

        // �������忴��������ķ���
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

        // ����Ŀ�귽�򣬺��� Y ��߶Ȳ�
        Vector3 direction = new Vector3(target.position.x - current.position.x, 0, target.position.z - current.position.z);

        // �������Ϊ��������������ת
        if (direction == Vector3.zero) return;

        // ������ת�Ƕȣ�Y�ᣩ��ʹ�� X �� Z ƽ��ķ���
        float angle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;

        // ���õ�ǰ�������ת�Ƕȣ������� Z ��
        target.eulerAngles = new Vector3(-90, 0, angle);

    }

}
