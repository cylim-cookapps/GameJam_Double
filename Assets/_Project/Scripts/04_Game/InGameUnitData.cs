using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;

namespace Pxp
{
    [Serializable]
    public class InGameUnitData
    {
        public int HeroId;
        public int Grade;

        public InGameUnitData(int heroId, int grade)
        {
            HeroId = heroId;
        }

        // HeroData를 Hashtable로 변환 (Photon 네트워크 전송용)
        public Hashtable ToHashtable()
        {
            return new Hashtable()
            {
                {"HeroId", HeroId},
                {"Grade", Grade},
            };
        }

        // Hashtable에서 HeroData 생성 (Photon 네트워크 수신용)
        public static InGameUnitData FromHashtable(Hashtable hash)
        {
            return new InGameUnitData(
                (int) hash["HeroId"],
                (int) hash["Grade"]
            );
        }
    }
}
