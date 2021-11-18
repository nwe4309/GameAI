using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    [SerializeField] private GameObject neutralNode;

    // Start is called before the first frame update
    void Start()
    {
        SpawnNodes(-400, -400, 800, 800, 25, 150);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void SpawnNodes(int leftBounds, int bottomBounds, int width, int height, float minSpacing, float maxSpacing)
    {
        for(int row = leftBounds; row <= width/2; row += (int)Random.Range(minSpacing, maxSpacing))
        {
            for(int col = bottomBounds; col <= height/2; col += (int)Random.Range(minSpacing,maxSpacing))
            {
                if (row == leftBounds && col == bottomBounds)
                    continue;

                Debug.Log("X: " + row + " Y: " + col);
                GameObject newNode = GameObject.Instantiate(neutralNode);
                newNode.transform.position = new Vector3(row, 0, col);
            }
            //Debug.Log("X: " + (leftBounds + row) + " Y: " + (bottomBounds + col));
        }
    }
}
