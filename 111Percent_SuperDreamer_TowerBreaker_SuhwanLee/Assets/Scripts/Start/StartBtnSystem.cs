using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartBtnSystem : MonoBehaviour
{
    //월드맵으로 이동
    public void clickView()
    {
        SceneManager.LoadScene("WorldMap");
    }
}
