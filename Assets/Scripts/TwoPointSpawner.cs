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
using UnityEngine.Animations;
using UnityEngine.UI;

public class TwoPointSpawner : MonoBehaviour
{
    #region PUBLIC MEMBERS
    public float NumberOfSegments;
    public float AlonThePath;
    public Transform PointA;
    public Transform PointB;
    public Transform PointC;
    public Transform PointD;
    public GameObject dominoPrefab;
    public GameObject dominoPrefabLooked;
    public GameObject lineSegment;
    public bool isDefault = true;
    public bool isLine = false;
    public bool isCircle = false;
    public bool isRectangle = false;
    public List<Transform> points;
    public BalloonPlacing balloonPlacing;
    //public LeanGameObjectPool objectPool;
    public MainController mainController;
    public MainUI mainUI;
    public GameObject reticleBallon;
    public GameObject reticleGhostDomino;
    public UndoRedoManager _undoRedoManager;

    public GameObject ButtonLine;
    public GameObject ButtonCircle;
    public GameObject ButtonRectangle;
    #endregion

    List<GameObject> firstObjs = new List<GameObject>();
    List<GameObject> lastObjs = new List<GameObject>();
    private List<Domino> holdDominos = new List<Domino>();
    bool isCorrect = false;

    // Update is called once per frame
    void Start()
    {
     
        if (mainController == null)
        {
            mainController = FindObjectOfType<MainController>();
        }
        if (mainUI == null)
        {
            mainUI = FindObjectOfType<MainUI>();
        }
        if (_undoRedoManager == null)
        {
            _undoRedoManager = FindObjectOfType<UndoRedoManager>();
        }
    }

    public void DrawLine()
    {
        holdDominos.Clear();
        //print("This is Line");
        var distance = Vector3.Distance(PointA.position, PointB.position); 
        NumberOfSegments = Mathf.Round(distance / 0.12f) + 1; 
        
        AlonThePath = 1 / (NumberOfSegments);//% along the path

        for (int i = 0; i < NumberOfSegments; i++)
        {

            Vector3 CreatPosition = PointA.position + (PointB.position - PointA.position) * (AlonThePath * i);

            GameObject domino = Instantiate(dominoPrefab);
            domino.GetComponent<SwitchOnRandomDomino>().colorID = PlayerPrefs.GetInt("ColorID");
            mainController.AddDomino(domino);
            domino.transform.position = CreatPosition;
            domino.transform.LookAt(PointB);

            Domino dominoTemp = new Domino();
            dominoTemp._dominoObj = domino;
            dominoTemp._dominoPosition = domino.transform.position;
            dominoTemp._dominoRotation = domino.transform.rotation;
            dominoTemp._dominoScale = domino.transform.localScale;
            holdDominos.Add(dominoTemp);
        }

        _undoRedoManager.LoadData(TransactionData.States.spawned, holdDominos);
        balloonPlacing.count = 0;
        Destroy(PointA.gameObject);
        Destroy(PointB.gameObject);
    }


    public void DrawCircle()
    {
        holdDominos.Clear();

        var radius = Vector3.Distance(PointA.position, PointB.position);

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
           
            var domino = Instantiate(dominoPrefab);
            domino.GetComponent<SwitchOnRandomDomino>().colorID = PlayerPrefs.GetInt("ColorID");
            mainController.AddDomino(domino);
            domino.transform.position = points[i];

            angle += 360/segments;
            domino.transform.rotation = Quaternion.Euler(new Vector3(0f, angle, 0f));

            Domino dominoTemp = new Domino();
            dominoTemp._dominoObj = domino;
            dominoTemp._dominoPosition = domino.transform.position;
            dominoTemp._dominoRotation = domino.transform.rotation;
            dominoTemp._dominoScale = domino.transform.localScale;
            holdDominos.Add(dominoTemp);
        }


        _undoRedoManager.LoadData(TransactionData.States.spawned, holdDominos);
        balloonPlacing.count = 0;
        Destroy(PointA.gameObject);
        Destroy(PointB.gameObject);
    }


    public void DrawRectangle()
    {

        holdDominos.Clear();

         PointC.position = new Vector3(PointB.position.x, PointA.position.y, PointA.position.z);
         PointD.position = new Vector3(PointA.position.x, PointA.position.y, PointB.position.z);
        Transform[] corners = { PointA, PointC, PointB, PointD };


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


        _undoRedoManager.LoadData(TransactionData.States.spawned, holdDominos);
        //setLookDominos();
        //points.Clear();
        Destroy(PointA.gameObject);
        Destroy(PointB.gameObject);
        //dominoARController.count = 0;
        balloonPlacing.count = 0;
    }


    public void setLookDominos()
    {
        ConstraintSource constraintSource = new ConstraintSource();
        for (int i = 0; i < firstObjs.Count; i++)
        {
            firstObjs[i].AddComponent<LookAtConstraint>().rotationOffset = new Vector3(0f, 45f, 0f);
            firstObjs[i].GetComponent<LookAtConstraint>().constraintActive = true;
        }

        for (int i = 1; i < firstObjs.Count; i++)
        {

            constraintSource.sourceTransform = lastObjs[i-1].transform;
            firstObjs[i].GetComponent<LookAtConstraint>().AddSource(constraintSource);
        }

        constraintSource.sourceTransform = lastObjs[3].transform;
        firstObjs[0].GetComponent<LookAtConstraint>().AddSource(constraintSource);

    }


    public void DrawLineSegment(Transform PointA, Transform PointB)
    {
        var distance = Vector3.Distance(PointA.position, PointB.position);
        NumberOfSegments = Mathf.Round(distance / 0.12f) + 1;

        AlonThePath = 1 / (NumberOfSegments);//% along the path

        for (int i = 0; i < NumberOfSegments; i++)
        {

            Vector3 CreatPosition = PointA.position + (PointB.position - PointA.position) * (AlonThePath * i);

            GameObject domino = Instantiate(dominoPrefab);
            domino.GetComponent<SwitchOnRandomDomino>().colorID = PlayerPrefs.GetInt("ColorID");
      
            mainController.AddDomino(domino);
            domino.transform.position = CreatPosition;
            domino.transform.LookAt(PointB);
            //GameObject segment = Instantiate(lineSegment);
            //segment.transform.position = CreatPosition;
            //segment.transform.LookAt(PointB);

            if (i == 0)
            {
                firstObjs.Add(domino); 
            }
            if (i == NumberOfSegments-1)
            {
                lastObjs.Add(domino);
            }

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
            Domino dominoTemp = new Domino();
            dominoTemp._dominoObj = domino;
            dominoTemp._dominoPosition = domino.transform.position;
            dominoTemp._dominoRotation = domino.transform.rotation;
            dominoTemp._dominoScale = domino.transform.localScale;
            holdDominos.Add(dominoTemp);
        }
    }

    public void RemoveBalloonReticles()
    {
        if (PointA != null)
        {
            Destroy(PointA.gameObject);
        }
        if (PointB != null)
        {
            Destroy(PointB.gameObject);
        }
        points.Clear();
        balloonPlacing.DespawnAll();
        balloonPlacing.count = 0;
    }
    public void LineToggle()
    {
        RemoveBalloonReticles();
        //mainUI.MakeButtonBGTransparent();
        //isLine = !isLine;
        isLine = true;
        isCircle = false;
        isRectangle = false;
        isDefault = false;

        if (isLine)
        {
            mainUI.ChangeButtonBG(mainUI.ButtonLine, ButtonBGColor.White);
        }
        reticleBallon.SetActive(isLine);
    }  
    public void CircleToggle()
    {
        RemoveBalloonReticles();
        isCircle = true;
        //isCircle = !isCircle;
        isLine = false;
        isRectangle = false;
        isDefault = false;
        if (isCircle)
        {
            mainUI.ChangeButtonBG(mainUI.ButtonCircle, ButtonBGColor.White);
        }
        reticleBallon.SetActive(isCircle);
    }
    public void RectangleToggle()
    {
        RemoveBalloonReticles();
        isRectangle = true;
        //isRectangle = !isRectangle;
        isCircle = false;
        isLine = false;
        isDefault = false;
        if (isRectangle)
        {
            mainUI.ChangeButtonBG(mainUI.ButtonRectangle, ButtonBGColor.White);
        }
        reticleBallon.SetActive(isRectangle);
    }
    public void DefaultToggle()
    {
        RemoveBalloonReticles();
        isCircle = false;
        isLine = false;
        isRectangle = false;
        isDefault = true;

        reticleBallon.SetActive(!isDefault);
    }
}

