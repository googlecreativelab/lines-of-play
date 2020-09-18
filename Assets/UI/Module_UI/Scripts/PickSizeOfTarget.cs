using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickSizeOfTarget : MonoBehaviour
{
    public RectTransform target;
    private RectTransform myRectTf;
    private void Start()
    {
        myRectTf = GetComponent<RectTransform>();
    }
    // Update is called once per frame
    void Update()
    {
        myRectTf.sizeDelta = target.rect.size;
    }
}
