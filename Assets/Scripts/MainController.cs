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
using UnityEngine.UI;

public class MainController : MonoBehaviour
{

    public GameObject reticle;
    public DeleteDomino deleteDomino;
    public TaptoForce taptoForce;
    public Text text;
    public GameObject coloringPanel;
    public GameObject placementBtn;
    public GameObject coloringBtn;
    public GameObject toppleBtn;
    public GameObject eraseBtn;
    public Sprite[] colors;
    public Image colorIndicator;
    public DominoARController dominoARController;
    public GameObject balloonReticle;
    public GameObject LineBtn;
    public GameObject circleBtn;
    public GameObject squareBtn;
    public GameObject customBtn;
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

    [Range(0, 255)]
    float dim = 100;
    Color dimWhite;
    // Start is called before the first frame update
    void Start()
    {

        PlayerPrefs.DeleteAll();

        dim = 125;
        dimWhite = new Color(1f, 1f, 1f, dim / 255);
        coloringBtn.GetComponent<Image>().color = dimWhite;
        eraseBtn.GetComponent<Image>().color = dimWhite;
        toppleBtn.GetComponent<Image>().color = dimWhite;
        //dominoARController.enabled = false;
        balloonReticle.SetActive(false);
        //Placement();
        StartCoroutine(setPlanesGuideUi());
    }

    IEnumerator setPlanesGuideUi()
    {
        yield return new WaitForSeconds(5f);
        Placement();
        //discoveryGuide.isUIEnabled = false;
        //mainCanvas.SetActive(false);
    }


    public void Placement()
    {

        if (reticle.activeSelf && !isPlacementBig)
        {
            reticle.SetActive(false);
            isPlacement = false;
            placementBtn.GetComponent<Image>().color = dimWhite;
        }
        else
        {
            reticle.SetActive(true);
            reticle.GetComponent<DominoPlacing>().prefab = dominoSmall;
            deleteDomino.enabled = false;
            taptoForce.enabled = false;
            isPlacement = true;

            placementBtn.GetComponent<Image>().color = Color.white;
            coloringBtn.GetComponent<Image>().color = dimWhite;
            eraseBtn.GetComponent<Image>().color = dimWhite;
            toppleBtn.GetComponent<Image>().color = dimWhite;


            //dominoARController.enabled = false;
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
            placementBtn.GetComponent<Image>().color = dimWhite;
        }
        else
        {
            reticle.SetActive(true);
            reticle.GetComponent<DominoPlacing>().prefab = dominoBig;
            deleteDomino.enabled = false;
            taptoForce.enabled = false;
            isPlacementBig = true;

            placementBtn.GetComponent<Image>().color = Color.white;
            coloringBtn.GetComponent<Image>().color = dimWhite;
            eraseBtn.GetComponent<Image>().color = dimWhite;
            toppleBtn.GetComponent<Image>().color = dimWhite;

            //dominoARController.enabled = false;
            balloonReticle.SetActive(false);
            GetComponent<Paintable>().enabled = false;
        }

    }


    public void Placement(bool status)
    {

        if (!status)
        {
            reticle.SetActive(false);
            //singleDominoBtn.GetComponent<Image>().sprite = singleDominoBtnSmall;
            placementBtn.GetComponent<Image>().color = Color.white;
        }
        else
        {
            reticle.SetActive(true);
            deleteDomino.enabled = false;
            taptoForce.enabled = false;

            placementBtn.GetComponent<Image>().color = Color.white;
            coloringBtn.GetComponent<Image>().color = dimWhite;
            eraseBtn.GetComponent<Image>().color = dimWhite;
            toppleBtn.GetComponent<Image>().color = dimWhite;

            //dominoARController.enabled = false;
            balloonReticle.SetActive(false);
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
            //reticle.SetActive(true);
            //placementBtn.SetActive(true);
            eraseBtn.SetActive(true);
            toppleBtn.SetActive(true);
            coloringBtn.SetActive(true);
            uiManager.stateColorPicker = false;

            placementBtn.GetComponent<Image>().color = Color.white;
            coloringBtn.GetComponent<Image>().color = dimWhite;
            eraseBtn.GetComponent<Image>().color = dimWhite;
            toppleBtn.GetComponent<Image>().color = dimWhite;
        }
        else
        {
            coloringPanel.SetActive(true);
            deleteDomino.enabled = false;
            taptoForce.enabled = false;
            reticle.SetActive(false);


            placementBtn.GetComponent<Image>().color = dimWhite;
            coloringBtn.GetComponent<Image>().color = Color.white;
            eraseBtn.GetComponent<Image>().color = dimWhite;
            toppleBtn.GetComponent<Image>().color = dimWhite;


            //placementBtn.SetActive(false);
            //eraseBtn.SetActive(false);
            //coloringBtn.SetActive(false);
            // toppleBtn.SetActive(false);
        }
    }

    public void ChangeColor(int colorID)
    {
        reticle.GetComponent<DominoPlacing>().colorID = colorID;
        colorIndicator.sprite = colors[colorID];
        PlayerPrefs.SetInt("ColorID", colorID);
        Coloring();
    }

    public void Erasing()
    {
        if (deleteDomino.isActiveAndEnabled)
        {
            deleteDomino.enabled = false;

            eraseBtn.GetComponent<Image>().color = dimWhite;
        }
        else
        {
            deleteDomino.enabled = true;
            reticle.SetActive(false);

            placementBtn.GetComponent<Image>().color = dimWhite;
            coloringBtn.GetComponent<Image>().color = dimWhite;
            toppleBtn.GetComponent<Image>().color = dimWhite;
            eraseBtn.GetComponent<Image>().color = Color.white;
            if (coloringPanel.activeSelf)
            {
                uiManager.toggleColorPicker(colors[0]);
            }

            //dominoARController.enabled = false;
            balloonReticle.SetActive(false);
        }
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
            toppleBtn.GetComponent<Image>().color = dimWhite;
        }
        else
        {
            taptoForce.enabled = true;
            deleteDomino.enabled = false;
            reticle.SetActive(false);

            placementBtn.GetComponent<Image>().color = dimWhite;
            coloringBtn.GetComponent<Image>().color = dimWhite;
            eraseBtn.GetComponent<Image>().color = dimWhite;
            toppleBtn.GetComponent<Image>().color = Color.white;


            dominoARController.enabled = false;
            GetComponent<Paintable>().enabled = false;
        }
    }

    public void TogglePainting()
    {
        if (paintable.isActiveAndEnabled)
        {
            paintable.enabled = false;
            customBtn.GetComponent<Image>().color = dimWhite;
        }
        else
        {
            paintable.enabled = true;
            customBtn.GetComponent<Image>().color = Color.white;
            //Placement(false);

            //dominoARController.enabled = false;
            balloonReticle.SetActive(false);
        }
    }

    public void SwitchEverythingOff()
    {
        reticle.SetActive(false);
        deleteDomino.enabled = false;
        taptoForce.enabled = false;

        paintable.enabled = false;
    }
}
