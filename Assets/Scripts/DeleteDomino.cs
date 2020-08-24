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

public class DeleteDomino : MonoBehaviour
{
    public GameObject Plane;
    public GameObject Domino;
    public Camera MainCamera;
    public float force = 1f;
    Ray ray;
    RaycastHit hit;
    UndoRedoManager _undoRedoManager;

    void Start()
    {
        //SetDominoObject();
        MainCamera = Camera.main;
        _undoRedoManager = FindObjectOfType<UndoRedoManager>();
    }
    private void Update()
    {
        ray = MainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit))
        {

            if (Input.GetMouseButtonDown(0))
            {
                if (hit.transform.gameObject.CompareTag("DetectedPlane"))
                {
                   // Instantiate(Domino, hit.point, Quaternion.identity);
                    //GameObject dominoPrefab =  Instantiate(Domino, hit.point, Quaternion.identity);
                    //dominoPrefab.transform.LookAt(MainCamera.transform);
                    //dominoPrefab.transform.rotation = Quaternion.Euler(0f, dominoPrefab.transform.rotation.y, 0f);
                }
                else if (hit.transform.gameObject.CompareTag("Domino"))
                {
                 
                    var temp = hit.transform.gameObject.transform.parent.gameObject;
                   
                    temp.SetActive(false);
                    _undoRedoManager.LoadData(TransactionData.States.deleted, temp, temp.transform.position, temp.transform.rotation, temp.transform.localScale);
                }

            }
        }
    }


}
