using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ManualMapGenerator : MapGenerator//, IPointerDownHandler
{
	[SerializeField] private Camera mainCamera;

	private int rampCode = 0;

	private GameObject currentRamp;

	private bool selected = false;
	private bool pressedWaitFinish = false;
	private Coroutine finishCountdown = null;
	
	private void Update()
	{
		if (Input.GetKey(KeyCode.UpArrow))
		{
			pressedWaitFinish = false;
			SelectPreviousRamp();
		}
		else if(Input.GetKey(KeyCode.DownArrow))
		{
			pressedWaitFinish = false;
			SelectNextRamp();
		}
		else if (Input.GetKey(KeyCode.Space))
		{
			pressedWaitFinish = false;
			Paste();
		}
		else if(Input.GetKey(KeyCode.RightArrow))
		{
			pressedWaitFinish = true;
			if(currentRamp== null || (currentRamp!=null && !currentRamp.name.Equals(finishPrefab.name+"(Clone)")))
				SelectPasteBlank();
			if (finishCountdown == null)
				finishCountdown = StartCoroutine(FinishCoroutine());
		}
	}

	IEnumerator FinishCoroutine()
	{
		yield return new WaitForSeconds(1.5f);
		if(pressedWaitFinish)
			SelectRamp(finishPrefab);
		finishCountdown = null;
	}
	
	private void SelectNextRamp()
	{
		rampCode++;
		CheckRampCode(ref rampCode);
		SelectRamp(rampsPrefabs[rampCode]);
	}

	private void SelectPreviousRamp()
	{
		rampCode--; 
		CheckRampCode(ref rampCode);
		SelectRamp(rampsPrefabs[rampCode]);
	}
	
	private void SelectPasteBlank()
	{
		rampCode = 0;
		SelectRamp(blankPrefab);
		StartCoroutine(MoveCamera());
	}

	private void SelectRamp(GameObject prefab)
	{
		if(!selected && currentRamp != null)
			DestroyImmediate(LastControllerGO);
		if(!selected)
		{
			StartCoroutine(MoveCamera());
			selected = true;
		}
		else
		{
			blocksGenerated -= prefabWidth[LastControllerGO.name];
			DestroyImmediate(LastControllerGO); // this destroys transform !!
		}
		currentRamp = InstantiateRoadMile(prefab);
		SetAlpha(LastControllerGO, 0.5f);
	}
	
	private IEnumerator MoveCamera()
	{
		int i = 0;
		var origin = mainCamera.transform.position;
		while (i<1000 && currentRamp != null) //TODO: FIX
		{
			i++;
			// mainCamera.transform.SetPositionAndRotation(new Vector3(origin.x + ((RectTransform) currentRamp.transform).rect.width, origin.y, origin.z), mainCamera.transform.rotation);
			// mainCamera.transform.SetPositionAndRotation(new Vector3(origin.x + prefabWidth[currentRamp.name], origin.y, origin.z), mainCamera.transform.rotation);
			mainCamera.transform.SetPositionAndRotation(new Vector3(blocksGenerated, origin.y, origin.z), mainCamera.transform.rotation);
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
		if (currentRamp != null && !currentRamp.name.Equals(finishPrefab.name + "(Clone)"))
		{
			SaveJson();
			Debug.LogWarning("GreatSuccess");
		}
		currentRamp = null;
		SetAlpha(LastControllerGO, 1f);
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
