/*
Copyright 2020 Google LLC

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    https://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

using GoogleARCore.Examples.HelloAR;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MainController : MonoBehaviour
{

    public GameObject reticle;
    public DeleteDomino deleteDomino;
    public TaptoForce taptoForce;
    public Text text;
    public GameObject remove_info;
    public GameObject coloringPanel;
    public GameObject placementBtn;
    public GameObject coloringBtn;
    public GameObject ButtonTopple;
    public Sprite[] colors;
    public Image colorIndicator;
    public DominoARController dominoARController;
    public GameObject balloonReticle;
    public Paintable paintable;
    public GameObject mainBtn;
    public GameObject singleDominoBtn;
    public Sprite singleDominoBtnSmall;
    public Sprite singleDominoBtnLarge;
    public List<GameObject> dominos;
    public UIManager uiManager;
    static int count = 0;
    public bool isPlacement = false;
    public bool isPlacementBig = false;
    [Header("Types of Dominos")]
    public GameObject dominoSmall;
    public GameObject dominoBig;


    //public PlaneDiscoveryGuide discoveryGuide;
    public GameObject mainCanvas;
    public MainUI mainUI;
    public ScrollViewSnap scrollViewSnap;
    public UndoRedoManager _undoRedoManager;
    [Range(0, 255)]
    float dim = 100;
    Color dimWhite;
    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 60;

        PlayerPrefs.DeleteKey("ColorID");
        //PlayerPrefs.DeleteAll();
        if (mainUI == null)
        {
            mainUI = FindObjectOfType<MainUI>();
        }
        dim = 150;
        dimWhite = new Color(1f, 1f, 1f, dim / 255);
        //mainUI.ButtonSingleDomino.GetComponent<Image>().color = Color.white;
        balloonReticle.SetActive(false);
        //Placement();
        //StartCoroutine(setPlanesGuideUi());
        ButtonTopple.GetComponent<Image>().color = dimWhite;
        _undoRedoManager = FindObjectOfType<UndoRedoManager>();
    }

    IEnumerator setPlanesGuideUi()
    {
        yield return new WaitForSeconds(2f);
        Placement();
    }
    private bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }

    private void Update()
    {
        if (IsPointerOverUIObject())
        {
            return;
        }
    }

    public void Placement()
    {
        if (reticle.activeSelf && !isPlacementBig)
        {
            reticle.SetActive(false);
            isPlacement = false;
            mainUI.ChangeButtonBG(mainUI.ButtonSingleDomino, ButtonBGColor.Clear);
        }
        else
        {
            reticle.SetActive(true);
            reticle.GetComponent<DominoPlacing>().prefab = dominoSmall;
            deleteDomino.enabled = false;
            taptoForce.enabled = false;
            isPlacement = true;

            mainUI.ChangeButtonBG(mainUI.ButtonSingleDomino, ButtonBGColor.White);
            balloonReticle.SetActive(false);
            GetComponent<Paintable>().enabled = false;
        }

    }


    public void Placement_Big()
    {
        if (reticle.activeSelf &&!isPlacement)
        {
            reticle.SetActive(false);
            isPlacementBig = false;
            mainUI.ChangeButtonBG(mainUI.ButtonJumboDomino, ButtonBGColor.Clear);
        }
        else
        {
            reticle.SetActive(true);
            reticle.GetComponent<DominoPlacing>().prefab = dominoBig;
            deleteDomino.enabled = false;
            taptoForce.enabled = false;
            isPlacementBig = true;

            mainUI.ChangeButtonBG(mainUI.ButtonJumboDomino, ButtonBGColor.White);

            balloonReticle.SetActive(false);
            GetComponent<Paintable>().enabled = false;
        }

    }
    public void Placement_Big(bool status)
    {
        if (!status)
        {
            reticle.SetActive(false);
            isPlacementBig = false;
            mainUI.ChangeButtonBG(mainUI.ButtonJumboDomino, ButtonBGColor.Clear);
        }
        else
        {
            reticle.SetActive(true);
            reticle.GetComponent<DominoPlacing>().prefab = dominoBig;
            deleteDomino.enabled = false;
            taptoForce.enabled = false;
            isPlacementBig = true;
            isPlacement = false;

            mainUI.ChangeButtonBG(mainUI.ButtonJumboDomino, ButtonBGColor.White);

            balloonReticle.SetActive(false);
            GetComponent<Paintable>().enabled = false;
        }

    }


    public void Placement(bool status)
    {
        if (!status)
        {
            reticle.SetActive(false);
            mainUI.ChangeButtonBG(mainUI.ButtonSingleDomino, ButtonBGColor.Clear);
        }
        else
        {
            reticle.SetActive(true);
            reticle.GetComponent<DominoPlacing>().prefab = dominoSmall;
            deleteDomino.enabled = false;
            taptoForce.enabled = false;
            isPlacement = true;
            isPlacementBig = false;

            mainUI.ChangeButtonBG(mainUI.ButtonSingleDomino, ButtonBGColor.White);
            balloonReticle.SetActive(false);
            GetComponent<Paintable>().enabled = false;
        }

    }

    public void switchOffColoringPanel()
    {

        if (coloringPanel.activeSelf)
        {
            uiManager.toggleColorPicker(colors[0]);
        }
    }

    public void Coloring()
    {

        if (coloringPanel.activeSelf)
        {
            coloringPanel.SetActive(false);
            mainUI.ButtonErase.SetActive(true);
            ButtonTopple.SetActive(true);
            coloringBtn.SetActive(true);
            uiManager.stateColorPicker = false;
        }
        else
        {
            coloringPanel.SetActive(true);
            deleteDomino.enabled = false;
            taptoForce.enabled = false;
            reticle.SetActive(false);

        }
    }

    public void ChangeColor(int colorID)
    {
        reticle.GetComponent<DominoPlacing>().colorID = colorID;
        colorIndicator.sprite = colors[colorID];
        PlayerPrefs.SetInt("ColorID", colorID);
        //Coloring();
    }

    int clickCounter = 0;
    public void Erasing()
    {
        //if (deleteDomino.isActiveAndEnabled)
        //{
        //    deleteDomino.enabled = false;

        //    mainUI.ChangeButtonBG(mainUI.ButtonErase, ButtonBGColor.Clear);
        //}
        //else
        //{
            deleteDomino.enabled = true;
            reticle.SetActive(false);
        //ButtonTopple.SetActive(false);
        //coloringBtn.SetActive(false);

            mainUI.ChangeButtonBG(mainUI.ButtonErase, ButtonBGColor.White);
            if (coloringPanel.activeSelf)
            {
                uiManager.toggleColorPicker(colors[0]);
            }

            //dominoARController.enabled = false;
            //if (PlayerPrefs.GetInt("RemoveInfo") == 1)
            //{
            //    PlayerPrefs.SetInt("RemoveInfo", 69);
              
            //}

        if (clickCounter == 1)
        {
            StartCoroutine(SwitchOffRemoveInfo());
        }

        clickCounter++;
        //}
    }
    IEnumerator SwitchOffRemoveInfo()
    {
        yield return new WaitForSeconds(0.75f);
        remove_info.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        remove_info.SetActive(false);
    }

    public void EraseLongPress()
    {
        StartCoroutine(CleanRoutine());
    }
    IEnumerator CleanRoutine()
    {
        yield return new WaitForEndOfFrame();
        int i = dominos.Count - 1;
        while (i >= 0)
        {
            Destroy(dominos[i]);
            dominos.RemoveAt(i);
            i--;
        }
        _undoRedoManager.undoBtn.GetComponent<Button>().interactable = false;
        _undoRedoManager.redoBtn.GetComponent<Button>().interactable = false;
    }
    public void AddDomino(GameObject domino)
    {
        dominos.Add(domino);
        domino.GetComponent<Domino>().id = count; 
        count++;
    }

    public void Topple()
    {
        if (taptoForce.isActiveAndEnabled)
        {
            taptoForce.enabled = false;
            ButtonTopple.GetComponent<Image>().color = dimWhite;
            //scrollViewSnap.MoveButton(scrollViewSnap.index);
            //StartCoroutine(scrollViewSnap.MoveToButtonOnStart(scrollViewSnap.index));
        }
        else
        {
            taptoForce.enabled = true;
            deleteDomino.enabled = false;
            reticle.SetActive(false);

            ButtonTopple.GetComponent<Image>().color = Color.white;

            dominoARController.enabled = false;
            GetComponent<Paintable>().enabled = false;
            balloonReticle.SetActive(false);
            dominoARController.GetComponent<TwoPointSpawner>().RemoveBalloonReticles();
        }
    }


    public void ToppleOff()
    {
        taptoForce.enabled = false;
        ButtonTopple.GetComponent<Image>().color = dimWhite;
    }

    public void TogglePainting()
    {
        //if (paintable.isActiveAndEnabled)
        //{
        //    paintable.enabled = false;
        //    mainUI.ChangeButtonBG(mainUI.ButtonDrawPath, ButtonBGColor.Clear);
        //}
        //else
        //{
            paintable.enabled = true;
            mainUI.ChangeButtonBG(mainUI.ButtonDrawPath, ButtonBGColor.White);
            balloonReticle.SetActive(false);
        //}
    }

    public void SwitchEverythingOff()
    {
        reticle.SetActive(false);
        deleteDomino.enabled = false;
        taptoForce.enabled = false;

        paintable.enabled = false;
    }

}
