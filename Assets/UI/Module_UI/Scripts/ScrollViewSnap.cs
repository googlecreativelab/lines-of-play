using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class ScrollViewSnap : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private bool dragging = false, snapped = false;
    public int index = 0;
    public Transform contentTf;
    [SerializeField]
    private Vector3 target;
    [SerializeField]
    private Transform[] kids;
    [SerializeField]
    private float[] distances;
    public Transform middlePoint;

    [SerializeField]
    private Text[] elementLabels;
    public Color textSelectedColor;
    public Color textUnselectedColor = Color.white;
    [SerializeField]
    private int lastIndex = -1;

    public delegate void OnIndexChanged(int index);
    public static event OnIndexChanged onIndexChanged;
    public RectTransform viewport;
    public GameObject aboutpanel;
    public MainController _mainController;

    private void Awake()
    {
        MainUI.Instance.scrollViewSnap = this;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        dragging = true;
        snapped = false;
        if (aboutpanel.activeSelf) { 
            aboutpanel.SetActive(false);
            MainUI.Instance.About_BG_ON();
        }
    }

    public void OnDrag(PointerEventData eventData)
    {

    }

    public void OnEndDrag(PointerEventData eventData)
    {
        dragging = false;
    }
    private void Start()
    {
        if (_mainController == null)
        {
            _mainController = FindObjectOfType<MainController>();
        }
        kids = new Transform[contentTf.transform.childCount];
        distances = new float[kids.Length];
        for (int i = 0; i < kids.Length; i++)
        {
            kids[i] = contentTf.transform.GetChild(i);
        }
        elementLabels = contentTf.GetComponentsInChildren<Text>();
        UpdateLabelsColor();
        StartCoroutine(MoveToButtonOnStart(2));
    }

    public IEnumerator MoveToButtonOnStart(int index)
    {
        yield return new WaitForSeconds(0.1f);
        kids[index].GetComponent<Button>().onClick.Invoke();
    }
    public bool isInvoked = false;
    int prevIndex;
    private void Update()
    {


        for (int i = 0; i < kids.Length; i++)
        {
            distances[i] = Mathf.Abs(Vector3.Distance(middlePoint.position, kids[i].position));
        }


        if (distances.Min() < 80)
        {
            index = Array.IndexOf(distances, distances.Min());

            if (!isInvoked && index != prevIndex)
            {
                isInvoked = true;
                kids[index].GetComponent<Button>().onClick.Invoke();

                prevIndex = index;
                //print(kids[index].name);
            }


        }
        else
        {
            isInvoked = false;
        }


        //if (!dragging && !snapped)
        //{

        //    snapped = true;
        //    for (int i = 0; i < kids.Length; i++)
        //    {
        //        distances[i] = Mathf.Abs(Vector3.Distance(middlePoint.position, kids[i].position));
        //    }
        //    index = Array.IndexOf(distances, distances.Min());
        //    //print(index);

        //    target = Vector3.zero;
        //    kids[index].GetComponent<Button>().onClick.Invoke();
        //}
    }

    public void MoveButton(int index)
    {
        this.index = index;
        target = new Vector3(index * -kids[0].GetComponent<RectTransform>().rect.width, 0, 0);


        if (index != 7)
        {
            if (aboutpanel.activeSelf)
            {
                aboutpanel.SetActive(false);
                MainUI.Instance.About_BG_ON();
            }
        }
        _mainController.ToppleOff();
        onIndexChanged(index);
        UpdateLabelsColor();
        StartCoroutine(MoveToPosition());
    }

    int count = 0;
    public IEnumerator MoveToPosition()
    {    
        count++;
        var currentPos = contentTf.localPosition;
        var t = 0f;
        while (t < 1)
        {
            t += Time.deltaTime / 0.3f;
            if (count > 0)
            {
                contentTf.localPosition = Vector3.Lerp(currentPos, target, t);
                float x = Mathf.Abs(Vector3.Distance(middlePoint.position, kids[index].position));
                if (index == 0)
                {
                    contentTf.localPosition += new Vector3(-x + 100, 0f, 0f);
                }
                else if (index == 7)
                {
                    contentTf.localPosition += new Vector3(-x -150, 0f, 0f);
                }
                else
                {
                    contentTf.localPosition += new Vector3(-x - 50, 0f, 0f);
                }
              
            }            
            yield return null;
        }


    }
    private void UpdateLabelsColor()
    {
        if (lastIndex != index)
        {
            foreach (var item in elementLabels)
            {
                item.color = textUnselectedColor;
            }
            elementLabels[index].color = textSelectedColor;
            lastIndex = index;
        }
       
    }
}
