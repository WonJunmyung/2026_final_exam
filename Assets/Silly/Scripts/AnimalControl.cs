using Codice.Client.BaseCommands;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Silly
{
    public enum AnimalName
    {
        Dog = 0,    // °­¾ÆÁö
        Cat,    // °í¾çÀÌ
        Wolf,   // ´Á´ë
        Fox,// ¿©¿ì
        Deer,   // »ç½¿
        Bear,   // °õ
        Tiger,   // È£¶ûÀÌ
        horse,   // ¸»
        Elephant,   // ÄÚ³¢¸®
        Dinosaur    // °ø·æ
    }
    
    


    public class AnimalControl : MonoBehaviour
    {
        public static AnimalControl Instance;
        public GameObject[] animalPrefab;

        public Animal currentAnimal = null;
        public GameObject Choice;

        private void Awake()
        {
            Instance = this;
        }
        // Start is called before the first frame update
        void Start()
        {
            //for (int i = 0; i < 2; i++)
            //{
            //    Vector2Int currentPos = new Vector2Int(i * 2, i * 2);
            //    Animal animal = Instantiate(animalPrifab[0], new Vector3(currentPos.x, 0, currentPos.y), animalPrifab[0].transform.rotation).GetComponent<Animal>();
            //    animal.animalName = AnimalName.Dog;
            //    animal.gameObject.name = "µ¿¹°" + i;
                
            //}
        }

        

        // Update is called once per frame
        void Update()
        {
            if(currentAnimal == null)
            {
                Choice.SetActive(false);
                return;
            }
            Choice.SetActive(true);
            Choice.transform.position = new Vector3(currentAnimal.transform.position.x, Choice.transform.position.y, currentAnimal.transform.position.z);
        }

        
    }
}
