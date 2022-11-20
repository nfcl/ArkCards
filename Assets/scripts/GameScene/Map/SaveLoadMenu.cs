using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace GameScene.Map
{
    /// <summary>
    /// 保存和加载菜单
    /// </summary>
    public class SaveLoadMenu : MonoBehaviour
    {
        /// <summary>
        /// 加载和保存模式
        /// </summary>
        private bool saveMode;

        /// <summary>
        /// 地图网格
        /// </summary>
        public HexGrid hexGrid;
        /// <summary>
        /// 菜单名称文本框
        /// </summary>
        public Text menuLabel; 
        /// <summary>
        /// 保存和加载按钮文本框
        /// </summary>
        public Text actionButtonLabel;
        /// <summary>
        /// 用户输入
        /// </summary>
        public InputField nameInput;
        /// <summary>
        /// 
        /// </summary>
        public RectTransform listContent;
        /// <summary>
        /// 列表项预制件
        /// </summary>
        public SaveLoadItem itemPrefab;

        /// <summary>
        /// 完善用户输入的保存路径
        /// </summary>
        private string GetSelectedPath()
        {
            string mapName = nameInput.text;
            if (mapName.Length == 0)
            {
                return null;
            }
            return Path.Combine(Application.persistentDataPath, mapName + ".map");
        }
        /// <summary>
        /// 保存地图
        /// </summary>
        private void Save(string path)
        {
            using (
                BinaryWriter writer =
                    new BinaryWriter(File.Open(path, FileMode.Create))
            )
            {
                writer.Write(1);
                hexGrid.Save(writer);
            }
        }
        /// <summary>
        /// 加载地图
        /// </summary>
        private void Load(string path)
        {
            if (!File.Exists(path))
            {
                Debug.LogError("File does not exist " + path);
                return;
            }
            using (
                BinaryReader reader =
                    new BinaryReader(File.Open(path, FileMode.Open))
            )
            {
                int header = reader.ReadInt32();
                if (header == 1)
                {
                    hexGrid.Load(reader);
                }
                else
                {
                    Debug.LogWarning("Unknown map format " + header);
                }
            }
        }
        /// <summary>
        /// 读取地图文件并填充列表
        /// </summary>
        private void FillList()
        {
            //清除旧的列表项
            for (int i = 0; i < listContent.childCount; i++)
            {
                Destroy(listContent.GetChild(i).gameObject);
            }
            //读取map文件
            string[] paths = Directory.GetFiles(Application.persistentDataPath, "*.map");
            //对文件进行排序
            Array.Sort(paths);
            //
            listContent.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, paths.Length * (60 + 10) - 10);
            Vector2 position = new Vector2(-390, listContent.rect.height / 2f - 10);
            //生成列表项
            for (int i = 0; i < paths.Length; i++)
            {
                SaveLoadItem item = Instantiate(itemPrefab);
                item.menu = this;
                item.transform.position = position;
                item.MapName = Path.GetFileNameWithoutExtension(paths[i]);
                item.transform.SetParent(listContent, false);

                position.y -= 70;
            }
        }

        /// <summary>
        /// 打开界面
        /// </summary>
        public void Open(bool saveMode)
        {
            this.saveMode = saveMode;
            if (saveMode)
            {
                menuLabel.text = "保存地图";
                actionButtonLabel.text = "保存";
            }
            else
            {
                menuLabel.text = "加载地图";
                actionButtonLabel.text = "加载";
            }
            FillList();
            gameObject.SetActive(true);
            HexMapCamera.Locked = true;
        }
        /// <summary>
        /// 关闭界面
        /// </summary>
        public void Close()
        {
            gameObject.SetActive(false);
            HexMapCamera.Locked = false;
        }
        /// <summary>
        /// 保存和加载按钮的处理
        /// </summary>
        public void Action()
        {
            string path = GetSelectedPath();
            if (path == null)
            {
                return;
            }
            if (saveMode)
            {
                Save(path);
            }
            else
            {
                Load(path);
            }
            Close();
        }
        /// <summary>
        /// 删除指定地图文件
        /// </summary>
        public void Delete()
        {
            string path = GetSelectedPath();
            if (path == null || File.Exists(path) == false)
            {
                return;
            }
            File.Delete(path);
            nameInput.text = "";
            FillList();
        }
        /// <summary>
        /// 选择地图文件
        /// </summary>
        /// <param name="name"></param>
        public void SelectItem(string name)
        {
            nameInput.text = name;
        }
    }
}