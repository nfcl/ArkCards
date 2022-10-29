using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StartScene
{
    namespace StartCanvas
    {
        public class StartSceneStartCanvasManager : MonoBehaviour
        {
            private PannelChange _pannelChange;

            public void Start()
            {
                PannelChangeInit();
            }

            /// <summary>
            /// 初始化场景切换类
            /// </summary>
            private void PannelChangeInit()
            {
                _pannelChange = new PannelChange();
                _pannelChange.AddPannel("Title", transform.Find("TitlePannel").GetComponent<CanvasGroup>());
                _pannelChange.AddPannel("Main", transform.Find("MainPannel").GetComponent<CanvasGroup>());
                _pannelChange.AddPannel("Option", transform.Find("OptionPannel").GetComponent<CanvasGroup>());
                _pannelChange.ChangeToPannel("Title");
            }

            /// <summary>
            /// 切换显示的Pannel
            /// </summary>
            /// <param name="name">要切换到的Pannel名称</param>
            public void TogglePannel(string name)
            {
#if UNITY_EDITOR
                if (false == _pannelChange.ChangeToPannel(name))
                {
                    Debug.Log($"StartSceneStartCanvasManager尝试切换到{{{name}}}失败");
                }
#else
                _pannelChange.ChangeToPannel(name);
#endif
            }

            /// <summary>
            /// <para/>退出
            /// <para/>直接退出程序
            /// </summary>
            public void Exit()
            {
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#else
    Application.Quit();
#endif
            }
        }
    }
}