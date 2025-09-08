using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTargetManager : SingletonMono<EnemyTargetManager>
{
    protected override void Awake()
    {
        base.Awake();
    }
    // ���д��ĵ���
    public List<MoveAlongPathWithOffset> enemies = new List<MoveAlongPathWithOffset>();

    // �ѱ�����ռ�õĵ��˼��ϣ���ֹ�ظ�����
    private HashSet<MoveAlongPathWithOffset> assignedTargets = new HashSet<MoveAlongPathWithOffset>();

    // �յ�λ��
    public Transform endPoint;

    private void Start()
    {
        
    }

    // ����µ���
    public void AddEnemy(MoveAlongPathWithOffset enemy)
    {
        if (!enemies.Contains(enemy))
            enemies.Add(enemy);
    }

    // �Ƴ����ˣ���������ʱ���ã�
    public void RemoveEnemy(MoveAlongPathWithOffset enemy)
    {
        enemies.Remove(enemy);
        assignedTargets.Remove(enemy);
    }


    public MoveAlongPathWithOffset GetAvailableTargetForTurret(Vector3 turretPos, float attackRadius)
    {
        MoveAlongPathWithOffset best = null;
        float maxProgress = -1f;
        float attackRadiusSqr = attackRadius * attackRadius;

        foreach (var enemy in enemies)
        {
            if (enemy == null) continue;
            if (enemy.enemyType == EnemyType.Dead || enemy.Hp <= 0) continue;
            if (enemy.oldHp < 1) continue;

            float distToTurretSqr = (enemy.transform.position - turretPos).sqrMagnitude;
            if (distToTurretSqr > attackRadiusSqr) continue;

            float progress = enemy.GetAccurateProgress();
            if (progress > maxProgress)
            {
                maxProgress = progress;
                best = enemy;
            }
        }

        if (best != null)
        {
            best.oldHp--; // ��ֹ�ظ�����
        }

        return best;
    }

    public MoveAlongPathWithOffset GetTargetForPlayer(Vector3 playerPos, float attackRadius)
    {
        MoveAlongPathWithOffset best = null;
        float maxProgress = -1f;
        float attackRadiusSqr = attackRadius * attackRadius;

        foreach (var enemy in enemies)
        {
            if (enemy == null) continue;
            if (enemy.enemyType == EnemyType.Dead || enemy.Hp <= 0) continue;
            if (enemy.oldHp < 1) continue;

            float distToPlayerSqr = (enemy.transform.position - playerPos).sqrMagnitude;
            if (distToPlayerSqr > attackRadiusSqr) continue;

            float progress = enemy.GetAccurateProgress();
            if (progress > maxProgress)
            {
                maxProgress = progress;
                best = enemy;
            }
        }

        if (best != null)
        {
            best.oldHp-=2;
        }

        return best;
    }
}
