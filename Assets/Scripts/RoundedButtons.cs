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

public class RoundedButtons : MonoBehaviour
{

    public Texture btnTexture;
    GUIStyle gUI;
    void OnGUI()
    {
        if (!btnTexture)
        {
            Debug.LogError("Please assign a texture on the inspector");
            return;
        }


        if (GUI.Button(new Rect(10, 10, 50, 50), btnTexture))
            Debug.Log("Clicked the button with an image");

        if (GUI.Button(new Rect(Screen.width / 4, Screen.height / 6, Screen.width / 2, Screen.height / 8), btnTexture))
            Debug.Log("Clicked the button with text");

        GUI.Box(new Rect(Screen.width / 4, Screen.height / 6, Screen.width / 2, Screen.height / 8), "Hello");
    }
}
