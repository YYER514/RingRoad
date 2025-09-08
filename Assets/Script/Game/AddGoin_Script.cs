using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddGoin_Script : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < transform.GetChild(0).GetChild(1).childCount; i++)
        {
            childTextPoints.Add(transform.GetChild(0).GetChild(1).GetChild(i));

        }
        SetCurrentText();
        max = currentGoinNumber;
        oldmax = max;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public List<Sprite> numberSprites;
    public List<Transform> childTextPoints;
    public Transform slider;

    private void OnTriggerStay(Collider other)
    {
        if (other.tag=="Player")
        {
            subeTime -= Time.deltaTime;
            if (max > 0&&GameManager.Instance.goinNumber>=1&& subeTime<0)
            {
                GameManager.Instance.goinNumber -= 1;
                GameManager.Instance.SetGoinText();
               var data= ObjectPool.Instance.Get("Goin");
                data.transform.position = other.transform.GetChild(3).transform.position;
                data.GetComponent<Goin_Script>().goinEndPoint = this.transform;
                data.GetComponent<Goin_Script>().selfAction = AddNumber;
                data.SetActive(true);
                max--;
                subeTime = 0.01F;
            }
        }
    }
    public bool endBoos;
    public int bossNumber;
    public void AddNumber()
    {
        currentGoinNumber -= 1;
        SetCurrentText();
        gameObject.transform.GetChild(0).GetChild(0).localScale = new Vector3(1-((float)currentGoinNumber / oldmax),1,1);

        if (currentGoinNumber<=0)
        {
            for (int i = 0; i < openTa.Count; i++)
            {
                openTa[i].SetActive(true);
            }
            gameObject.SetActive(false);
            if (endBoos)
            {
                CreateEnemy_Script.Instance.StopEnemy();
                SpereLook_Scipt.Instance.SetThirdTip();
            }
            else
            {
                CreateEnemy_Script.Instance.CreateBoss(bossNumber);
            }

            if (win)
            {
                GameManager.Instance.Win();
            }
        }
      
    }

    public bool win;

    public List<GameObject> openTa;

    float subeTime;
    public int currentGoinNumber;
    private int max;
    private int oldmax;

    public void SetText()
    {
        currentGoinNumber--;
        if (currentGoinNumber>-1)
        {
            SetCurrentText();
        }
    }

    public void SetCurrentText()
    {
        var data = currentGoinNumber.ToString().ToCharArray();
        for (int i = 0; i < childTextPoints.Count; i++)
        {
            for (int t = 0; t < childTextPoints[i].childCount; t++)
            {
                childTextPoints[i].GetChild(t).gameObject.SetActive(false);
            }
        }

        for (int t = 0; t < childTextPoints[data.Length-1].childCount; t++)
        {
            childTextPoints[data.Length - 1].GetChild(t).gameObject.SetActive(true);
            childTextPoints[data.Length - 1].GetChild(t).GetComponent<SpriteRenderer>().sprite =numberSprites[SelfExtensions.CharToInt(data[t])];
        }



    }

}
