namespace GameScene
{
    namespace Operator
    {
        /// <summary>
        /// 干员状态的枚举
        /// </summary>
        public enum OperatorState
        {
            Team            = 0b0001,   //在小队中
            Building        = 0b0010,   //在建筑中
            Died            = 0b0100,   // 已死亡
            Other           = 0b1000,   //其他情况     
        }
    }
}