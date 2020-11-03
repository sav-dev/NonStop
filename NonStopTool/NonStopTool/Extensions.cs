using System.Drawing;
using System.IO;

namespace NonStopTool
{
    public static class Extensions
    {
        public static double Luminance(this Color color)
        {
            return 0.299 * color.R + 0.587 * color.G + 0.114 * color.B;
        }

        public static void WriteLineIfNotNull(this TextWriter logger)
        {
            if (logger != null)
            {
                logger.WriteLine();
            }
        }

        public static void WriteLineIfNotNull(this TextWriter logger, string line)
        {
            if (logger != null)
            {
                logger.WriteLine(line);
            }
        }

        public static void WriteLineIfNotNull(this TextWriter logger, string format, params object[] args)
        {
            if (logger != null)
            {
                logger.WriteLine(string.Format(format, args));
            }
        }
    }
}
