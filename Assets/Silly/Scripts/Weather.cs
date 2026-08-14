using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

namespace Silly
{
    public enum WeatherState
    {
        rain,       // ∫Ò
        blur,       // »Â∏≤
        Thunder,     // √µµ’
        none,       // ∏º¿Ω
        
        
    }

    
    public class Weather : MonoBehaviour
    {
        public static Weather Instance;

        public GameObject[] objWeather;

        public WeatherState weatherState = WeatherState.none;

        GameObject groundThunder;

        private void Awake()
        {
            Instance = this;
        }
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
        }

        public void SetDayEvent()
        {
            weatherState = (WeatherState)UnityEngine.Random.Range(0, (int)WeatherState.none + 1);
            foreach (GameObject weather in objWeather)
            {
                weather.SetActive(false);
            }
            if (weatherState < WeatherState.Thunder)
            {
                objWeather[(int)weatherState].SetActive(true);
            }
            else if (weatherState == WeatherState.Thunder)
            {
                GameObject groundThunder = MapController.Instance.GetRandomPosition();
                groundThunder.transform.GetChild(0).GetComponent<Renderer>().material.color = UnityEngine.Color.blue;
                objWeather[(int)WeatherState.Thunder].transform.position = groundThunder.transform.position;
                //Invoke("Thunder", 3.0f);
                StartCoroutine(Thunder());
            }
        }

        IEnumerator Thunder()
        {
            yield return new WaitForSeconds(3.0f);
            objWeather[(int)WeatherState.Thunder].SetActive(true);
            yield return new WaitForSeconds(2.0f);
            objWeather[(int)WeatherState.Thunder].SetActive(false);
            groundThunder.transform.GetChild(0).GetComponent<Renderer>().material.color = UnityEngine.Color.black;
            
        }

    }
}
