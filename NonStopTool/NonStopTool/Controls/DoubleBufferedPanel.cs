using System.Windows.Forms;

namespace NonStopTool.Controls
{
    public class DoubleBufferedPanel : Panel
    {
        public DoubleBufferedPanel() : base()
        {
            this.DoubleBuffered = true;
        }
    }
}