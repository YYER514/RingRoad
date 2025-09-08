using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JTL_Script : MonoBehaviour
{
    public Transform firePoint;
    public Transform attackPoint;

    public Transform target;

    public float fireRate = 0.2f; // һ������
    public float fireTimer = 0f;
    public AudioSource audioSource;
    public Animator parentAni;
    void Update()
    {
        if (GameManager.Instance.gameType==GameType.GameEnd)
        {

            return;
        }
        if (parentAni.GetCurrentAnimatorStateInfo(0).IsName("Null"))
        {
            transform.parent.GetChild(2).gameObject.SetActive(true);
           
        }
        fireTimer -= Time.deltaTime;
        if (fireTimer<=0)
        {
            var data =EnemyTargetManager.Instance.GetAvailableTargetForTurret(transform.position, 7.2f);
            if (data != null)
            {
                LookAtTarget(data.transform);
                Fire(data);
                fireTimer = 0.08f;
                if (!audioSource.isPlaying)
                {
                    audioSource.Play();
                }
                if (!selfAni.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
                {
                    selfAni.Play("Attack");
                }
            }
            else
            {
                if (!selfAni.GetCurrentAnimatorStateInfo(0).IsName("JTL"))
                {
                    selfAni.Play("JTL");
                }
                if (audioSource.isPlaying)
                {
                    audioSource.Stop();
                }
            }
        }


      

      
    }

    public Animator selfAni;
    public List<ParticleSystem> allEffects;
    void Fire(MoveAlongPathWithOffset enemy)
    {
        GameObject bullet = ObjectPool.Instance.Get("Shoot");
        bullet.transform.position = attackPoint.position;
        bullet.transform.rotation = attackPoint.rotation;
        if (!allEffects[0].isPlaying)
        {
            for (int i = 0; i < allEffects.Count; i++)
            {
                allEffects[i].Play();
            }
        }

    
        bullet.GetComponent<Shoot_Script>().enemy = enemy;
        bullet.GetComponent<Shoot_Script>().transform.GetChild(1).gameObject.SetActive(true);
        bullet.SetActive(true);
    }

    void LookAtTarget(Transform target)
    {
        Vector3 targetPos = target.position;

        // Y����ת��ֻ�ø�������Y�ῴ��Ŀ�꣩
        Vector3 flatDir = transform.position-new Vector3(targetPos.x, transform.position.y, targetPos.z) ;
        if (flatDir != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(flatDir);

        // X����ת����������Ŀ�꿴��
        if (firePoint != null)
        {
            Vector3 dir = target.position - firePoint.position;
            Quaternion lookRot = Quaternion.LookRotation(dir);
            Vector3 angles = lookRot.eulerAngles;
            firePoint.localEulerAngles = new Vector3(Mathf.Clamp(-angles.x,-30,0), 0f, 0f); // ֻ����X����ת
        }
    }
    private void Start()
    {
        audioSource = gameObject.GetComponent<AudioSource>();
    }
}
