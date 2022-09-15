using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserSelector : MapGenerator
{
    [SerializeField] protected GameObject player;
    public GameObject selector;
    public GameObject toDisable=null;
    public GameObject parentToSlide;
    public float slideLeft = -7.5f;
    public float slideRight = 7.5f;
    private Vector3 pos;
    private int _selectedRamp;
    private bool _pressed;

    private int _startingBlankPole = 20;
    public float scrollSpeed;
    private bool canPlace=false;

    // Start is called before the first frame update
    void Start()
    {
        if (toDisable) 
        {
            toDisable.SetActive(false);
        }
        pos = selector.transform.localPosition;
        _selectedRamp = (int)(pos.x / 1.5);
        Debug.Log("Selected ramp:" + _selectedRamp);
        _pressed=false;
        for (int i = 0; i < _startingBlankPole-5; i++)
        {
            InstantiateRoadMile(blankPrefab);
        }
        InstantiateRoadMile(startPrefab);
        for (int i = 0; i < 5; i++)
        {
            InstantiateRoadMile(blankPrefab);
        }
        StartCoroutine(MoveMap((int)(blocksGenerated + this.transform.position.x - player.transform.position.x)));
        InstantiateRoadMile(rampsPrefabs[0]);
        for(int i = 1; i < 10; i++)
        {
            InstantiateRoadMile(blankPrefab);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(!_pressed && Input.GetKeyDown(KeyCode.LeftArrow) && canPlace)
        {
            _pressed=true;
            if (_selectedRamp > 0)
            {
                _selectedRamp--;
                pos = selector.transform.localPosition;
                selector.transform.Translate(-1.5f, 0, 0);
                if (selector.transform.position.x < slideLeft)
                {
                    parentToSlide.transform.Translate(1.5f, 0, 0);
                }
                Debug.Log("Selected ramp:" + _selectedRamp);
                placeRampTest(_selectedRamp);
            }
        }
        if (!_pressed && Input.GetKeyDown(KeyCode.RightArrow) && canPlace)
        {
            _pressed = true;
            if (_selectedRamp < 20)
            {
                _selectedRamp++;
                pos = selector.transform.localPosition;
                selector.transform.Translate(1.5f, 0, 0);
                if (selector.transform.position.x > slideRight)
                {
                    parentToSlide.transform.Translate(-1.5f, 0, 0);
                }
                Debug.Log("Selected ramp:" + _selectedRamp);
                placeRampTest(_selectedRamp);
            }
        }
        if(!_pressed && Input.GetKeyDown(KeyCode.Space) && canPlace)
        {
            placeRamp(_selectedRamp);
        }
        if(Input.GetKeyUp(KeyCode.LeftArrow) | Input.GetKeyUp(KeyCode.RightArrow) | Input.GetKeyUp(KeyCode.Space))
        {
            _pressed = false;
        }
        if (Input.GetKeyUp(KeyCode.A))
        {
            SaveMap();
        }
        if (Input.GetKeyUp(KeyCode.D))
        {
            LoadMap();
        }
    }

    private void placeRamp(int selectedRamp)
    {
        StartCoroutine(MoveMap(prefabWidth[instantiated[instantiated.Count - 10].name]));
        Debug.Log(instantiated[instantiated.Count - 10].name);
        for (int i = 0; i < 9; i++)
        {
            //blocksGenerated -= prefabWidth[instantiated[instantiated.Count - i - 1].name];
            //DestroyImmediate(instantiated[instantiated.Count - i - 1]);
            DestroyRoadMile(0);
        }
        InstantiateRoadMile(rampsPrefabs[selectedRamp]);
        for (int i = 1; i < 10; i++)
        {
            InstantiateRoadMile(blankPrefab);
        }
    }

    private void placeRampTest(int selectedRamp)
    {
        for (int i = 0; i < 10; i++)
        {
            //mapData[rampsGenerated]="";
            //rampsGenerated--;
            //blocksGenerated -= prefabWidth[instantiated[instantiated.Count - i - 1].name];
            //DestroyImmediate(instantiated[instantiated.Count-i-1]);
            DestroyRoadMile(0);
        }
        InstantiateRoadMile(rampsPrefabs[selectedRamp]);
        for (int i = 1; i < 10; i++)
        {
            InstantiateRoadMile(blankPrefab);
        }
    }

    private IEnumerator MoveMap(int x)
    {
        float _x = x;
        canPlace = false;
        while (_x > 0)
        {
            float d= scrollSpeed * Time.fixedDeltaTime;
            _x -= d;
            this.transform.Translate(-d, 0, 0);
            yield return new WaitForFixedUpdate();
        }
        canPlace = true;
    }
}