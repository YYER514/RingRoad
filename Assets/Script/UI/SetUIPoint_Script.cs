using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetUIPoint_Script : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

      
    }
    public Transform targetPoint;


    void Update()
    {


        StateImage.rectTransform.anchoredPosition = ReturnPoint(targetPoint);


    }

    public Image StateImage;


    Vector3 oldPos;

    public Vector2 ReturnPoint(Transform target)
    {
        if (target == null)
        {
            gameObject.SetActive(false);
        }
        Vector3 worldPosition = target.position;
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(worldPosition);

        Vector3 normalizedPosition = new Vector3(
        screenPosition.x / Screen.width,
        screenPosition.y / Screen.height,
        screenPosition.z);
        RectTransform canvasRect = StateImage.canvas.GetComponent<RectTransform>();
        var anchoredPosition = new Vector2(
         normalizedPosition.x * canvasRect.sizeDelta.x - canvasRect.sizeDelta.x * 0.5f, normalizedPosition.y * canvasRect.sizeDelta.y - canvasRect.sizeDelta.y * 0.5f);
        return anchoredPosition;
    }

   

}
