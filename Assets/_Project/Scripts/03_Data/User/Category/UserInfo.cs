using System;
using System.Collections;
using System.Collections.Generic;
using CookApps.Obfuscator;
using Newtonsoft.Json;
using Pxp.Data;
using UnityEngine;

namespace Pxp
{
    [Serializable]
    [JsonObject(MemberSerialization.OptIn)]
    public class UserInfo : UserData
    {
        [JsonProperty("profileId")]
        public ObfuscatorInt ProfileId { get; private set; }

        [JsonProperty("level")]
        public ObfuscatorInt Level { get; private set; }

        [JsonProperty("exp")]
        public ObfuscatorInt Exp { get; private set; }

        [JsonProperty("wave")]
        public ObfuscatorInt Wave { get; private set; }

        [JsonProperty("tryCount")]
        public ObfuscatorInt TryCount { get; private set; }

        [JsonProperty("killCount")]
        public ObfuscatorInt KillCount { get; private set; }

        public override Enum_UserData Category => Enum_UserData.Info;
        public UserLevel CurrentLevel => SpecDataManager.Inst.UserLevel.Get(Level);

        public override void CheckAndCreate()
        {
            if (Level == 0)
                Level = 1;
            if (Wave == 0)
                Wave = 1;
        }

        public void SetWave(int wave)
        {
            if (Wave < wave)
            {
                Wave = wave;
                UserManager.Inst.SaveCheck(Enum_UserData.Info);
            }

            TryCount++;
        }

        public void LevelUp()
        {
            UserManager.Inst.SaveCheck(Enum_UserData.Info);
        }
    }
}
