using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChoseUI : MonoBehaviour
{
  private enum TextObj
    {
        Start,//スタートのテキスト
        Exit,//終わりのテキスト
    }
    [SerializeField]
    GameObject StartUI = null;
    [SerializeField]
    GameObject ExitUI = null;
    private void Awake()
    {
        ExitUI.GetComponent<TextUI>().enabled = false;
    }

    private void Update()
    {
        if(Input.GetKey(KeyCode.UpArrow))
        {
            ExitUI.GetComponent<TextUI>().enabled = false; 
            StartUI.GetComponent<TextUI>().enabled = true;
            ExitUI.GetComponent<Text>().color = Color.white ;
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            StartUI.GetComponent<TextUI>().enabled = false;
            ExitUI.GetComponent<TextUI>().enabled = true;
            StartUI.GetComponent<Text>().color = Color.green;
        }
    }
}
