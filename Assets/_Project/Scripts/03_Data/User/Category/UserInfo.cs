using System;
using System.Collections;
using System.Collections.Generic;
using CookApps.Obfuscator;
using Newtonsoft.Json;
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

        public override Enum_UserData Category => Enum_UserData.Info;

        public override void CheckAndCreate()
        {
            if (Level == 0)
                Level = 1;
        }

        public void LevelUp()
        {
            UserManager.Inst.SaveCheck(Enum_UserData.Info);
        }
    }
}
