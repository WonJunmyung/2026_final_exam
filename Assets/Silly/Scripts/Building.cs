using System;
using UnityEngine;

namespace Silly
{
    public enum BuildData
    {
        /// <summary>
        /// 먹이통
        /// 포만도 회복
        /// 10
        /// 드래그 가능
        /// 동물이 먹으면 자동 제거
        /// </summary>
        EatBox,
        /// <summary>
        /// 물통
        /// 수분유지 회복
        /// 10
        /// 드래그 가능
        /// 동물이 마시면 자동 제거
        /// 오아시스 포함
        /// </summary>
        WaterBox,
        /// <summary>
        /// 나무
        /// 체력 회복
        /// 게임시간 30분마다 보유 중인 나무 중 임의의 하나에서 랜덤아이템박스 생성
        /// 20
        /// 드래그 가능
        /// 제거되지 않음
        /// </summary>
        Tree
        ///// 오아시스
        ///// 수분유지 회복
        ///// (고정 배치)
        ///// 드래그 불가능
        //Oasis
    }
    public class Building : MonoBehaviour
    {
        public Vector2Int size = new Vector2Int(1,1);
        public BuildData buildData = BuildData.EatBox;

        public void SetBuilding(BuildData buildNum)
        {
            buildData = buildNum;
            int buildSize = 0;
            switch (buildNum)
            {
                case BuildData.EatBox:
                    buildSize = 1;
                    break;
                case BuildData.WaterBox:
                    buildSize = 1;
                    break;
                case BuildData.Tree:
                    buildSize = 1;
                    break;
                //case BuildData.Oasis:
                //    buildSize = 1;
                //    break;
            }
            
            size = new Vector2Int(buildSize, buildSize);
            this.transform.localScale *= buildSize;
        }

        public void SetColor(Color buildColor)
        {
            this.transform.GetChild(0).GetComponent<Renderer>().material.color = buildColor;
        }


        public void SetCurrentPos()
        {
            Vector2Int currentPos = new Vector2Int(Mathf.RoundToInt(this.transform.position.x), Mathf.RoundToInt(this.transform.position.z));

            this.transform.position = new Vector3(currentPos.x, 0, currentPos.y);

        }

        public void PlayBuilding(Animal animal)
        {
            switch (buildData)
            {
                case BuildData.EatBox:
                    Destroy(this.gameObject);
                    break;
                case BuildData.WaterBox:
                    Destroy(this.gameObject);
                    break;
                case BuildData.Tree:
                    
                    break;
                //case BuildData.Oasis:
                //    break;
            }
        }
    }
}
