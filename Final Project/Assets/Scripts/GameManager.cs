using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager manager;
    public float bottomOfWorldY = 0;
    public float waitToRestartTime = 5;

    // Start is called before the first frame update
    void Start()
    {
        manager = this;
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        StartCoroutine(WaitToRestartLevel());
    }


    IEnumerator WaitToRestartLevel()
    {
        yield return new WaitForSeconds(waitToRestartTime);
    }
}
