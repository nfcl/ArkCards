using GameScene.Operator;
using System.Collections.Generic;

namespace GameScene
{
    namespace Team
    {
        /// <summary>
        /// ����������
        /// �洢��һ֧С�ӵ���Ϣ
        /// </summary>
        public class SingleTeam
        {
            /// <summary>
            /// С������
            /// </summary>
            private string _teamName;
            /// <summary>
            /// <para/>С�ӳ�Ա
            /// </summary>
            private List<SingleOperator> _members;

            public string TeamName { get { return _teamName; } }
        }
    }
}