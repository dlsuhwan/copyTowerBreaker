using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PlayerHp : MonoBehaviour
{
    private GameObject[] hp = new GameObject[3];
    //private GameObject hpZone;
    int hpCount = 3;

    void Awake()
    {
        hpCount = 3;

        //hp피 찾기
        GameObject obj = GameObject.FindWithTag("Heart");

        hp[0] = obj.transform.GetChild(0).gameObject;
        hp[1] = obj.transform.GetChild(1).gameObject;
        hp[2] = obj.transform.GetChild(2).gameObject;
    }
    
    void Update()
    {
        //피가 0이면 끝
        if (hpCount == 0)
        {
            GameManager.dieFloor = true;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //hp 위치 조정 및 hp 마이너스
        if (collision.gameObject.tag == "HPzone")
        {
            gameObject.GetComponent<Animator>().Play("Hero_Hited");

            gameObject.transform.position = gameObject.transform.position;

            hpCount--;
            if (hpCount == 2)
            {
                hp[1].gameObject.SetActive(false);
                hp[0].GetComponent<RectTransform>().anchoredPosition = new Vector2(-15, -80);
                hp[2].GetComponent<RectTransform>().anchoredPosition = new Vector2(15, -80);
            }
            else if (hpCount == 1)
            {
                hp[0].GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -80);
                hp[2].SetActive(false);
            }
            else if (hpCount == 0)
            {
                hp[0].SetActive(false);
            }
            else return;
        }
    }
}
