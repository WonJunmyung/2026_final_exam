using Codice.Client.BaseCommands;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Silly
{
    public class AnimalControl : MonoBehaviour
    {
        public GameObject[] animalPrifab;

        // Start is called before the first frame update
        void Start()
        {
            for (int i = 0; i < 2; i++)
            {
                Vector2Int currentPos = new Vector2Int(i * 2, i * 2);
                Animal animal = Instantiate(animalPrifab[0], new Vector3(currentPos.x, 0, currentPos.y), animalPrifab[0].transform.rotation).GetComponent<Animal>();
                animal.gameObject.name = "µ¿¹°" + i;
                
            }
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
