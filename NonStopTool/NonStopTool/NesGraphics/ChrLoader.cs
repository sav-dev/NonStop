using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NonStopTool.NesGraphics
{
    public class ChrLoader
    {
        ////private MyBitmap[][] sprites;
        ////
        ////public ChrLoader(string file, Palette[] palettes)
        ////{
        ////    var bytes = File.ReadAllBytes(file);
        ////    var spritesGreyScale = new List<MyBitmap>();
        ////
        ////    // each 16 bytes = 1 sprite
        ////    for (var i = 0; i < 4096; i += 16)
        ////    {
        ////        var sprite = new MyBitmap(Constants.SpriteWidth, Constants.SpriteHeight);
        ////        
        ////        // first 8 bytes = low bits
        ////        // next 8 bytes = high bits
        ////        for (var y = 0; y < 8; y++)
        ////        {
        ////            var lowByte = bytes[i + y];
        ////            var highByte = bytes[i + y + 8];
        ////
        ////            for (var x = 0; x < 8; x++)
        ////            {
        ////                var mask = (byte)(1 << 7 - x);
        ////                var lowBit = (byte)((lowByte & mask) >> 7 - x);
        ////                var highBit = (byte)((highByte & mask) >> 7 - x);
        ////
        ////                var color = 0;
        ////                if (lowBit != 0)
        ////                {
        ////                    color += 1;
        ////                }
        ////
        ////                if (highBit != 0)
        ////                {
        ////                    color += 2;
        ////                }
        ////
        ////                sprite.SetPixel(MyBitmap.NesGreyscale[color], x, y);
        ////            }
        ////        }
        ////
        ////        spritesGreyScale.Add(sprite);
        ////    }
        ////
        ////    this.sprites = new MyBitmap[4][];
        ////    for (var i = 0; i < 4; i++)
        ////    {
        ////        this.sprites[i] = spritesGreyScale.Select(s =>
        ////        {
        ////            var s2 = s.Clone();
        ////            s2.UpdateColors(MyBitmap.NesGreyscale, palettes[i].ActualColors);
        ////            return s2;
        ////        }).ToArray();
        ////    }
        ////}
        ////
        ////public MyBitmap GetSprite(int spriteId, int paletteId)
        ////{
        ////    return this.sprites[paletteId][spriteId];
        ////}
    }
}
