using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Characters;
using Battlefield;

public class WallDefenderSpawner : MonoBehaviour
{
    public Transform unitParents;
    public ScenePointBehavior leftSpawnPoint;
    public ScenePointBehavior pointSpawnPoint;

    public List<BaseCharacter> unitsSpawned;

    [Header("Flag Mechanics")]
    public bool enemyOnSight;
    public List<SpriteRenderer> flags;
    public Gradient flagColor;
    public bool isConverting;
    public float curValue;
    private float targetValue;
    public void Update()
    {
        if(isConverting)
        {
            if(enemyOnSight)
            {
                curValue += 0.025f;
            }
            else
            {
                curValue -= 0.025f;
            }

            for (int i = 0; i < flags.Count; i++)
            {
                flags[i].color = flagColor.Evaluate(curValue);
            }

            if(curValue >= targetValue)
            {
                isConverting = false;
            }
        }
    }
    public void StartSpawning(GameObject thisUnit, bool teleportTo = false)
    {
        if(teleportTo)
        {
            for (int i = 0; i < leftSpawnPoint.neighborPoints.Count; i++)
            {
                SpawnLeft(thisUnit, null, leftSpawnPoint.neighborPoints[i]);
            }

            for (int i = 0; i < pointSpawnPoint.neighborPoints.Count; i++)
            {
                SpawnLeft(thisUnit, null, pointSpawnPoint.neighborPoints[i]);
            }
        }
        else
        {
            StartCoroutine(SpawnInterval(thisUnit,leftSpawnPoint.neighborPoints.Count, leftSpawnPoint));

            StartCoroutine(SpawnInterval(thisUnit, pointSpawnPoint.neighborPoints.Count, pointSpawnPoint));
        }

        enemyOnSight = true;
        isConverting = true;
        targetValue = 1;
    }
    public void StartRetreating()
    {
        for (int i = 0; i < unitsSpawned.Count; i++)
        {
            BaseCharacter thisChar = unitsSpawned[i];
            unitsSpawned[i].OrderMovement(thisChar.myMovements.spawnPoint,()=> RemoveAndDestroyUnit(thisChar));
        }
        enemyOnSight = false;
        isConverting = true;
        targetValue = 0;
    }
    public void RemoveAndDestroyUnit(BaseCharacter thisChar)
    {
        thisChar.OrderToBanish();
        DestroyImmediate(thisChar.gameObject);
        unitsSpawned.Remove(thisChar);
    }
    public void SpawnLeft(GameObject thisUnitPrefab, ScenePointBehavior targetPoint, ScenePointBehavior spawnPoint)
    {
        GameObject tmp = GameObject.Instantiate(thisUnitPrefab, spawnPoint.transform.position, Quaternion.identity, null);

        if(unitsSpawned == null)
        {
            unitsSpawned = new List<BaseCharacter>();
        }

        unitsSpawned.Add(tmp.GetComponent<BaseCharacter>());

        int idx = unitsSpawned.Count - 1;

        unitsSpawned[idx].transform.localScale = new Vector3(0.75f, 0.75f, 1);
        unitsSpawned[idx].SpawnInThisPosition(spawnPoint);
        unitsSpawned[idx].unitInformation.curSpeed = 0.125f;
        unitsSpawned[idx].unitInformation.origSpeed = 0.125f;
        if(targetPoint != null)
        {
            unitsSpawned[idx].OrderMovement(targetPoint);
        }

    }

    IEnumerator SpawnInterval(GameObject thisUnit,int count, ScenePointBehavior spawnPoint)
    {
        yield return new WaitForSeconds(0.25f);

        SpawnLeft(thisUnit, spawnPoint.neighborPoints[count-1], spawnPoint);

        count -= 1;

        if(count > 0)
        {
            StartCoroutine(SpawnInterval(thisUnit, count, spawnPoint));
        }
    }
}
