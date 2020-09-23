using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace OctoMedia.Api.Utilities
{
    public static class FileUtility
    {
        public static async Task<string> GetMimeType(Stream stream)
        {
            byte[] buffer = new byte[32];

            await stream.ReadAsync(buffer);
            stream.Position = 0;

            if (IsJpeg(buffer))
                return "image/jpeg";

            if (IsPng(buffer))
                return "image/png";

            if (IsGif(buffer))
                return "image/gif";

            if (IsMp4(buffer))
                return "video/mp4";

            Debugger.Break();
            throw new NotImplementedException();
        }

        private static bool ContainsArray(byte[] toSearch, byte[] signature, int offset = 1)
        {
            int startIndex = offset - 1;

            for (int i = 0; i < signature.Length; i++)
            {
                if (toSearch[i + startIndex] != signature[i])
                    return false;
            }

            return true;
        }

        private static bool IsJpeg(byte[] buffer)
        {
            if (buffer[0] != 0xff)
                return false;

            if (buffer[1] != 0xd8)
                return false;

            if (buffer[2] != 0xff)
                return false;

            if (buffer[3] < 0xE0 || buffer[3] > 0xE8)
            {
                if (!(buffer[3] == 0xDB || buffer[3] == 0xFE))
                    return false;
            }

            return true;
        }

        private static bool IsPng(byte[] buffer)
        {
            byte[] signature = ByteArrayFromString(" PNG");
            signature[0] = 0x89;

            return ContainsArray(buffer, signature);
        }

        private static bool IsGif(byte[] buffer)
        {
            byte[] signature = ByteArrayFromString("GIF8");

            return ContainsArray(buffer, signature);
        }

        private static bool IsMp4(byte[] buffer)
        {
            byte[] signatureM4V = ByteArrayFromString("ftypM4V");
            byte[] signatureMSNV = ByteArrayFromString("ftypMSNV");
            byte[] signatureISOM = ByteArrayFromString("ftypisom");

            return ContainsArray(buffer, signatureM4V, 5) ||
                   ContainsArray(buffer, signatureMSNV, 5) ||
                   ContainsArray(buffer, signatureISOM, 5);
        }

        private static byte[] ByteArrayFromString(string signature)
        {
            return signature.Select(s => (byte) s).ToArray();
        }
    }
}