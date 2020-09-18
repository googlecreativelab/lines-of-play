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

using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TransactionData
{
    public States state;
    public List<Domino> _dominos;
    //public Vector3 objectPosition;
    //public Quaternion objectRotation;
    //public Vector3 objectScale;


    public enum States
    {
        spawned,
        deleted,
        Toppled
    }
}