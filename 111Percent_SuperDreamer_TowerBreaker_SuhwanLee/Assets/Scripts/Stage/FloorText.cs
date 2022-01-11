using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FloorText : MonoBehaviour
{
    public static FloorText instance;
    [SerializeField] private Text text;
    public static int NowFloor;
    void Start()
    {
        instance = this;
    }
    //매 함수 호출 마다 라운드 초기화 및 Text화
    public void texting()
    {
        NowFloor++;
        text.text = "제 " + NowFloor.ToString()+ " / 111" + " 층";
    }
}
