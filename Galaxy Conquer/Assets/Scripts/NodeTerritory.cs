using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Galaxy
{
    public class NodeTerritory : MonoBehaviour
    {
        [SerializeField] public Enums.Team currentOwner;
        [SerializeField] protected float captureProgress;


        [SerializeField] protected float captureThreshold;

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
            }
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
