using System;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [SerializeField] private GameObject startPrefab;
    [SerializeField] private GameObject finishPrefab;
    [SerializeField] protected GameObject blankPrefab;
    [SerializeField] protected List<GameObject> rampsPrefabs;
    
    private protected readonly string offensiveErrorMessagePrefix = "U dumbass! ";

    private protected Dictionary<string, int> prefabWidth = new Dictionary<string, int>();
    
    private List<GameObject> instantiated = new List<GameObject>(); //TODO: replace with Queue<GameObject> ?

    public int blocksGenerated = 0;
    
    public GameObject StartControllerGO => instantiated[0];
    public GameObject LastControllerGO => instantiated == null || instantiated.Count == 0 ? null : instantiated[instantiated.Count-1];
    
    private protected virtual void Awake()
    {
        CheckIfSerializedFieldsSet();
        CollectWidth();
        GenerateStart();
    }

    private protected virtual void CheckIfSerializedFieldsSet()
    {
        if (blocksGenerated != 0)
        {
            throw new Exception(offensiveErrorMessagePrefix + "blocksGenerated must be == 0");
        }
        if (startPrefab == null || finishPrefab == null || blankPrefab == null)
        {
            throw new Exception(offensiveErrorMessagePrefix + "check that you set prefabs");
        }
    }

    private void CollectWidth()
    {
        string instantiatedGOSuffix = "(Clone)";
        prefabWidth[blankPrefab.name+instantiatedGOSuffix] = 2;
        prefabWidth[startPrefab.name+instantiatedGOSuffix] = 4;
        prefabWidth[finishPrefab.name+instantiatedGOSuffix] = 6;
        foreach (var ramp in rampsPrefabs)
        {
            prefabWidth[ramp.name + instantiatedGOSuffix] = Convert.ToInt32( ((RectTransform) ramp.transform).rect.width );
            /*
             *  Next commented code also works. BUT
             *  GetComponent(s) is very heavy operation.
             *  You should avoid it at all cost in while, Update, for, etc.
             *  Better use [Serializefield] to component where possible.
             *  In this case I use GetComponentsInChildren to avoid MonoBehaviour script in every prefab
             *  Don't give a fuck at this stage :D
             */
              // prefabWidth[ramp.name + instantiatedGOSuffix] = ramp.GetComponentsInChildren<SpriteRenderer>().Length;
        }
    }

    private void GenerateStart()
    {
        InstantiateRoadMile(startPrefab);
    }

    protected void GenerateFinish()
    {
        InstantiateRoadMile(finishPrefab);
    }
    
    protected GameObject InstantiateRoadMile(GameObject prefab, bool countAsBloscksGenerated = true)
    {
        var go = Instantiate(prefab, this.transform);
        var goName = go.name;
        Vector2 originPos = new Vector2();
        if(LastControllerGO == null)
            originPos = go.transform.localPosition;
        else
            originPos = LastControllerGO.transform.localPosition;
        Vector2 newPos = originPos;
        if (!countAsBloscksGenerated)
            newPos = new Vector2(originPos.x, originPos.y);
        else
        {
            newPos = new Vector2(originPos.x + blocksGenerated, originPos.y);
            blocksGenerated += prefabWidth[goName];
        }
        go.transform.SetPositionAndRotation(newPos, go.transform.rotation); //this is the only way to set position correctly
        instantiated.Add(go);
        return go;
    }

    private void OnDestroy()
    {
        //No need because GarbageCollector (GC) exists.
        //But we are nerds and don't want PC to take care about memory.
        foreach (var inst in instantiated)
        {
            Destroy(inst);
        }
    }
}
