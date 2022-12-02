using Newtonsoft.Json;

namespace Tool
{
    public static class DataRead
    {
        /// <summary>
        /// 将path位置的json文件读取为json字符串并转换为json对象
        /// </summary>
        /// <param name="pathWithoutExtenion">读入的json文件地址</param>
        public static T JsonReader<T>(string path)
        {
            System.IO.StreamReader sr = new System.IO.StreamReader(path);
            string str = sr.ReadToEnd();
            sr.Close();
            return JsonConvert.DeserializeObject<T>(str);
        }
    }
}