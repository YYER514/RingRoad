using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class CreateEnemy_Script : SingletonMono<CreateEnemy_Script>
{
    protected override void Awake()
    {
        base.Awake();
    }
    // Start is called before the first frame update
    void Start()
    {
        InitCreateEnemy();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public List<LinePoints> linePoints;
    public GameObject boss;
    public GameObject maxBoss;

   public int level = 2;
    public int oldLevel;
    IEnumerator CerateEnemy()
    {
          oldLevel = level;
    while (GameManager.Instance.gameType != GameType.GameEnd)
    {
        if (GameManager.Instance.gameType != GameType.GamePlay)
        {
            yield return null;
        }
        else
        {
            int currentIndex = 0;
            for (int i = 0; i < 50; i++)
            {
                var data = ObjectPool.Instance.Get("ChildEnemy");
                CreateEnemy(data, currentIndex);

                currentIndex = currentIndex + 1 > 2 ? 0 : currentIndex + 1;
                
                // 增加生成间隔，给每个僵尸更多空间
                yield return new WaitForSeconds(Random.Range(0.2f, 0.4f));
            }
            
            for (int i = 0; i < level; i++)
            {
                var data = Instantiate(boss);
                CreateEnemy(data, currentIndex); // Boss也按路径循环生成
                currentIndex = currentIndex + 1 > 2 ? 0 : currentIndex + 1;
                yield return new WaitForSeconds(Random.Range(0.6f, 0.9f));
            }

            oldLevel += 2;
            level = oldLevel;
            yield return new WaitForSeconds(0.5f);
        }
    }
    }

    public void CreateBoss(int createIndex)
    {
        StartCoroutine(CreateBossIE(createIndex));
    }

    IEnumerator CreateBossIE(int Number)
    {
        for (int i = 0; i < Number; i++)
        {
            var data = Instantiate(boss);
            CreateEnemy(data, 0);
            yield return new WaitForSeconds(Random.Range(0.6f, 0.9f));
        }
    }

    void CreateEnemy(GameObject data,int currentIndex)
    {
        data.GetComponent<MoveAlongPathWithOffset>().pathPoints = linePoints[currentIndex].points;
        var datamessage = data.GetComponent<MoveAlongPathWithOffset>();
        for (int i = 0; i < datamessage.hits.Count; i++)
        {
            if (datamessage.hits[i].GetComponent<Animator>()!=null)
            {
                datamessage.hits[i].GetComponent<Animator>().enabled = false;
                datamessage. hits[i].materials = GameManager.Instance.enemyRen.materials;
            }
        }
        datamessage.xOffset = Random.Range(-datamessage.maxXOffset, datamessage.maxXOffset);
        data.transform.position = linePoints[currentIndex].points[0].transform.position + new Vector3(datamessage.xOffset, 0, 0);
        data.transform.rotation = Quaternion.Euler(0, 180.0f, 0);
        data.SetActive(true);
    }


    public void InitCreateEnemy()
    {
        StartCoroutine("CerateEnemy");
    }

    public void StopEnemy()
    {
        StopCoroutine("CerateEnemy");
        var data = Instantiate(maxBoss);
      
        CreateEnemy(data, 0);
    }



}
