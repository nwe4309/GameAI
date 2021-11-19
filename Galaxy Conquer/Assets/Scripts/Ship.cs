using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Galaxy
{
    public class Ship : MonoBehaviour
    {
        [SerializeField] public Enums.Team currentOwner;
        [SerializeField] private int health;

        [SerializeField] private GameObject laserShot;

        private float timer;

        [SerializeField] public List<GameObject> nearbyNodes;
        private GameObject nodeDetector;
        private float startingRadius;

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
                    // If not doing anything then start detecting the nearest nodes
                    currentState = Enums.ShipState.Detect;
                    break;
                case Enums.ShipState.Detect:
                    // Increase search radius until at least 3 nodes are detected within range of the ship
                    if (nearbyNodes.Count < 3)
                    {
                        DetectNodes();
                    }
                    break;
                case Enums.ShipState.Move:
                    break;
                case Enums.ShipState.Capture:
                    break;
                case Enums.ShipState.Fight:
                    break;

            }
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