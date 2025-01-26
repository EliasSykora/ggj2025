using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Intro : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Invoke("StartGame", 3f);
    }

    // Update is called once per frame
    void StartGame()
    {
        SceneManager.LoadScene(1);
    }
}
