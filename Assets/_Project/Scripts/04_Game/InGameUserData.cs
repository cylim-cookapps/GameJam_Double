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
        public List<InGameHeroData> Heroes;

        public InGameUserData(int index, string id, string name, int level, List<InGameHeroData> heroes)
        {
            Index = index;
            Id = id;
            Name = name;
            Level = level;
            Heroes = heroes;
        }

        // PlayerData를 Hashtable로 변환 (Photon 네트워크 전송용)
        public Hashtable ToHashtable()
        {
            Hashtable playerHash = new Hashtable()
            {
                {"Index", Index},
                {"Id", Id},
                {"Name", Name},
                {"Level", Level}
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

            return new InGameUserData(
                (int) hash["Index"],
                (string) hash["Id"],
                (string) hash["Name"],
                (int) hash["Level"],
                heroes
            );
        }
    }
}
