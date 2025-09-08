using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Shoot_Script : MonoBehaviour
{
    public float speed = 20f;
    public float lifeTime = 2.0f;
    public Player_Script player;
    public MoveAlongPathWithOffset playerEnemy;
    public MoveAlongPathWithOffset enemy;
    private Vector3 moveDirection;

    // ��ʼ������
    public void InitDirection(Vector3 direction,float addSpeed)
    {
        moveDirection = direction.normalized;
      
        target = playerEnemy.transform.GetChild(1);
        speed = addSpeed;
    }

    private void OnEnable()
    {
        // �Զ����٣�Ҳ���Ի��յ�����أ�
        Invoke(nameof(DestroySelf), lifeTime);
    }

    private void OnDisable()
    {
        CancelInvoke();
    }


    private Transform target;

    public void SetTarget(Transform t)
    {
        target = t;
    }
    public float rotateSpeed = 5f; // ������ת�ٶ�

    void Update()
    {
        if (enemy != null)
        {
            if (enemy == null)
            {
                DestroySelf();
                return;
            }
            else if (target == null)
            {


                target = enemy.transform.GetChild(1);

            }
            if (!target.parent.gameObject.activeInHierarchy)
            {
                DestroySelf();
                return;
            }

            // ���㷽�򣬺��� Y ���죬ֻת Y �ᣨ����XZ������
            Vector3 direction = target.position - transform.position;



            // ֻ�з���Ϊ��ʱ����ת
            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 1);
            }

            // ʹ�÷���ֱ���ƶ��������� transform.forward
            transform.position += direction.normalized * speed * Time.deltaTime;
            float distance = enemy.maxBoos ? 1.6f : 0.9f;
            // �ӽ�Ŀ��ʱ����
            if (Vector3.Distance(target.position, transform.position) < distance)
            {
                enemy.HitSelf(true);
                DestroySelf();
            }
        }
        else
        {
            if (target == null|| playerEnemy==null)
            {
                DestroySelf();
                return;
            }
            if (!target.parent.gameObject.activeInHierarchy)
            {
                DestroySelf();
                return;
            }

            // 计算方向，包含Y轴（完整3D方向）用于玩家子弹
            Vector3 direction = target.position - transform.position;
            // direction.y = 0; // 注释掉这行，允许子弹考虑高度差

            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotateSpeed * Time.deltaTime);
            }

            // ���ֵ�ǰ����ǰ��
            transform.position += direction.normalized * speed * Time.deltaTime;
            if (Vector3.Distance(target.position, transform.position) < 1.2f)
            {
                playerEnemy.HitSelf(false);
                DestroySelf();
            }
        }
       

    }


    void DestroySelf()
    {
        target = null;
        enemy = null;
        speed = 15.0f;
        ObjectPool.Instance.Return("Shoot",gameObject);
        gameObject.transform.GetChild(0).gameObject.SetActive(false);
        gameObject.transform.GetChild(1).gameObject.SetActive(false);

    }
}
