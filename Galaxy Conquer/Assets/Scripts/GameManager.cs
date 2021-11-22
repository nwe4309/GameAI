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

        private List<GameObject> redTeamShips;
        private List<GameObject> blueTeamShips;
        private List<GameObject> orangeTeamShips;
        private List<GameObject> greenTeamShips;

        // Start is called before the first frame update
        void Start()
        {
            redTeamShips = new List<GameObject>();
            blueTeamShips = new List<GameObject>();
            orangeTeamShips = new List<GameObject>();
            greenTeamShips = new List<GameObject>();

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

        public void HandleCombat(List<GameObject> ships)
        {
            // Set each ship in the list to the fight state and "sort" them by team
            foreach(GameObject ship in ships)
            {
                Ship currentShip = ship.GetComponent<Ship>();
                currentShip.currentState = Enums.ShipState.Fight;

                //switch (currentShip.currentTeam)
                //{
                //    case Enums.Team.Red:
                //        redTeamShips.Add(ship);
                //        break;
                //    case Enums.Team.Blue:
                //        blueTeamShips.Add(ship);
                //        break;
                //    case Enums.Team.Orange:
                //        orangeTeamShips.Add(ship);
                //        break;
                //    case Enums.Team.Green:
                //        greenTeamShips.Add(ship);
                //        break;
                //}
            }

            // Tell each ship who to fight (the closest enemy)
            AssignTargets(ships);
        }

        private void AssignTargets(List<GameObject> ships)
        {
            foreach(GameObject ship in ships)
            {
                GameObject closestEnemy = null;
                
                // Find closest enemy
                foreach(GameObject potentialEnemy in ships)
                {
                    // If the two ships being looked at are not on the same team
                    if(potentialEnemy.GetComponent<Ship>().currentTeam != ship.GetComponent<Ship>().currentTeam)
                    {
                        // If this is the first enemy in the list or it's closer to the current ship, set it as closest
                        if(closestEnemy == null || Vector3.Distance(ship.transform.position, potentialEnemy.transform.position) < Vector3.Distance(ship.transform.position, closestEnemy.transform.position))
                        {
                            closestEnemy = potentialEnemy;
                        }
                    }
                }

                // Should always have a closest enemy since this method won't be called unless there are rivaling ships
                ship.GetComponent<Ship>().targetShip = closestEnemy;
            }
        }

    }
}
