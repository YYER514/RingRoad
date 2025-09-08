using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetGoinUI_Script : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public List<Transform> childTextPoints;
    public List<Sprite> numberSprites;
    public void SetCurrentText(int goinNumber)
    {
        var data = goinNumber.ToString().ToCharArray();
        for (int i = 0; i < childTextPoints.Count; i++)
        {
            for (int t = 0; t < childTextPoints[i].childCount; t++)
            {
                childTextPoints[i].GetChild(t).gameObject.SetActive(false);
            }
        }

        for (int t = 0; t < childTextPoints[data.Length - 1].childCount; t++)
        {
            childTextPoints[data.Length - 1].GetChild(t).gameObject.SetActive(true);
            childTextPoints[data.Length - 1].GetChild(t).GetComponent<Image>().sprite = numberSprites[SelfExtensions.CharToInt(data[t])];
        }
    }
}
