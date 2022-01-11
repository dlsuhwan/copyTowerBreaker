using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHp : MonoBehaviour
{
    public int hp;
    public static float enemySpeed=0.5f;
    public static int strong = 20;

    Animator animator;

    [SerializeField] private GameObject hitedTexit;

    void Start()
    {
        //애니메이터 컴포넌트
        animator = GetComponent<Animator>();
        
        //1초후 이동
        Invoke("enemyMove",1f);    
    }
    void enemyMove()
    {
        //만약 해당 객체 이름에 따른 애니메이션 실행
        if(gameObject.name == "Knight_nomal(Clone)") animator.Play("Knight_move");
        else if(gameObject.name== "Swordman_normal(Clone)") animator.Play("Swordman_move"); 
        
        //객체 이동 코루틴 실행
        StartCoroutine("Move");
    }
    IEnumerator Move()
    {
        //기본 방어를 사용할 때를 제외하고 객체 이동
        while (true) 
        {
            if(!PlayManager.shield)
                transform.Translate(-enemySpeed * Time.deltaTime, 0, 0);
            yield return new WaitForFixedUpdate();
        }
    }
    private void Update()
    {
        if(hp<=0)
        {
            //남은 적수 뺴기
            EnemySystem.enemyCount--;
            Debug.Log($"남은 적 수 : {EnemySystem.enemyCount}");

            //listIndex 받기
            int listIndex = EnemySystem.objList.IndexOf(gameObject);
            
            //list빼기
            EnemySystem.objList.RemoveAt(listIndex);
            //적 객체 삭제
            Destroy(gameObject);            

            //ui찾아서 빨간줄 긋기
            GameObject uiObj = EnemySystem.imgList[listIndex].gameObject;
            uiObj.transform.GetChild(0).gameObject.SetActive(true);
            //리스트 빼기
            EnemySystem.imgList.RemoveAt(listIndex);
            //적이미지 객체 삭제
            Destroy(uiObj, 0.5f);

        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 피 깍기 //해당 tag 가 weapon이면 적이 무기에 맞았다는 것.
        if (collision.gameObject.tag == "weapon")
        {   
            //적 객체 공격 당할 시 공격 스킬 fill의 양 더하기
            float fill = GameObject.Find("Skill_attck").transform.GetChild(0).gameObject.GetComponent<Image>().fillAmount;
            GameObject.Find("Skill_attck").transform.GetChild(0).gameObject.GetComponent<Image>().fillAmount = fill + 0.05f;
            
            //적 객체 hp 깍기
            if (hp > 0)
            {
                hp = hp - strong;
                Debug.Log("enemy ID : " + gameObject.name + " | hp : " + hp);
                Instantiate(hitedTexit, transform.position, Quaternion.identity);
            }
        }
    }
}
