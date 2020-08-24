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

public class ObjectTracker : MonoBehaviour
{

    public  List<GameObject> dominos;
    public  List<Vector3> dominoPositions;
    public  List<Quaternion> dominoRotations;

    public Vector3 DominoDefaultPosition;
    public Vector3 DominoDefaultRotation;
    public Vector3 DominoDefaultScale;

    public int deleteID;
    public GameObject detectedPlaneGenerator;
    public bool planeState = true;
    public bool isSelecting= true;
    public GameObject SelectToggleBtn;

    void Start()
    {
        
    }


    public void Reset()
    {
        for (int i = 0; i < dominos.Count; i++)
        {
            dominos[i].transform.position = dominoPositions[i];
            dominos[i].transform.rotation = dominoRotations[i];
            for (int j = 0; j < dominos[i].transform.GetChild(0).transform.childCount; j++)
            {
                print(dominos[i].transform.GetChild(0).transform.childCount);
                if (dominos[i].transform.GetChild(0).transform.GetChild(j).gameObject.activeSelf)
                {
                    print(dominos[i].transform.GetChild(0).transform.GetChild(j).gameObject.name);
                    dominos[i].transform.GetChild(0).transform.GetChild(j).GetComponent<Rigidbody>().isKinematic = true;
                    dominos[i].transform.GetChild(0).transform.GetChild(j).transform.localPosition = DominoDefaultPosition;
                    dominos[i].transform.GetChild(0).transform.GetChild(j).transform.localRotation = Quaternion.Euler(DominoDefaultRotation);
                    dominos[i].transform.GetChild(0).transform.GetChild(j).transform.localScale = DominoDefaultScale;
                    dominos[i].transform.GetChild(0).transform.GetChild(j).GetComponent<Rigidbody>().isKinematic = false;
                }
            }
        }
    }

    public void DeleteDomino()
    {
   

        for (int i = 0; i < dominos.Count; i++)
        {
                if (dominos[i].transform.GetChild(0).gameObject.activeSelf)
                {
                    
                    if (dominos[i].transform.GetChild(0).gameObject.GetComponent<Domino>().id == deleteID)
                    {
                        Destroy(dominos[i]);
                        dominos.RemoveAt(i);
                        dominoPositions.RemoveAt(i);
                        dominoRotations.RemoveAt(i);
                    }
                }
            
        }

    }

    public void PlaneToggle()
    {
        planeState = !planeState;
        print("Child Planes "+detectedPlaneGenerator.transform.childCount + " State: " + planeState);
        for (int i = 0; i < detectedPlaneGenerator.transform.childCount; i++)
        {
            detectedPlaneGenerator.transform.GetChild(i).GetComponent<MeshRenderer>().enabled = planeState;
            print(detectedPlaneGenerator.transform.GetChild(i).GetComponent<MeshRenderer>().enabled);
        }

        //detectedPlaneGenerator.GetComponent<DetectedPlaneGenerator>().ShowPlane = planeState;
    }

    public void ClearAll()
    {
        StartCoroutine(CleanRoutine());
     
    }
    IEnumerator CleanRoutine()
    {
        yield return new WaitForEndOfFrame();
     
        int i = dominos.Count -1;
        while (i >= 0 )
        {
            Destroy(dominos[i]);
            dominos.RemoveAt(i);
            dominoPositions.RemoveAt(i);
            dominoRotations.RemoveAt(i);
            i--;
        }
    }

    public void SelectionToggle()
    {
        isSelecting = !isSelecting;
        //GetComponent<HelloARController>().enabled = isSelecting;
        if (isSelecting)
        {
            SelectToggleBtn.GetComponentInChildren<Text>().text = "Spwan";
        }
        else
        {
            SelectToggleBtn.GetComponentInChildren<Text>().text = "Select";
        }
    }
}
