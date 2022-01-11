using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayManager : MonoBehaviour
{
    public static PlayManager instance;                                     

    private Animator animator;

    [SerializeField] private GameObject playerPrefab;
    private GameObject player;
    private bool playerMove;                                                //게임 종료 외에 플레이어 이동을 위해 이용

    private GameObject weaponPoint;
    
    private bool attack;                                                    //코루틴 버튼 연속 공격
    public static bool shield;                                              //EnemyHp.cs 에서 이동간 사용 (방어 스킬 사용 제외하기 위해 )

    public static int skullReward;                                          //EnemySystem.cs에서 적 객체 카운트 받아옴

    [SerializeField] private GameObject shieldRigidbody;                    //기본 방어 rigidbody 

    [SerializeField] private GameObject SkillEffectPrefab;                  //스킬 이펙트
    [SerializeField] private GameObject SkillAttackPrefab;                  //공격 스킬 프리펩
    [SerializeField] private Sprite attackImg;

    [Header("Item")]
    [SerializeField] private Text weaponBox;
    [SerializeField] private Text skull;

    [Header("Btns")]
    [SerializeField] private Button skillDashBtn;
    [SerializeField] private Button skillAttackBtn;
    [SerializeField] private Button justDashBtn;
    [SerializeField] private Button justAttackBtn;
    [SerializeField] private Button justShieldBtn;

    [Header("BtnSprite")]
    [SerializeField] private Sprite[] justDashImg = new Sprite[3];          //대쉬 on, off, 눌림 버튼
    [SerializeField] private Sprite[] justShieldImg = new Sprite[3];        //방어 on, off, 눌림 버튼
    [SerializeField] private Sprite[] justAttackImg = new Sprite[3];        //공격 on, off, 눌림 버튼

    [Header("BtnImg")]
    [SerializeField] private GameObject skillOnDashImgBG;                   //대쉬 스킬 사용 가능 icon
    [SerializeField] private GameObject skillOnAttackImgBG;                 //공격 스킬 사용 가능 icon
    [SerializeField] private Image justDashImgBG;                           //눌린 대쉬 버튼 Img
    [SerializeField] private Image justShieldImgBG;                         //눌린 방어 버튼 Img

    [Header("EndUI")]
    [SerializeField] private GameObject reGameUI;                           //이어하기
    [SerializeField] private GameObject WinUI;                              //Win
    [SerializeField] private GameObject LoseUI;                             //Lose

    private AudioSource audio;
    [SerializeField] private AudioClip btnAudio;
    [SerializeField] private AudioClip knifeAudio;

    void Awake()
    {
        //인스턴스 자기 자신으로 선언
        instance = this;
    }
    void Start()
    {
        //플레이어 생성, 
        player = Instantiate(playerPrefab, transform.position, Quaternion.identity);

        //weaponPoint 설정 후 객체 OFF, 공격 할 때만 사용하기 위해
        weaponPoint = player.transform.GetChild(1).GetChild(3).GetChild(0).GetChild(0).gameObject;
        weaponPoint.SetActive(false);

        //에니메이터 컴포넌트
        animator = player.GetComponent<Animator>();

        //오디오 소스 컴포넌트
        audio = GetComponent<AudioSource>();

        //코인 Text 화
        skull.text = GameManager.coinSkull.ToString();

        //플레이어 등장 코루틴
        StartCoroutine("playerStart");
    }
    void Update()
    {
        //플레이어 포지션 일정 범위 이상 넘어 가지 않도록 함
        if (playerMove)
        {
            if (player.transform.position.x >= 1.7f)
            {
                player.transform.position = new Vector3(1.7f, player.transform.position.y, 0);
            }
        }   

        //게임 끝나면 UI 보여주기
        //게임 지면
        if (GameManager.dieFloor)
        {
            //논리형 변수 dieFloor 거짓으로
            GameManager.dieFloor = false;

            //LoseUI 객체 On
            LoseUI.SetActive(true);
        }
        //게임 이기고 층수가 10으로 나누었을때 0이면
        if (GameManager.clearFloor && FloorText.NowFloor % 10 == 0)
        {
            //논리형 변수 clearFloor 거짓으로
            GameManager.clearFloor = false;

            //skull 코인 더하기 // 기존 코인 + 해당 라운드 객체 카운트
            GameManager.coinSkull += skullReward;

            //코인 시각화
            skull.text = GameManager.coinSkull.ToString();

            //10층 간격으로 층수 더하기
            GameManager.floor++;

            //WinUI 객체 On
            WinUI.SetActive(true);

            //데이터 저장
            GameManager.instance.dataSave();
        }
        //게임 클리어만 한경우
        else if(GameManager.clearFloor)
        {
            //논리형 변수 clearFloor 거짓으로
            GameManager.clearFloor = false;

            //skull 코인 더하기 // 기존 코인 + 해당 라운드 객체 카운트
            GameManager.coinSkull += skullReward;

            //코인 시각화
            skull.text = GameManager.coinSkull.ToString();

            //Go >> 버튼 생성
            reGameUI.SetActive(true);

            //버튼 종료
            BtnUiOff();
        }

        //적 객체가 카운트가 없으면 버튼 끄기
        if (EnemySystem.objList.Count == 0) BtnUiOff();

        //공격 버튼 누를시 코루틴 실행
        if (attack) StartCoroutine("Attacking");
    }
    
    //시작 후 플레이어 자리 셋팅
    IEnumerator playerStart()
    {
        playerMove = true;

        //플레이어 이동시 HPzone 콜라이더 박스 OFF
        GameObject.FindWithTag("HPzone").GetComponent<BoxCollider2D>().enabled = false;
        
        //애니메이션 시작
        animator.Play("Hero_move");

        //객체 이동
        while (player.transform.position.x <= -2.3f)
        {
            player.transform.Translate(5f * Time.deltaTime, 0, 0);
            //Debug.Log($"player pos : {player.transform.position}");
            yield return new WaitForFixedUpdate();
        }

        //플레이어 이동 종료 후 HPzone 콜라이더 박스 ON
        GameObject.FindWithTag("HPzone").GetComponent<BoxCollider2D>().enabled = true;

        //현재 진행 중인 층 Text 시각화
        FloorText.instance.texting();
    }
    //종료 후 플레이어 자리 셋팅

    IEnumerator playerEnd()
    {
        playerMove = false;
        
        //이동 애니메이션 실행
        animator.Play("Hero_move");
        
        //화면에서 보이지 않을 때 까지 이동
        while (player.transform.position.x <= 3.5f)
        {
            player.transform.Translate(5f * Time.deltaTime, 0, 0);
            //Debug.Log($"player pos : {player.transform.position}");
            yield return new WaitForFixedUpdate();
        }
        //첫 생성자리로 이동
        player.transform.position = transform.position;
        //시작 화면으로 이동
        StartCoroutine("playerStart");
    }
    
    //게임 이어하기
    public void regame()
    {
        audio.clip = btnAudio;
        audio.Play();

        //계속하기 누를 시 WinUI객체 Off
        if (WinUI.activeSelf) WinUI.SetActive(false);
        
        //코루틴 실행
        StartCoroutine("playerEnd");
        
        //새로운 적 객체 실행
        EnemySystem.instance.CreatetimeWait();
    }

    //시작 1초후 버튼 on
    public void BtnUiOn()
    {
        justDashBtn.GetComponent<Image>().sprite = justDashImg[0];
        justShieldBtn.GetComponent<Image>().sprite = justShieldImg[0];
        justAttackBtn.GetComponent<Image>().sprite = justAttackImg[0];
    }
    //게임 종료시 버튼 이미지 꺼짐
    public void BtnUiOff()
    {
        justDashBtn.GetComponent<Image>().sprite = justDashImg[2];
        justShieldBtn.GetComponent<Image>().sprite = justShieldImg[2];
        justAttackBtn.GetComponent<Image>().sprite = justAttackImg[2];
    }

    //떠나기 버튼 누를 시 데이터 저장 후 게임 오브젝트 삭제하고 나가기.
    //첫 화면이 이미 GameManager가 있기 떄문
    public void BackScene()
    {
        audio.clip = btnAudio;
        audio.Play();

        GameManager.instance.dataSave();
        Destroy(GameObject.Find("GameManager"));
        SceneManager.LoadScene("StartScene");
    }

    #region GameSkills
    //스킬 버튼 클릭 on
    //대쉬 스킬 누를 시
    public void SkillDash()
    {
        //대쉬 스킬 fill의 양이 1이 아니면 실행할 수 없음
        if (skillDashBtn.transform.GetChild(0).GetComponent<Image>().fillAmount != 1f) return;
        
        //대쉬 스킬 실행
        else
        {
            //스킬 사용 가능 이미지 OFF
            skillOnDashImgBG.SetActive(false);
            //스킬 사용 했으므로 다시 fill의 양 채우기 위해 0으로 변경
            skillDashBtn.transform.GetChild(0).GetComponent<Image>().fillAmount = 0f;
            //스킬 코루틴 실행
            StartCoroutine("dashSkill");
        }
    }
    //공격 스킬 누를 시
    public void SkillAttack()
    {
        //공격 스킬 fill의 양이 1이 아니면 실행할 수 없음
        if (skillAttackBtn.transform.GetChild(0).GetComponent<Image>().fillAmount != 1f) return;
        
        //공격 스킬 실행
        else
        {
            //스킬 사용 가능 이미지 OFF
            skillOnAttackImgBG.SetActive(false);
            //스킬 사용 했으므로 다시 fill의 양 채우기 위해 0으로 변경
            skillAttackBtn.transform.GetChild(0).GetComponent<Image>().fillAmount = 0.0f;
            //스킬 코루틴 실행
            StartCoroutine("attackSkill");
        }
    }
    //대쉬 스킬 코루틴 
    IEnumerator dashSkill()
    {
        //오디오 재생이 없을 시 오디오 재생
        audio.clip = knifeAudio;
        if (!audio.isPlaying) audio.Play();

        //검은화면에 십자가 모양의 스킬 시전 애니메이션 실행 후 객체 종료
        GameObject effet = Instantiate(SkillEffectPrefab, player.transform.position, Quaternion.identity);
        effet.GetComponent<Animator>().Play("SkillDash");
        Destroy(effet, 0.5f);

        //공격 시 데미지 상승
        EnemyHp.strong = 50;

        //대쉬 애니메이션 실행
        animator.Play("Hero_SkillDash");

        weaponPoint.SetActive(true);
        
        GameObject des = new GameObject();
        Destroy(des, 0.5f);

        //포지션 이동하면서 공격
        while (des!=null)
        {
            audio.clip = knifeAudio;
            audio.Play();
            player.transform.Translate(10f * Time.deltaTime, 0, 0);
            yield return new WaitForFixedUpdate();
        }
        player.transform.position = new Vector3(player.transform.position.x, transform.position.y, 0);

        weaponPoint.SetActive(false);
        //공격 시 데미지 되돌리기
        EnemyHp.strong = 20;
    }
    IEnumerator attackSkill()
    {
        GameObject effet = Instantiate(SkillEffectPrefab, player.transform.position, Quaternion.identity);
        effet.GetComponent<Animator>().Play("SkillDash");
        Destroy(effet, 0.5f);

        EnemyHp.strong = 50;
        GameObject obj = Instantiate(SkillAttackPrefab, weaponPoint.transform.position + new Vector3(-0.5f, 1.5f,0), Quaternion.identity);

        animator.Play("Hero_Attack");
        obj.GetComponent<Animator>().Play("SkillSword");

        while (obj.transform.position.x <=  3f)
        {
            // 오디오 연속 재생
            audio.clip = knifeAudio;
            audio.Play();
            obj.transform.Translate(10f * Time.deltaTime, 0, 0);
            
            yield return new WaitForFixedUpdate();
        }

        EnemyHp.strong = 20;
        Destroy(obj.gameObject);
    }

    //일반 버튼 클릭 on //event Trigger pointDown 사용
    public void BtnEventOn(string btn)
    {
        //적 객체 카운트가 0이면 실행 X
        if (EnemySystem.objList.Count == 0) return;

        //함수 실행시 string 형 변수를 받아 해당 스킬이 무엇인지 판별
        switch (btn)
        {
            case "dash":
                //눌린 이미지를 켜고 해당 fill 의 양이 1이 아니면 이미 실행 되고 있으므로 리턴
                justDashImgBG.gameObject.SetActive(true);
                if (justDashImgBG.GetComponent<Image>().fillAmount != 1) return;

                //이동 애니메이션 실행
                animator.Play("Hero_move");

                //코루틴을 실행해 객체 이동
                StartCoroutine("dashMove");
                
                //쿨타임 실행
                MinusFill("dash");
                break;

            case "shield":
                //눌린 이미지를 켜고 해당 fill 의 양이 1이 아니면 이미 실행 되고 있으므로 리턴
                justShieldImgBG.gameObject.SetActive(true);
                if (justShieldImgBG.GetComponent<Image>().fillAmount != 1) return;

                //방어 애니메이션 실행
                animator.Play("Hero_shield");
                
                //코루틴 실행하여 방어 실행
                StartCoroutine("shieldUse");

                //쿨타임 실행
                MinusFill("shield");
                break;

            case "attack":
                attack = true;

                justAttackBtn.GetComponent<Image>().sprite = justAttackImg[1];
                weaponPoint.SetActive(true);

                break;

            default:
                break;
        }
    }
    //일반 버튼 클릭 off //event Trigger pointExit 사용
    public void BtnEventOff(string btn)
    {
        //클릭 해체시 기본 공격 이미지만 변경 //나머지는 이미 변경 됨
        switch (btn)
        {
            case "dash":
                break;
            case "shield":
                break;
            case "attack":
                attack = false;
                weaponPoint.GetComponent<SpriteRenderer>().sprite = attackImg;
                weaponPoint.SetActive(false);
                justAttackBtn.GetComponent<Image>().sprite = justAttackImg[0];
                break;
            default:
                break;
        }
    }
    //대쉬
    IEnumerator dashMove()
    {
        //적 오브젝트 리스트 카운트가 0이면 스킬 사용할 수 없음.
        if (EnemySystem.objList.Count == 0) yield return null;
        else
        {
            //기본 대쉬를 사용 할때 마다 스킬 대쉬버튼의 fill의양 더하고, 1이되면 스킬 사용 가능 이미지 ON
            skillDashBtn.transform.GetChild(0).GetComponent<Image>().fillAmount = skillDashBtn.transform.GetChild(0).GetComponent<Image>().fillAmount + 0.05f;
            if (skillDashBtn.transform.GetChild(0).GetComponent<Image>().fillAmount == 1f) skillOnDashImgBG.SetActive(true);

            //대쉬를 사용해 이동할 포지션을 얻기위해 가장 앞에 있는 적 객체 포지션가져옴
            GameObject target = EnemySystem.objList[0];
            
            //첫 번째 객체 포지션 x값 -1 보다 작을 때 까지 플레이어 이동
            while ( target != null && player.transform.position.x <= target.transform.position.x - 1f )
            {
                player.transform.Translate(10f * Time.deltaTime, 0, 0);
                //Debug.Log($"player pos : {player.transform.position}");
                yield return new WaitForFixedUpdate();
            }
        }
    }
    //기본 방어
    IEnumerator shieldUse()
    {
        //오디오 재생이 없을 시 오디오 재생
        audio.clip = knifeAudio;
        if (!audio.isPlaying) audio.Play();

        //적 오브젝트 리스트 카운트가 0이면 스킬 사용할 수 없음.
        if (EnemySystem.objList.Count == 0) yield return null;
        else
        {
            //방어를를 사용하기위해 가장 앞에 있는 적 객체 포지션가져옴
            GameObject target = EnemySystem.objList[0].gameObject;

            //플레이어가 적 객체 포지션보다 작으면서 적 객체 포지션 + 1 보다 클 때 방어 사용 가능
            if (player.transform.position.x <= target.transform.position.x && player.transform.position.x > target.transform.position.x - 1f)
            {
                //한대 때리기
                weaponPoint.SetActive(true);
                weaponPoint.GetComponent<SpriteRenderer>().sprite = attackImg;
                Debug.Log("쉴드 활성화");
                //쉴드 활성화 때 shield 값을 ture으로 주어 적객체 이동을 묶음. 1초후 false 변경
                shield = true;
                Invoke("shieldEnd", 0.5f);

                //객체를 밀어내기위해 눈에 보이지 않는 게임 오브젝트 생성 후 rigidbody 컴포넌트 
                GameObject shieldShoot = Instantiate(shieldRigidbody, weaponPoint.transform.position + new Vector3(-0.5f, 0, 0), Quaternion.identity);
                shieldShoot.GetComponent<Rigidbody2D>().mass = 10;

                //1초 후 삭제
                Destroy(shieldShoot, 0.5f);

                //객체가 삭제되지 않을 때 까지 적객체 밀어내기
                while (shieldShoot != null)
                {
                    shieldShoot.GetComponent<Rigidbody2D>().AddForce(new Vector3(1000, 0, 0));
                    
                    yield return new WaitForFixedUpdate();
                }
                if(shieldRigidbody!=null) weaponPoint.SetActive(false);
            }
        }
    }
    void shieldEnd()
    {
        shield = false;
    }

    //버튼 on off 를 통해 update에서 코루틴 실행
    //기본 공격
    IEnumerator Attacking()
    {
        //오디오 재생이 없을 시 오디오 재생
        audio.clip = knifeAudio;
        if (!audio.isPlaying) audio.Play();

        //스킬 공격버튼의 fill의 양의 1이면 스킬 사용 가능 표시 이미지 ON
        if (skillAttackBtn.transform.GetChild(0).GetComponent<Image>().fillAmount == 1f) skillOnAttackImgBG.SetActive(true);

        //플레이어 애니메이션 및 무기 애니메이션 실행
        animator.Play("Hero_Attack");
        weaponPoint.GetComponent<Animator>().Play("Weapon_Cut");

        yield return new WaitForSeconds(0.2f);
    }

    //대쉬, 방어 쿨타임
    private string fillstr;
    void MinusFill(string str)
    {
        fillstr = str;
        StartCoroutine("fill"); //코루틴 실행
    }
    IEnumerator fill()
    {
        //사용한 스킬 이름과 스킬이미지 fill의 양이 1이면 쿨타임 시작
        //대쉬
        if (fillstr == "dash" && justDashImgBG.GetComponent<Image>().fillAmount == 1)
        {
            //스킬 눌린 이미지로 변경
            justDashBtn.GetComponent<Image>().sprite = justDashImg[1];

            //반복문으로  fill의 양이 0보다 작을 때까지 쿨타이 시작
            while (justDashImgBG.GetComponent<Image>().fillAmount > 0f)
            {
                justDashImgBG.GetComponent<Image>().fillAmount = justDashImgBG.GetComponent<Image>().fillAmount - 0.1f;
                yield return new WaitForSeconds(0.05f);
            }

            //쿨타임 끝나면 다시 fill양 1로 돌린후 안 눌린 이미지 변경
            justDashImgBG.GetComponent<Image>().fillAmount = 1.0f;
            justDashImgBG.gameObject.SetActive(false);
            justDashBtn.GetComponent<Image>().sprite = justDashImg[0];

        }
        //사용한 스킬 이름과 스킬이미지 fill의 양이 1이면 쿨타임 시작
        //방어
        else if (fillstr == "shield" && justShieldImgBG.GetComponent<Image>().fillAmount == 1)
        {
            //스킬 눌린 이미지로 변경
            justShieldBtn.GetComponent<Image>().sprite = justShieldImg[1];

            //반복문으로  fill의 양이 0보다 작을 때까지 쿨타이 시작
            while (justShieldImgBG.GetComponent<Image>().fillAmount > 0f)
            {
                justShieldImgBG.GetComponent<Image>().fillAmount = justShieldImgBG.GetComponent<Image>().fillAmount - 0.1f;
                yield return new WaitForSeconds(0.1f);
            }

            //쿨타임 끝나면 다시 fill양 1로 돌린후 안 눌린 이미지 변경
            justShieldImgBG.GetComponent<Image>().fillAmount = 1.0f;
            justShieldImgBG.gameObject.SetActive(false);
            justShieldBtn.GetComponent<Image>().sprite = justShieldImg[0];
        }
    }
    #endregion
}
