using DocumentFormat.OpenXml.Wordprocessing;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using pb.Data.OpenXml;

namespace pb.Data.Mongo.Serializers
{
    public static class OXmlCommonSerializer
    {
        public static OXmlRunFonts ReadRunFonts(BsonReader bsonReader)
        {
            bsonReader.ReadStartDocument();
            OXmlRunFonts value = new OXmlRunFonts();
            while (true)
            {
                BsonType bsonType = bsonReader.ReadBsonType();
                if (bsonType == BsonType.EndOfDocument)
                    break;
                string name = bsonReader.ReadName();
                switch (name.ToLower())
                {
                    case "ascii":
                        if (bsonType == BsonType.Null)
                            break;
                        if (bsonType != BsonType.String)
                            throw new PBException($"wrong RunFonts Ascii value {bsonType}");
                        value.Ascii = bsonReader.ReadString();
                        break;
                    case "asciitheme":
                        if (bsonType == BsonType.Null)
                            break;
                        if (bsonType != BsonType.String)
                            throw new PBException($"wrong RunFonts AsciiTheme value {bsonType}");
                        value.AsciiTheme = bsonReader.ReadString().zParseEnum<ThemeFontValues>(ignoreCase: true);
                        break;
                    case "complexscript":
                        if (bsonType == BsonType.Null)
                            break;
                        if (bsonType != BsonType.String)
                            throw new PBException($"wrong RunFonts ComplexScript value {bsonType}");
                        value.ComplexScript = bsonReader.ReadString();
                        break;
                    case "complexscripttheme":
                        if (bsonType == BsonType.Null)
                            break;
                        if (bsonType != BsonType.String)
                            throw new PBException($"wrong RunFonts ComplexScriptTheme value {bsonType}");
                        value.ComplexScriptTheme = bsonReader.ReadString().zParseEnum<ThemeFontValues>(ignoreCase: true);
                        break;
                    case "eastasia":
                        if (bsonType == BsonType.Null)
                            break;
                        if (bsonType != BsonType.String)
                            throw new PBException($"wrong RunFonts EastAsia value {bsonType}");
                        value.EastAsia = bsonReader.ReadString();
                        break;
                    case "eastasiatheme":
                        if (bsonType == BsonType.Null)
                            break;
                        if (bsonType != BsonType.String)
                            throw new PBException($"wrong RunFonts EastAsiaTheme value {bsonType}");
                        value.EastAsiaTheme = bsonReader.ReadString().zParseEnum<ThemeFontValues>(ignoreCase: true);
                        break;
                    case "highansi":
                        if (bsonType == BsonType.Null)
                            break;
                        if (bsonType != BsonType.String)
                            throw new PBException($"wrong RunFonts HighAnsi value {bsonType}");
                        value.HighAnsi = bsonReader.ReadString();
                        break;
                    case "highansitheme":
                        if (bsonType == BsonType.Null)
                            break;
                        if (bsonType != BsonType.String)
                            throw new PBException($"wrong RunFonts HighAnsiTheme value {bsonType}");
                        value.HighAnsiTheme = bsonReader.ReadString().zParseEnum<ThemeFontValues>(ignoreCase: true);
                        break;
                    case "hint":
                        if (bsonType == BsonType.Null)
                            break;
                        if (bsonType != BsonType.String)
                            throw new PBException($"wrong RunFonts Hint value {bsonType}");
                        value.Hint = bsonReader.ReadString().zParseEnum<FontTypeHintValues>(ignoreCase: true);
                        break;
                    default:
                        throw new PBException($"unknow PageSize value \"{name}\"");
                }
            }
            bsonReader.ReadEndDocument();
            return value;
        }

        public static void WriteRunFonts(BsonWriter bsonWriter, OXmlRunFonts runFonts)
        {
            //if (runFonts.Ascii != null)
            bsonWriter.WriteString("Ascii", runFonts.Ascii);
            if (runFonts.AsciiTheme != null)
                bsonWriter.WriteString("AsciiTheme", runFonts.AsciiTheme.ToString());
            if (runFonts.ComplexScript != null)
                bsonWriter.WriteString("ComplexScript", runFonts.ComplexScript);
            if (runFonts.ComplexScriptTheme != null)
                bsonWriter.WriteString("ComplexScriptTheme", runFonts.ComplexScriptTheme.ToString());
            if (runFonts.EastAsia != null)
                bsonWriter.WriteString("EastAsia", runFonts.EastAsia);
            if (runFonts.EastAsiaTheme != null)
                bsonWriter.WriteString("EastAsiaTheme", runFonts.EastAsiaTheme.ToString());
            if (runFonts.HighAnsi != null)
                bsonWriter.WriteString("HighAnsi", runFonts.HighAnsi);
            if (runFonts.HighAnsiTheme != null)
                bsonWriter.WriteString("HighAnsiTheme", runFonts.HighAnsiTheme.ToString());
            if (runFonts.Hint != null)
                bsonWriter.WriteString("Hint", runFonts.Hint.ToString());
        }
    }
}
