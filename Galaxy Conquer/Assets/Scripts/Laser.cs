using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Galaxy
{
    public class Laser : MonoBehaviour
    {
        [SerializeField] public Enums.Team currentOwner;

        [SerializeField] private float speed;
        [SerializeField] public int damage;
        [SerializeField] private float lifeTime;

        private float timer;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            timer += Time.deltaTime;

            if (timer >= lifeTime)
            {
                timer = 0;
                Destroy(gameObject);
            }

            transform.Translate(0, speed*Time.deltaTime, 0);
        }
    }
}
