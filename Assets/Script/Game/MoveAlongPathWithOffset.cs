﻿﻿﻿using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
public enum EnemyType
{
    Idle,
    Run,
    Dead
}

public class MoveAlongPathWithOffset : MonoBehaviour
{
    public bool IsBoss;
    public float Hp = 1;
    public bool maxBoos;

    public EnemyType enemyType;
    public List<Transform> pathPoints;
    public float moveSpeed = 3f;
    public float maxXOffset = 1f;
    public float oldHp;
    public float oldHpNew;
    public float xOffset;

    private List<Vector3> curvedPoints = new List<Vector3>();
    private List<float> segmentLengths = new List<float>(); // 每段距离
    private float totalPathLength = 0f;
    private int currentIndex = 0;

    public Animator selfAni;
    public bool dead;
    public List<SkinnedMeshRenderer> hits;

    private void OnEnable()
    {
        // 重置状态变量，防止对象池重用时状态错误
        currentIndex = 0;
        dead = false;
        dis = 0;
        
        Hp = maxBoos ? 500 : (IsBoss ? 200f : 1f);
        oldHp = Hp;
        oldHpNew = Hp;

        if (EnemyTargetManager.Instance != null)
        {
            EnemyTargetManager.Instance.AddEnemy(this);
            GenerateCurvePath();
            enemyType = EnemyType.Run;
        }
    }

    private void Start()
    {
        if (enemyType != EnemyType.Run)
        {
            EnemyTargetManager.Instance.AddEnemy(this);
            GenerateCurvePath();
            enemyType = EnemyType.Run;
        }
    }

    void Update()
    {
        dis += Time.deltaTime;
        if (GameManager.Instance.winJudge && !dead)
        {
            if (!selfAni.GetCurrentAnimatorStateInfo(0).IsName("Dead"))
            {
                dead = true;
                selfAni.Play("Dead");
                Invoke("RemoveThis", 1.0f);
            }
        }

        if (GameManager.Instance.gameType == GameType.GameEnd || GameManager.Instance.gameType == GameType.Start)
        {
            if (!selfAni.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
            {
                selfAni.Play("Idle");
            }
            return;
        }

        if (dead) return;

        // 安全检查：如果路径点为空或越界，直接返回
        if (curvedPoints.Count == 0)
        {
            Debug.LogWarning($"Enemy {gameObject.name} has empty curvedPoints! Removing from game.");
            EnemyTargetManager.Instance.RemoveEnemy(this);
            if (!IsBoss)
            {
                ObjectPool.Instance.Return("ChildEnemy", gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
            currentIndex = 0;
            return;
        }

        if (currentIndex >= curvedPoints.Count && transform.position.x > -0.4f)
        {
            EnemyTargetManager.Instance.RemoveEnemy(this);

            if (!IsBoss)
            {
                dis = 0;
                ObjectPool.Instance.Return("ChildEnemy", gameObject);
                Blood_Script.Instance.SubHP(2);
                AudioManager_Script.Instance.BoosAttackF();
            }
            else
            {
                Destroy(gameObject);
                Blood_Script.Instance.SubHP(20);
                AudioManager_Script.Instance.chidEnemyAttackF();
            }

            currentIndex = 0;
            return;
        }

        Vector3 targetPos = curvedPoints[currentIndex];
        float step = moveSpeed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, targetPos, step);

        if (!selfAni.GetCurrentAnimatorStateInfo(0).IsName("Run"))
            selfAni.Play("Run");

        Vector3 lookDir = targetPos - transform.position;
        lookDir.y = 0f;
        if (lookDir != Vector3.zero && currentIndex >= 1)
        {
            Quaternion targetRotation = Quaternion.LookRotation(lookDir, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
        }

        float endDis = IsBoss ? 1.8f : 0.1f;
        if (Vector3.Distance(transform.position, targetPos) < endDis)
        {
            currentIndex++;
        }

        // 测试进度（可删）
        // Debug.Log("进度百分比: " + (GetAccurateProgress() * 100f).ToString("F1") + "%");
    }

    bool hitOpen;
    IEnumerator StartHit()
    {
        float elapsed = 0f;
        float duration = 0.15f;
        Color color = Color.white;
        Color colorRed = Color.red;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            foreach (var hit in hits)
            {
                hit.materials[0].SetColor("_BaseColor", Color.Lerp(color, colorRed, t));
            }
            yield return null;
        }
        foreach (var hit in hits)
        {
            hit.materials[0].SetColor("_BaseColor", colorRed);
        }

        elapsed = 0;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            foreach (var hit in hits)
            {
                hit.materials[0].SetColor("_BaseColor", Color.Lerp(colorRed, color, t));
            }
            yield return null;
        }
        foreach (var hit in hits)
        {
            hit.materials[0].SetColor("_BaseColor", color);
        }

        hitOpen = false;
    }

    void GenerateCurvePath()
    {
        curvedPoints.Clear();
        segmentLengths.Clear();
        totalPathLength = 0f;

        // 详细的路径点检查和调试信息
        if (pathPoints == null)
        {
            Debug.LogError($"Enemy {gameObject.name}: pathPoints is null!");
            return;
        }

        if (pathPoints.Count < 2)
        {
            Debug.LogError($"Enemy {gameObject.name}: pathPoints count is {pathPoints.Count}, need at least 2 points!");
            return;
        }

        // 检查路径点是否有null值
        for (int i = 0; i < pathPoints.Count; i++)
        {
            if (pathPoints[i] == null)
            {
                Debug.LogError($"Enemy {gameObject.name}: pathPoints[{i}] is null!");
                return;
            }
        }

        for (int i = 0; i < pathPoints.Count - 1; i++)
        {
            Vector3 p0 = i == 0 ? pathPoints[i].position : pathPoints[i - 1].position;
            Vector3 p1 = pathPoints[i].position;
            Vector3 p2 = pathPoints[i + 1].position;
            Vector3 p3 = (i + 2 < pathPoints.Count) ? pathPoints[i + 2].position : p2;

            for (int j = 0; j < 20; j++)
            {
                float t = j / 20f;
                Vector3 point = CatmullRom(p0, p1, p2, p3, t);
                point += new Vector3(xOffset, 0f, 0f);

                if (curvedPoints.Count > 0)
                {
                    float segLen = Vector3.Distance(curvedPoints[curvedPoints.Count - 1], point);
                    segmentLengths.Add(segLen);
                    totalPathLength += segLen;
                }

                curvedPoints.Add(point);
            }
        }

        Vector3 endPos = pathPoints[pathPoints.Count - 1].position + new Vector3(xOffset, 0f, 0f);
        float finalSegLen = Vector3.Distance(curvedPoints[curvedPoints.Count - 1], endPos);
        segmentLengths.Add(finalSegLen);
        totalPathLength += finalSegLen;
        curvedPoints.Add(endPos);

        Debug.Log($"Enemy {gameObject.name}: Generated {curvedPoints.Count} curve points from {pathPoints.Count} path points");
    }

    Vector3 CatmullRom(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        return 0.5f * (
            (2f * p1) +
            (-p0 + p2) * t +
            (2f * p0 - 5f * p1 + 4f * p2 - p3) * (t * t) +
            (-p0 + 3f * p1 - 3f * p2 + p3) * (t * t * t)
        );
    }

    public float dis;

    public float GetAccurateProgress()
    {
      
        return dis;
    }
    void RemoveThis()
    {
        EnemyTargetManager.Instance.RemoveEnemy(this);
        if (!IsBoss)
            ObjectPool.Instance.Return("ChildEnemy", gameObject);
        else
            Destroy(gameObject);

        dead = false;
        currentIndex = 0;
        dis = 0;
    }

    public void HitSelf(bool isPlayer)
    {
        if (dead) return;

        if (!hitOpen)
        {
            hitOpen = true;
            StartCoroutine(StartHit());
        }

        var data = ObjectPool.Instance.Get(isPlayer ? "HitEffectJtl" : "HitEffectSJ");
        data.transform.position = transform.GetChild(1).position;
        data.SetActive(true);

        oldHpNew -= isPlayer ? 2 : 1;

        if (oldHpNew <= 0)
        {
            dead = true;
            selfAni.Play("Dead");
            enemyType = EnemyType.Dead;
            foreach (var hit in hits)
            {
                Animator a = hit.GetComponent<Animator>();
                if (a != null) a.enabled = true;
            }

            if (!IsBoss)
            {
                AudioManager_Script.Instance.childEnemyHitF();
                CreateGoin();
                CreateGoin();
            }
            else
            {
                AudioManager_Script.Instance.boosHitF();
                for (int i = 0; i < 40; i++)
                    CreateGoin();
            }

            Invoke("RemoveThis", 1.0f);
        }
    }

    void CreateGoin()
    {
        var data = ObjectPool.Instance.Get("Goin");
        data.transform.position = transform.GetChild(1).transform.position;
        data.transform.rotation = Quaternion.Euler(Random.Range(-90, 90), Random.Range(-90, 90), Random.Range(-90, 90));
        data.SetActive(true);
    }
}
