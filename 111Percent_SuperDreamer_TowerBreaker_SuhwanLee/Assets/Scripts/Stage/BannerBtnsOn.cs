using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BannerBtnsOn : MonoBehaviour
{
    [SerializeField] private GameObject btn;
    void Start()
    {
        Invoke("BtnsOn", 2f);   //2초후 버튼 On
    }
    void BtnsOn()
    {
        btn.SetActive(true);
    }
}
