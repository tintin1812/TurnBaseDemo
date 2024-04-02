using System;
using System.Collections;
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

        public static void FadeOut(this GObject obj, GTweenCallback callback)
        {
            obj.TweenFade(0, 0.5f).OnComplete(callback);
        }

        public static void FadeOutDelay(this GObject obj, float delay)
        {
            obj.visible = true;
            obj.alpha = 1;
            obj.TweenFade(1, 0.25f).SetDelay(delay).OnComplete(() => { obj.visible = false; });
        }
    }

    public static class TaskUtil
    {
        public static async void CallAwait(Func<Task> callable)
        {
            if (Application.isEditor)
            {
                var task = callable();
                await task;
                return;
            }

            try
            {
                var task = callable();
                await task;
            }
            catch (Exception ex)
            {
                if (Application.isEditor)
                {
                    Debug.LogError(ex);
                }
                else
                {
                    Debug.LogWarning(ex);
                }
            }
        }

        public static async Task Delay(float seconds)
        {
            var task = new TaskCompletionSource<object>();
            Timers.inst.StartCoroutine(DelayImpl(seconds, task));
            await task.Task;
        }

        private static IEnumerator DelayImpl(float seconds, TaskCompletionSource<object> task)
        {
            yield return new WaitForSeconds(seconds);
            task.SetResult(null);
        }
    }
}