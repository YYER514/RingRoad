
using System.Collections;

using UnityEngine;


public class Goin_Script : MonoBehaviour
{
    public float addTime;
    
    private void Start()
    {
        transform.SetParent(null);
        if (selfAction == null)
        {
            StartCoroutine(FloatingBeforeFly());
        }
        else
        {
            isEndGoin = true;
            StartCoroutine(FlyToSunGoinRoutine());
        }

    }
    public float floatHeight = 5.0f;
    public float floatDuration = 0.25f;
    public float jumpHeight = 5.0f; // �����߶�
    public float jumpDuration = 0.3f;
    public Vector3 offsetRange = new Vector3(5.0f, 0f, 5.0f); // XZ ƽ�����ƫ�Ʒ�Χ
    private IEnumerator FloatingBeforeFly()
    {
        Vector3 startPos = transform.position;

        // 1. ���������㣨XZ����ƫ�ƣ�
        Vector3 randomOffset = new Vector3(
            Random.Range(-offsetRange.x, offsetRange.x),
            0,
            Random.Range(-offsetRange.z, offsetRange.z)
        );
        Vector3 landingPoint = startPos + randomOffset;
        // 保持原有的Y坐标，不强制设置为0.2f，让金币停留在敌人所在的平面上
        // landingPoint.y = 0.2f; // 注释掉这行
        // 2. �������̣�������Ч����
        float timer = 0f;
        while (timer < jumpDuration)
        {
            timer += Time.deltaTime;
            float t = timer / jumpDuration;

            // �����߲�ֵ
            Vector3 horizontal = Vector3.Lerp(startPos, landingPoint, t);
            float height = Mathf.Sin(t * Mathf.PI) * jumpHeight;
            transform.position = horizontal + Vector3.up * height;

            yield return null;
        }

        transform.position = landingPoint;

        // 3. ��㸡�� + ͬʱ�ָ���ת
        IEnumerator floatRoutine = FloatingAtPosition(landingPoint);
        IEnumerator rotateRoutine = RotateToFixedRotation(Quaternion.Euler(0, 0, 90), floatDuration * 0.8f); // ���θ���ʱ��

        // ����ִ������Э��
        yield return StartCoroutine(RunParallel(floatRoutine, rotateRoutine));
        // 4. ��ʼ����Ŀ��
        enddMoveOver = true;
        if (selfAction == null)
        {
            gameObject.transform.GetChild(0).gameObject.SetActive(true);
        }
    }
    private IEnumerator RotateToFixedRotation(Quaternion targetRotation, float duration)
    {
        Quaternion startRotation = transform.rotation;
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float t = timer / duration;
            transform.rotation = Quaternion.Slerp(startRotation, targetRotation, t);
            yield return null;
        }

        transform.rotation = targetRotation;
    }
    private IEnumerator RunParallel(IEnumerator routine1, IEnumerator routine2)
    {
        bool done1 = false;
        bool done2 = false;

        IEnumerator Wrapper(IEnumerator routine, System.Action onDone)
        {
            yield return StartCoroutine(routine);
            onDone();
        }

        yield return StartCoroutine(Wrapper(routine1, () => done1 = true));
        yield return StartCoroutine(Wrapper(routine2, () => done2 = true));

        while (!done1 || !done2)
        {
            yield return null;
        }
    }
    private IEnumerator FloatingAtPosition(Vector3 centerPos)
    {
        Vector3 upPos = centerPos + Vector3.up * floatHeight;

        float timer = 0f;
        while (timer < floatDuration)
        {
            timer += Time.deltaTime;
            float t = timer / floatDuration;
            transform.position = Vector3.Lerp(centerPos, upPos, t);
            yield return null;
        }

        timer = 0f;
        while (timer < floatDuration)
        {
            timer += Time.deltaTime;
            float t = timer / floatDuration;
            transform.position = Vector3.Lerp(upPos, centerPos, t);
            yield return null;
        }
    }
    private void OnEnable()
    {
        enddMoveOver = false;
    }
    public System.Action selfAction;
    private void FixedUpdate()
    {
        if (GameManager.Instance.gameType==GameType.GameEnd)
        {
            Destroy(gameObject);
            return;
        }
      
    }

    public bool startMoveOver;
    public bool enddMoveOver;
    public bool firstJudge;
    private void Update()
    {
        if (enddMoveOver&&!RockerController.Instance.inputJudge&& !isEndGoin)
        {
            float distance = Vector3.Distance(transform.position, RockerController.Instance.Player.transform.position);
            if (distance < triggerDistance && !isFlying && !firstJudge)
            {
                firstJudge = true;
                StartCoroutine(FlyToPlayerRoutine());
            }
        }
    
    }

    public bool isEndGoin;

    public Vector3 endPoint;

    public bool DelayOpen;

    IEnumerator DelayOverEnd()
    {
        yield return new WaitForSeconds(0.25f + addTime);

        DelayOpen = true;

    }

  

    public Vector3 Move3DObjectToUIPosition(RectTransform uiElement, Transform object3D, Camera mainCamera)
    {
        Vector3 screenPosition = uiElement.position; // UI λ�ã���Ļ���꣩

        // ��ȡ 3D ���嵱ǰ��������� Z ֵ
        float objectZ = object3D.position.z;

        // �� 3D ��������� Z ����ת��Ϊ��Ļ�ռ�� Z ����
        Vector3 objectScreenPosition = mainCamera.WorldToScreenPoint(object3D.position);
        screenPosition.z = objectScreenPosition.z; // �� UI ת����� Z ���ƥ�� 3D ����� Z

        // ת��Ϊ��������
        Vector3 worldPosition = mainCamera.ScreenToWorldPoint(screenPosition);
        return new Vector3(worldPosition.x, worldPosition.y, objectZ); // ����ԭ Z ֵ

    }

    IEnumerator FlyToPlayerRoutine()
    {
        isFlying = true;

        Vector3 startPos = transform.position;
        Vector3 endPos = RockerController.Instance.Player.transform.position;
        Vector3 controlPoint = (startPos + endPos) / 2 + Vector3.up * height;

        float t = 0;
        while (t < 1f)
        {
            t += Time.deltaTime / flyDuration;
            Vector3 m1 = Vector3.Lerp(startPos, controlPoint, t);
            Vector3 m2 = Vector3.Lerp(controlPoint, RockerController.Instance.Player.transform.position, t);
            transform.position = Vector3.Lerp(m1, m2, t);
            yield return null;
        }
        GameManager.Instance.goinNumber += 1;
       GameManager.Instance.SetGoinText();
        
        transform.position = endPos;
        AudioManager_Script.Instance.GoinF();
        Destroy(gameObject);
    }

    public Transform goinEndPoint;

    IEnumerator FlyToSunGoinRoutine()
    {
       

        Vector3 startPos = transform.position;
        Vector3 endPos = goinEndPoint.position;
        Vector3 controlPoint = (startPos + endPos) / 2 + Vector3.up * height;

        float t = 0;
        while (t < 1f)
        {
            t += Time.deltaTime / flyDuration;
            Vector3 m1 = Vector3.Lerp(startPos, controlPoint, t);
            Vector3 m2 = Vector3.Lerp(controlPoint, endPos, t);
            transform.position = Vector3.Lerp(m1, m2, t);
            yield return null;
        }
        if (selfAction!=null)
        {
            selfAction();
            selfAction = null;
        }
        transform.position = endPos;
        AudioManager_Script.Instance.GoinF();
        Destroy(gameObject);
    }


    public Transform player;
    public float triggerDistance = 2f;
    public float height = 2f;
    public float flyDuration = 0.5f;
    private bool isFlying = false;
}
