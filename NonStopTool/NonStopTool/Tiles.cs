using NonStopTool.Includes;
using NonStopTool.NesGraphics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace NonStopTool
{
    public partial class Tiles : Form
    {
        private MyBitmap bitmap;
        private MyBitmap smallImagesBitmap;
        private List<MyBitmap> tiles;
        private List<Point> positions;

        public Tiles()
        {
            InitializeComponent();
        }

        private void BrowseButtonClick(object sender, EventArgs e)
        {
            var fileOpenDialog = new OpenFileDialog();
            fileOpenDialog.Filter = "PNG files|*.png";
            var result = fileOpenDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                this.inputTextBox.Text = fileOpenDialog.FileName;
            }
        }

        private void LoadButtonClick(object sender, EventArgs e)
        {
            var fileName = this.inputTextBox.Text;
            if (!File.Exists(fileName))
            {
                MessageBox.Show("File doesn't exit");
            }

            this.bitmap = MyBitmap.FromFile(fileName);

            if (this.bitmap.Width != Constants.BgWidth || this.bitmap.Height != Constants.BgHeight)
            {
                MessageBox.Show($"Incorrect size, should be {Constants.BgWidth}x{Constants.BgHeight}");
            }

            this.tiles = new List<MyBitmap>();
            this.positions = new List<Point>();
            var indices = new int[32, 24];

            for (var y = 0; y < this.bitmap.Height / Constants.NesTileSize; y++)                
            {
                for (var x = 0; x < this.bitmap.Width / Constants.NesTileSize; x++)
                {
                    var smallImage = bitmap.GetPart(x * Constants.NesTileSize, y * Constants.NesTileSize, Constants.NesTileSize, Constants.NesTileSize);
                    if (this.tiles.Any(v => v.Equals(smallImage)))
                    {
                        indices[x, y] = this.tiles.IndexOf(smallImage);
                    }
                    else
                    {
                        this.tiles.Add(smallImage);
                        this.positions.Add(new Point(x * Constants.NesTileSize, y * Constants.NesTileSize));
                        indices[x, y] = this.tiles.Count - 1;
                    }
                }
            }

            const int entryWidth = 65;
            const int horizontalSpacing = 8;
            const int entriesInRow = 7;
            const int leftPadding = 12;
            const int imageWidth = entryWidth + (entryWidth + horizontalSpacing) * (entriesInRow - 1) + leftPadding;
            const int entryHeight = 33;
            const int verticalSpacing = 2;
            const int topPadding = 12;
            var rowCount = (this.tiles.Count + entriesInRow - 1) / entriesInRow;
            var imageHeight = rowCount * entryHeight + (rowCount - 1) * verticalSpacing + topPadding;

            this.smallImagesBitmap = new MyBitmap(imageWidth, imageHeight);

            var font = new Font("Calibri", 8);
            for (var i = 0; i < this.tiles.Count; i++)
            {
                var x = (i % entriesInRow) * (entryWidth + horizontalSpacing) + leftPadding;
                var y = (i / entriesInRow) * (entryHeight + verticalSpacing) + topPadding;
                var tile = this.tiles[i].Scale(2);
                var subImage = new MyBitmap(entryWidth, entryHeight);
                subImage.DrawImage(tile, (entryWidth - tile.Width) / 2, 0);

                var tempBitmap = subImage.ToBitmap();
                using (var g = Graphics.FromImage(tempBitmap))
                {                    
                    g.DrawString(string.Format("{0:D3}: {1:D3}/{2:D3}", i, this.positions[i].X, this.positions[i].Y), font, Brushes.Black, 0, 18);
                }

                this.smallImagesBitmap.DrawImage(MyBitmap.FromBitmap(tempBitmap), x, y);
            }           

            this.smallImagesPanel.BackgroundImage = this.smallImagesBitmap.ToBitmap();

            var str = string.Empty;
            for (var y = 0; y < this.bitmap.Height / Constants.NesTileSize; y++)
            {                
                for (var x = 0; x < this.bitmap.Width / Constants.NesTileSize; x++)
                {
                    str += indices[x, y].ToString("D3") + " ";
                }

                str += Environment.NewLine;
            }

            this.textBox.Text = str;
            this.UpdateImage();
        }

        private MyBitmap GenerateImage()
        {
            var result = new MyBitmap(Constants.BgWidth, Constants.BgHeight);
            var lines = this.textBox.Lines.Where(l => !string.IsNullOrEmpty(l)).ToList();
            if (lines.Count != Constants.BgHeight / Constants.NesTileSize)
            {
                MessageBox.Show($"Invalid number of lines in the text box, should be {Constants.BgHeight / Constants.NesTileSize}");
                return null;
            }

            var i = 0;
            foreach (var line in lines)
            {
                var split = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (split.Length != Constants.BgWidth / Constants.NesTileSize)
                {
                    MessageBox.Show($"Invalid number of ids in line {i}, should be {Constants.BgWidth / Constants.NesTileSize}");
                    return null;
                }

                var ids = new List<int>();
                foreach (var s in split)
                {
                    int id;
                    if (!int.TryParse(s, out id))
                    {
                        MessageBox.Show($"Non-number in line {i}: {s}");
                        return null;
                    }

                    ids.Add(id);
                }

                for (var j = 0; j < ids.Count; j++)
                {
                    var id = ids[j];
                    var tile = this.tiles[id];
                    var x = j * Constants.NesTileSize;
                    var y = i * Constants.NesTileSize;
                    result.DrawImage(tile, x, y);
                }

                i++;
            }

            return result;
        }

        private void UpdateImage()
        {
            var image = GenerateImage();
            if (image == null)
            {
                return;
            }

            this.pictureBox.Image = image.Scale(2).ToBitmap();
        }

        private void ExportButtonClick(object sender, EventArgs e)
        {
            var image = GenerateImage();
            if (image == null)
            {
                return;
            }

            var fileSaveDialog = new SaveFileDialog();
            fileSaveDialog.Filter = "png file|*.png";
            var result = fileSaveDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                image.ToBitmap().Save(fileSaveDialog.FileName);
            }
        }

        private void UpdateButtonClick(object sender, EventArgs e)
        {
            this.UpdateImage();
        }
    }
}
