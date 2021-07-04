using System;
using UnityEngine;
using Random = System.Random;

public class AutoMapGenerator : MapGenerator
{
    [SerializeField] private int lengthOfWay = 20;
    [SerializeField] private int minimumBlankBetweenRamps = 3;
    
    private protected override void Awake()
    {
        base.Awake();
        GenerateMap(lengthOfWay);
        GenerateFinish();
    }

    private protected override void CheckIfSerializedFieldsSet()
    {
        base.CheckIfSerializedFieldsSet();
        if (lengthOfWay < 0 || minimumBlankBetweenRamps < 0)
        {
            throw new Exception(offensiveErrorMessagePrefix + "must be >0");
        }
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
}
