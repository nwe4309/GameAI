using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Galaxy
{
    public class Ship : MonoBehaviour
    {
        [SerializeField] public Enums.Team currentTeam;
        [SerializeField] private int health;
        [SerializeField] private float speed;
        [SerializeField] private float rotationSpeed;
        [SerializeField] private float minShootSpeed;
        [SerializeField] private float maxShootSpeed;

        [SerializeField] private float chosenShootSpeed;

        [SerializeField] private GameObject laserShot;

        private float shootTimer;

        [SerializeField] public List<GameObject> nearbyNodes;
        private GameObject nodeDetector;
        private float startingDetectorRadius;

        [SerializeField] private GameObject closestNode;

        [SerializeField] private GameObject targetNode;
        [SerializeField] private GameObject previousTargetNode;

        [SerializeField] public Enums.ShipState currentState;

        private NavMeshAgent navMeshAgent;

        [SerializeField] public GameObject targetShip;

        // Start is called before the first frame update
        void Start()
        {
            currentState = Enums.ShipState.Idle;
            nodeDetector = transform.GetChild(1).gameObject;
            startingDetectorRadius = nodeDetector.GetComponent<SphereCollider>().radius;

            navMeshAgent = GetComponent<NavMeshAgent>();
            navMeshAgent.baseOffset = 0;

            chosenShootSpeed = Random.Range(minShootSpeed, maxShootSpeed);
            // Idk where to put this
            previousTargetNode = targetNode;
        }

        // Update is called once per frame
        void Update()
        {
            HandleStates();
        }

        private void HandleStates()
        {
            switch(currentState)
            {
                case Enums.ShipState.Idle:

                    if(targetNode != null)
                        targetNode.GetComponent<NodeTerritory>().shipsNearby.Remove(gameObject);
                    targetNode = null;
                    nearbyNodes.Clear();

                    // If not doing anything then start detecting the nearest nodes
                    currentState = Enums.ShipState.Detect;
                    break;
                case Enums.ShipState.Detect:
                    // Increase search radius until at least 3 nodes are detected within range of the ship
                    if (nearbyNodes.Count < 3)
                    {
                        DetectNodes();
                    }
                    else
                    {
                        FindClosestNode(nearbyNodes);
                        // Reset the detect radius
                        nodeDetector.GetComponent<SphereCollider>().radius = startingDetectorRadius;
                        currentState = Enums.ShipState.Move;
                    }
                    break;
                case Enums.ShipState.Move:
                    if (targetNode == null)
                    {
                        DetermineTarget();
                        if (targetNode != null)
                            nearbyNodes.Clear();
                    }
                    else
                    {
                        //Vector2 targetNodePostion = new Vector2(targetNode.transform.position.x - targetNode.transform.localScale.x/2, targetNode.transform.position.z - targetNode.transform.localScale.z/2);
                        //targetNode.GetComponent<Renderer>().bounds.ClosestPoint(transform.position);
                        // If not close to the target node
                        // Also try to account for size of target
                        if (Vector3.Distance(transform.position, targetNode.transform.position) - targetNode.GetComponent<Renderer>().bounds.size.x/2 >= 30)
                        {
                            // Seek it
                            SeekTarget(targetNode.transform);
                        }
                        else
                        {
                            // You've reached the target node
                        
                            // If the target node is already captured and not being captured by the enemy team, go back to idle
                            if(targetNode.GetComponent<NodeTerritory>().currentOwner == currentTeam && targetNode.GetComponent<NodeTerritory>().CaptureProgress == 0)
                            {
                                currentState = Enums.ShipState.Idle;
                            }
                            else
                            {
                                // Otherwise capture it
                                currentState = Enums.ShipState.Capture;
                                targetNode.GetComponent<NodeTerritory>().shipsNearby.Add(gameObject);
                            }
                        }
                    }

                    break;
                case Enums.ShipState.Capture:

                    if (targetNode.GetComponent<NodeTerritory>().currentOwner == currentTeam && targetNode.GetComponent<NodeTerritory>().CaptureProgress == 0)
                    {
                        currentState = Enums.ShipState.Idle;
                    }
                    else
                    {
                        if (targetNode.GetComponent<NodeTerritory>().canCapture == true)
                        {
                            targetNode.GetComponent<NodeTerritory>().IncreaseCaptureProgress(1, currentTeam);
                        }
                    }

                    break;
                case Enums.ShipState.Fight:

                    if(targetShip == null)
                    {
                        currentState = Enums.ShipState.Idle;
                        break;
                    }

                    // Play with turn speed for fights
                    SeekTarget(targetShip.transform);

                    if (shootTimer >= chosenShootSpeed)
                    {
                        Shoot();
                        shootTimer = 0;
                        // Reassign shoot speed
                        chosenShootSpeed = Random.Range(minShootSpeed, maxShootSpeed);
                    }

                    shootTimer += Time.deltaTime;
                    break;

            }
        }

        private void SeekTarget(Transform target)
        {
            //Vector3 direction = target.position - transform.position;
            //
            //transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
            //transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(direction), rotationSpeed * Time.deltaTime);

            navMeshAgent.destination = target.position;
        }

        private void LookAtTarget(Transform target)
        {
            Vector3 direction = target.position - transform.position;
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(direction), rotationSpeed * Time.deltaTime);
        }

        public void Shoot()
        {
            GameObject newShot = GameObject.Instantiate(laserShot,transform.position + transform.forward ,transform.rotation);
            newShot.GetComponent<Laser>().currentOwner = currentTeam;
            newShot.transform.Rotate(new Vector3(90, 0, 0));
        }

        private void DetectNodes()
        {
            nodeDetector.GetComponent<SphereCollider>().radius += 100 * Time.deltaTime;
        }

        private void FindClosestNode(List<GameObject> nodeList)
        {
            closestNode = nearbyNodes[0];
            foreach (GameObject node in nodeList)
            {
                if(Vector3.Distance(transform.position,node.transform.position) <= Vector3.Distance(transform.position, closestNode.transform.position))
                {
                    closestNode = node;
                }
            }
        }
        private void DetermineTarget()
        {
            // Check if all the nodes are neutral with no capture progress
            #region Neutral Nodes with no capture progress
            int neutralNoProgCounter = 0;
            foreach (GameObject node in nearbyNodes)
            {
                if (node.GetComponent<NodeTerritory>().currentOwner == Enums.Team.Neutral && node.GetComponent<NodeTerritory>().CaptureProgress == 0)
                    neutralNoProgCounter++;
            }
            // If all are neutral with no progress, pick a random one
            if(neutralNoProgCounter == nearbyNodes.Count)
            {
                targetNode = nearbyNodes[Random.Range(0, nearbyNodes.Count)];
                return;
            }
            #endregion

            // Check if any are neutral and pick the one with the most capture progress
            #region Any neutral
            List<GameObject> neutralNodes = new List<GameObject>();
            foreach(GameObject node in nearbyNodes)
            {
                if(node.GetComponent<NodeTerritory>().currentOwner == Enums.Team.Neutral)
                {
                    neutralNodes.Add(node);
                }
            }
            if (neutralNodes.Count > 0)
            {
                // Find the closest node that has the most current progress
                GameObject closestNodeWithProg = neutralNodes[0];
                foreach (GameObject node in neutralNodes)
                {
                    if (Vector3.Distance(transform.position, node.transform.position) <= Vector3.Distance(transform.position, closestNodeWithProg.transform.position) && node.GetComponent<NodeTerritory>().CaptureProgress >= closestNodeWithProg.GetComponent<NodeTerritory>().CaptureProgress)
                    {
                        closestNodeWithProg = node;
                    }
                }

                targetNode = closestNodeWithProg;
                return;
            }
            #endregion

            // Check if any have capture progress
            #region Any with capture progress
            List<GameObject> nodesWithCaptureProgress = new List<GameObject>();
            foreach (GameObject node in nearbyNodes)
            {
                // If any are currently being captured
                if (node.GetComponent<NodeTerritory>().CaptureProgress != 0)
                {
                    nodesWithCaptureProgress.Add(node);
                }
                    
            }
            // If any
            if (nodesWithCaptureProgress.Count > 0)
            {
                // Find the closest node
                GameObject closestCaptureProgressNode = nodesWithCaptureProgress[0];
                foreach (GameObject node in nodesWithCaptureProgress)
                {
                    if (Vector3.Distance(transform.position, node.transform.position) <= Vector3.Distance(transform.position, closestCaptureProgressNode.transform.position))
                    {
                        closestCaptureProgressNode = node;
                    }
                }

                targetNode = closestCaptureProgressNode;
                return;
            }
            #endregion

            // Check if all the nodes are on the same team as this ship
            #region All on current team
            int currentTeamCounter = 0;
            foreach (GameObject node in nearbyNodes)
            {
                if (node.GetComponent<NodeTerritory>().currentOwner == currentTeam)
                    currentTeamCounter++;
            }
            // If all on the same team, pick a random node
            if(currentTeamCounter == nearbyNodes.Count)
            {
                GameObject tempNode = nearbyNodes[Random.Range(0, nearbyNodes.Count)];
                if (tempNode != previousTargetNode)
                {
                    targetNode = tempNode;
                }
                return;
            }
            #endregion

            // Check if all of them are on an enemy team
            #region All on enemy team
            int enemyTeamCounter = 0;
            GameObject closestEnemyNode = null;
            foreach (GameObject node in nearbyNodes)
            {
                // If the node is of an enemy team
                if (node.GetComponent<NodeTerritory>().currentOwner != currentTeam)
                {
                    // Incement the counter
                    enemyTeamCounter++;

                    // Keep track of the closest enemy node
                    if(closestEnemyNode == null || Vector3.Distance(transform.position,node.transform.position) < Vector3.Distance(transform.position, closestEnemyNode.transform.position))
                    {
                        closestEnemyNode = node;
                    }
                }
            }
            // If all on the enemy team, pick the closest node
            if (enemyTeamCounter == nearbyNodes.Count)
            {
                targetNode = closestEnemyNode;
                return;
            }
            #endregion

            // Check if any are on the enemy team
            #region Any on enemy team
            // Find all on the enemy team
            List<GameObject> enemyTeam = new List<GameObject>();
            foreach (GameObject node in nearbyNodes)
            {
                if (node.GetComponent<NodeTerritory>().currentOwner != currentTeam)
                {
                    enemyTeam.Add(node);
                }
            }
            // If any
            if (enemyTeam.Count > 0)
            {
                // Find the closest node
                GameObject closestEnemyTeamNode = enemyTeam[0];
                foreach (GameObject node in enemyTeam)
                {
                    if (Vector3.Distance(transform.position, node.transform.position) <= Vector3.Distance(transform.position, closestEnemyTeamNode.transform.position))
                    {
                        closestEnemyTeamNode = node;
                    }
                }

                targetNode = closestEnemyTeamNode;
                return;
            }
            #endregion

            // Check if any of them are on your team
            #region Any on your team
            // Find all on the same team
            List<GameObject> sameTeam = new List<GameObject>();
            foreach (GameObject node in nearbyNodes)
            {
                if (node.GetComponent<NodeTerritory>().currentOwner == currentTeam)
                {
                    sameTeam.Add(node);
                }
            }
            // If any
            if (sameTeam.Count > 0)
            {
                // Find the closest node
                GameObject closestSameTeamNode = sameTeam[0];
                foreach (GameObject node in sameTeam)
                {
                    if (Vector3.Distance(transform.position, node.transform.position) <= Vector3.Distance(transform.position, closestSameTeamNode.transform.position))
                    {
                        closestSameTeamNode = node;
                    }
                }

                targetNode = closestSameTeamNode;
                Debug.Log("Any on same");
                return;
            }
            #endregion


            Debug.Log("No path found :(((");
        }

        private void OnTriggerEnter(Collider other)
        {
            if(other.CompareTag("Laser"))
            {
                if (other.gameObject.GetComponent<Laser>().currentOwner != currentTeam)
                {
                    health -= other.gameObject.GetComponent<Laser>().damage;
                    Destroy(other.gameObject);

                    // If the ship reaches zero health, destroy it
                    if(health <= 0)
                    {
                        if(targetNode != null)
                        {
                            targetNode.GetComponent<NodeTerritory>().shipsNearby.Remove(gameObject);
                        }
                        Destroy(gameObject);
                    }
                }
            }
        }
    }
}