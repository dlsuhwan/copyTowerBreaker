using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StageBtn : MonoBehaviour
{
    [SerializeField] private GameObject stopPanel;
    
    private AudioSource audio;
     void Start()
    {
        audio = GetComponent<AudioSource>();
    }
    //stop버튼 누를 시
    public void OnStopPanel()
    {
        audio.Play();
        Time.timeScale = 0.0f;
        stopPanel.SetActive(true);
    }
    //계속하기 버튼 누를 시
    public void OffStopPanel()
    {
        audio.Play();
        Time.timeScale = 1f;
        stopPanel.SetActive(false);
    }
    //나가기 버튼 누를 시
    public void gameEixt()
    {
        audio.Play();
        GameManager.instance.dataSave();
        Destroy(GameObject.Find("GameManager"));
        SceneManager.LoadScene("StartScene");
    }
}
