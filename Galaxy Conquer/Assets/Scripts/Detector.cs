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
            // If collding with a node
            if (other.CompareTag("Node"))
            {
                gameObject.transform.parent.gameObject.GetComponent<Ship>().nearbyNodes.Add(other.gameObject);
            }
        }
    }
}