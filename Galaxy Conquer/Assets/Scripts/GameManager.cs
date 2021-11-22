using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Galaxy
{
    public class GameManager : MonoBehaviour
    {

        [SerializeField] private GameObject neutralNode;

        public Material redTeam;
        public Material blueTeam;
        public Material orangeTeam;
        public Material greenTeam;
        public Material neutral;
        

        // Start is called before the first frame update
        void Start()
        {
            //SpawnNodes(-400, -400, 800, 800, 50, 150);
        }

        // Update is called once per frame
        void Update()
        {

        }

        /// <summary>
        /// Randomly spawn nodes around the map
        /// </summary>
        /// <param name="leftBounds">Left most bounds of the spawn area</param>
        /// <param name="bottomBounds">Bottom most bounds of the spawn area</param>
        /// <param name="width">How wide the spawn area is</param>
        /// <param name="height">How tall the spawn area is</param>
        /// <param name="minSpacing">The minimum spacing between nodes</param>
        /// <param name="maxSpacing">The maximum spacing between nodes</param>
        private void SpawnNodes(int leftBounds, int bottomBounds, int width, int height, float minSpacing, float maxSpacing)
        {
            for (int row = leftBounds; row <= width / 2; row += (int)Random.Range(minSpacing, maxSpacing))
            {
                for (int col = bottomBounds; col <= height / 2; col += (int)Random.Range(minSpacing, maxSpacing))
                {
                    if (row == leftBounds && col == bottomBounds)
                        continue;

                    GameObject newNode = GameObject.Instantiate(neutralNode);
                    newNode.transform.position = new Vector3(row, 0, col);
                }
            }
        }
    }
}
