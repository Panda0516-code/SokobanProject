using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TextUI : MonoBehaviour
{
    [SerializeField]
    private float speed = 1.0f;
    private Text text;
    private float time;
    [Header("移動したいシーンの名前")]
    public string sceneName = "";
    private void Start()
    {
        text = this.gameObject.GetComponent<Text>();
    }
    private void Update()
    {
        text.color = Getalphacoler(text.color);
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ChangeScene();
        }
    }
    public void ChangeScene()
    {
        if (sceneName == "")
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;//ゲームプレイ終了
#else
    Application.Quit();//ゲームプレイ終了
#endif
        }
        else { SceneManager.LoadSceneAsync(sceneName); }
        
        
    }
    Color Getalphacoler(Color color)
    {
        time += Time.deltaTime  * speed;
        color.a = Mathf.Sin(time);
        return color;

    }
}

