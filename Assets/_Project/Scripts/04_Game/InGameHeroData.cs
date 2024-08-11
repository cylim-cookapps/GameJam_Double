using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using UnityEngine.Serialization;

namespace Pxp
{

    [Serializable]
    public class HeroLevelData
    {
        public int Owner;
        public int HeroId;
        public int Upgrade;
        public int Coin;

        public HeroLevelData(int owner, int heroId, int upgrade, int coin)
        {
            Owner = owner;
            HeroId = heroId;
            Upgrade = upgrade;
            Coin = coin;
        }

        // HeroLevelRequest를 Hashtable로 변환 (Photon 네트워크 전송용)
        public Hashtable ToHashtable()
        {
            return new Hashtable()
            {
                {"Index", Owner},
                {"HeroId", HeroId},
                {"Upgrade", Upgrade},
                {"Coin", Coin}
            };
        }

        // Hashtable에서 HeroLevelRequest 생성 (Photon 네트워크 수신용)
        public static HeroLevelData FromHashtable(Hashtable hash)
        {
            return new HeroLevelData(
                (int) hash["Index"],
                (int) hash["HeroId"],
                (int) hash["Upgrade"],
                (int) hash["Coin"]
            );
        }
    }

    [Serializable]
    public class HeroMoveData
    {
        public int Owner;
        public int Index;

        public HeroMoveData(int owner, int index)
        {
            Owner = owner;
            Index = index;
        }

        // HeroMoveRequest를 Hashtable로 변환 (Photon 네트워크 전송용)
        public Hashtable ToHashtable()
        {
            return new Hashtable()
            {
                {"Owner", Owner},
                {"Index", Index}
            };
        }

        // Hashtable에서 HeroMoveRequest 생성 (Photon 네트워크 수신용)
        public static HeroMoveData FromHashtable(Hashtable hash)
        {
            return new HeroMoveData(
                (int) hash["Owner"],
                (int) hash["Index"]
            );
        }
    }

    [Serializable]
    public class InGameHeroData
    {
        public int HeroId;
        public int Level;
        public int Star;
        public int Upgrade;
        public int Merge;

        public InGameHeroData(int heroId, int level, int star, int upgrade, int merge)
        {
            HeroId = heroId;
            Level = level;
            Star = star;
            Upgrade = upgrade;
            Merge = merge;
        }

        // HeroData를 Hashtable로 변환 (Photon 네트워크 전송용)
        public Hashtable ToHashtable()
        {
            return new Hashtable()
            {
                {"HeroId", HeroId},
                {"Level", Level},
                {"Star", Star},
                {"Upgrade", Upgrade},
                {"Merge", Merge}
            };
        }

        // Hashtable에서 HeroData 생성 (Photon 네트워크 수신용)
        public static InGameHeroData FromHashtable(Hashtable hash)
        {
            return new InGameHeroData(
                (int) hash["HeroId"],
                (int) hash["Level"],
                (int) hash["Star"],
                (int) hash["Upgrade"],
                (int) hash["Merge"]
            );
        }
    }
}
