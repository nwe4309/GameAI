using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

        public GameObject[] nodesOnField;
        public List<GameObject> basesCaptured;

        private int redTeamPoints;
        private int blueTeamPoints;
        private int orangeTeamPoints;
        private int greenTeamPoints;

        [SerializeField] Text redPointsText;
        [SerializeField] Text bluePointsText;
        [SerializeField] Text orangePointsText;
        [SerializeField] Text greenPointsText;

        [SerializeField] private float numOfNodesModifier;


        // Start is called before the first frame update
        void Start()
        {
            //SpawnNodes(-400, -400, 800, 800, 50, 150);
            nodesOnField = GameObject.FindGameObjectsWithTag("Node");
            basesCaptured = new List<GameObject>();
        }

        // Update is called once per frame
        void Update()
        {
            // Populate UI with how many nodes belong to each team

            if(Input.GetKeyDown(KeyCode.Alpha1))
            {
                Time.timeScale = 1.0f;
                Debug.Log("1x Speed");
            }
            else if(Input.GetKeyDown(KeyCode.Alpha2))
            {
                Time.timeScale = 2.0f;
                Debug.Log("2x Speed");
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                Time.timeScale = 4.0f;
                Debug.Log("4x Speed");
            }
            else if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                Time.timeScale = 10.0f;
                Debug.Log("10x Speed");
            }

            redTeamPoints = 0;
            blueTeamPoints = 0;
            orangeTeamPoints = 0;
            greenTeamPoints = 0;
            basesCaptured.Clear();
            foreach(GameObject node in nodesOnField)
            {
                switch (node.GetComponent<NodeTerritory>().currentOwner)
                {
                    case Enums.Team.Red:
                        redTeamPoints++;
                        if(node.GetComponent<Base>() != null)
                        {
                            basesCaptured.Add(node);
                        }
                        break;
                    case Enums.Team.Blue:
                        blueTeamPoints++;
                        if (node.GetComponent<Base>() != null)
                        {
                            basesCaptured.Add(node);
                        }
                        break;
                    case Enums.Team.Orange:
                        orangeTeamPoints++;
                        if (node.GetComponent<Base>() != null)
                        {
                            basesCaptured.Add(node);
                        }
                        break;
                    case Enums.Team.Green:
                        greenTeamPoints++;
                        if (node.GetComponent<Base>() != null)
                        {
                            basesCaptured.Add(node);
                        }
                        break;
                }
            }

            redPointsText.text = "Red Team Points: " + redTeamPoints;
            bluePointsText.text = "Blue Team Points: " + blueTeamPoints;
            orangePointsText.text = "Orange Team Points: " + orangeTeamPoints;
            greenPointsText.text = "Green Team Points: " + greenTeamPoints;

            foreach(GameObject nodeBase in basesCaptured)
            {
                Base baseScript = nodeBase.GetComponent<Base>();
                switch (baseScript.currentOwner)
                {
                    case Enums.Team.Red:
                        baseScript.minSpawnTime = baseScript.originalMinSpawnTime - redTeamPoints * numOfNodesModifier;
                        baseScript.maxSpawnTime = baseScript.originalMaxSpawnTime - redTeamPoints * numOfNodesModifier;
                        break;
                    case Enums.Team.Blue:
                        baseScript.minSpawnTime = baseScript.originalMinSpawnTime - blueTeamPoints * numOfNodesModifier;
                        baseScript.maxSpawnTime = baseScript.originalMaxSpawnTime - blueTeamPoints * numOfNodesModifier;
                        break;
                    case Enums.Team.Orange:
                        baseScript.minSpawnTime = baseScript.originalMinSpawnTime - orangeTeamPoints * numOfNodesModifier;
                        baseScript.maxSpawnTime = baseScript.originalMaxSpawnTime - orangeTeamPoints * numOfNodesModifier;
                        break;
                    case Enums.Team.Green:
                        baseScript.minSpawnTime = baseScript.originalMinSpawnTime - greenTeamPoints * numOfNodesModifier;
                        baseScript.maxSpawnTime = baseScript.originalMaxSpawnTime - greenTeamPoints * numOfNodesModifier;
                        break;
                }
            }
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
