using System;
using System.Collections.Generic;
using MongoDB.Bson;

namespace pb.Data.Mongo
{
    [Flags]
    public enum EnumerateElementsOptions
    {
        None = 0,
        LowercaseName
    }

    public class EnumerateElements
    {
        private class NameElements
        {
            public string Name;
            public IEnumerator<BsonElement> Elements;
        }

        private EnumerateElementsOptions _options = EnumerateElementsOptions.None;

        public EnumerateElementsOptions Options { get { return _options; } set { _options = value; } }

        public BsonDocument GetFlatDocument(BsonDocument document)
        {
            BsonDocument flatDocument = new BsonDocument();
            flatDocument.AllowDuplicateNames = true;
            flatDocument.AddRange(GetElements(document));
            //foreach (BsonElement element in new EnumerateElements { Options = options }.GetElements(document))
            //{
            //    if (!flatDocument.Contains(element.Name))
            //        flatDocument.Add(element);
            //    else
            //        Trace.WriteLine("warning duplicate \"{0}\" : value1 {1} value2 {2}", element.Name, flatDocument[element.Name], element.Value);
            //}
            return flatDocument;
        }

        public IEnumerable<BsonElement> GetElements(BsonDocument document)
        {
            bool lowercaseName = (_options & EnumerateElementsOptions.LowercaseName) == EnumerateElementsOptions.LowercaseName;
            Stack<NameElements> nameElementsStack = new Stack<NameElements>();
            string parentName = null;
            IEnumerator<BsonElement> elements = document.Elements.GetEnumerator();
            while (true)
            {
                if (elements.MoveNext())
                {
                    BsonElement element = elements.Current;
                    string name = element.Name;
                    if (lowercaseName)
                        name = name.ToLowerInvariant();
                    string elementName = parentName != null ? parentName + "." + name : name;
                    switch (element.Value.BsonType)
                    {
                        case BsonType.Document:
                            nameElementsStack.Push(new NameElements { Name = parentName, Elements = elements });
                            parentName = elementName;
                            elements = ((BsonDocument)element.Value).Elements.GetEnumerator();
                            break;
                        case BsonType.Array:
                        case BsonType.Binary:
                        case BsonType.Boolean:
                        case BsonType.DateTime:
                        case BsonType.Double:
                        case BsonType.Int32:
                        case BsonType.Int64:
                        case BsonType.JavaScript:
                        case BsonType.JavaScriptWithScope:
                        //case BsonType.EndOfDocument:
                        case BsonType.MaxKey:
                        case BsonType.MinKey:
                        case BsonType.Null:
                        case BsonType.ObjectId:
                        case BsonType.RegularExpression:
                        case BsonType.String:
                        case BsonType.Symbol:
                        case BsonType.Timestamp:
                        //case BsonType.Undefined:
                            yield return new BsonElement(elementName, element.Value);
                            break;
                    }
                }
                else
                {
                    if (nameElementsStack.Count == 0)
                        break;
                    NameElements nameElements = nameElementsStack.Pop();
                    parentName = nameElements.Name;
                    elements = nameElements.Elements;
                }
            }
        }

        public static IEnumerable<BsonElement> GetFlatElements(BsonDocument document, EnumerateElementsOptions options = EnumerateElementsOptions.None)
        {
            return new EnumerateElements { Options = options }.GetElements(document);
        }

        public static BsonDocument GetFlatDocument(BsonDocument document, EnumerateElementsOptions options = EnumerateElementsOptions.None)
        {
            //return new BsonDocument(new EnumerateElements { Options = options }.GetElements(document));
            //BsonDocument flatDocument = new BsonDocument();
            //flatDocument.AllowDuplicateNames = true;
            //flatDocument.AddRange(new EnumerateElements { Options = options }.GetElements(document));
            //foreach (BsonElement element in new EnumerateElements { Options = options }.GetElements(document))
            //{
            //    if (!flatDocument.Contains(element.Name))
            //        flatDocument.Add(element);
            //    else
            //        Trace.WriteLine("warning duplicate \"{0}\" : value1 {1} value2 {2}", element.Name, flatDocument[element.Name], element.Value);
            //}
            //return flatDocument;
            return new EnumerateElements { Options = options }.GetFlatDocument(document);
        }
    }
}
