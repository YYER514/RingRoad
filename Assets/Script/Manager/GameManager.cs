using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
public enum GameType
{
    Start,
    GamePlay,
    GameEnd
}
public class GameManager : SingletonMono<GameManager>
{
    public GameType gameType;
    protected override void Awake()
    {
   
        base.Awake();
       
    }
    bool isEnd;
    public void OpenEndJump()
    {
        if (isEnd) return;
        isEnd = true;
    
        StartCoroutine("DelayJump");
    }
    IEnumerator DelayJump()
    {
        yield return new WaitForSeconds(1.0f);
       
    }

    public int goinNumber;

    public SkinnedMeshRenderer enemyRen;

    public SetGoinUI_Script setGoinUI_Script;

    public void SetGoinText()
    {
    
        setGoinUI_Script.SetCurrentText(goinNumber);
        SpereLook_Scipt.Instance.SetFirstTip();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            Win();
        }
    }

    public GameObject lose;
    public GameObject winUI;

    public List<GameObject> winObjs;

    public void Win()
    {
        GameManager.Instance.gameType = GameType.GameEnd;
        winJudge = true;
        RockerController.Instance.gameObject.SetActive(false);
        StartTransition();
    }
    public bool winJudge;
    public Color fromColor = Color.white;
    public Color toColor = Color.black;
    public float duration = 3f;

    private Coroutine transitionCoroutine;
    bool isFrist;
    public void StartTransition()
    {
        if (isFrist)
        {
            return;
        }
        isFrist = true;
        SetCamera_Script.Instance.WinFuntcion();
    }

    public void OpenEnd()
    {
        if (transitionCoroutine != null)
            StopCoroutine(transitionCoroutine);

        transitionCoroutine = StartCoroutine(LerpAmbientColor(fromColor, toColor, duration));
    }

    public Light oldlight;
    public Light newlight;

    public List<Light> lights;
    public List<float> endIns;

    IEnumerator LerpAmbientColor(Color startColor, Color endColor, float time)
    {
        float elapsed = 0f;
        float current = oldlight.intensity;
        Quaternion rotation = oldlight.transform.rotation;
        oldlight.GetComponent<Animator>().enabled = true;
        while (elapsed < time)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / time;
            RenderSettings.ambientLight = Color.Lerp(startColor, endColor, t);
            oldlight.intensity = Mathf.Lerp(current, newlight.intensity, t);
            for (int i = 0; i < lights.Count; i++)
            {
                lights[i].intensity = Mathf.Lerp(0, endIns[i], t);
            }
            Debug.Log(t);
            yield return null;
        }

        yield return new WaitForSeconds(1.0f);
        winUI.SetActive(true);
        OpenEndJump();

    }

    public void Jump()
    {
  
    }


    public GameObject arrow;

}
