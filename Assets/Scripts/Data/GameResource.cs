using System;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;
using Utility;

namespace Data
{
    [Serializable]
    public class AxieResource
    {
        [JsonProperty] public string AxieId;
        [JsonProperty] public string Genes;
    }

    [Serializable]
    public class MatchResource
    {
        [JsonProperty] public AxieResource Attacker;
        [JsonProperty] public AxieResource Defender;
    }

    [CreateAssetMenu(fileName = "GameResource", menuName = "TurnBase/GameResource")]
    public class GameResource : ScriptableObject
    {
        [SerializeField] public MatchResource MatchResource;

        [Button]
        public async void FetchData()
        {
            await GameDataUtil.FetchAxieGenes(MatchResource.Attacker);
            await GameDataUtil.FetchAxieGenes(MatchResource.Defender);
        }
    }
}