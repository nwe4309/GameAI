using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Galaxy
{
    public class Detector : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnTriggerEnter(Collider other)
        {
            // If colliding with a node
            // Also only add new nodes to the list if the ship is in the detect state
            if (other.CompareTag("Node") && transform.parent.gameObject.GetComponent<Ship>().currentState == Enums.ShipState.Detect)
            {
                transform.parent.gameObject.GetComponent<Ship>().nearbyNodes.Add(other.gameObject);
            }
        }
    }
}