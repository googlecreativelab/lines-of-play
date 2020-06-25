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
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BalloonPlacing : MonoBehaviour
{
    public Vector2 ScreenPosition = new Vector2(0.5f, 0.5f);
    public GameObject reticle;
    public GameObject prefab;
    public Transform target;
    public int count = 0;
    public TwoPointSpawner pointSpwaner;

        void Start()
    {
        target = Camera.main.transform;

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
            if (hit.transform.CompareTag("DetectedPlane") || hit.transform.CompareTag("Domino"))
            {
                reticle.transform.position = hit.point;
                Vector3 targetPostition = new Vector3(target.position.x,
                               this.transform.position.y,
                               target.position.z);
                this.transform.LookAt(targetPostition);


                if ((Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended) || Input.GetMouseButtonUp(0))
                {

                    if (IsPointerOverUIObject())
                    {
                        return;
                    }


                    GameObject gameObject = Instantiate(prefab, reticle.transform.position, reticle.transform.rotation);
                    print("PPOS " + gameObject.transform.position);
                    pointSpwaner.points.Add(gameObject.transform);
                    count += 1;
                    print(count);
                    if (count == 1)
                    {
                        pointSpwaner.PointA = gameObject.transform;
                    }
                    else if (count == 2)
                    {
                        pointSpwaner.PointB = gameObject.transform;

                        if (pointSpwaner.isLine)
                        {
                            pointSpwaner.DrawLine();
                        }
                        else if (pointSpwaner.isCircle)
                        {
                            pointSpwaner.DrawCircle();
                        }
                        else if (pointSpwaner.isRectangle)
                        {
                            pointSpwaner.DrawRectangle();
                        }
                    }
                }
            }

        }

    }
}
