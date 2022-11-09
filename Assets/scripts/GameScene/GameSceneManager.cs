using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameScene
{
    public class GameSceneManager : MonoBehaviour
    {
        private Operator.OperatorCollection     _operators;             //��Ա��Ϣ
        private Team.TeamCollection             _teams;                 //С����Ϣ

        public void Start()
        {
            //��ͼ��ʼ��
            MapInit();
            //��Ա��ʼ��
            OperatorInit();
            //С�ӳ�ʼ��
            TeamInit();
        }

        /// <summary>
        /// ��ͼ��ʼ��
        /// </summary>
        public void MapInit()
        {

        }

        /// <summary>
        /// ��Ա��Ϣ��ʼ��
        /// </summary>
        public void OperatorInit()
        {
            //��Ա���ϳ�ʼ��
            _operators = new Operator.OperatorCollection();
            //���ļ���ȡ��Ҹ�Ա��Ϣ
            //TODO
        }

        /// <summary>
        /// С����Ϣ��ʼ��
        /// </summary>
        public void TeamInit()
        {
            //С�Ӽ��ϳ�ʼ��
            _teams = new Team.TeamCollection();
            //���ļ���ȡ���С����Ϣ
            //TODO
        }
    }
}