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
using UnityEngine.UI;

public class UndoRedoManager : MonoBehaviour
{
    //for deleting neeed   <Enum.spawned, gameobject, vector3 postion>
    public TransactionData UndoStack;
    public TransactionData redoStack;
    public GameObject undoBtn;
    public GameObject redoBtn;


    void Start()
    {
        undoBtn.GetComponent<Button>().interactable = false;
        redoBtn.GetComponent<Button>().interactable = false;
    }


    public void SwitchOffOtherFunctionality()
    {
        //GetComponent<MainController>().reticle.SetActive(true);
        //GetComponent<MainController>().deleteDomino.enabled = true;
        //GetComponent<MainController>().Placement();
        //GetComponent<MainController>().Erasing();
    }

    public void Undo()
    {
        SwitchOffOtherFunctionality();
        if (UndoStack.state == TransactionData.States.spawned)
        {
            //delete
            print("delete-Undo");
            foreach (var item in UndoStack._dominos)
            {
                item._dominoObj.SetActive(false);
            }
            //UndoStack._object.SetActive(false);
            redoStack = UndoStack;
            redoStack.state = TransactionData.States.deleted;
        }
        else if (UndoStack.state == TransactionData.States.deleted)
        {
            //Spawn
            print("Spwan-Undo");
            foreach (var item in UndoStack._dominos)
            {
                item._dominoObj.SetActive(true);
            }
            //UndoStack._object.SetActive(true);
            redoStack = UndoStack;
            redoStack.state = TransactionData.States.spawned;
        }

        undoBtn.GetComponent<Button>().interactable = false;
        redoBtn.GetComponent<Button>().interactable = true;
    }

    public void Redo()
    {
        SwitchOffOtherFunctionality();
        if (redoStack.state == TransactionData.States.deleted)
        {
            foreach (var item in UndoStack._dominos)
            {
                item._dominoObj.SetActive(true);
            }

            //redoStack._object.SetActive(true);
            UndoStack = redoStack;
            UndoStack.state = TransactionData.States.spawned;
        }

        else if (redoStack.state == TransactionData.States.spawned)
        {

            foreach (var item in UndoStack._dominos)
            {
                item._dominoObj.SetActive(false);
            }
            //Spawn
            //redoStack._object.SetActive(false);
            UndoStack = redoStack;
            UndoStack.state = TransactionData.States.deleted;
        }
        undoBtn.GetComponent<Button>().interactable = true;
        redoBtn.GetComponent<Button>().interactable = false;
    }

    public void DeleteItem()
    {
        foreach (var item in UndoStack._dominos)
        {
            item._dominoObj.SetActive(false);
        }

        //delete
        //UndoStack._object.SetActive(false);
        redoStack = UndoStack;
        redoStack.state = TransactionData.States.deleted;
    }

    public void PlaceItem()
    {
        foreach (var item in UndoStack._dominos)
        {
            item._dominoObj.SetActive(true);
        }
        //UndoStack._object.SetActive(true);
        redoStack = UndoStack;
        redoStack.state = TransactionData.States.spawned;
    }

    //public void LoadData(TransactionData.States state, GameObject gameObject, Vector3 position, Quaternion rotation, Vector3 scale)
    //{
    //    UndoStack.state = state;
    //    UndoStack._object = gameObject;
    //    UndoStack.objectPosition = position;
    //    UndoStack.objectRotation = rotation;
    //    UndoStack.objectScale = scale;

    //    undoBtn.GetComponent<Button>().interactable = true;
    //}
    public void LoadData(TransactionData.States state, List<Domino> dominos)
    {
        UndoStack.state = state;
        UndoStack._dominos = dominos;
        //UndoStack.objectPosition = position;
        //UndoStack.objectRotation = rotation;
        //UndoStack.objectScale = scale;

        undoBtn.GetComponent<Button>().interactable = true;
    }
}
