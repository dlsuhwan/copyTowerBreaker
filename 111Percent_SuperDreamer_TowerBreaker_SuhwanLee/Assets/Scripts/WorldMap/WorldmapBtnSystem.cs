using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WorldmapBtnSystem : MonoBehaviour
{
    [Header("Worldmap")]
    [SerializeField] private Button[] worldMapBtns = new Button[5];
    [SerializeField] private GameObject[] worldMapTowerImg = new GameObject[5];

    [Header("Floor")]
    [SerializeField] private GameObject FloorPanel;

    [SerializeField] private Sprite[] floorTopImg = new Sprite[3];
    [SerializeField] private Sprite[] floorImg = new Sprite[3];
    [SerializeField] private Button[] floorBtns = new Button[11];

    [Header("Reward")]
    [SerializeField] private Button[] rewardBtns = new Button[5];
    [SerializeField] private GameObject[] rewardTakedImg = new GameObject[5];

    private string stageName = "";
    private int floor = 0;
    private bool[] floorB;

    private AudioSource audio;

    void Start()
    {
        audio = GetComponent<AudioSource>();
        PlayerPrefs.SetInt("ClearStage1", 1);

        //진행하고있는 스테이지 확인
        for (int i = 0; i < 5; i++)
        {
            if (PlayerPrefs.HasKey("ClearStage" + (i + 1).ToString()))
            {
                worldMapBtns[i].interactable = true;
                worldMapTowerImg[i].SetActive(true);
            }
            else
            {
                worldMapBtns[i].interactable = false;
                worldMapTowerImg[i].SetActive(false);
            }
        }

        //클릭할 영역의 최소 알파값 설정으로 빈공간 터치 X.
        for (int i = 0; i<worldMapBtns.Length; i++)
        {
            worldMapBtns[i].GetComponent<Image>().alphaHitTestMinimumThreshold = 0.1f;
        }
        for(int i = 0; i<rewardBtns.Length; i++)
        {
            rewardBtns[i].GetComponent<Image>().alphaHitTestMinimumThreshold = 0.1f;
            rewardBtns[i].interactable = false;
        }
    }

    //웰드맵 클릭 on, off 이벤트
    public void ImgOn(int n)
    {
        audio.Play();
        if (worldMapBtns[n].interactable) worldMapTowerImg[n].SetActive(false);
        
    }
    public void ImgOff(int n)
    {
        if (!worldMapBtns[n].interactable) worldMapTowerImg[n].SetActive(false);
        else worldMapTowerImg[n].SetActive(true);
    }

    //월드맵 클릭 후 이벤트
    public void ClickWorldMap(string stageName)
    {
        audio.Play();

        GameManager.stageName = stageName;
        this.stageName = stageName;

        floorB = new bool[11];

        PlayerPrefs.SetInt(stageName + "ClearFloor" + (0).ToString(), 1);
        FloorPanel.SetActive(true);


        //Stage 클리어 전적 확인
        if (PlayerPrefs.HasKey(stageName+"Floor"))
        {
            floor = PlayerPrefs.GetInt(stageName + "Floor");
            GameManager.floor = floor;
            Debug.Log(floor);

            //Floor 버튼 켜기
            for (int i = 0; i <= floor; i++)
            {
                //탑 버튼 켜기
                if (i == 10)
                {
                    floorBtns[i].GetComponent<Image>().sprite = floorTopImg[1];
                }
                //아래 버튼
                else
                {
                    floorBtns[i].GetComponent<Image>().sprite = floorImg[1];
                }
            }

            //리워트 게이지 체크
            rewardGauge();
        }
    }
    //월드맵 언 클릭
    public void UnClickWorldMap()
    {
        audio.Play();
        FloorPanel.SetActive(false);
    }

    //리워드 게이지 확인 및 리워드 전적 체크
    void rewardGauge()
    {
        //게이지 체우기 클리어 floor * 0.1f
        GameObject.Find("TowerReward").transform.GetChild(0).GetChild(0).GetComponent<Image>().fillAmount = floor * 0.1f;

        //버튼 on
        foreach(Button b in rewardBtns)
        {
            b.interactable = true;
        }

        //리워드 전적 확인
        for(int i = 0; i < floor; i++)
        {
            Debug.Log(PlayerPrefs.HasKey(stageName + "Gauge" + (i + 1).ToString()));
            //해당 리워드 키를 가지고 있으면 버튼 off
            if (PlayerPrefs.HasKey(stageName + "Gauge" + (i+1).ToString()))
            {
                if(PlayerPrefs.GetInt(stageName + "Gauge" + (i+1).ToString())==1)
                {
                    rewardBtns[i].interactable = false;
                    rewardTakedImg[i].SetActive(true);
                }
            }
        }
    }
    public void GetReward(int num)
    {
        audio.Play();

        //해당 리워드보다 클리수 층수가 낮으면 리턴
        if (floor < num*2) return;

        //각 리워드 보상 내용 더하기
        switch(num)
        {
            case 1 : 
                GameManager.hellWeaponBox = GameManager.hellWeaponBox + 3;
                break;
            case 2 :
                GameManager.coinSkull = GameManager.coinSkull + 3000;
                break;
            case 3 :
                GameManager.deepHellWeaponBox = GameManager.deepHellWeaponBox + 1;
                break;
            case 4 :
                GameManager.coinGoldSkull = GameManager.coinGoldSkull + 100;
                break;
            case 5 :
                GameManager.deepHellShieldBox = GameManager.deepHellShieldBox + 1;
                break;
            default :
                break;
        }
        
        //리워드 버튼 잠그기
        rewardBtns[num-1].interactable = false;
        rewardTakedImg[num - 1].SetActive(true);

        //리워드 데이터 셋
        PlayerPrefs.SetInt(stageName + "Gauge" + num.ToString(), 1);
        PlayerPrefs.Save();

        //데이터 저장
        GameManager.instance.dataSave();
    }

    //층수 클릭 on, off시 sprite 변경
    public void eventDown(int f)
    {
        //버튼 조작시 넘어본 정수형 번호로 층수를 판별, 해당 층수가 클리어 되어 1으로 저장되있거나, 첫번째 0층 일경우
        if (PlayerPrefs.GetInt(stageName + "ClearFloor" + (f).ToString()) == 1 || f==0)
        {
            //마지막 층수는 다른 image
            if (f == 10) floorBtns[f].GetComponent<Image>().sprite = floorTopImg[2];
            else floorBtns[f].GetComponent<Image>().sprite = floorImg[2];
            
            //해당 버튼 누를시 이동 가능한지 판별하는 bool 값
            floorB[f] = true;
        }
    }
    public void eventExit(int f)
    {
        //버튼 조작시 넘어본 정수형 번호로 층수를 판별, 해당 층수가 클리어 되어 1으로 저장되있거나, 첫번째 0층 일경우
        if (PlayerPrefs.GetInt(stageName + "ClearFloor" + (f).ToString()) == 1 || f == 0)
        {
            //마지막 층수는 다른 image
            if (f == 10) floorBtns[f].GetComponent<Image>().sprite = floorTopImg[1];
            else floorBtns[f].GetComponent<Image>().sprite = floorImg[1];
        }
    }

    //게임 맵으로 이동 버튼 누를시
    public void ClickFloor(int f)
    {
        audio.Play();

        Debug.Log($"GameManager.floor : {GameManager.floor}");
        Debug.Log($"stageName : {stageName}");
        
        //진행 라운드 Text 저장
        FloorText.NowFloor = f * 10;

        //열린 층 수 일때만 이동
        if (floorB[f])
        {
            Destroy(GameObject.Find("ManagerCanvas"));
            SceneManager.LoadScene(stageName);
        }
    }

    //뒤로가기 버튼 누를 시
    public void BackScene()
    {
        audio.Play();
        Destroy(GameObject.Find("ManagerCanvas"));
        Destroy(GameObject.Find("GameManager"));
        SceneManager.LoadScene("StartScene");
    }
}
