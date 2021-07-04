using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManualMapGenerator : MapGenerator
{
    [SerializeField] private Camera mainCamera;

    private int rampCode = 0;

    private GameObject currentRamp;

    private bool selected = false;
    
    private void Update()
    {
        if (Input.GetKey(KeyCode.UpArrow))
        {
            SelectPreviousRamp();
        }
        else if(Input.GetKey(KeyCode.DownArrow))
        {
            SelectNextRamp();
        }
        else if(Input.GetKey(KeyCode.Space))
        {
            Paste();
        }
    }

    private void SelectNextRamp()
    {
        if(!selected && currentRamp != null)
            DestroyImmediate(LastControllerGO);
        if(!selected)
        {
            currentRamp = InstantiateRoadMile(rampsPrefabs[rampCode]);
            StartCoroutine(MoveCamera());
            selected = true;
            rampCode++;
            CheckRampCode(ref rampCode);
            SetAlpha(LastControllerGO, 0.5f);
        }
        else
        {
            Debug.Log("aaa zalupa");

            DestroyImmediate(LastControllerGO); // this destroys transform ?
            rampCode++;
            CheckRampCode(ref rampCode);
            currentRamp = InstantiateRoadMile(rampsPrefabs[rampCode], false);
            SetAlpha(LastControllerGO, 0.5f);
        }
    }

    private void SelectPreviousRamp()
    {
        // if(!selected && currentRamp != null)
        //     DestroyImmediate(LastControllerGO);
        // if(!selected)
        // {
        //     selected = true;
        //     rampCode--;
        //     CheckRampCode(ref rampCode);
        //     currentRamp = rampsPrefabs[rampCode];
        //     StartCoroutine(MoveCamera());
        //     InstantiateRoadMile(currentRamp,false);
        //     SetAlpha(LastControllerGO, 0.5f);
        // }
    }
    
    private IEnumerator MoveCamera()
    {
        int i = 0;
        var origin = mainCamera.transform.position;
        while (i<1000 && currentRamp != null) //TODO: FIX
        {
            i++;
            // mainCamera.transform.SetPositionAndRotation(new Vector3(origin.x + ((RectTransform) currentRamp.transform).rect.width, origin.y, origin.z), mainCamera.transform.rotation);
            Debug.Log(currentRamp.name);
            mainCamera.transform.SetPositionAndRotation(new Vector3(origin.x + prefabWidth[currentRamp.name], origin.y, origin.z), mainCamera.transform.rotation);
            yield return null;
        }
    }

    public void CheckRampCode(ref int rampCode)
    {
        if (rampCode < 0)
            rampCode = rampsPrefabs.Count-1 + rampCode;
        else if (rampCode == rampsPrefabs.Count)
            rampCode = rampsPrefabs.Count - rampCode;
    }
    
    private void SetAlpha(GameObject go, float alpha)
    {
        SpriteRenderer[] spriteRenderers = go.GetComponentsInChildren<SpriteRenderer>(); //bad
        foreach (var sprite in spriteRenderers)
        {
            var prevCol = sprite.color;
            sprite.color = new Color(prevCol.r, prevCol.g, prevCol.b, alpha);
        }
    }

    private void Paste()
    {
        selected = false;
        currentRamp = null;
        SetAlpha(LastControllerGO, 1f);
    }
    
    private void PasteNewRamp()
    {
        InstantiateRoadMile(currentRamp);
    }

    private void PasteBlank()
    {
        InstantiateRoadMile(blankPrefab);
    }

    private void PasteFinish()
    {
        GenerateFinish();
    }

    #region Zhek0s Task TODO
    private void SaveJson()
    {
        //TODO: zhek0s
    }
    
    private void LoadJson()
    {
        //TODO: zhek0s
    }
    #endregion
}
