using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace Pxp
{
    public static class ExtendedRandomNicknameGenerator
    {
        private static string[] adjectives = new string[]
        {
            "Happy", "Brave", "Clever", "Swift", "Mighty", "Silent", "Wise", "Fiery", "Gentle", "Lucky",
            "Shiny", "Calm", "Wild", "Proud", "Cute", "Sly", "Bold", "Jolly", "Kind", "Tough",
            "Agile", "Daring", "Eager", "Fancy", "Goofy", "Honest", "Jazzy", "Keen", "Lively", "Merry",
            "Noble", "Quirky", "Radiant", "Silly", "Tender", "Unique", "Vivid", "Witty", "Zealous", "Bright"
        };

        private static string[] nouns = new string[]
        {
            "Wolf", "Eagle", "Tiger", "Panda", "Dragon", "Phoenix", "Unicorn", "Lion", "Bear", "Falcon",
            "Dolphin", "Panther", "Hawk", "Cobra", "Jaguar", "Raven", "Rhino", "Shark", "Owl", "Fox",
            "Leopard", "Lynx", "Mongoose", "Otter", "Penguin", "Raccoon", "Sloth", "Turtle", "Viper", "Yak",
            "Badger", "Cheetah", "Deer", "Elephant", "Flamingo", "Giraffe", "Hedgehog", "Ibex", "Koala", "Lemur"
        };

        public static string GenerateNickname(bool useNumber = true)
        {
            string adjective = adjectives[Random.Range(0, adjectives.Length)];
            string noun = nouns[Random.Range(0, nouns.Length)];
            string number = useNumber ? Random.Range(10, 1000).ToString() : "";

            return string.Join("", new[] {adjective, noun, number}.Where(s => !string.IsNullOrEmpty(s)));
        }

        public static List<string> GenerateUniqueNicknames(int count, bool useNumber = true)
        {
            HashSet<string> uniqueNicknames = new HashSet<string>();

            while (uniqueNicknames.Count < count)
            {
                string nickname = GenerateNickname(useNumber);
                uniqueNicknames.Add(nickname);
            }

            return new List<string>(uniqueNicknames);
        }

        public static string GenerateNicknameWithFormat(string format)
        {
            return format
                .Replace("[ADJ]", adjectives[Random.Range(0, adjectives.Length)])
                .Replace("[NOUN]", nouns[Random.Range(0, nouns.Length)])
                .Replace("[NUM]", Random.Range(10, 1000).ToString());
        }
    }
}
