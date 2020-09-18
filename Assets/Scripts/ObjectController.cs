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

public class ObjectController : MonoBehaviour
{

    public GameObject Plane;
    public GameObject Domino;
    public Camera MainCamera;
    public float force = 1f;
    Ray ray;
    RaycastHit hit;
    // Start is called before the first frame update
    void Start()
    {
        //SetDominoObject();
        MainCamera = Camera.main;
    }
    private void Update()
    {
        ray = MainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit))
        {

            if (Input.GetMouseButtonDown(0))
            {
                if (hit.transform.gameObject.CompareTag("Plane"))
                {
                    Instantiate(Domino, hit.point, Quaternion.identity);

                }
                else if(hit.transform.gameObject.CompareTag("Domino"))
                {
                  
                    GetComponent<ObjectTracker>().deleteID = hit.transform.gameObject.transform.parent.GetComponent<Domino>().id;
             
                }
               
            }
        }
    }

    public void SetPlaneObject()
    {
        //GetComponent<PawnManipulator>().PawnPrefab = Plane;
    }
    public void SetDominoObject()
    {
       // GetComponent<PawnManipulator>().enabled = false;
        //GetComponent<PawnManipulator>().PawnPrefab = Domino;
    }

}
