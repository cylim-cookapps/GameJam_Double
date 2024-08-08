using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;

namespace Pxp
{
    [Serializable]
    public class InGameHeroData
    {
        public int HeroId;
        public int Level;
        public int Star;
        public int Upgrade;

        public InGameHeroData(int heroId, int level, int star, int upgrade)
        {
            HeroId = heroId;
            Level = level;
            Star = star;
            Upgrade = upgrade;
        }

        // HeroData를 Hashtable로 변환 (Photon 네트워크 전송용)
        public Hashtable ToHashtable()
        {
            return new Hashtable()
            {
                {"HeroId", HeroId},
                {"Level", Level},
                {"Star", Star},
                {"Upgrade", Upgrade}
            };
        }

        // Hashtable에서 HeroData 생성 (Photon 네트워크 수신용)
        public static InGameHeroData FromHashtable(Hashtable hash)
        {
            return new InGameHeroData(
                (int) hash["HeroId"],
                (int) hash["Level"],
                (int) hash["Star"],
                (int) hash["Upgrade"]
            );
        }
    }
}
