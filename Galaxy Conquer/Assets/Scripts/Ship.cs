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

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            transform.Translate(0, 0, 50 * Time.deltaTime);

            timer += Time.deltaTime;

            if(timer >= 2)
            {
                Shoot();
                timer = 0;
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

        private void OnTriggerEnter(Collider other)
        {
            if(other.CompareTag("Laser"))
            {
                if (other.gameObject.GetComponent<Laser>().currentOwner != currentOwner)
                {
                    Debug.Log("HIT " + name);
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