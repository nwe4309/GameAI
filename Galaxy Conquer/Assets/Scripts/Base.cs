using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Galaxy
{
    public class Base : NodeTerritory
    {
        [SerializeField] private GameObject redShip;
        [SerializeField] private GameObject blueShip;
        [SerializeField] private GameObject orangeShip;
        [SerializeField] private GameObject greenShip;

        [SerializeField] private float minSpawnTime, maxSpawnTime;

        [SerializeField] private float currentSpawnTime;
        private float timer;

        // Start is called before the first frame update
        protected override void Start()
        {
            base.Start();
            currentSpawnTime = Random.Range(minSpawnTime, maxSpawnTime);
        }

        // Update is called once per frame
        protected override void Update()
        {
            base.Update();

            timer += Time.deltaTime;

            if(timer >= currentSpawnTime)
            {
                // Spawn the ship of whatever currently owns it
                switch (currentOwner)
                {
                    case Enums.Team.Red:
                        SpawnUnit(redShip);
                        break;
                    case Enums.Team.Blue:
                        SpawnUnit(blueShip);
                        break;
                    case Enums.Team.Orange:
                        SpawnUnit(orangeShip);
                        break;
                    case Enums.Team.Green:
                        SpawnUnit(greenShip);
                        break;
                }
            }
        }

        public void SpawnUnit(GameObject shipToSpawn)
        {
            // Reset spawn timer and re-randomize spawn time
            // Done here so if you spawn a unit outside of the timer, it doesn't spawn another one really quickly
            timer = 0;
            currentSpawnTime = Random.Range(minSpawnTime, maxSpawnTime);

            // Spawn the new ship in front of the base
            GameObject newShip = GameObject.Instantiate(shipToSpawn,transform.position + (transform.forward*transform.localScale.x), transform.rotation);
            newShip.GetComponent<Ship>().currentTeam = currentOwner;
            newShip.name = currentOwner.ToString() + " Team Ship";
        }
    }
}
