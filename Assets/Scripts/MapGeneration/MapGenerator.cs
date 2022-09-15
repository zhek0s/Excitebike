using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEditor;
using UnityEngine;
using UnityEngine.Playables;

public class MapGenerator : MonoBehaviour
{
    [SerializeField] protected GameObject startPrefab;
    [SerializeField] protected GameObject finishPrefab;
    [SerializeField] protected GameObject blankPrefab;
    [SerializeField] protected List<GameObject> rampsPrefabs;
    
    private protected readonly string offensiveErrorMessagePrefix = "U dumbass! ";

    private protected Dictionary<string, int> prefabWidth = new Dictionary<string, int>();
    private protected Dictionary<string, GameObject> prefabName = new Dictionary<string, GameObject>();

    protected List<GameObject> instantiated = new List<GameObject>(); //TODO: replace with Queue<GameObject> ?

    public int blocksGenerated = 0;
    public int rampsGenerated=0;
    
    public GameObject StartControllerGO => instantiated[0];
    public GameObject LastControllerGO => instantiated == null || instantiated.Count == 0 ? null : instantiated[instantiated.Count-1];
    
    private protected virtual void Awake()
    {
        CheckIfSerializedFieldsSet();
        CollectWidth();
        //GenerateStart();
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
        prefabName[blankPrefab.name] = blankPrefab;
        prefabName[startPrefab.name] = startPrefab;
        prefabName[finishPrefab.name] = finishPrefab;
        foreach (var ramp in rampsPrefabs)
        {
            prefabWidth[ramp.name + instantiatedGOSuffix] = Convert.ToInt32( ((RectTransform) ramp.transform).rect.width );
            prefabName[ramp.name] = ramp;
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
        Vector2 originPos = new Vector2(this.transform.position.x,this.transform.position.y);
        /*if(LastControllerGO == null)
            originPos = go.transform.localPosition;
        else
            originPos = LastControllerGO.transform.localPosition;*/
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
        rampsGenerated++;
        return go;
    }

    protected void DestroyRoadMile(int pointerFromLast)
    {
        rampsGenerated--;
        blocksGenerated -= prefabWidth[instantiated[instantiated.Count - pointerFromLast - 1].name];
        DestroyImmediate(instantiated[instantiated.Count - pointerFromLast - 1]);
        instantiated.RemoveAt(instantiated.Count - pointerFromLast - 1);
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

    protected void SaveMap()
    {
        Dictionary<int,string> map = new Dictionary<int, string>();
        for (int i = 0; i < rampsGenerated; i++)
        {
            map[i]=(instantiated[i].name.Replace("(Clone)", null));
        }
        string json = JsonConvert.SerializeObject(map);
        string destination = Application.persistentDataPath + "/track.dat";
        FileStream file;
        if (File.Exists(destination)) file = File.OpenWrite(destination);
        else file = File.Create(destination);
        BinaryFormatter bf = new BinaryFormatter();
        bf.Serialize(file, json);
        file.Close();
        Debug.Log("Saved!");
    }

    protected void LoadMap()
    {
        string destination = Application.persistentDataPath + "/track.dat";
        FileStream file;
        if (File.Exists(destination)) file = File.OpenRead(destination);
        else
        {
            Debug.LogError("File not found");
            return;
        }
        BinaryFormatter bf = new BinaryFormatter();
        string json = (string)bf.Deserialize(file);
        file.Close();
        Dictionary<int, string> map = new Dictionary<int, string>();
        map = JsonConvert.DeserializeObject<Dictionary<int,string>>(json);
        while (instantiated.Count > 0)
        {
            DestroyRoadMile(0);
        }
        for (int i = 0; i < map.Count; i++)
        {
            InstantiateRoadMile(prefabName[map[i]]);
        }
        Debug.Log("Loading...");
    }
}

