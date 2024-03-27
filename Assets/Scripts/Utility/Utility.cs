using System.Threading.Tasks;
using FairyGUI;
using UnityEngine;
using UnityEngine.Networking;

namespace Utility
{
    public static class Utility
    {
        public static bool IsNullOrEmpty(this string str)
        {
            return string.IsNullOrEmpty(str);
        }

        public static async Task<string> RequestUrlStr(string url)
        {
            var www = new UnityWebRequest(url);
            www.downloadHandler = new DownloadHandlerBuffer();
            var operation = www.SendWebRequest();
            while (!operation.isDone)
            {
                await Task.Delay(100);
            }

            if (www.error == null)
            {
                return www.downloadHandler.text;
            }
            else
            {
                Debug.LogWarning(www.error);
            }

            return "";
        }

        public static void setOnClick(this GButton bt, EventCallback0 callback)
        {
            bt.onClick.Clear();
            bt.onClick.Add(callback);
        }
    }
}