using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace NetTopologySuite.IO.Esri.Dbf
{
    internal static class DbfEncoding
    {
        /// <summary>
        /// The Latin1 Encoding
        /// </summary>
        internal static readonly Encoding Latin1 = Encoding.GetEncoding(28591); // ISO-8859-1

        private static readonly IDictionary<byte, Encoding> LanguageDriverIdToEncoding = new Dictionary<byte, Encoding>();
        private static readonly IDictionary<Encoding, byte> EncodingToLanguageDriverId = new Dictionary<Encoding, byte>();

        internal static void WriteMissingEncodingMessage(string encodingName)
        {
            Debug.WriteLine("DBF encoding not found: " + encodingName + ". To fix it do the following:");
            Debug.WriteLine("- Add reference to to the System.Text.Encoding.CodePages.dll to your project.");
            Debug.WriteLine("- Put that line somewhere in your code:");
            Debug.WriteLine("  Encoding.RegisterProvider(CodePagesEncodingProvider.Instance); ");
        }

        static DbfEncoding()
        {
            // Encoding.RegisterProvider(CodePagesEncodingProvider.Instance); // Kepp this library clear. Do not include unnecessary dependices.

            // https://support.esri.com/en/technical-article/000013192

            AddLanguageDriverId(0, Encoding.UTF8);        // For unknown LDID
            AddLanguageDriverId(0x03, Encoding.Default);  // OS Default
            AddLanguageDriverId(0x57, Encoding.Default);  // OS Default

            AddLanguageDriverId(0x01, 437);  // United States MSDOS
            AddLanguageDriverId(0x02, 850);  // International MSDOS
            AddLanguageDriverId(0x08, 865);  // Danish OEM
            AddLanguageDriverId(0x09, 437);  // Dutch OEM
            AddLanguageDriverId(0x0A, 850);  // Dutch OEM*
            AddLanguageDriverId(0x0B, 437);  // Finnish OEM
            AddLanguageDriverId(0x0D, 437);  // French OEM
            AddLanguageDriverId(0x0E, 850);  // French OEM*
            AddLanguageDriverId(0x0F, 437);  // German OEM
            AddLanguageDriverId(0x10, 850);  // German OEM*
            AddLanguageDriverId(0x11, 437);  // Italian OEM
            AddLanguageDriverId(0x12, 850);  // Italian OEM*
            AddLanguageDriverId(0x13, 932);  // Japanese Shift-JIS
            AddLanguageDriverId(0x14, 850);  // Spanish OEM*
            AddLanguageDriverId(0x15, 437);  // Swedish OEM
            AddLanguageDriverId(0x16, 850);  // Swedish OEM*
            AddLanguageDriverId(0x17, 865);  // Norwegian OEM
            AddLanguageDriverId(0x18, 437);  // Spanish OEM
            AddLanguageDriverId(0x19, 437);  // English OEM (Britain)
            AddLanguageDriverId(0x1A, 850);  // English OEM (Britain)*
            AddLanguageDriverId(0x1B, 437);  // English OEM (U.S.)
            AddLanguageDriverId(0x1C, 863);  // French OEM (Canada)
            AddLanguageDriverId(0x1D, 850);  // French OEM*
            AddLanguageDriverId(0x1F, 852);  // Czech OEM
            AddLanguageDriverId(0x22, 852);  // Hungarian OEM
            AddLanguageDriverId(0x23, 852);  // Polish OEM
            AddLanguageDriverId(0x24, 860);  // Portuguese OEM
            AddLanguageDriverId(0x25, 850);  // Portuguese OEM*
            AddLanguageDriverId(0x26, 866);  // Russian OEM
            AddLanguageDriverId(0x37, 850);  // English OEM (U.S.)*
            AddLanguageDriverId(0x40, 852);  // Romanian OEM
            AddLanguageDriverId(0x4D, 936);  // Chinese GBK (PRC)
            AddLanguageDriverId(0x4E, 949);  // Korean (ANSI/OEM)
            AddLanguageDriverId(0x4F, 950);  // Chinese Big5 (Taiwan)
            AddLanguageDriverId(0x50, 874);  // Thai (ANSI/OEM)
            AddLanguageDriverId(0x58, 1252); // Western European ANSI
            AddLanguageDriverId(0x59, 1252); // Spanish ANSI
            AddLanguageDriverId(0x64, 852);  // Eastern European MSDOS
            AddLanguageDriverId(0x65, 866);  // Russian MSDOS
            AddLanguageDriverId(0x66, 865);  // Nordic MSDOS
            AddLanguageDriverId(0x67, 861);  // Icelandic MSDOS
            AddLanguageDriverId(0x6A, 737);  // Greek MSDOS (437G)
            AddLanguageDriverId(0x6B, 857);  // Turkish MSDOS
            AddLanguageDriverId(0x6C, 863);  // FrenchCanadian MSDOS
            AddLanguageDriverId(0x78, 950);  // Taiwan Big 5
            AddLanguageDriverId(0x79, 949);  // Hangul (Wansung)
            AddLanguageDriverId(0x7A, 936);  // PRC GBK
            AddLanguageDriverId(0x7B, 932);  // Japanese Shift-JIS
            AddLanguageDriverId(0x7C, 874);  // Thai Windows/MSDOS
            AddLanguageDriverId(0x86, 737);  // Greek OEM
            AddLanguageDriverId(0x87, 852);  // Slovenian OEM
            AddLanguageDriverId(0x88, 857);  // Turkish OEM
            AddLanguageDriverId(0xC8, 1250); // Eastern European Windows
            AddLanguageDriverId(0xC9, 1251); // Russian Windows
            AddLanguageDriverId(0xCA, 1254); // Turkish Windows
            AddLanguageDriverId(0xCB, 1253); // Greek Windows
            AddLanguageDriverId(0xCC, 1257);  // Baltic Windows
        }

        private static void AddLanguageDriverId(byte ldid, Encoding encoding)
        {
            if (!LanguageDriverIdToEncoding.ContainsKey(ldid))
                LanguageDriverIdToEncoding.Add(ldid, encoding);

            if (!EncodingToLanguageDriverId.ContainsKey(encoding))
                EncodingToLanguageDriverId.Add(encoding, ldid);
        }

        private static void AddLanguageDriverId(byte ldid, int codePage)
        {
            try
            {
                var encoding = Encoding.GetEncoding(codePage);
                AddLanguageDriverId(ldid, encoding);
            }
            catch (NotSupportedException ex)
            {
                Debug.WriteLine($"Failed to get codepage for language driver:{ldid}.");
                WriteMissingEncodingMessage("CP:" + codePage.ToString());
                Debug.WriteLine(ex);
            }

        }

        /// <summary>
        /// Get the language driver id for an encoding
        /// </summary>
        /// <param name="encoding">The encoding</param>
        /// <returns>A language driver id</returns>
        public static byte GetLanguageDriverId(Encoding encoding)
        {
            if (EncodingToLanguageDriverId.TryGetValue(encoding, out var ldid))
                return ldid;

            return 0; // 0x03;
        }


        /// <summary>
        /// Get the Encoding for an language driver id 
        /// </summary>
        /// <param name="ldid">Language Driver ID</param>
        /// <returns>Encoding</returns>
        public static Encoding GetEncodingForLanguageDriverId(byte ldid)
        {
            if (LanguageDriverIdToEncoding.TryGetValue(ldid, out var encoding))
                return encoding;

            return null;
        }

    }
}
