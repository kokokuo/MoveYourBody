using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing.Imaging;
using System.Drawing;

namespace KinectSimulation
{
    static class SaveToJpeg
    {
        /////////////////////////////////////////////////////////////////
        //jepg 編碼部分 可以達到好的品質
        public static void SaveJpeg (string path, Image img, int quality)
        {
            EncoderParameter qualityParam
            = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, quality);

            ImageCodecInfo jpegCodec = GetEncoderInfo(@"image/jpeg");

            EncoderParameters encoderParams = new EncoderParameters(1);

            encoderParams.Param[0] = qualityParam;

            System.IO.MemoryStream mss = new System.IO.MemoryStream();

            System.IO.FileStream fs
            = new System.IO.FileStream(path, System.IO.FileMode.Create
            , System.IO.FileAccess.ReadWrite);

            img.Save(mss, jpegCodec, encoderParams);
            byte[] matriz = mss.ToArray();
            fs.Write(matriz, 0, matriz.Length);

            mss.Close();
            fs.Close();
        }
        private static ImageCodecInfo GetEncoderInfo(string mimeType)
        {
            // Get image codecs for all image formats
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();

            // Find the correct image codec
            for (int i = 0; i < codecs.Length; i++)
                if (codecs[i].MimeType == mimeType)
                    return codecs[i];
            return null;
        }
    }
}
