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

        [SerializeField] private GameObject laserShot;

        private float timer;

        [SerializeField] public List<GameObject> nearbyNodes;
        private GameObject nodeDetector;
        private float startingDetectorRadius;

        [SerializeField] private GameObject closestNode;

        [SerializeField] private GameObject targetNode;

        [SerializeField] public Enums.ShipState currentState;

        private NavMeshAgent navMeshAgent;

        // Start is called before the first frame update
        void Start()
        {
            currentState = Enums.ShipState.Idle;
            nodeDetector = transform.GetChild(1).gameObject;
            startingDetectorRadius = nodeDetector.GetComponent<SphereCollider>().radius;

            navMeshAgent = GetComponent<NavMeshAgent>();
            navMeshAgent.baseOffset = 0;
        }

        // Update is called once per frame
        void Update()
        {
            //transform.Translate(0, 0, 50 * Time.deltaTime);

            timer += Time.deltaTime;

            if(timer >= 2)
            {
                //Shoot();
                timer = 0;
            }

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
                        if(targetNode != null)
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
                        
                            // If the target node is already captured, go back to idle
                            if(targetNode.GetComponent<NodeTerritory>().currentOwner == currentTeam)
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

                    if (targetNode.GetComponent<NodeTerritory>().currentOwner == currentTeam)
                    {
                        currentState = Enums.ShipState.Idle;
                    }
                    else
                    {
                        Debug.Log("not same team " + targetNode.name);
                        if (targetNode.GetComponent<NodeTerritory>().canCapture == true)
                        {
                            Debug.Log("capture? " + targetNode.name);
                            targetNode.GetComponent<NodeTerritory>().IncreaseCaptureProgress(1, currentTeam);
                        }
                    }

                    break;
                case Enums.ShipState.Fight:
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

        public void Shoot()
        {
            GameObject newShot = GameObject.Instantiate(laserShot,transform.position + transform.forward ,transform.rotation);
            newShot.GetComponent<Laser>().currentOwner = currentTeam;
            newShot.transform.Rotate(new Vector3(90, 0, 0));
            //newShot.transform.position = transform.position;
            //newShot.transform.forward = transform.forward;
            //newShot.transform.TransformDirection(Vector3(0,0));
            //newShot.transform.rotation = transform.rotation;

        }

        private void DetectNodes()
        {
            nodeDetector.GetComponent<SphereCollider>().radius++;
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
                targetNode = nearbyNodes[Random.Range(0, nearbyNodes.Count)];
                return;
            }
            #endregion
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