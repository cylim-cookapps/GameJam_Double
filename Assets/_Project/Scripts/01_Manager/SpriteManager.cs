using UnityEngine;
using System.Collections.Generic;
using UnityEngine.U2D;

namespace Pxp
{
    public class SpriteManager : Singleton<SpriteManager>
    {
        private Dictionary<string, Sprite> spriteCache = new Dictionary<string, Sprite>();
        private SpriteAtlas _spriteAtlas;

        public Sprite GetSprite(string spriteName)
        {
            if (_spriteAtlas == null)
            {
                _spriteAtlas = Resources.Load<SpriteAtlas>("UI");
            }

            if (spriteCache.TryGetValue(spriteName, out Sprite cachedSprite))
            {
                return cachedSprite;
            }

            var sprite = _spriteAtlas.GetSprite(spriteName);
            if (sprite != null)
            {
                spriteCache.Add(spriteName, sprite);
            }

            return sprite;
        }

        public void ClearCache()
        {
            spriteCache.Clear();
        }

        public void ReloadAtlas()
        {
            ClearCache();
        }
    }
}
