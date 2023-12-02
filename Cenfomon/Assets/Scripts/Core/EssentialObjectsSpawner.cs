using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EssentialObjectsSpawner : MonoBehaviour
{
    [SerializeField] GameObject essentialObjectsPrefab;

    private void Awake()
    {
        var existingObjects = FindObjectsOfType<EssentialObjects>();
        if(existingObjects.Length == 0)
        {
            //Si existe un grid, se spawnea al centro del grid
            var spawnPos = new Vector3(0,0,0);

            var grid = FindObjectOfType<Grid>();
            if(grid != null )
                spawnPos = grid.transform.position;
            
            Instantiate(essentialObjectsPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        }
            
    }
}
