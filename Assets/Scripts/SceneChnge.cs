using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChnge : MonoBehaviour
{
    [Header("移動したいシーンの名前")]
    public string sceneName = "";
    
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            ChangeScene();
        }
    }
    public void ChangeScene()
    {
        SceneManager.LoadSceneAsync(sceneName);
    }
}
