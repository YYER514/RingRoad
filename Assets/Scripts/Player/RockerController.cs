using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using static UnityEngine.GraphicsBuffer;


public class RockerController : SingletonMono<RockerController>,IPointerDownHandler,IPointerUpHandler,IDragHandler
{

    protected override void Awake()
    {
        base.Awake();
    }

    public Player_Script player_Script;
    public GameObject Player;
    public GameObject PlayerParent;
   // public Transform Bullt;
    //旋转平滑系数
    private float turnSmoothTime = 0.1f;
    private float turnSmoothVelocity;
    public float speed;
    public Animator PlayerAni;

  
   
    public void OnDrag(PointerEventData eventData)
    {
    

        if (dragOver)
        {
            return;

        }

        
      
        time = 3;

        for (int i = 0; i < handTips.Count; i++)
        {
            handTips[i].SetActive(false);
            GameManager.Instance.gameType = GameType.GamePlay;
        }

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            yaoGanBGPos, eventData.position, eventData.enterEventCamera, out outPos
            );
        float S = Vector2.Distance(Vector2.zero, outPos);
        if (S > R)
        {
            outPos = outPos.normalized * R;
        }


        // 考虑摄像机的旋转
        Transform cameraTransform = Camera.main.transform;
        Vector3 adjustedDirection = Quaternion.Euler(0, cameraTransform.eulerAngles.y, 0) * new Vector3(outPos.x, 0, outPos.y);
     
        if (player_Script.enemy==null)
        {
        
            if (PlayerAni.GetCurrentAnimatorStateInfo(0).IsName(type + "Idle"))
            {
                PlayerAni.Play(type + "Run");

            }
            Quaternion targetRotation = Quaternion.LookRotation(adjustedDirection);
            Player.transform.eulerAngles = new Vector3(0, targetRotation.eulerAngles.y, 0);
        }
        else
        {
            if (player_Script.enemy != null)
            {
                if (player_Script.enemy.gameObject.activeInHierarchy)
                {
                    Vector3 direction = player_Script.enemy.transform.position - player_Script.transform.position;
                    Quaternion targetRotation = Quaternion.LookRotation(direction);
                    Player.transform.eulerAngles = new Vector3(0, targetRotation.eulerAngles.y, 0);
                }
            }
        }
        
      
       
        //PlayerAni.speed = Vector2.Distance(outPos, Vector2.zero) / 90;
        yaoGanLight.transform.up = outPos.normalized;
        yaoGanPos.localPosition = outPos;

    }

    public string type;

    public bool IsRotation;

    bool FindEnemyDistance()
    {
        bool returnJudge=false;
        if (true)
        {

        }
        return returnJudge;
    }
   






   // public GameObject Hand;
    public List<RectTransform> rectTransforms;
    private void Update()
    {

     

        if (Screen.width>Screen.height)
        {
         
          //  handTips[0].gameObject.transform.position = selfPoints[0].transform.position;
            handTips[0].gameObject.transform.localScale = new Vector3(2.0f,2.0f);


        }
        else
        {
           
            //handTips[0].gameObject.transform.position = selfPoints[1].transform.position;
            handTips[0].gameObject.transform.localScale = new Vector3(2.0f, 2.0f);
        }

        time -= Time.deltaTime;

        if (time<=0&& !handTips[0].activeInHierarchy)
        {
            for (int i = 0; i < handTips.Count; i++)
            {
              //  handTips[i].SetActive(true);
            }
        }

        if (inputJudge)
        {
            if (Screen.width > Screen.height)
            {
                yaoGanBGPos.transform.position = crossPoint.position;
            }
            else
            {
                yaoGanBGPos.transform.position = verPoint.position;

            }
        }

        if (inputJudge)
        {
            if (PlayerAni.GetCurrentAnimatorStateInfo(0).IsName(type + "RunAttack"))
            {
                AnimatorStateInfo currentStateInfo = PlayerAni.GetCurrentAnimatorStateInfo(0); // 假设我们在第0层
                float normalizedTime = currentStateInfo.normalizedTime;
                PlayerAni.Play(type + "IdleAttack", 0, normalizedTime);
            }

            if (PlayerAni.GetCurrentAnimatorStateInfo(0).IsName(type + "Run"))
            {
                PlayerAni.Play(type + "Idle");
            }

        }
    
    }

   public bool inputJudge = true;

    public Transform crossPoint;
    public Transform verPoint;
    

    private void FixedUpdate()
    {
   
        Transform cameraTransform = Camera.main.transform;
        Vector3 adjustedDirection = Quaternion.Euler(0, cameraTransform.eulerAngles.y, 0) * new Vector3(outPos.x, 0, outPos.y);
     
        PlayerParent.transform.Translate(adjustedDirection.normalized * Time.deltaTime*speed);
        if (!inputJudge)
        {
            if (PlayerAni.GetCurrentAnimatorStateInfo(0).IsName(type + "Idle"))
            {
                PlayerAni.Play(type + "Run");

            }
            else if (PlayerAni.GetCurrentAnimatorStateInfo(0).IsName(type + "IdleAttack"))
            {
                AnimatorStateInfo currentStateInfo = PlayerAni.GetCurrentAnimatorStateInfo(0); // 假设我们在第0层
                float normalizedTime = currentStateInfo.normalizedTime;
                PlayerAni.Play(type + "RunAttack", 0, normalizedTime);
            }
         
                if (player_Script.enemy != null)
                {
                    if (player_Script.enemy.gameObject.activeInHierarchy)
                    {
                        Vector3 direction = player_Script.enemy.transform.position - player_Script.transform.position;
                        Quaternion targetRotation = Quaternion.LookRotation(direction);
                        Player.transform.eulerAngles = new Vector3(0, targetRotation.eulerAngles.y, 0);
                    }
                }
            else
            {
                Quaternion targetRotation = Quaternion.LookRotation(adjustedDirection);
                Player.transform.eulerAngles = new Vector3(0, targetRotation.eulerAngles.y, 0);
            }
            

        }
        else
        {
            selfRig.velocity= Vector3.zero;
            if (player_Script.enemy != null)
            {
                if (player_Script.enemy.gameObject.activeInHierarchy)
                {
                    Vector3 direction = player_Script.enemy.transform.position - player_Script.transform.position;
                    Quaternion targetRotation = Quaternion.LookRotation(direction);
                    Player.transform.eulerAngles = new Vector3(0, targetRotation.eulerAngles.y, 0);
                }
            }
          
        }
       


    }

    public void OnPointerDown(PointerEventData eventData)
    {
        dragOver = false;

        inputJudge = false;
        //暂停人物自动移动
        //GameManager.Instance.PlayerStopAuto();
        // Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            transform as RectTransform, eventData.pressPosition, eventData.enterEventCamera, out outPos
            );
        yaoGanBGPos.GetComponent<Animator>().enabled = false;
        yaoGanBGPos.localPosition = outPos;
        outPos = Vector2.zero;
        yaoGanPos.localPosition = outPos;
    }
    bool dragOver;
    public void OnEnable()
    {
        dragOver = true;
        time = 3;

    }

    public void OnDisable()
    {
        inputJudge = false;
        time = 3;


     
        PlayerAni.speed = 1;
        PlayerAni.Play(type+"Idle");
        selfRig.velocity = Vector3.zero;
        outPos = Vector2.zero;
        yaoGanPos.localPosition = outPos;
        if (Screen.width>Screen.height)
        {
            yaoGanBGPos.anchoredPosition = new Vector2(-274f, 247);
        }
        else
        {
            yaoGanBGPos.anchoredPosition = new Vector2(-274f, 361);
        }
     
        yaoGanLight.localRotation = Quaternion.identity;
        Debug.Log("禁用");
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        time = 3;
        inputJudge = true;
      

        if (IsRotation)
        {
            if (PlayerAni.GetCurrentAnimatorStateInfo(0).IsName(type + "RunAttack"))
            {
                AnimatorStateInfo currentStateInfo = PlayerAni.GetCurrentAnimatorStateInfo(0); // 假设我们在第0层
                float normalizedTime = currentStateInfo.normalizedTime;
                PlayerAni.Play(type + "IdleAttack", 0, normalizedTime);
            }
        }
        else
        {

            if (PlayerAni.GetCurrentAnimatorStateInfo(0).IsName(type + "RunAttack"))
            {
                AnimatorStateInfo currentStateInfo = PlayerAni.GetCurrentAnimatorStateInfo(0); // 假设我们在第0层
                float normalizedTime = currentStateInfo.normalizedTime;
                PlayerAni.Play(type + "IdleAttack", 0, normalizedTime);
            }
            else
            {
                PlayerAni.Play(type + "Idle");
            }

        
        }

      
       outPos = Vector2.zero;
        yaoGanPos.localPosition = outPos;

        if (Screen.width > Screen.height)
        {
            yaoGanBGPos.anchoredPosition = new Vector2(290, 215);
        }
        else
        {
            yaoGanBGPos.anchoredPosition = new Vector2(547, 244);
        }
        yaoGanLight.localRotation = Quaternion.identity;
    }
    private RectTransform yaoGanPos;
    private RectTransform yaoGanBGPos;
    private RectTransform yaoGanLight;
    public float R;//半径
    public Vector2 outPos;
    // Start is called before the first frame update
    void Start()
    {
        selfRig = PlayerParent.GetComponent<Rigidbody>();
            yaoGanPos = transform.GetChild(0).GetChild(0) as RectTransform;
        yaoGanBGPos = transform.GetChild(0) as RectTransform;
        yaoGanLight= transform.GetChild(0).GetChild(1) as RectTransform;
    }

    public float time = 2;

    public List<GameObject> handTips;
    public List<Transform> selfPoints;

    private Rigidbody selfRig;
}
