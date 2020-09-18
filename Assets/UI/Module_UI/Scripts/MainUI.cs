using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum Gradient { Red, Orange, Yellow, Green, Turqoaise, Blue };
public enum Tool { Remove, JumboDomino, Domino, Line, Circle, Rectangle, Draw, About };
public enum ButtonBGColor { Clear, White};
public class MainUI : Singleton<MainUI>
{
    public ScrollViewSnap scrollViewSnap;
    public ColorPicker colorPicker;

    //You can fetch the current gradient from any script through MainUI.Instance.currentGradient
    public Gradient currentGradient = Gradient.Blue;
    //Same for the tool
    public Tool currentTool;

    //All these elements will have a Lock Unlock function, which if called, will call respective animations set on them.
    public Button buttonUndo, buttonRedo, buttonRefresh, buttonTopple;

    //Set these values and buttons will lock themselves / grey out
    public int undoCount, redoCount, toppleCount;

    [Header("ScrollMenuButtons")]
    public GameObject ButtonSingleDomino;
    public GameObject ButtonJumboDomino;
    public GameObject ButtonDrawPath;
    public GameObject ButtonLine;
    public GameObject ButtonCircle;
    public GameObject ButtonRectangle;
    public GameObject ButtonErase;
    public GameObject ButtonAbout;
    public GameObject ButtonColorSelection;
    public GameObject ButtonTopple;

    public GameObject remove_info;
    public GameObject[] ScrollMenuButtons;

    private void Start()
    {
        ScrollViewSnap.onIndexChanged += ScrollViewSnap_onIndexChanged;
    }

    //Here is where the tool changes based on index sent by the snapping scrol view.
    private void ScrollViewSnap_onIndexChanged(int index)
    {
        currentTool = (Tool)index;
        //Add your code here to change tool
    }

    private void Update()
    {
        //if (undoCount == 0)
        //    buttonUndo.interactable = false;
        //else if (undoCount > 0)
        //    buttonUndo.interactable = true;

        //if (redoCount == 0 )
        //    buttonRedo.interactable = false;
        //else if (redoCount > 0 )
        //    buttonRedo.interactable = true;

        //if (toppleCount == 0)
        //    buttonTopple.interactable = false;
        //else if (toppleCount > 0)
        //    buttonTopple.interactable = true;
    }


    public void MakeButtonBGTransparent()
    {
        ButtonSingleDomino.GetComponent<Image>().color = Color.clear;
        ButtonJumboDomino.GetComponent<Image>().color = Color.clear;
        ButtonDrawPath.GetComponent<Image>().color = Color.clear;
        ButtonLine.GetComponent<Image>().color = Color.clear;
        ButtonCircle.GetComponent<Image>().color = Color.clear;
        ButtonRectangle.GetComponent<Image>().color = Color.clear;
        ButtonErase.GetComponent<Image>().color = Color.clear;
        ButtonAbout.GetComponent<Image>().color = Color.clear;

        ButtonColorSelection.SetActive(true);
        ButtonTopple.SetActive(true);
    }


    public void ChangeButtonBG(GameObject button, ButtonBGColor buttonBGColor)
    {
        MakeButtonBGTransparent();
        switch (buttonBGColor)
        {
            case ButtonBGColor.Clear:
                button.GetComponent<Image>().color = Color.clear;
                break;
            case ButtonBGColor.White:
                button.GetComponent<Image>().color = Color.white;
                break;
            default:
                break;
        }
    }

    public void AboutBtnBG()
    {
        ChangeButtonBG(ButtonAbout, ButtonBGColor.White);
    }

    public void About_BG_OFF()
    {
        buttonUndo.gameObject.SetActive(false);
        buttonRedo.gameObject.SetActive(false);
        colorPicker.gameObject.SetActive(false);
        buttonTopple.gameObject.SetActive(false);
        buttonRefresh.gameObject.SetActive(false);
    }

    public void About_BG_ON()
    {
        buttonUndo.gameObject.SetActive(true);
        buttonRedo.gameObject.SetActive(true);
        colorPicker.gameObject.SetActive(true);
        buttonTopple.gameObject.SetActive(true);
        buttonRefresh.gameObject.SetActive(true);
    }   

    public void ShowHowtoRemoveInfo()
    {
        if (PlayerPrefs.GetInt("RemoveInfo") == 0)
        {
            PlayerPrefs.SetInt("RemoveInfo", 1);
            StartCoroutine(SwitchOffRemoveInfo());
        }
    }

    IEnumerator SwitchOffRemoveInfo()
    {
        yield return new WaitForSeconds(0.75f);
        remove_info.SetActive(true);
        yield return new WaitForSeconds(3f);
        remove_info.SetActive(false);
    }
}       
        