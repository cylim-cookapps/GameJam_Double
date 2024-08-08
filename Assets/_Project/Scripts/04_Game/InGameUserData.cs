using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using UnityEngine.Serialization;

namespace Pxp
{
    [Serializable]
    public class InGameUserData
    {
        public int Index;
        public string Id;
        public string Name;
        public int Level;
        public int Coin;
        public int Chip;
        public int Summon;
        public List<InGameHeroData> Heroes;
        public List<InGameUnitData> Units;

        public InGameUserData(int index, string id, string name, int level, int coin, int chip, int summon, List<InGameHeroData> heroes, List<InGameUnitData> units)
        {
            Index = index;
            Id = id;
            Name = name;
            Level = level;
            Coin = coin;
            Chip = chip;
            Summon = summon;
            Heroes = heroes;
            Units = units;
        }

        // PlayerData를 Hashtable로 변환 (Photon 네트워크 전송용)
        public Hashtable ToHashtable()
        {
            Hashtable playerHash = new Hashtable()
            {
                {"Index", Index},
                {"Id", Id},
                {"Name", Name},
                {"Level", Level},
                {"Coin", Coin},
                {"Chip", Chip},
                {"Summon", Summon}
            };

            if (Heroes != null && Heroes.Count > 0)
            {
                object[] heroesArray = new object[Heroes.Count];
                for (int i = 0; i < Heroes.Count; i++)
                {
                    heroesArray[i] = Heroes[i].ToHashtable();
                }

                playerHash.Add("Heroes", heroesArray);
            }

            if (Units != null && Units.Count > 0)
            {
                object[] unitsArray = new object[Units.Count];
                for (int i = 0; i < Units.Count; i++)
                {
                    unitsArray[i] = Units[i].ToHashtable();
                }

                playerHash.Add("Units", unitsArray);
            }

            return playerHash;
        }

        // Hashtable에서 PlayerData 생성 (Photon 네트워크 수신용)
        public static InGameUserData FromHashtable(Hashtable hash)
        {
            List<InGameHeroData> heroes = new List<InGameHeroData>();
            if (hash.ContainsKey("Heroes"))
            {
                object[] heroesArray = (object[]) hash["Heroes"];
                foreach (object heroObj in heroesArray)
                {
                    if (heroObj is Hashtable heroHash)
                    {
                        heroes.Add(InGameHeroData.FromHashtable(heroHash));
                    }
                }
            }

            List<InGameUnitData> units = new List<InGameUnitData>();
            if (hash.ContainsKey("Units"))
            {
                object[] unitsArray = (object[]) hash["Units"];
                foreach (object unitObj in unitsArray)
                {
                    if (unitObj is Hashtable unitHash)
                    {
                        units.Add(InGameUnitData.FromHashtable(unitHash));
                    }
                }
            }

            return new InGameUserData(
                (int) hash["Index"],
                (string) hash["Id"],
                (string) hash["Name"],
                (int) hash["Level"],
                (int) hash["Coin"],
                (int) hash["Chip"],
                (int) hash["Summon"],
                heroes,
                units
            );
        }
    }
}
