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
    public GameObject ghostDomino;
    public int count = 0;
    public TwoPointSpawner pointSpwaner;
    bool isCorrect = false;
    public float NumberOfSegments;
    public float AlonThePath;
    List<GameObject> usedObjects = new List<GameObject>();
    public MainController _mainController;
    void Start()
    {
        target = Camera.main.transform;
        if (_mainController == null)
        {
            _mainController = FindObjectOfType<MainController>();
        }
    }

    /// <summary>
    /// This function prevents spawing object mistakenly while inyeracting with the UI.
    /// </summary>
    /// <returns></returns>
    private bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }

    //float delay = 1f;
    void Update()
    {
        //delay -= 0.5f;
        //if (delay < 0)
        //{
            //delay = 1f;

            var center = new Vector2(Screen.width * ScreenPosition.x, Screen.height * ScreenPosition.y);
            Ray mRay = Camera.main.ScreenPointToRay(center);
            RaycastHit hit;
            if (Physics.Raycast(mRay, out hit))
            {
                if (hit.transform.CompareTag("DetectedPlane")/* || hit.transform.CompareTag("Domino")*/)
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
                        //print("PPOS " + gameObject.transform.position);
                        pointSpwaner.points.Add(gameObject.transform);
                        _mainController.ToppleOff();
                        count += 1;
                        //print(count);
                        if (count == 1)
                        {
                            pointSpwaner.PointA = gameObject.transform;

                        }
                        else if (count == 2)
                        {
                            pointSpwaner.PointB = gameObject.transform;
                            DespawnAll();

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

                    if (count == 1)
                    {
                        if (pointSpwaner.isLine)
                        {
                            DrawLine(pointSpwaner.PointA, reticle.transform);
                        }
                        else if (pointSpwaner.isCircle)
                        {
                            DrawCircle(pointSpwaner.PointA, reticle.transform);
                        }
                        else if (pointSpwaner.isRectangle)
                        {
                            DrawRectangle(pointSpwaner.PointA, reticle.transform);
                        }

                    }
                }

            }
        //}
    }


    /// <summary>
    /// Spawning Ghost Dominos to preview Line
    /// </summary>
    /// <param name="PointA"> Point from where the line starts</param>
    /// <param name="PointB"> Point where the line ends</param>
    public void DrawLine(Transform PointA, Transform PointB)
    {
        //print("This is Line");
       DespawnAll();
        var distance = Vector3.Distance(PointA.position, PointB.position);
        NumberOfSegments = Mathf.Round(distance / 0.12f) + 1;

        AlonThePath = 1 / (NumberOfSegments);//% along the path

        for (int i = 1; i < NumberOfSegments; i++)
        {

            Vector3 CreatPosition = PointA.position + (PointB.position - PointA.position) * (AlonThePath * i);

            GameObject domino = ObjectPooler.Instance.Spawn(ghostDomino,Vector3.zero, Quaternion.identity);
            //domino.GetComponent<SwitchOnRandomDomino>().colorID = PlayerPrefs.GetInt("ColorID");
            domino.transform.position = CreatPosition;
            domino.transform.LookAt(PointB);
            usedObjects.Add(domino);
        }

        //balloonPlacing.count = 0;
        //Destroy(PointA.gameObject);
        //Destroy(PointB.gameObject);
    }

    /// <summary>
    ///  Spawning Ghost Dominos to preview Circle
    /// </summary>
    /// <param name="PointA">Centre point of the circle</param>
    /// <param name="PointB">Point on mark the circumference of the circle</param>
    public void DrawCircle(Transform PointA, Transform PointB)
    {
       DespawnAll();
        var radius = Vector3.Distance(PointA.position, PointB.position);
        //print(radius);
        int segments = (int)((2 * Mathf.PI * radius) / 0.12f);
        var angle = 90f;
        //print("Circumference: " + (2 * Mathf.PI * radius));
        var pointCount = segments; // add extra point to make startpoint and endpoint the same to close the circle
        var points = new Vector3[pointCount];

        for (int i = 0; i < pointCount; i++)
        {
            var rad = Mathf.Deg2Rad * (i * 360f / segments);
            //print(rad);
            points[i] = new Vector3((Mathf.Sin(rad) * radius) + PointA.position.x, PointA.position.y, (Mathf.Cos(rad) * radius) + PointA.position.z);

            //var domino = Instantiate(ghostDomino);
            var domino = ObjectPooler.Instance.Spawn(ghostDomino, Vector3.zero, Quaternion.identity);
            usedObjects.Add(domino);
            domino.transform.position = points[i];

            angle += 360 / segments;
            domino.transform.rotation = Quaternion.Euler(new Vector3(0f, angle, 0f));
        }
    }

    /// <summary>
    ///  Spawning Ghost Dominos to preview Rectangle
    /// </summary>
    /// <param name="PointA">Initial point of the Diagnol</param>
    /// <param name="PointB">End point of the Diagnol</param>
    public void DrawRectangle(Transform PointA, Transform PointB)
    {
       DespawnAll();

        pointSpwaner.PointC.position = new Vector3(PointB.position.x, PointA.position.y, PointA.position.z);
        pointSpwaner.PointD.position = new Vector3(PointA.position.x, PointA.position.y, PointB.position.z);
        Transform[] corners = { PointA, pointSpwaner.PointC, PointB, pointSpwaner.PointD };


        if (((PointA.position.x < PointB.position.x) && (PointA.position.z > PointB.position.z))
    || ((PointA.position.x > PointB.position.x) && (PointA.position.z < PointB.position.z)))
        {
            //print("Correct Square");
            isCorrect = true;
        }
        else if (((PointA.position.x > PointB.position.x) && (PointA.position.z > PointB.position.z))
            || ((PointA.position.x < PointB.position.x) && (PointA.position.z < PointB.position.z)))
        {
            //print("Wrong Square");
            isCorrect = false;
        }



        for (int j = 1; j < corners.Length; j++)//4
        {
            var pointA = corners[j - 1];// 0,1,2
            var pointB = corners[j];//1,2,3
            DrawLineSegment(pointA, pointB);
        }
        if (corners.Length > 2)
        {
            DrawLineSegment(corners[corners.Length - 1], corners[0]);
        }
    }

    /// <summary>
    /// Generate other corners of the Rectangle based on PointA and PointB,
    /// Also Draws Line segements between these 4 corners to form a Rectangle 
    /// </summary>
    /// <param name="PointA"></param>
    /// <param name="PointB"></param>
    public void DrawLineSegment(Transform PointA, Transform PointB)
    {
        var distance = Vector3.Distance(PointA.position, PointB.position);
        NumberOfSegments = Mathf.Round(distance / 0.12f) + 1;

        AlonThePath = 1 / (NumberOfSegments);//% along the path

        for (int i = 0; i < NumberOfSegments; i++)
        {

            Vector3 CreatPosition = PointA.position + (PointB.position - PointA.position) * (AlonThePath * i);

            GameObject domino = ObjectPooler.Instance.Spawn(ghostDomino, Vector3.zero, Quaternion.identity);
            usedObjects.Add(domino);
            domino.transform.position = CreatPosition;
            domino.transform.LookAt(PointB);

            //These conditions correct the domino rotation and position which are placed at the corners.
            if (i == 0 && isCorrect)
            {
                if (domino.transform.rotation.eulerAngles.y == 0 || domino.transform.rotation.eulerAngles.y == 45)
                {
                    domino.transform.rotation = Quaternion.Euler(new Vector3(0f, -45f, 0f));
                }
                else if (domino.transform.rotation.eulerAngles.y == 90 || domino.transform.rotation.eulerAngles.y == -90)
                {
                    domino.transform.rotation = Quaternion.Euler(new Vector3(0f, 45f, 0f));
                }
                else if (domino.transform.rotation.eulerAngles.y == 270 || domino.transform.rotation.eulerAngles.y == -270)
                {
                    domino.transform.rotation = Quaternion.Euler(new Vector3(0f, 45f, 0f));
                }
                else if (domino.transform.rotation.eulerAngles.y == 180 || domino.transform.rotation.eulerAngles.y == -180)
                {
                    domino.transform.rotation = Quaternion.Euler(new Vector3(0f, -45f, 0f));
                }

                if (domino.transform.position.x == 0 && domino.transform.position.z < 2.0f)
                {
                    domino.transform.position += new Vector3(-0.035f, 0f, 0.035f);
                }
                else if (domino.transform.position.x < 0 && domino.transform.position.z < 2.0f)
                {
                    domino.transform.position += new Vector3(+0.035f, 0f, 0.035f);
                }
                else if (domino.transform.position.x == 0 && domino.transform.position.z > 2.0f)
                {
                    domino.transform.position += new Vector3(-0.035f, 0f, -0.035f);
                }
                else if (domino.transform.position.x < 0 && domino.transform.position.z > 2.0f)
                {
                    domino.transform.position += new Vector3(+0.035f, 0f, -0.035f);
                }


            }
            else if (i == 0 && !isCorrect)
            {
                if (domino.transform.rotation.eulerAngles.y == 0 || domino.transform.rotation.eulerAngles.y == 45)
                {
                    domino.transform.rotation = Quaternion.Euler(new Vector3(0f, 45f, 0f));
                }
                else if (domino.transform.rotation.eulerAngles.y == 90 || domino.transform.rotation.eulerAngles.y == -90)
                {
                    domino.transform.rotation = Quaternion.Euler(new Vector3(0f, -45f, 0f));
                }
                else if (domino.transform.rotation.eulerAngles.y == 270 || domino.transform.rotation.eulerAngles.y == -270)
                {
                    domino.transform.rotation = Quaternion.Euler(new Vector3(0f, -45f, 0f));
                }
                else if (domino.transform.rotation.eulerAngles.y == 180 || domino.transform.rotation.eulerAngles.y == -180)
                {
                    domino.transform.rotation = Quaternion.Euler(new Vector3(0f, 45f, 0f));
                }

                if (domino.transform.position.x == 0 && domino.transform.position.z < 2.0f)
                {
                    domino.transform.position += new Vector3(-0.035f, 0f, 0.035f);
                }
                else if (domino.transform.position.x < 0 && domino.transform.position.z < 2.0f)
                {
                    domino.transform.position += new Vector3(+0.035f, 0f, 0.035f);
                }
                else if (domino.transform.position.x == 0 && domino.transform.position.z > 2.0f)
                {
                    domino.transform.position += new Vector3(-0.035f, 0f, -0.035f);
                }
                else if (domino.transform.position.x < 0 && domino.transform.position.z > 2.0f)
                {
                    domino.transform.position += new Vector3(+0.035f, 0f, -0.035f);
                }

            }
        }
    }

    public void DespawnAll()
    {
        foreach (var usedObject in usedObjects)
        {
            ObjectPooler.Instance.Despawn(usedObject);
        }
    }
}
