namespace GameScene
{
    namespace Operator
    {
        /// <summary>
        /// ��Ա״̬��ö��
        /// </summary>
        public enum OperatorState
        {
            Team            = 0b0001,   //��С����
            Building        = 0b0010,   //�ڽ�����
            Died            = 0b0100,   // ������
            Other           = 0b1000,   //�������     
        }
    }
}