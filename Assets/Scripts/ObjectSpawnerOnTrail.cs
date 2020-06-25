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

public class ObjectSpawnerOnTrail : MonoBehaviour
{
    public Transform reticle;
    public GameObject objectPrefab;

    GameObject spwanedObject;
    float elapsed = 0;

    float randTime;
   
    void Start()
    {
        elapsed = 0;
        randTime = 0.2f;
    }

   
    void Update()
    {
        if (Input.GetMouseButtonDown(0)
                    && Input.mousePosition.y < Screen.height * 0.8)
        {
            Instantiate(objectPrefab, reticle.position, reticle.rotation);
        }
        elapsed += Time.fixedDeltaTime;


        if (elapsed > randTime)
        {
            spwanedObject = Instantiate(objectPrefab, reticle.position, reticle.rotation);
            spwanedObject.transform.LookAt(reticle);
            randTime = 0.2f;
            elapsed = 0;
        }
    }
}
