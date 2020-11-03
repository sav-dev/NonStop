using System;
using System.Drawing;
using NonStopTool.Includes;

namespace NonStopTool.NesGraphics
{
    public class NesPalette
    {
        private const int PaletteSquareSize = 16;
        private const int PaletteSquaresPerRow = 16;
        private const int PaletteColors = 64;
        
        public static Color[] Colors;
        
        static NesPalette()
        {
            Colors = new Color[PaletteColors];
        
            var image = new Bitmap(FileConstants.PalettePicture);
            var rows = (int)Math.Ceiling((double)(PaletteColors / PaletteSquaresPerRow));
            for (var row = 0; row < rows; row++)
            {
                for (var column = 0; column < PaletteSquaresPerRow && row * PaletteSquaresPerRow + column < PaletteColors; column++)
                {
                    var index = row * PaletteSquaresPerRow + column;
                    Colors[index] = image.GetPixel(column * PaletteSquareSize, row * PaletteSquareSize);
                }
            }
        }
    }
}
