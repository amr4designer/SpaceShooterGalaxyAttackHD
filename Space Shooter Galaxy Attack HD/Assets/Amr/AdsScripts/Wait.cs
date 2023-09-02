using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Wait : MonoBehaviour
{
    public float wait_time = 15f;
    void Start()
    {
        StartCoroutine(Wait_For_Intro());
    }
    IEnumerator Wait_For_Intro()
    {
        yield return new WaitForSeconds(wait_time);
        SceneManager.LoadScene(1);

    }
}