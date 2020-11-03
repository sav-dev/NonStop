using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace NonStopTool.NesGraphics
{
    public class MyBitmap : IEquatable<MyBitmap>
    {
        public static readonly Color GridColor = Color.FromArgb(235, 235, 180);

        public static readonly Color XYColor = Color.Yellow;
        public static readonly Color GunColor = Color.Red;
        public static readonly Color PlatformBoxColor = Color.Blue;
        public static readonly Color ThreatBoxColor = Color.Orange;

        public static readonly Color[] NesGreyscale = new[] { NesPalette.Colors[15], NesPalette.Colors[0], NesPalette.Colors[16], NesPalette.Colors[32] };

        private string fileName;

        private int width;
        private int height;
        private Color[][] pixels;

        private int[] colorSkips;

        public int Width { get { return this.width; } }
        public int Height { get { return this.height; } }
       
        public Size Size { get { return new Size(this.width, this.height); } }
        public string FileName { get { return this.fileName; } }

        public static MyBitmap FromFileWithParams(string file)
        {
            var split = file.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            var bitmap = FromFile(split[0]);
            if (split.Length > 1)
            {
                var colorSkips = new List<int>();
                for (var i = 1; i < split.Length; i++)
                {
                    colorSkips.Add(int.Parse(split[i]));
                }

                bitmap.colorSkips = colorSkips.ToArray();
            }

            return bitmap;
        }

        public static MyBitmap FromFile(string file)
        {
            var bitmap = new Bitmap(file);
            var result = FromBitmap(bitmap);
            result.fileName = file;
            return result;
        }

        public static MyBitmap FromBitmap(Bitmap bitmap)
        {
            var result = new MyBitmap(bitmap.Width, bitmap.Height);
            for (var i = 0; i < bitmap.Width; i++)
            {
                for (var j = 0; j < bitmap.Height; j++)
                {
                    result.SetPixel(bitmap.GetPixel(i, j), i, j);
                }
            }
            
            return result;
        }

        public MyBitmap Clone()
        {
            var newBitmap = new MyBitmap(this.Width, this.Height);
            for (var i = 0; i < this.Width; i++)
            {
                for (var j = 0; j < this.Height; j++)
                {
                    newBitmap.SetPixel(this.GetPixel(i, j), i, j);
                }
            }

            return newBitmap;
        }

        public MyBitmap Crop(int newWidth, int newHeight)
        {
            var newBitmap = new MyBitmap(newWidth, newHeight);
            for (var i = 0; i < newWidth; i++)
            {
                for (var j = 0; j < newHeight; j++)
                {
                    newBitmap.SetPixel(this.GetPixel(i, j), i, j);
                }
            }

            return newBitmap;
        }

        public void DrawGrid()
        {
            var gridColor = GridColor;

            for (var i = 0; i < this.width; i++)
            {
                this.SetPixel(gridColor, i, 0);
            }

            for (var i = 0; i < this.height; i++)
            {
                this.SetPixel(gridColor, 0, i);
            }

            for (var i = 0; i < this.width; i++)
            {
                this.SetPixel(gridColor, i, this.height - 1);
            }

            for (var i = 0; i < this.height; i++)
            {
                this.SetPixel(gridColor, this.width - 1, i);
            }
        }

        public Bitmap ToBitmap(int alpha = 255, Color? backgroundColor = null)
        {
            var result = new Bitmap(this.width, this.height);
            for (var i = 0; i < this.Width; i++)
            {
                for (var j = 0; j < this.Height; j++)
                {
                    var color = this.GetPixel(i, j);
                    int a;
                    if (backgroundColor.HasValue)
                    {
                        if (color.R == backgroundColor.Value.R && color.G == backgroundColor.Value.G && color.B == backgroundColor.Value.B)
                        {
                            a = 0;
                        }
                        else
                        {
                            a = alpha;
                        }
                    }
                    else
                    {
                        a = alpha;
                    }

                    result.SetPixel(i, j, Color.FromArgb(a, color));
                }
            }

            return result;
        }

        public MyBitmap(int width, int height, Color? backColor = null)
        {
            this.pixels = new Color[width][];
            for (var i = 0; i < width; i++)
            {
                this.pixels[i] = new Color[height];
                for (var j = 0; j < height; j++)
                {
                    this.pixels[i][j] = backColor.HasValue ? backColor.Value : Color.White;
                }
            }

            this.width = width;
            this.height = height;
        }

        public Color GetPixel(int x, int y)
        {
            return this.pixels[x][y];
        }

        public int GetNesPixel(int x, int y)
        {
            return NesGreyscale.ToList().IndexOf(this.pixels[x][y]);
        }

        public void SetPixel(Color color, int x, int y)
        {
            this.pixels[x][y] = color;
        }

        public void DrawRectangle(Color color, int x1, int y1, int x2, int y2)
        {
            var minX = Math.Min(x1, x2);
            var maxX = Math.Max(x1, x2);

            var minY = Math.Min(y1, y2);
            var maxY = Math.Max(y1, y2);

            for (var x = minX; x <= maxX; x++)
            {
                this.SetPixel(color, x, y1);
                this.SetPixel(color, x, y2);
            }

            for (var y = minY; y <= maxY; y++)
            {
                this.SetPixel(color, x1, y);
                this.SetPixel(color, x2, y);
            }
        }

        public void Resize(int newWidth, int newHeight, Color? backColor = null)
        {
            var newBitmap = new MyBitmap(newWidth, newHeight, backColor);
            newBitmap.DrawImage(this, 0, 0);
            this.Set(newBitmap);
        }

        private void Set(MyBitmap newBitmap)
        {
            this.pixels = newBitmap.pixels;
            this.width = newBitmap.width;
            this.height = newBitmap.height;
        }

        public void DrawImage(MyBitmap image, int x, int y, bool resize = false, Color? backColor = null)
        {
            if ((x + image.width > this.width || y + image.height > this.height) && resize)
            {
                this.Resize(Math.Max(x + image.width, this.width), Math.Max(y + image.height, this.height), backColor);
            }

            // todo: not go out of bounds

            for (var i = 0; i < image.Width; i++)
            {
                if (i + x >= this.width)
                {
                    break;
                }

                for (var j = 0; j < image.Height; j++)
                {
                    if (j + y >= this.height)
                    {
                        break;
                    }

                    this.SetPixel(image.GetPixel(i, j), i + x, j + y);
                }
            }
        }

        public MyBitmap GetPart(int x, int y, int width, int height)
        {
            var result = new MyBitmap(width, height);
            for (var i = 0; i < width; i++)
            {
                for (var j = 0; j < height; j++)
                {
                    result.SetPixel(this.GetPixel(i + x, j + y), i, j);
                }
            }

            return result;
        }

        public MyBitmap Scale(int zoom)
        {
            var result = new MyBitmap(this.width * zoom, this.height * zoom);

            for (var x = 0; x < this.width; x++)
            {
                for (var y = 0; y < this.height; y++)
                {
                    var pixel = this.GetPixel(x, y);
                    for (var xx = 0; xx < zoom; xx++)
                    {
                        for (var yy = 0; yy < zoom; yy++)
                        {
                            result.SetPixel(pixel, x * zoom + xx, y * zoom + yy);
                        }
                    }
                }
            }

            return result;
        }

        public MyBitmap ReverseVertically()
        {
            var result = new MyBitmap(this.width, this.height);
            for (var x = 0; x < this.width; x++)
            {
                for (var y = 0; y < this.height; y++)
                {
                    result.SetPixel(this.GetPixel(x, y), x, this.height - y - 1);
                }
            }

            return result;
        }

        public MyBitmap ReverseHorizontally()
        {
            var result = new MyBitmap(this.width, this.height);
            for (var x = 0; x < this.width; x++)
            {
                for (var y = 0; y < this.height; y++)
                {
                    result.SetPixel(this.GetPixel(x, y), this.width - x - 1, y);
                }
            }

            return result;
        }

        public Color[] UniqueColors()
        {
            var results = new List<Color>();
            foreach (var row in pixels)
            {
                foreach (var color in row)
                {
                    results.Add(color);
                }
            }
            
            return results.Distinct().OrderBy(c => c.Luminance()).ToArray();
        }

        public bool Equals(MyBitmap other)
        {
            if (other == null)
            {
                return false;
            }

            if (this.width != other.width || this.height != other.height)
            {
                return false;
            }

            for (var x = 0; x < this.width; x++)
            {
                for (var y = 0; y < this.height; y++)
                {
                    if (this.pixels[x][y] != other.pixels[x][y])
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public void MakeNesGreyscale()
        {
             this.UpdateColors(this.UniqueColors(), NesGreyscale);
        }

        public void UpdateColors(Color[] sourceColors, Color[] targetColors)
        {
            this.UpdateColors(sourceColors.ToList(), targetColors.ToList());
        }

        public void UpdateColors(List<Color> sourceColors, List<Color> targetColors)
        {
            if (this.colorSkips != null)
            {
                var colorsToRemove = new List<Color>();
                foreach (var skip in colorSkips)
                {
                    colorsToRemove.Add(targetColors[skip]);
                }

                foreach (var color in colorsToRemove)
                {
                    targetColors.Remove(color);
                }
            }

            if (sourceColors.Count != targetColors.Count)
            {
                 throw new Exception("Invalid number of colors provided");
            }

            if (this.UniqueColors().Any(c => !sourceColors.Contains(c)))
            {
                throw new Exception("Not all source colors provided");
            }

            for (var x = 0; x < this.width; x++)
            {
                for (var y = 0; y < this.height; y++)
                {
                    var color = this.pixels[x][y];
                    var index = sourceColors.IndexOf(color);
                    var newColor = targetColors[index];
                    this.pixels[x][y] = newColor;
                }
            }
        }

        public override string ToString()
        {
            if (this.fileName != null)
            {
                return string.Format("Bitmap {0} x {1} ({2})", this.width, this.height, this.fileName);
            }

            return string.Format("Bitmap {0} x {1}", this.width, this.height);
        }

        public bool IsNesColor(int i)
        {
            return this.IsSolidColor(NesGreyscale[i]);
        }

        public bool IsSolidColor(Color color)
        {
            for (var x = 0; x < this.width; x++)
            {
                for (var y = 0; y < this.height; y++)
                {
                    if (this.pixels[x][y] != color)
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}
