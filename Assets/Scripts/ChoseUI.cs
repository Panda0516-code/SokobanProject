using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChoseUI : MonoBehaviour
{

    [SerializeField]
    GameObject StartUI = null;//スタートテキスト
    [SerializeField]
    GameObject ExitUI = null;//exitのテキスト
    private void Awake()
    {
        ExitUI.GetComponent<TextUI>().enabled = false;//exitのテキストを非表示
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.UpArrow))//上↑を押したとき
        {
            ExitUI.GetComponent<TextUI>().enabled = false; //exitをfalseに
            StartUI.GetComponent<TextUI>().enabled = true;//startをtrueに
            ExitUI.GetComponent<Text>().color = Color.white;//exitを白に
            StartUI.GetComponent<Text>().color = Color.green;//startを緑に
        }
        if (Input.GetKey(KeyCode.DownArrow))//↓を押したとき
        {
            StartUI.GetComponent<TextUI>().enabled = false;//startをfalseに
            ExitUI.GetComponent<TextUI>().enabled = true;//exitをtrueに
            StartUI.GetComponent<Text>().color = Color.white;//startを白に
            ExitUI.GetComponent<Text>().color = Color.green;//exitを緑に
        }
    }
}
