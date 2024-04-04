using System;
using System.Collections;
using System.Threading.Tasks;
using FairyGUI;
using Gui;
using UnityEngine;
using UnityEngine.Networking;

namespace Utility
{
    public static class Util
    {
        public static void ShowNotiText(string content)
        {
            var textNotification = TextNoti.CreateInstance();
            GRoot.inst.AddChild(textNotification);
            textNotification.scale = new Vector2(0.7f, 0.7f);
            textNotification.Label.text = content;
            textNotification.Center();
            textNotification.y -= GRoot.inst.size.y * 0.3f;
            textNotification.TweenMoveY(textNotification.y - GRoot.inst.size.y * 0.1f, 1.0f);
            textNotification.FadeOutAndDispose(1.0f, 1.0f);
        }

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

        public static void FadeOutAndDispose(this GObject obj, float duration, float delay)
        {
            obj.visible = true;
            obj.alpha = 1;
            obj.TweenFade(0, duration).SetDelay(delay).OnComplete(obj.Dispose);
        }

        public static int VToAngle(int x, int y)
        {
            return x switch
            {
                > 0 => 90,
                < 0 => 270,
                _ => y switch
                {
                    > 0 => 180,
                    < 0 => 0,
                    _ => 0
                }
            };
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