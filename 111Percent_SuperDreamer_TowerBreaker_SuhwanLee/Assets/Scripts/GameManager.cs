using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    //coin value
    private Text[] coins = new Text[3];

    public static int coinSkull;
    public static int coinGoldSkull;
    public static int coinSoul;

    //weapon value
    public static int hellWeaponBox;
    public static int deepHellWeaponBox;
    public static int deepHellGearBox;
    public static int deepHellShieldBox;
    
    //현재 진행중인 스테이지
    public static string stageName = "";

    //현재 진행 중인 층
    public static int floor = 0;
    //클리어 체크
    public static bool clearFloor = false;
    public static bool dieFloor = false;

    void Start()
    {
        instance = this;

        //해상도
        Screen.SetResolution(720, 1280, true);
               
        //객체 씬 이동간 삭제 X
        DontDestroyOnLoad(this.gameObject);

        //시간흐름
        if (Time.timeScale != 1.0f) Time.timeScale = 1.0f;

        //coin 텍스트
        CoinText();

        //스켈, 골드스켈, 소울 데이터 확인
        CoinCheck();

        //무기상자 데이터 확인
        WeaponBoxCheck();
    }
    
    void CoinText()
    {
        Transform tr = GameObject.Find("ManagerCanvas").transform.GetChild(0);

        coins[0] = tr.GetChild(0).GetChild(0).GetComponent<Text>();
        coins[1] = tr.GetChild(1).GetChild(0).GetComponent<Text>();
        coins[2] = tr.GetChild(2).GetChild(0).GetComponent<Text>();
    }

    //코인 확인
    void CoinCheck()
    {
        //데이터 저장
        if (PlayerPrefs.HasKey("coinSkull")) coinSkull = PlayerPrefs.GetInt("coinSkull");
        if (PlayerPrefs.HasKey("coinGoldSkull")) coinGoldSkull = PlayerPrefs.GetInt("coinGoldSkull");
        if (PlayerPrefs.HasKey("coinSoul")) coinSoul = PlayerPrefs.GetInt("coinSoul");
        
        //저장한 데이터 시각화
        dataView();
    }

    //무기 상자 확인
    void WeaponBoxCheck()
    {
        //데이터 저장
        if (PlayerPrefs.HasKey("hellWeaponBox")) hellWeaponBox = PlayerPrefs.GetInt("hellWeaponBox");
        if (PlayerPrefs.HasKey("deepHellWeaponBox")) deepHellWeaponBox = PlayerPrefs.GetInt("deepHellWeaponBox");
        if (PlayerPrefs.HasKey("deepHellGearBox")) deepHellGearBox = PlayerPrefs.GetInt("deepHellGearBox"); 
        if (PlayerPrefs.HasKey("deepHellShieldBox")) deepHellShieldBox = PlayerPrefs.GetInt("deepHellShieldBox");

        //저장한 데이터 시각화
        dataView();
    }
    
    //데이터 시각화
    void dataView()
    {
        coins[0].text = coinSkull.ToString();
        coins[1].text = coinGoldSkull.ToString();
        coins[2].text = coinSoul.ToString();
    }

    //데이터 저장
    public void dataSave()
    {
        //데이터 저장
        PlayerPrefs.SetInt("coinSkull", coinSkull);
        PlayerPrefs.SetInt("coinGoldSkull", coinGoldSkull);
        PlayerPrefs.SetInt("coinSoul", coinSoul);
        PlayerPrefs.SetInt("hellWeaponBox", hellWeaponBox);
        PlayerPrefs.SetInt("deepHellWeaponBox", deepHellWeaponBox);
        PlayerPrefs.SetInt("deepHellGearBox", deepHellGearBox);
        PlayerPrefs.SetInt("deepHellShieldBox", deepHellShieldBox);
        
        //해당 스테이지 층수 저장
        PlayerPrefs.SetInt(stageName + "Floor", floor);
        
        //층수 고를 때 event를 위해 저장
        for(int i=0; i<=floor; i++)
        {
            PlayerPrefs.SetInt(stageName + "ClearFloor" + (i).ToString(), 1);
        }

        PlayerPrefs.Save();

        Debug.Log($"coinSkull : {coinSkull}, coinGoldSkull : {coinGoldSkull}, coinSoul : {coinSoul}");
        Debug.Log($"hellWeaponBox : {hellWeaponBox}, deepHellWeaponBox : {deepHellWeaponBox}, deepHellGearBox : {deepHellGearBox}, deepHellShieldBox : {deepHellShieldBox}");
        Debug.Log($"stageName : {stageName}, floor : {floor}");
    }
}
