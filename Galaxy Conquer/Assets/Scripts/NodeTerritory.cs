using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Galaxy
{
    public class NodeTerritory : MonoBehaviour
    {
        protected GameManager gameManager;
        [SerializeField] public Enums.Team currentOwner;
        [SerializeField] protected float captureProgress;


        [SerializeField] protected float captureThreshold;

        [SerializeField] public List<GameObject> shipsNearby;
        public bool canCapture;

        public float CaptureProgress
        {
            get { return captureProgress; }
        }

        public void IncreaseCaptureProgress(float amount, Enums.Team team)
        {
            captureProgress += amount * Time.deltaTime;
            
            if(captureProgress >= captureThreshold)
            {
                captureProgress = 0;
                currentOwner = team;

                Renderer renderer = GetComponent<Renderer>();
                switch (currentOwner)
                {
                    case Enums.Team.Red:
                        renderer.material = gameManager.redTeam;
                        break;
                    case Enums.Team.Blue:
                        renderer.material = gameManager.blueTeam;
                        break;
                    case Enums.Team.Orange:
                        renderer.material = gameManager.orangeTeam;
                        break;
                    case Enums.Team.Green:
                        renderer.material = gameManager.greenTeam;
                        break;
                    case Enums.Team.Neutral:
                        renderer.material = gameManager.neutral;
                        break;

                }
            }
        }

        // Start is called before the first frame update
        protected virtual void Start()
        {
            gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
            canCapture = true;
        }

        // Update is called once per frame
        protected virtual void Update()
        {
            if (shipsNearby.Count > 0)
            {
                Enums.Team capturingTeam = shipsNearby[0].GetComponent<Ship>().currentTeam;
                int sameTeam = 0;
                // Loop through nearby ships backwards to account for removal
                for (int i = shipsNearby.Count - 1; i >= 0; i--)
                {
                    // If there are multiple teams at the same node, halt capturing progress
                    if(shipsNearby[i].GetComponent<Ship>().currentTeam != capturingTeam)
                    {
                        canCapture = false;

                        // Send the gamemanager an update list of nearby ships and break out of the loop
                        gameManager.HandleCombat(shipsNearby);
                        break;
                    }
                    else
                    {
                        sameTeam++;
                    }
                }

                // If all the ships are from the same team, it can be captured
                if(sameTeam == shipsNearby.Count)
                {
                    canCapture = true;
                }
            }
        }
    }
}
