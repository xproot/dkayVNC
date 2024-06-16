using AnimatedGif;
using RemoteViewing.Vnc;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace dkayVNC.Utils
{
    internal class Framebuffer
    {
        public static Bitmap GetRfbBitmap()
        {
            if (Program.RfbClient.IsConnected)
            {
                Bitmap _rfbframebuffer = new Bitmap(Program.RfbClient.Framebuffer.Width, Program.RfbClient.Framebuffer.Height, PixelFormat.Format32bppRgb);

                var _fbrect = new Rectangle(0, 0, Program.RfbClient.Framebuffer.Width, Program.RfbClient.Framebuffer.Height);
                var data = _rfbframebuffer.LockBits(_fbrect, ImageLockMode.WriteOnly, PixelFormat.Format32bppRgb);
                try
                {
                    VncPixelFormat.CopyFromFramebuffer(Program.RfbClient.Framebuffer, new VncRectangle(0, 0, Program.RfbClient.Framebuffer.Width, Program.RfbClient.Framebuffer.Height), data.Scan0, data.Stride, 0, 0);
                }
                finally
                {
                    _rfbframebuffer.UnlockBits(data);
                }
                return _rfbframebuffer;
            }

            Bitmap _novideoframebuffer = new Bitmap(640, 480, PixelFormat.Format32bppArgb);
            Graphics _canvas = Graphics.FromImage(_novideoframebuffer);

            _canvas.DrawString(DateTime.UtcNow.ToString(), new Font(FontFamily.GenericMonospace, 12, FontStyle.Regular), new SolidBrush(Color.White), 0, 0);

            _canvas.DrawString("Not connected", new Font(FontFamily.GenericMonospace, 12, FontStyle.Regular), new SolidBrush(Color.Black), 100, 100);
            _canvas.DrawString("Not connected", new Font(FontFamily.GenericMonospace, 12, FontStyle.Regular), new SolidBrush(Color.White), 99, 99);

            _canvas.DrawString(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString(), new Font(FontFamily.GenericMonospace, 12, FontStyle.Regular), new SolidBrush(Color.Green), 200, 200);

            return _novideoframebuffer;
        }

        public static MemoryStream GetRfbMemoryStream(int frames = 0)
        {
            MemoryStream _ms = new MemoryStream();
            if (frames < 1)
            {
                Bitmap _bitmap = GetRfbBitmap();
                _bitmap.Save(_ms, System.Drawing.Imaging.ImageFormat.Png);
                _bitmap.Dispose();
            }
            else
            {
                AnimatedGifCreator gif = new AnimatedGifCreator(_ms, 83);
                //1 instead of 0 so it's offset by one
                for (int i = 1; i < frames; i++)
                {
                    gif.AddFrameAsync(GetRfbBitmap(), -1, GifQuality.Bit8).GetAwaiter().GetResult();
                }
                //suggestion by dkay: make the last longer
                gif.AddFrameAsync(GetRfbBitmap(), 2000, GifQuality.Bit8).GetAwaiter().GetResult();
                gif.Dispose();
            }
            _ms.Position = 0;
            return _ms;
        }
    }
}
