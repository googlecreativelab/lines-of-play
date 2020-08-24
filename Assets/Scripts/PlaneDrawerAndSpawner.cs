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

public class PlaneDrawerAndSpawner : MonoBehaviour
{
    public GameObject trailPrefab;
    private GameObject thisTrail;
    private Vector3 startPos;
    private Plane objPlane;
    // Start is called before the first frame update
    void Start()
    {
        //objPlane = new Plane(Camera.main.transform.forward * -1, this.transform.position);
    }

    void Update()
    {

        if ((Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) || Input.GetMouseButtonDown(0))
        {
            Ray mRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            //thisTrail = (GameObject)Instantiate(trailPrefab, this.transform.position, Quaternion.identity);
            RaycastHit hit;
            float rayDistance =100;
            if (Physics.Raycast(mRay, out hit, rayDistance, 0))
            {
                if (hit.transform.CompareTag("DetectedPlane"))
                {
                    thisTrail = (GameObject)Instantiate(trailPrefab, hit.point + Vector3.up * 0.1f, Quaternion.identity);
                    startPos = mRay.GetPoint(rayDistance);
                }
            }
        }
        else if (((Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved) || Input.GetMouseButton(0)))
        {

            Ray mRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            //thisTrail = (GameObject)Instantiate(trailPrefab, this.transform.position, Quaternion.identity);
            RaycastHit hit;
            float rayDistance = 100;
            if (Physics.Raycast(mRay, out hit, rayDistance, 0))
            {
                if (hit.transform.CompareTag("DetectedPlane"))
                {
                    thisTrail.transform.position = mRay.GetPoint(rayDistance);
                }
            }
        }
        else if ((Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended) || Input.GetMouseButtonUp(0))
        {
            if (Vector3.Distance(thisTrail.transform.position, startPos) < 0.1)
            {
                Destroy(thisTrail);
            }
        }
    }
}
