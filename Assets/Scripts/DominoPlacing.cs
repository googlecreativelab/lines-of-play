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

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DominoPlacing : MonoBehaviour
{
    public Vector2 ScreenPosition = new Vector2(0.5f, 0.5f);
    public GameObject reticle;
    public GameObject prefab;
    public int colorID;
    public Text txt;
    public UndoRedoManager _undoRedoManager;
    public Material dominoMat;
    public Transform target;
    public int count = 0;
    TwoPointSpawner pointSpwaner;
    public MainController mainController;
    public static event Action onPlacedObject;
    private List<Domino> holdDominos = new List<Domino>();
    public GameObject swipe_panel;
    public GameObject btn;
    // Start is called before the first frame update
    void Start()
    {
        _undoRedoManager = FindObjectOfType<UndoRedoManager>();
        pointSpwaner = FindObjectOfType<TwoPointSpawner>();
        target = Camera.main.transform;
        if (mainController == null) 
        {
            mainController = FindObjectOfType<MainController>();
        }
    }

    private bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }


    void Update()
    {
        var center = new Vector2(Screen.width * ScreenPosition.x, Screen.height * ScreenPosition.y);
        Ray mRay = Camera.main.ScreenPointToRay(center);
        RaycastHit hit;
        if (Physics.Raycast(mRay, out hit))
        {

            if (hit.transform.CompareTag("DetectedPlane") /*|| hit.transform.CompareTag("Domino")*/)
            {

                reticle.transform.position = hit.point;

                Vector3 targetPostition = new Vector3(target.position.x,
                               this.transform.position.y,
                               target.position.z);
                this.transform.LookAt(targetPostition);

                if (RectTransformUtility.RectangleContainsScreenPoint(btn.GetComponent<RectTransform>(), Input.mousePosition))
                {
                    return;
                }      
                if (RectTransformUtility.RectangleContainsScreenPoint(swipe_panel.GetComponent<RectTransform>(), Input.mousePosition))
                {
                    return;
                }

                if ((Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended) || Input.GetMouseButtonUp(0))
                {
                    if (IsPointerOverUIObject())
                    {
                        return;
                    }

                    if (pointSpwaner.isDefault)
                    {

                        onPlacedObject?.Invoke();

                        holdDominos.Clear();
                        GameObject dominoSpawned = Instantiate(prefab, reticle.transform.position, reticle.transform.rotation);
                        dominoSpawned.GetComponent<SwitchOnRandomDomino>().colorID = colorID;
                        mainController.AddDomino(dominoSpawned);

                        Domino domino = new Domino();
                        domino._dominoObj = dominoSpawned;
                        domino._dominoPosition = dominoSpawned.transform.position;
                        domino._dominoRotation = dominoSpawned.transform.rotation;
                        domino._dominoScale = dominoSpawned.transform.localScale;
                        holdDominos.Add(domino);

                        _undoRedoManager.LoadData(TransactionData.States.spawned, holdDominos);
                    }

                }


            }

        }

    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.CompareTag("DetectedPlane"))
    //    {
    //        dominoMat.color = Color.green;
    //    }
    //    else
    //    {
    //        dominoMat.color = Color.white;
    //    }
    //}
}
