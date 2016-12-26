using pb.Data.OpenXml;

namespace pb.Data.Mongo.Serializers
{
    static partial class RunSerializer
    {
        // register OXmlElementSerializer, ... in provider PBSerializationProvider_v2
        public static void InitOpenXml(bool traceSerializer = false)
        {
            //Trace.WriteLine($"RunSerializer.InitOXmlElement() : traceSerializer {traceSerializer}");

            if (traceSerializer)
            {
                OXmlElementSerializer.Trace = true;
                OXmlParagraphElementSerializer.Trace = true;
            }

            PBSerializationProvider_v2.Instance.RegisterSerializer(typeof(OXmlElement), new OXmlElementSerializer());
            PBSerializationProvider_v2.Instance.RegisterSerializer(typeof(OXmlParagraphElement), new OXmlParagraphElementSerializer());
            PBSerializationProvider_v2.Instance.RegisterSerializer(typeof(OXmlBreakElement), new OXmlBreakElementSerializer());
            PBSerializationProvider_v2.Instance.RegisterSerializer(typeof(OXmlTextElement), new OXmlTextElementSerializer());
            PBSerializationProvider_v2.Instance.RegisterSerializer(typeof(OXmlSimpleFieldElement), new OXmlSimpleFieldElementSerializer());
            PBSerializationProvider_v2.Instance.RegisterSerializer(typeof(OXmlDocSectionElement), new OXmlDocSectionElementSerializer());
            PBSerializationProvider_v2.Instance.RegisterSerializer(typeof(OXmlDocDefaultsRunPropertiesElement), new OXmlDocDefaultsRunPropertiesElementSerializer());
            PBSerializationProvider_v2.Instance.RegisterSerializer(typeof(OXmlOpenHeaderElement), new OXmlOpenHeaderElementSerializer());
            PBSerializationProvider_v2.Instance.RegisterSerializer(typeof(OXmlOpenFooterElement), new OXmlOpenFooterElementSerializer());
            PBSerializationProvider_v2.Instance.RegisterSerializer(typeof(OXmlStyleElement), new OXmlStyleElementSerializer());
            PBSerializationProvider_v2.Instance.RegisterSerializer(typeof(OXmlPictureElement), new OXmlPictureElementSerializer());
        }

        // unregister OXmlElementSerializer, ... in provider PBSerializationProvider_v2
        public static void EndOpenXml()
        {
            PBSerializationProvider_v2.Instance.UnregisterSerializer(typeof(OXmlElement));
            PBSerializationProvider_v2.Instance.UnregisterSerializer(typeof(OXmlParagraphElement));
            PBSerializationProvider_v2.Instance.UnregisterSerializer(typeof(OXmlBreakElement));
            PBSerializationProvider_v2.Instance.UnregisterSerializer(typeof(OXmlTextElement));
            PBSerializationProvider_v2.Instance.UnregisterSerializer(typeof(OXmlSimpleFieldElement));
            PBSerializationProvider_v2.Instance.UnregisterSerializer(typeof(OXmlDocSectionElement));
            PBSerializationProvider_v2.Instance.UnregisterSerializer(typeof(OXmlDocDefaultsRunPropertiesElement));
            PBSerializationProvider_v2.Instance.UnregisterSerializer(typeof(OXmlOpenHeaderElement));
            PBSerializationProvider_v2.Instance.UnregisterSerializer(typeof(OXmlOpenFooterElement));
            PBSerializationProvider_v2.Instance.UnregisterSerializer(typeof(OXmlStyleElement));
            PBSerializationProvider_v2.Instance.UnregisterSerializer(typeof(OXmlPictureElement));

            OXmlElementSerializer.Trace = false;
            OXmlParagraphElementSerializer.Trace = false;
        }
    }
}
