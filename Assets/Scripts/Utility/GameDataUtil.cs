using System;
using System.Threading.Tasks;
using Data;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Networking;

namespace Utility
{
    public static class GameDataUtil
    {
        public static async Task<bool> FetchAxieGenes(AxieResource axieResource)
        {
            if (axieResource.AxieId.IsNullOrEmpty()) return false;
            var result = await FetchAxieGenes(axieResource.AxieId);
            if (result.IsNullOrEmpty())
            {
                Debug.Log($"Fetch Axie Genes Error, AxieId: ${axieResource.AxieId}");
                return false;
            }

            try
            {
                var jResult = JObject.Parse(result);
                var genesStr = (string)jResult["data"]?["axie"]?["newGenes"];
                axieResource.Genes = genesStr;
                return true;
            }
            catch (Exception e)
            {
                Debug.LogWarning(e);
                return false;
            }
        }

        private static async Task<string> FetchAxieGenes(string axieId)
        {
            var searchString = "{ axie (axieId: \"" + axieId + "\") { id, genes, newGenes}}";
            var jPayload = new JObject { new JProperty("query", searchString) };
            var wr = new UnityWebRequest("https://graphql-gateway.axieinfinity.com/graphql", "POST");
            var jsonToSend = new System.Text.UTF8Encoding().GetBytes(jPayload.ToString().ToCharArray());
            wr.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);
            wr.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            wr.SetRequestHeader("Content-Type", "application/json");
            wr.timeout = 10;
            var operation = wr.SendWebRequest();
            while (!operation.isDone)
            {
                await Task.Delay(100);
            }

            if (wr.error == null)
            {
                return wr.downloadHandler.text;
            }
            else
            {
                Debug.LogWarning(wr.error);
            }

            return "";
        }
    }
}