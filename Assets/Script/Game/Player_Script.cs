using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Script : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public Animator selfAni;
    public bool isAttack;
    public MoveAlongPathWithOffset enemy;
    public List<ParticleSystem> goin;
    public List<ParticleSystem> particleSystems;
    bool noattack;
    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.gameType==GameType.GameEnd)
        {
            return;
        }
        attackTime -= Time.deltaTime;
         Debug.Log($"Attack conditions - isAttack: {isAttack}, attackTime: {attackTime}, animationCheck: {!selfAni.GetCurrentAnimatorStateInfo(1).IsName("Attack")}");
        if (!isAttack&& attackTime<0&& !selfAni.GetCurrentAnimatorStateInfo(1).IsName("Attack"))
        {
            enemy = null;
            enemy =  EnemyTargetManager.Instance.GetTargetForPlayer(transform.position,4.5f);
          Debug.Log($"Found enemy: {enemy != null}, enemy count: {EnemyTargetManager.Instance.enemies.Count}");
            if (enemy != null)
            {
                 Debug.Log($"Attacking enemy: {enemy.name}, HP: {enemy.Hp}, oldHp: {enemy.oldHp}, distance: {Vector3.Distance(transform.position, enemy.transform.position)}");
                closestEnemy = null;
                isAttack = true;
               
                selfAni.Play("Attack",1,0);
                for (int i = 0; i < particleSystems.Count; i++)
                {
                    particleSystems[i].Play();
                }
                
                // 发射子弹
                Shoot();
              
                attackTime = attackSpeed; // 使用可配置的攻击速度
            }
            else
            {
                selfAni.Play("Null", 1, 0);
            }
        }
        if (isAttack&&selfAni.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
        {
            selfAni.SetLayerWeight(1, 1);
        }
        else if(isAttack)
        {
            closestEnemy = null;
            selfAni.SetLayerWeight(1, 0.5f);
        }
        
        // 检查攻击动画是否结束，如果结束则重置攻击状态
        if (isAttack && !selfAni.GetCurrentAnimatorStateInfo(1).IsName("Attack"))
        {
            isAttack = false;
            Debug.Log("Attack animation finished, resetting isAttack to false");
        }
    }

    public float attackTime;
    public float attackSpeed = 1.0f; // 攻击间隔时间，可在Inspector中调整

    public Transform bulletSpawnPoint;
    public GameObject bulletPrefab;
    public MoveAlongPathWithOffset closestEnemy;
    void LookAtClosestEnemy()
    {
         closestEnemy = null;
        float closestDistance = Mathf.Infinity;

        foreach (var enemy in EnemyTargetManager.Instance.enemies)
        {
            if (enemy == null || enemy.enemyType == EnemyType.Dead)
                continue;

            float distance = Vector3.Distance(transform.GetChild(3).position, enemy.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestEnemy = enemy;
            }
        }

        if (closestEnemy != null)
        {
            Vector3 targetPosition = closestEnemy.transform.position;
            // ֻ���� Y ����ת�������߶ȣ�
            Vector3 direction = targetPosition - transform.GetChild(3).position;
            direction.y = 0f; // ���Ը߶Ȳ�
            if (direction != Vector3.zero)
            {
                Quaternion lookRotation = Quaternion.LookRotation(direction);
                transform.GetChild(1).rotation = lookRotation;
            }
        }
    }
    public void Shoot()
    {
        GameObject bulletObj = ObjectPool.Instance.Get("Shoot");
        bulletObj.transform.position = bulletSpawnPoint.position;
        bulletObj.transform.rotation = Quaternion.Euler(0, bulletSpawnPoint.eulerAngles.y,0);
        // 计算到目标方向，包含Y轴（完整3D方向）
        Vector3 toTarget = enemy.transform.position - bulletSpawnPoint.position;
        Vector3 fullDirection = toTarget.normalized; // 保持完整的3D方向，包括高度差

        // 设置旋转
        if (fullDirection != Vector3.zero)
        {
            Quaternion fullRotation = Quaternion.LookRotation(fullDirection);
            bulletObj.transform.rotation = fullRotation; // 使用完整的3D旋转
        }

        // ��ʼ������
        Shoot_Script bullet = bulletObj.GetComponent<Shoot_Script>();
        bullet.transform.GetChild(0).gameObject.SetActive(true);
        bullet.player = this;
        if (enemy!=null)
        {
            bullet.playerEnemy = enemy;
            bulletObj.SetActive(true);
            if (bullet != null)
            {
                bullet.InitDirection(fullDirection, 18.0f*0.6f);
            }
        }
        else
        {
            Destroy(bullet);
        }
      
      
      
    }
}
