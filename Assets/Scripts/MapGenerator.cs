using System;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class MapGenerator : MonoBehaviour
{
    [SerializeField] private int lengthOfWay = 20;
    [SerializeField] private int minimumBlankBetweenRamps = 3;
    
    [SerializeField] private GameObject startPrefab;
    [SerializeField] private GameObject finishPrefab;
    [SerializeField] private GameObject blankPrefab;
    [SerializeField] private List<GameObject> rampsPrefabs;

    private Dictionary<string, int> prefabWidth = new Dictionary<string, int>();
    
    private List<GameObject> instantiated = new List<GameObject>(); //TODO: replace with Queue<GameObject> ?

    public int blocksPassed = 0;
    
    public GameObject StartControllerGO => instantiated[0];

    void Awake()
    {
        CheckIfSerializedFieldsSet();
        CollectWidth();
        GenerateStart();
        GenerateMap(lengthOfWay);
        GenerateFinish();
    }

    private void CheckIfSerializedFieldsSet()
    {
        string offensiveErrorMessagePrefix = "U dumbass! ";
        if (lengthOfWay < 0 || minimumBlankBetweenRamps < 0)
        {
            throw new Exception(offensiveErrorMessagePrefix + "must be >0");
        }
        if (blocksPassed != 0)
        {
            throw new Exception(offensiveErrorMessagePrefix + "blocksPassed must be == 0");
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
        }
        
        /*
         *  Next commented code also works. BUT
         *  GetComponent(s) is very heavy operation.
         *  You should avoid it at all cost in while, Update, for, etc.
         *  Better use [Serializefield] to component where possible.
         *  In this case I use GetComponentsInChildren to avoid MonoBehaviour script in every prefab
         *  Don't give a fuck at this stage :D
         */
        
        // foreach (var ramp in rampsPrefabs)
        // {
        //     prefabWidth[ramp.name + instantiatedGOSuffix] = ramp.GetComponentsInChildren<SpriteRenderer>().Length;
        // }
    }

    private void GenerateStart()
    {
        InstantiateRoadMile(startPrefab);
    }

    private void GenerateFinish()
    {
        InstantiateRoadMile(finishPrefab);
    }
    
    private void InstantiateRoadMile(GameObject prefab)
    {
        var go = Instantiate(prefab, this.transform);
        var goName = go.name;
        Vector2 originPos = go.transform.localPosition;
        Vector2 newPos = new Vector2(originPos.x + blocksPassed, originPos.y);
        go.transform.SetPositionAndRotation(newPos, go.transform.rotation); //this is the only way to set position correctly
        instantiated.Add(go);
        blocksPassed += prefabWidth[goName];
    }
    
    private void GenerateMap(int mapLenght)
    {
        var rnd = new Random();
        int count = minimumBlankBetweenRamps;
        for (int i=0; i<mapLenght; i++)
        {
            bool willBeBlank = true;
            if (count == 0)
            {
                willBeBlank = Convert.ToBoolean(rnd.Next(0, 2));
            }
            else
            {
                count--;
            }
            if (willBeBlank)
            {
                InstantiateRoadMile(blankPrefab);
            }
            else
            {
                InstantiateRoadMile(rampsPrefabs[rnd.Next(rampsPrefabs.Count)]);
                count = minimumBlankBetweenRamps;
            }
        }
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
