using PdfSharp.Fonts;
namespace WebApplication13.Resolver
{
    public class CustomFontResolver : IFontResolver
    {
        private string webRootPath;
        public CustomFontResolver(string _webRootPath)
        {
            webRootPath = _webRootPath;
        }

        public byte[] GetFont(string faceName)
        {
            string fontPath = $"{this.webRootPath}fonts/{faceName}.ttf";

            using (var fontStream = new System.IO.FileStream(fontPath, System.IO.FileMode.Open))
            {
                var fontData = new byte[fontStream.Length];
                fontStream.Read(fontData, 0, (int)fontStream.Length);
                return fontData;
            }
        }

        public FontResolverInfo ResolveTypeface(string familyName, bool isBold, bool isItalic)
        {
            string styleSuffix = (isBold && isItalic) ? "BoldItalic" :
                                isBold ? "Bold" :
                                isItalic ? "Italic" : "";

            string resourceName = $"{familyName}{styleSuffix}";
            return new FontResolverInfo(resourceName);
        }
    }
}