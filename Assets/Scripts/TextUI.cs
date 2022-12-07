using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
//タイトルのテキストに入ってる
//テキストを点滅させたり
//シーン移動するスクリプト
public class TextUI : MonoBehaviour
{
    public float speed = 1.0f;//点滅する速度
    private Text text;//テキスト本体
    private float time;//点滅する感覚の時間
    [Header("移動したいシーンの名前")]
    public string sceneName = "";
    private void Start()
    {
        //textに入ってるゲームオブジェクトからテキストコンポーネントを取得
        text = this.gameObject.GetComponent<Text>();
    }
    private void Update()
    {
        //テキストのaの値を増減させて、点滅させている
        text.color = Getalphacoler(text.color);
        if (Input.GetKeyDown(KeyCode.Space))//スペースを押すと
        {
            ChangeScene();//これが読み込まれてシーンを移動したりする
        }
    }
    public void ChangeScene()//シーンを変えるメソッド
    {
        try
        {
            if (sceneName == "")//何も入力されていなければ、プレイ終了
            {
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;//ゲームプレイ終了
#else
    Application.Quit();//ゲームプレイ終了
#endif
            }
            else { SceneManager.LoadSceneAsync(sceneName); }//入っているシーン名に飛ぶ


        }
        catch (System.IndexOutOfRangeException)
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;//ゲームプレイ終了
#else
    Application.Quit();//ゲームプレイ終了
#endif
        }



    }
    Color Getalphacoler(Color color)//テキストのα値をいじるクラス
    {
        time += Time.deltaTime * speed;
        color.a = Mathf.Sin(time);
        return color;

    }
}

