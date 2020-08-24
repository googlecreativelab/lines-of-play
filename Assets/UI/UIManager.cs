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
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    // REFERENCES
    [Header("References")]
    public Animation main; 
    public Animation colorPicker;

    [Header("Icons")]
    public Image IconMain;
    public Image IconColorPicker;

    [Header("Buttons")]
    public GameObject ButtonMain;
    public GameObject ButtonColorPicker;
    public GameObject ButtonErase;
    public GameObject ButtonMenu;
    public GameObject ButtonRedo;
    public GameObject ButtonUndo;

    [Header("PieButtons")]
    public GameObject ButtonSmartPath;
    public GameObject ButtonDrawPath;
    public GameObject ButtonLine;
    public GameObject ButtonCircle;
    public GameObject ButtonRectangle;
    public GameObject ButtonSingleDomino;
    public GameObject[] pieButtons;
    [Header("Settings")]
    public float duration = 0.5f;

    private bool stateMain = false;
    public bool stateColorPicker = false;
    private int previousBtnIndex = 0;

    // Start is called before the first frame update
    void Start()
    {

    }

    // TOGGLE MAIN BUTTON
    public void toggleMain(Sprite icon)
    {
        if(!stateMain)
        {
            ButtonColorPicker.SetActive(false);
            ButtonErase.SetActive(false);
            //ButtonUndo.SetActive(false);
            //ButtonRedo.SetActive(false);
            main["MainFadeIn"].speed = duration;
            main.Play("MainFadeIn");
            stateMain = true;
        }
        else
        {
            ButtonColorPicker.SetActive(true);
            ButtonErase.SetActive(true);
            //ButtonUndo.SetActive(true);
            //ButtonRedo.SetActive(true);
            main["MainFadeOut"].speed = duration;
            main.Play("MainFadeOut");
            IconMain.sprite = icon;
            stateMain = false;
        }
    }

    // TOGGLE COLOR PICKER
    public void toggleColorPicker(Sprite color)
    {
        if(!stateColorPicker)
        {
            colorPicker["ColorPickerFadeIn"].speed = duration;
            colorPicker.Play("ColorPickerFadeIn");
            stateColorPicker = true;
            ButtonColorPicker.GetComponent<Button>().interactable = false;
            StartCoroutine(disableColorButton());
        }
        else
        {
            colorPicker["ColorPickerFadeOut"].speed = duration;
            colorPicker.Play("ColorPickerFadeOut");
            IconColorPicker.sprite = color;
            stateColorPicker = false;
        }
    }

    IEnumerator disableColorButton()
    {
        yield return new WaitForSeconds(duration);
        ButtonColorPicker.GetComponent<Button>().interactable = true;
    }

    public void PieButtonSelected(int index)
    {
        if (index == 6)
        {
            ButtonSingleDomino.SetActive(false);
        }
        else
        {
            ButtonSingleDomino.transform.parent = pieButtons[index].transform;
            pieButtons[index].GetComponent<Image>().enabled = false;
            pieButtons[index].GetComponent<Button>().enabled = false;
            ButtonSingleDomino.SetActive(true);
            ButtonSingleDomino.transform.position = pieButtons[index].transform.position;
        }

        if (previousBtnIndex != index)
        {
            pieButtons[previousBtnIndex].GetComponent<Image>().enabled = true;
            pieButtons[previousBtnIndex].GetComponent<Button>().enabled = true;
        }
        previousBtnIndex = index;
    }
}
