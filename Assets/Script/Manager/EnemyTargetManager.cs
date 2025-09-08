using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTargetManager : SingletonMono<EnemyTargetManager>
{
    protected override void Awake()
    {
        base.Awake();
    }
    // 所有存活的敌人
    public List<MoveAlongPathWithOffset> enemies = new List<MoveAlongPathWithOffset>();

    // 已被炮塔占用的敌人集合，防止重复分配
    private HashSet<MoveAlongPathWithOffset> assignedTargets = new HashSet<MoveAlongPathWithOffset>();

    // 终点位置
    public Transform endPoint;

    private void Start()
    {
        
    }

    // 添加新敌人
    public void AddEnemy(MoveAlongPathWithOffset enemy)
    {
        if (!enemies.Contains(enemy))
            enemies.Add(enemy);
    }

    // 移除敌人（比如死亡时调用）
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
            best.oldHp--; // 防止重复分配
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
