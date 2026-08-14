using UnityEngine;
using UnityEngine.UI;

namespace Silly
{
    public class UIMenu : MonoBehaviour
    {
        public static UIMenu Instance;
        // 현실 1분 = 게임 속 24분, 현실 1시간(3600초) = 게임 속 24시간(1440분).
        public float timeCal = 24.0f;
        public float gameTime = 0f;
        public float tempTime = 0;
        public Text timeText;
        float secondDay = 86400f;
        int day = 0;
        public GameObject UIlv;
        public GameObject UIStatus;
        public Text Point;
        public int point = 100;
        public int calMinute = 0;

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
            gameTime += Time.deltaTime * timeCal;
            tempTime += Time.deltaTime * timeCal;
            int tempDay = ((int)gameTime / (int)secondDay);

            int hour = Mathf.FloorToInt(gameTime / 3600f);
            int minute = Mathf.FloorToInt((gameTime % 3600f)/60f);
            int second = Mathf.FloorToInt((gameTime % 3600f) % 60f );
            timeText.text = day + "일 - " + hour + ":" + minute + ":" + second;

            if(day < tempDay)
            {
                day = tempDay;
                Weather.Instance.SetDayEvent();
            }
            if(Mathf.FloorToInt((tempTime % 3600f) / 60f) > 9)
            {
                point += 1;
                tempTime = 0;

            }
            SetPoint();
            SetUIState();
        }

        public void OpenLv(int lv)
        {
            
            Time.timeScale = 0;
            UIlv.transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
            UIlv.transform.GetChild(1).GetChild(0).gameObject.SetActive(true);
            UIlv.transform.GetChild(2).GetChild(0).gameObject.SetActive(true);
            switch (lv)
            {
                case 1:
                    UIlv.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = "늑대";
                    UIlv.transform.GetChild(1).GetChild(0).GetComponent<Text>().text = "여우";
                    UIlv.transform.GetChild(2).GetChild(0).GetComponent<Text>().text = "사슴";

                    break;
                case 2:
                    UIlv.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = "곰";
                    UIlv.transform.GetChild(1).GetChild(0).GetComponent<Text>().text = "호랑이";
                    UIlv.transform.GetChild(2).GetChild(0).GetComponent<Text>().text = "말";
                    break;
                case 3:
                    UIlv.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = "코끼리";
                    UIlv.transform.GetChild(1).gameObject.SetActive(false);
                    UIlv.transform.GetChild(2).gameObject.SetActive(false);
                    break;
                case 4:
                    UIlv.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = "공룡";
                    UIlv.transform.GetChild(1).gameObject.SetActive(false);
                    UIlv.transform.GetChild(2).gameObject.SetActive(false);
                    break;

            }
        }

        public void SetPoint()
        {
            Point.text = point.ToString();
        }

        public void SetUIState()
        {
            if(AnimalControl.Instance.currentAnimal == null)
            {
                return;
            }
            else
            {
                UIStatus.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = "레벨 : " + AnimalControl.Instance.currentAnimal.lv;
                UIStatus.transform.GetChild(1).GetChild(0).GetComponent<Text>().text = "경험치 : " + AnimalControl.Instance.currentAnimal.exp;
                UIStatus.transform.GetChild(2).GetChild(0).GetComponent<Text>().text = "배고픔 : " + AnimalControl.Instance.currentAnimal.hunger;
                UIStatus.transform.GetChild(3).GetChild(0).GetComponent<Text>().text = "목마름 : " + AnimalControl.Instance.currentAnimal.water;
                UIStatus.transform.GetChild(4).GetChild(0).GetComponent<Text>().text = "체력 : " + AnimalControl.Instance.currentAnimal.hp;
                


            }
        }
    }
}
