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
using UnityEngine.SceneManagement;

public class UIManagerStart : MonoBehaviour
{
    // REFERENCES
    public Animation Lines;
    public Animation Of;
    public Animation Play;
    public Animation L;
    public Animation Y;
    public Animation Domino;

    public float duration;


    void Start()
    {
        StartCoroutine(Transition());
        PlayerPrefs.DeleteAll();
    }

    IEnumerator Transition()
    {
        Lines["Lines"].speed = duration;
        Lines.Play("Lines");
        yield return new WaitForSeconds(0.5f);
        Of["Of"].speed = duration;
        Of.Play("Of");
        yield return new WaitForSeconds(0.5f);
        Play["Play"].speed = duration;
        Play.Play("Play");
        yield return new WaitForSeconds(1);
        L["L"].speed = duration;
        Y["Y"].speed = duration;
        Domino["Domino"].speed = duration;
        L.Play("L");
        Y.Play("Y");
        Domino.Play("Domino");
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene("MainScene");
    }
}
