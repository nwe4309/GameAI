using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Galaxy
{
    public class Base : NodeTerritory
    {
        [SerializeField] private GameObject ship;

        [SerializeField] private float minSpawnTime, maxSpawnTime;

        [SerializeField] private float currentSpawnTime;
        private float timer;

        // Start is called before the first frame update
        void Start()
        {
            currentSpawnTime = Random.Range(minSpawnTime, maxSpawnTime);
        }

        // Update is called once per frame
        void Update()
        {
            timer += Time.deltaTime;

            if(timer >= currentSpawnTime)
            {
                SpawnUnit();
            }
        }

        public void SpawnUnit()
        {
            // Reset spawn timer and re-randomize spawn time
            // Done here so if you spawn a unit outside of the timer, it doesn't spawn another one really quickly
            timer = 0;
            currentSpawnTime = Random.Range(minSpawnTime, maxSpawnTime);

            // Spawn the new ship to the left of the base
            GameObject newShip = GameObject.Instantiate(ship);
            newShip.transform.position = new Vector3(transform.position.x - transform.localScale.x - newShip.transform.localScale.x, 0, transform.position.z);
            newShip.GetComponent<Ship>().currentOwner = currentOwner;
            newShip.name = currentOwner.ToString() + " Team Ship";
        }
    }
}
