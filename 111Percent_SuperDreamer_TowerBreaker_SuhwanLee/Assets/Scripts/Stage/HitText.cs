using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HitText : MonoBehaviour
{
    Text text;
    int hitedScore;

    void Start()
    {
        //hit데미지 가져오기
        hitedScore = EnemyHp.strong;

        //자식오브젝트 Text 설정
        text = gameObject.transform.GetChild(0).GetComponent<Text>();
        //Text 가시화
        text.text = hitedScore.ToString();

        //Text 객체 알파값조정 및 포지션 설정
        StartCoroutine("hitedTexting");
    }
    IEnumerator hitedTexting()
    {
        //2초후 제거
        Destroy(gameObject,2f);

        //객체가 널이 아닐 때 까지 
        if(gameObject!=null)
        {
            //color 값 설정
            Color color = gameObject.transform.GetChild(0).GetComponent<Text>().color;
            
            //알파 값이 0보다 큰동안 반복
            while (color.a > 0)
            {
                //포지션 y값 이동
                transform.Translate(0, 0.5f * Time.deltaTime, 0);

                //알파값 마이너스
                color.a -= Time.deltaTime * 1f;
                //다시 알파값 텍스트 color에 초기화
                gameObject.transform.GetChild(0).GetComponent<Text>().color = color;
                
                yield return new WaitForFixedUpdate();
            }
        }
    }
    
}
