using System;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using UnityEngine;
using Utility;

namespace Data
{
    [Serializable]
    public class AxieData
    {
        [JsonProperty] public string AxieId;
        [JsonProperty] public string Genes;
    }

    [Serializable]
    public class MatchData
    {
        [JsonProperty] public AxieData Attacker;
        [JsonProperty] public AxieData Defender;
    }

    [CreateAssetMenu(fileName = "GameData", menuName = "RPG/GameData")]
    public class GameData : ScriptableObject
    {
        [SerializeField] public MatchData MatchData;

        [Button]
        public async void FetchData()
        {
            await GameDataUtil.FetchAxieGenes(MatchData.Attacker);
            await GameDataUtil.FetchAxieGenes(MatchData.Defender);
        }
    }
}