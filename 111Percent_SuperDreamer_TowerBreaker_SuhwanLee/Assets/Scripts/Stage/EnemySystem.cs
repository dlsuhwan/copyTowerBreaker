using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class EnemySystem : MonoBehaviour
{
    public static EnemySystem instance;
    
    //적 객체, state 이미지 list 선언
    public static List<GameObject> objList;
    public static List<GameObject> imgList;

    //적 객체 카운트
    public static int enemyCount;

    //적 객체 종류 2가지
    [SerializeField] private GameObject swordmanPrefab;
    [SerializeField] private GameObject knightPrefab;

    //적 state 이미지 객체
    [SerializeField] private GameObject enemyUiPrefab;

    //적 객체와 state 이미지 생성 포인트
    [SerializeField] private Transform enemyUiSpwanPoint;
    [SerializeField] private Transform enemySpwanPoint;

    //해당 객체
    private GameObject enemy;
    private GameObject enemyUi;

    //객체 HP
    private int knightHp = 70;
    private int swordmanHp = 50;


    void Awake()
    {
        //자기 자신 선언
        instance = this;
    }
    void Start()
    {
        //적 생성
        StartCoroutine("EnemyCreate");
    }

    // Update is called once per frame
    void Update()
    {
        //적 카운트가 0이면 해당 라운드 클리어
        if (enemyCount == 0)
        {
            enemyCount = 1;
            GameManager.clearFloor = true;
        }
    }

    //5초후 객체 생성 코루틴 실행
    public void CreatetimeWait()
    {
        Invoke("create", 3f);
    }
    //객체 생성 코루틴 실행
    void create()
    {
        StartCoroutine("EnemyCreate");
    }

    IEnumerator EnemyCreate()
    {
        //각 층마다 생성할 적 갯수
        enemyCount = Random.Range(15, 20);

        //각 라운드 리워드 지정
        PlayManager.skullReward = enemyCount;

        //현재 가장 앞에 있는 적 위치, state 위치를 저장
        objList = new List<GameObject>();
        imgList = new List<GameObject>();

        //적 객체 생성
        for (int j = 0; j < enemyCount; j++)
        {
            //생성될 때 마다 x값을 0.5f씩 곱하면서 이동 생성
            Vector3 spwanPoint = enemySpwanPoint.position + new Vector3(j * 0.5f, 0, 0);

            //적 객체 생성 5개중 1의 확률로 knight 생성
            if (Random.Range(0, 5) == 4)
            {
                enemy = Instantiate(knightPrefab, spwanPoint, Quaternion.identity);
                enemy.GetComponent<EnemyHp>().hp = knightHp;
            }
            else
            {
                enemy = Instantiate(swordmanPrefab, spwanPoint, Quaternion.identity);
                enemy.GetComponent<EnemyHp>().hp = swordmanHp;
            }
            //생성한 객체 리스트에 더함
            objList.Add(enemy);
        }

        //적 UI 생성
        //첫 생성 위치 Vector 저장
        Vector3 uiPos = enemyUiSpwanPoint.transform.position;

        //반복문을 통해 매번 위치가 다른 객체 생성
        for(int i=0; i<enemyCount; i++)
        {
            //i가 짝수면 +자리 곱
            if (i%2==0)
            {
                if(i==0) enemyUi = Instantiate(enemyUiPrefab, new Vector3(uiPos.x + (i + 10), uiPos.y, uiPos.z), Quaternion.identity);
                enemyUi = Instantiate(enemyUiPrefab, new Vector3(uiPos.x + ((i+1) * 10), uiPos.y, uiPos.z), Quaternion.identity);
            }
            //i가 홀수면 -자리 곱
            else
            {
                enemyUi = Instantiate(enemyUiPrefab, new Vector3(uiPos.x + (i * -10), uiPos.y, uiPos.z), Quaternion.identity);
            }
            //객체가 이미지이므로 Canvas안에 넣기
            enemyUi.transform.SetParent(enemyUiSpwanPoint);

            //리스트에 넣고 포지션별 정렬
            imgList.Add(enemyUi);
            imgList.Sort((x, y) => x.transform.position.x.CompareTo(y.transform.position.x));
            //Debug.Log("uipos: " + enemyUi.GetComponent<RectTransform>().anchoredPosition);
        }

        //객체 모두 생성 후 버튼 ON
        PlayManager.instance.BtnUiOn();
        yield return null;
    }
}
