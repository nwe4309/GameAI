using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Galaxy
{
    public class Ship : MonoBehaviour
    {
        [SerializeField] public Enums.Team currentOwner;
        [SerializeField] private int health;
        [SerializeField] private float speed;
        [SerializeField] private float rotationSpeed;

        [SerializeField] private GameObject laserShot;

        private float timer;

        [SerializeField] public List<GameObject> nearbyNodes;
        private GameObject nodeDetector;
        private float startingRadius;

        [SerializeField] private GameObject closestNode;

        [SerializeField] private GameObject targetNode;

        [SerializeField] public Enums.ShipState currentState;

        // Start is called before the first frame update
        void Start()
        {
            currentState = Enums.ShipState.Idle;
            nodeDetector = transform.GetChild(1).gameObject;
            startingRadius = nodeDetector.GetComponent<SphereCollider>().radius;
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
                        currentState = Enums.ShipState.Move;
                    }
                    break;
                case Enums.ShipState.Move:
                    if (targetNode == null)
                        DetermineTarget();

                    if(Vector3.Distance(transform.position,targetNode.transform.position) >= 8)
                    {
                        SeekTarget(targetNode.transform);
                    }

                    break;
                case Enums.ShipState.Capture:
                    break;
                case Enums.ShipState.Fight:
                    break;

            }
        }

        private void SeekTarget(Transform target)
        {
            Vector3 direction = target.position - transform.position;

            transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(direction), rotationSpeed * Time.deltaTime);
        }

        public void Shoot()
        {
            GameObject newShot = GameObject.Instantiate(laserShot,transform.position + transform.forward ,transform.rotation);
            newShot.GetComponent<Laser>().currentOwner = currentOwner;
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
        }

        private void OnTriggerEnter(Collider other)
        {
            if(other.CompareTag("Laser"))
            {
                if (other.gameObject.GetComponent<Laser>().currentOwner != currentOwner)
                {
                    health -= other.gameObject.GetComponent<Laser>().damage;
                    Destroy(other.gameObject);

                    // If the ship reaches zero health, destroy it
                    if(health <= 0)
                    {
                        Destroy(gameObject);
                    }
                }
            }
        }
    }
}