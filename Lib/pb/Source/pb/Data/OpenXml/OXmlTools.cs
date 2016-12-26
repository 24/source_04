namespace pb.Data.OpenXml
{
    public static class OXmlTools
    {
        private const int _emusInPixel = 9525;

        public static long PixelToEmus(int length)
        {
            return (long)length * _emusInPixel;
        }
    }
}
