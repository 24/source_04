﻿using System;
using System.Collections.Generic;
using System.Text;
using MongoDB.Bson;
using pb.Linq;
using pb.Text;

namespace pb.Data.Mongo
{
    public class TwoDocumentsElement
    {
        public string Name;
        public BsonValue Value1;
        public BsonValue Value2;
    }

    public class TwoDocuments
    {
        public BsonValue Key;
        public BsonDocument Document1;
        public BsonDocument Document2;
    }

    [Flags]
    public enum DocumentComparatorOptions
    {
        ReturnNotEqualDocuments             = 0x0001,
        ReturnEqualDocuments                = 0x0002,
        ReturnAllDocuments                  = 0x0003,
        ResultDocumentsSource               = 0x0004,
        ResultNotEqualElements              = 0x0010,
        ResultEqualElements                 = 0x0020,
        ResultAllElements                   = 0x0030,
        DontSetDocumentReference            = 0x0100,
        StringComparisonIgnoreCase          = 0x1000,
        StringComparisonIgnoreWhiteSpace    = 0x2000
    }

    public enum DocumentsType
    {
        Document1AndDocument2AreNull,
        Document1IsNull,
        Document2IsNull,
        Document1AndDocument2AreNotNull
    }

    public enum CompareElementResult
    {
        Equal,
        NotEqual,
        OnlyValue1,
        OnlyValue2
    }

    public class ElementResult
    {
        public string Name;
        public CompareElementResult CompareResult;
        public BsonValue Value1;
        public BsonValue Value2;
    }

    public class ElementResultAggregate
    {
        public string Name;
        public int NbValues;
        public int NbValuesEqual;
        public int NbValuesNotEqual;
        public int NbNoValue1;
        public int NbNoValue2;
    }

    [Flags]
    public enum CompareDocumentsResultOptions
    {
        Result              = 0x0001,
        DocumentsResult     = 0x0002,
        AggregateResult     = 0x0004,
        Default             = Result | DocumentsResult
    }

    //CompareDocumentsResultAggregate
    public class CompareDocumentsResult
    {
        private string _source1 = null;
        private string _source2 = null;
        private int _nbDocumentsCompared = 0;
        private int _nbDocumentsEqual = 0;
        private int _nbDocumentsNotEqual = 0;
        private int _nbDocument1AndDocument2NotNull = 0;
        private int _nbDocument1AndDocument2Null = 0;
        private int _nbDocument1Null = 0;
        private int _nbDocument2Null = 0;
        private Dictionary<string, ElementResultAggregate> _elementsAggregate = new Dictionary<string, ElementResultAggregate>();
        private List<CompareDocumentResult> _compareDocumentResults = new List<CompareDocumentResult>();

        public string Source1 { get { return _source1; } set { _source1 = value; } }
        public string Source2 { get { return _source2; } set { _source2 = value; } }
        public int NbDocumentsCompared { get { return _nbDocumentsCompared; } }
        public int NbDocumentsEqual { get { return _nbDocumentsEqual; } }
        public int NbDocumentsNotEqual { get { return _nbDocumentsNotEqual; } }
        public bool Equal { get { return _nbDocumentsNotEqual == 0; } }
        public IEnumerable<CompareDocumentResult> CompareDocumentResults { get { return _compareDocumentResults; } }

        public void AddResult(CompareDocumentResult result, bool keepResult)
        {
            _nbDocumentsCompared++;
            if (result.Equal)
                _nbDocumentsEqual++;
            else
                _nbDocumentsNotEqual++;
            switch (result.DocumentsType)
            {
                case DocumentsType.Document1AndDocument2AreNotNull:
                    _nbDocument1AndDocument2NotNull++;
                    break;
                case DocumentsType.Document1AndDocument2AreNull:
                    _nbDocument1AndDocument2Null++;
                    break;
                case DocumentsType.Document1IsNull:
                    _nbDocument1Null++;
                    break;
                case DocumentsType.Document2IsNull:
                    _nbDocument2Null++;
                    break;
            }

            foreach (ElementResult element in result.Elements)
            {
                ElementResultAggregate elementAggregate;
                if (_elementsAggregate.ContainsKey(element.Name))
                    elementAggregate = _elementsAggregate[element.Name];
                else
                {
                    elementAggregate = new ElementResultAggregate();
                    elementAggregate.Name = element.Name;
                    _elementsAggregate.Add(element.Name, elementAggregate);
                }
                elementAggregate.NbValues++;
                if (element.CompareResult == CompareElementResult.Equal)
                    elementAggregate.NbValuesEqual++;
                else if (element.CompareResult == CompareElementResult.NotEqual)
                    elementAggregate.NbValuesNotEqual++;
                else if (element.CompareResult == CompareElementResult.OnlyValue1)
                    elementAggregate.NbNoValue2++;
                else if (element.CompareResult == CompareElementResult.OnlyValue2)
                    elementAggregate.NbNoValue1++;
            }

            if (keepResult)
                _compareDocumentResults.Add(result);
        }

        public IEnumerable<BsonDocument> GetResultDocuments(CompareDocumentsResultOptions options = CompareDocumentsResultOptions.Default)
        {
            if ((options & CompareDocumentsResultOptions.Result) == CompareDocumentsResultOptions.Result)
            {
                yield return GetResultDocument();
            }
            if ((options & CompareDocumentsResultOptions.DocumentsResult) == CompareDocumentsResultOptions.DocumentsResult)
            {
                foreach (CompareDocumentResult compareDocumentResult in _compareDocumentResults)
                    yield return compareDocumentResult.GetResultDocument();
            }
            if ((options & CompareDocumentsResultOptions.AggregateResult) == CompareDocumentsResultOptions.AggregateResult)
            {
                yield return GetResultAggregateDocument();
            }
        }

        private BsonDocument GetResultDocument()
        {
            return new BsonDocument { { "result", Equal ? "equal" : "not equal" }, { "source1", _source1 }, { "source2", _source2 },
                { "NbDocumentsCompared", _nbDocumentsCompared },
                { "NbDocumentsEqual", _nbDocumentsEqual }, { "NbDocumentsNotEqual", _nbDocumentsNotEqual },
                { "NbDocument1AndDocument2NotNull", _nbDocument1AndDocument2NotNull }, { "NbDocument1AndDocument2Null", _nbDocument1AndDocument2Null },
                { "NbDocument1Null", _nbDocument1Null }, { "NbDocument2Null", _nbDocument2Null } };
        }

        private BsonDocument GetResultAggregateDocument()
        {
            BsonDocument results = new BsonDocument();
            //results.Add("NbDocumentsCompared", _nbDocumentsCompared);
            //results.Add("NbDocumentsEqual", _nbDocumentsEqual);
            //results.Add("NbDocumentsNotEqual", _nbDocumentsNotEqual);
            //results.Add("NbDocument1AndDocument2NotNull", _nbDocument1AndDocument2NotNull);
            //results.Add("NbDocument1AndDocument2Null", _nbDocument1AndDocument2Null);
            //results.Add("NbDocument1Null", _nbDocument1Null);
            //results.Add("NbDocument2Null", _nbDocument2Null);

            BsonArray elements = new BsonArray();
            results.Add("Elements", elements);

            foreach (ElementResultAggregate elementAggregate in _elementsAggregate.Values)
            {
                BsonDocument element = new BsonDocument();
                elements.Add(element);
                element.Add("Name", elementAggregate.Name);
                element.Add("NbValues", elementAggregate.NbValues);
                element.Add("NbValuesEqual", elementAggregate.NbValuesEqual);
                element.Add("NbValuesNotEqual", elementAggregate.NbValuesNotEqual);
                element.Add("NbNoValue1", elementAggregate.NbNoValue1);
                element.Add("NbNoValue2", elementAggregate.NbNoValue2);
            }

            return results;
        }
    }

    public class CompareDocumentResult
    {
        private DocumentComparatorOptions _comparatorOptions;
        private TwoDocuments _twoBsonDocuments = null;
        private bool _allElementsEqual = true;
        private DocumentsType _documentsType;
        private List<ElementResult> _elements = new List<ElementResult>();
        private bool _dontSetDocumentReference = false;
        //private bool _useDefaultDocumentReference = false;
        private IEnumerable<string> _documentReferenceElements1 = null;      // list of reference element from document 1
        private IEnumerable<string> _documentReferenceElements2 = null;      // list of reference element from document 2

        public CompareDocumentResult(TwoDocuments twoBsonDocuments, DocumentComparatorOptions comparatorOptions)
        {
            _twoBsonDocuments = twoBsonDocuments;
            _comparatorOptions = comparatorOptions;
        }

        public bool Equal
        {
            get
            {
                return _documentsType == DocumentsType.Document1AndDocument2AreNull
                    || (_documentsType == DocumentsType.Document1AndDocument2AreNotNull && _allElementsEqual);
            }
        }
        public DocumentsType DocumentsType { get { return _documentsType; } set { _documentsType = value; } }
        public List<ElementResult> Elements { get { return _elements; } }
        public bool DontSetDocumentReference { get { return _dontSetDocumentReference; } set { _dontSetDocumentReference = value; } }
        public IEnumerable<string> DocumentReferenceElements1 { get { return _documentReferenceElements1; } set { _documentReferenceElements1 = value; } }
        public IEnumerable<string> DocumentReferenceElements2 { get { return _documentReferenceElements2; } set { _documentReferenceElements2 = value; } }

        public BsonDocument GetResultDocument()
        {
            // result: 'equal' or 'not equal'
            // key: key
            // reference { document1: {}, document2: {} }
            // difference: 'name, ...', 'document1 is null', 'document2 is null'
            // elements:
            //   name: { compare: 'equal', value1: value }
            //   name: { compare: 'not equal', value1: value1, value2: value2 }
            //   name: { compare: 'only value1', value1: value1 }
            //   name: { compare: 'only value2', value2: value2 }
            // document1
            // document2

            bool resultNotEqualElements;
            if ((_comparatorOptions & DocumentComparatorOptions.ResultNotEqualElements) == DocumentComparatorOptions.ResultNotEqualElements)
                resultNotEqualElements = true;
            else
                resultNotEqualElements = false;

            bool resultEqualElements;
            if ((_comparatorOptions & DocumentComparatorOptions.ResultEqualElements) == DocumentComparatorOptions.ResultEqualElements)
                resultEqualElements = true;
            else
                resultEqualElements = false;

            BsonDocument result = new BsonDocument();
            result.Add("result", Equal ? "equal" : "not equal");
            // if _twoBsonDocuments.Key is null key is not added
            result.Add("key", _twoBsonDocuments.Key);
            // if GetReference() is null reference is not added
            result.Add("reference", GetReference());
            result.Add("difference", "");

            BsonDocument elements = null;
            if (resultNotEqualElements || resultEqualElements)
            {
                elements = new BsonDocument();
                elements.AllowDuplicateNames = true;
                result.Add("elements", elements);
            }

            StringBuilder sb = new StringBuilder();
            if (_documentsType == DocumentsType.Document1AndDocument2AreNull)
                sb.Append("document1 is null, document2 is null");
            else if (_documentsType == DocumentsType.Document1IsNull)
                sb.Append("document1 is null");
            else if (_documentsType == DocumentsType.Document2IsNull)
                sb.Append("document2 is null");
            else  // CompareDocumentResult.Document1AndDocument2AreNotNull
            {
                foreach (ElementResult element in _elements)
                {
                    BsonDocument compare = null;
                    if (element.CompareResult == CompareElementResult.Equal)
                    {
                        if (resultEqualElements)
                            compare = new BsonDocument { { "compare", "equal" }, { "value1", element.Value1 } };
                    }
                    else
                    {
                        if (element.CompareResult == CompareElementResult.NotEqual)
                        {
                            if (resultNotEqualElements)
                                compare = new BsonDocument { { "compare", "not equal" }, { "value1", element.Value1 }, { "value2", element.Value2 } };
                        }
                        else if (element.CompareResult == CompareElementResult.OnlyValue1)
                        {
                            if (resultNotEqualElements)
                                compare = new BsonDocument { { "compare", "only value1" }, { "value1", element.Value1 } };
                        }
                        else // if (element.CompareResult == CompareElementResult.OnlyValue2)
                        {
                            if (resultNotEqualElements)
                                compare = new BsonDocument { { "compare", "only value2" }, { "value2", element.Value2 } };
                        }
                        if (sb.Length > 0)
                            sb.Append(", ");
                        sb.Append(element.Name);
                    }
                    if (compare != null)
                        elements.Add(element.Name, compare);
                }
            }
            result["difference"] = sb.ToString();

            //result = new BsonDocument { { "result", result } };
            if ((_comparatorOptions & DocumentComparatorOptions.ResultDocumentsSource) == DocumentComparatorOptions.ResultDocumentsSource)
            {
                result.Add("document1", _twoBsonDocuments.Document1);
                result.Add("document2", _twoBsonDocuments.Document2);
            }
            return result;
        }

        private BsonDocument GetReference()
        {
            BsonDocument reference = null;
            if (!_dontSetDocumentReference)
            {
                if (_documentReferenceElements1 != null || _documentReferenceElements2 != null)
                {
                    reference = new BsonDocument();
                    if (_documentReferenceElements1 != null)
                    {
                        BsonDocument documentReference1 = new BsonDocument();
                        reference.Add("document1", documentReference1);
                        if (_twoBsonDocuments.Document1 != null)
                        {
                            foreach (string element in _documentReferenceElements1)
                            {
                                BsonValue value = _twoBsonDocuments.Document1.zGet(element);
                                if (value != null)
                                    documentReference1.zSet(element, value);
                            }
                        }
                        else
                            documentReference1.Add("error", "document 1 is null");
                    }
                    if (_documentReferenceElements2 != null)
                    {
                        BsonDocument documentReference2 = new BsonDocument();
                        reference.Add("document2", documentReference2);
                        if (_twoBsonDocuments.Document2 != null)
                        {
                            foreach (string element in _documentReferenceElements2)
                            {
                                BsonValue value = _twoBsonDocuments.Document2.zGet(element);
                                if (value != null)
                                    documentReference2.zSet(element, value);
                            }
                        }
                        else
                            documentReference2.Add("error", "document 2 is null");
                    }
                }
                else
                {
                    reference = new BsonDocument();
                    if (_twoBsonDocuments.Document1 != null)
                    {
                        bool first = true;
                        foreach (BsonElement element in _twoBsonDocuments.Document1)
                        {
                            if (element.Value.IsBsonDocument)
                            {
                                if (first)
                                    //reference.Add(element);
                                    reference = element.Value.AsBsonDocument;
                                break;
                            }
                            reference.Add(element);
                            first = false;
                        }
                    }
                    else if (_twoBsonDocuments.Document2 != null)
                    {
                        bool first = true;
                        foreach (BsonElement element in _twoBsonDocuments.Document2)
                        {
                            if (element.Value.IsBsonDocument)
                            {
                                if (first)
                                    //reference.Add(element);
                                    reference = element.Value.AsBsonDocument;
                                break;
                            }
                            reference.Add(element);
                            first = false;
                        }
                    }
                    else
                        reference.Add("error", "document 1 and document 2 are null");
                }
            }
            return reference;
        }

        public void AddElement(string name, CompareElementResult compareResult, BsonValue value1 = null, BsonValue value2 = null)
        {
            _elements.Add(new ElementResult { Name = name, CompareResult = compareResult, Value1 = value1, Value2 = value2 });
            if (compareResult != CompareElementResult.Equal)
                _allElementsEqual = false;
        }
    }

    public class DocumentComparator
    {
        private DocumentComparatorOptions _comparatorOptions;
        private EnumerateElementsOptions _enumerateElementsOptions = EnumerateElementsOptions.None;
        private Dictionary<string, string> _elementsToCompare = null;
        private EnumerateElements _enumerateElements = null;
        private bool _dontSetDocumentReference = false;
        //private bool _useDefaultDocumentReference = false;
        private IEnumerable<string> _documentReferenceElements1 = null;      // list of reference element from document 1
        private IEnumerable<string> _documentReferenceElements2 = null;      // list of reference element from document 2

        private bool _returnNotEqualDocuments = false;
        private bool _returnEqualDocuments = false;
        private bool _stringComparisonIgnoreCase = false;
        private bool _stringComparisonIgnoreWhiteSpace = false;

        public DocumentComparator()
        {
            SetComparatorOptions(DocumentComparatorOptions.ReturnNotEqualDocuments | DocumentComparatorOptions.ResultNotEqualElements);
        }

        public bool DontSetDocumentReference { get { return _dontSetDocumentReference; } set { _dontSetDocumentReference = value; } }

        public void SetComparatorOptions(DocumentComparatorOptions options)
        {
            _comparatorOptions = options;

            if ((options & DocumentComparatorOptions.ReturnNotEqualDocuments) == DocumentComparatorOptions.ReturnNotEqualDocuments)
                _returnNotEqualDocuments = true;
            else
                _returnNotEqualDocuments = false;

            if ((options & DocumentComparatorOptions.ReturnEqualDocuments) == DocumentComparatorOptions.ReturnEqualDocuments)
                _returnEqualDocuments = true;
            else
                _returnEqualDocuments = false;

            if ((options & DocumentComparatorOptions.DontSetDocumentReference) == DocumentComparatorOptions.DontSetDocumentReference)
                _dontSetDocumentReference = true;
            else
                _dontSetDocumentReference = false;

            if ((options & DocumentComparatorOptions.StringComparisonIgnoreCase) == DocumentComparatorOptions.StringComparisonIgnoreCase)
                _stringComparisonIgnoreCase = true;
            else
                _stringComparisonIgnoreCase = false;

            if ((options & DocumentComparatorOptions.StringComparisonIgnoreWhiteSpace) == DocumentComparatorOptions.StringComparisonIgnoreWhiteSpace)
                _stringComparisonIgnoreWhiteSpace = true;
            else
                _stringComparisonIgnoreWhiteSpace = false;
        }

        public void SetEnumerateElementsOptions(EnumerateElementsOptions enumerateElementsOptions)
        {
            _enumerateElementsOptions = enumerateElementsOptions;
        }

        public void SetElementsToCompare(IEnumerable<string> elementsToCompare)
        {
            if (elementsToCompare == null)
                return;
            _elementsToCompare = new Dictionary<string, string>();
            foreach (string name in elementsToCompare)
                _elementsToCompare.Add(name, null);
        }

        public void SetDocumentReference(IEnumerable<string> elements)
        {
            if (elements != null)
            {
                List<string> documentReferenceElements1 = new List<string>();
                List<string> documentReferenceElements2 = new List<string>();
                // each element name must start with document1. or document2.
                foreach (string element in elements)
                {
                    if (element.StartsWith("document1."))
                        documentReferenceElements1.Add(element.Substring(10));
                    else if (element.StartsWith("document2."))
                        documentReferenceElements2.Add(element.Substring(10));
                    else
                        throw new PBException("document reference must start with document1. or document2. : \"{0}\"", element);
                }
                if (documentReferenceElements1.Count > 0)
                    _documentReferenceElements1 = documentReferenceElements1;
                if (documentReferenceElements2.Count > 0)
                    _documentReferenceElements2 = documentReferenceElements2;
            }
        }

        public CompareDocumentsResult Compare(IEnumerable<TwoDocuments> twoBsonDocumentsList)
        {
            CompareDocumentsResult resultAggregate = new CompareDocumentsResult();
            foreach (CompareDocumentResult result in _Compare(twoBsonDocumentsList))
            {
                bool equal = result.Equal;
                resultAggregate.AddResult(result, (!equal && _returnNotEqualDocuments) || (equal && _returnEqualDocuments));
            }
            return resultAggregate;
        }

        private IEnumerable<CompareDocumentResult> _Compare(IEnumerable<TwoDocuments> twoBsonDocumentsList)
        {
            foreach (TwoDocuments twoBsonDocuments in twoBsonDocumentsList)
            {
                CompareDocumentResult result = CompareBsonDocuments(twoBsonDocuments);
                //bool equal = result.Equal;
                //if ((!equal && _returnNotEqualDocuments) || (equal && _returnEqualDocuments))
                yield return result;
            }
        }

        private CompareDocumentResult CompareBsonDocuments(TwoDocuments twoBsonDocuments)
        {
            BsonDocument document1 = twoBsonDocuments.Document1;
            BsonDocument document2 = twoBsonDocuments.Document2;

            CompareDocumentResult result = new CompareDocumentResult(twoBsonDocuments, _comparatorOptions);
            result.DontSetDocumentReference = _dontSetDocumentReference;
            result.DocumentReferenceElements1 = _documentReferenceElements1;
            result.DocumentReferenceElements2 = _documentReferenceElements2;
            if (document1 == null && document2 == null)
                result.DocumentsType = DocumentsType.Document1AndDocument2AreNull;
            else if (document1 == null)
                result.DocumentsType = DocumentsType.Document1IsNull;
            else if (document2 == null)
                result.DocumentsType = DocumentsType.Document2IsNull;
            else
            {
                result.DocumentsType = DocumentsType.Document1AndDocument2AreNotNull;
                foreach (var twoDocElement in EnumerateTwoBsonDocumentsElements(document1, document2))
                {
                    CompareElementResult compareElements;
                    if (twoDocElement.Value1 != null && twoDocElement.Value2 != null)
                    {
                        //if (twoDocElement.Value1.IsString && twoDocElement.Value2.IsString)
                        //{
                        //    if (_stringComparisonIgnoreWhiteSpace)
                        //    {
                        //    }

                        //}
                        //if (_stringComparisonIgnoreCase && twoDocElement.Value1.IsString && twoDocElement.Value2.IsString)
                        //{
                        //    if (string.Equals(twoDocElement.Value1.AsString, twoDocElement.Value2.AsString, StringComparison.InvariantCultureIgnoreCase))
                        //        compareElements = CompareElementResult.Equal;
                        //    else
                        //        compareElements = CompareElementResult.NotEqual;
                        //}
                        //else if (twoDocElement.Value1 == twoDocElement.Value2)
                        //    compareElements = CompareElementResult.Equal;
                        //else
                        //    compareElements = CompareElementResult.NotEqual;
                        if (Equals(twoDocElement.Value1, twoDocElement.Value2))
                            compareElements = CompareElementResult.Equal;
                        else
                            compareElements = CompareElementResult.NotEqual;
                    }
                    else if (twoDocElement.Value1 != null)
                        compareElements = CompareElementResult.OnlyValue1;
                    else // if (twoDocElement.Value2 != null)
                        compareElements = CompareElementResult.OnlyValue2;
                    result.AddElement(twoDocElement.Name, compareElements, twoDocElement.Value1, twoDocElement.Value2);
                }
            }
            return result;
        }

        private bool Equals(BsonValue value1, BsonValue value2)
        {
            if (value1.IsString && value2.IsString)
            {
                if (_stringComparisonIgnoreWhiteSpace)
                    return StringCompare.EqualsIgnoreWhiteSpace(value1.AsString, value2.AsString, _stringComparisonIgnoreCase);

                if (_stringComparisonIgnoreCase)
                    return string.Equals(value1.AsString, value2.AsString, StringComparison.InvariantCultureIgnoreCase);
            }
            return value1 == value2;
        }

        private IEnumerable<TwoDocumentsElement> EnumerateTwoBsonDocumentsElements(BsonDocument document1, BsonDocument document2)
        {
            if (_enumerateElements == null)
                _enumerateElements = new EnumerateElements { Options = _enumerateElementsOptions };
            document1 = _enumerateElements.GetFlatDocument(document1);
            document2 = _enumerateElements.GetFlatDocument(document2);

            // enumerate first all elements of document1
            foreach (var element in document1.Elements)
            {
                if (_elementsToCompare != null && !_elementsToCompare.ContainsKey(element.Name))
                    continue;
                TwoDocumentsElement twoDocElement = new TwoDocumentsElement();
                twoDocElement.Name = element.Name;
                twoDocElement.Value1 = element.Value;
                if (document2.Contains(element.Name))
                    twoDocElement.Value2 = document2[element.Name];
                yield return twoDocElement;

            }

            // then enumerate elements of document2 that dont exist in document1
            foreach (var element in document2.Elements)
            {
                if (!document1.Contains(element.Name) && (_elementsToCompare == null || _elementsToCompare.ContainsKey(element.Name)))
                {
                    TwoDocumentsElement twoDocElement = new TwoDocumentsElement();
                    twoDocElement.Name = element.Name;
                    twoDocElement.Value2 = element.Value;
                    yield return twoDocElement;
                }
            }
        }

        // IEnumerable<CompareDocumentResult>
        public static CompareDocumentsResult CompareBsonDocumentFilesWithKey(string file1, string file2, string key1, string key2, JoinType joinType = JoinType.InnerJoin,
            DocumentComparatorOptions comparatorOptions = DocumentComparatorOptions.ReturnNotEqualDocuments, EnumerateElementsOptions enumerateElementsOptions = EnumerateElementsOptions.None,
            IEnumerable<string> elementsToCompare = null)
        {
            IEnumerable<TwoDocuments> query =
                zMongo.BsonRead<BsonDocument>(file1).zJoin(
                    zMongo.BsonRead<BsonDocument>(file2),
                    document1 => document1[key1],
                    document2 => document2[key2],
                    (document1, document2) => new TwoDocuments { Key = document1 != null ? document1.zGet(key1) : document2.zGet(key2), Document1 = document1, Document2 = document2 },
                    joinType);

            DocumentComparator comparator = new DocumentComparator();
            comparator.SetComparatorOptions(comparatorOptions);
            comparator.SetEnumerateElementsOptions(enumerateElementsOptions);
            comparator.SetElementsToCompare(elementsToCompare);
            CompareDocumentsResult result = comparator.Compare(query);
            result.Source1 = file1;
            result.Source2 = file2;
            return result;
        }

        // IEnumerable<CompareDocumentResult>
        public static CompareDocumentsResult CompareBsonDocumentFiles(string file1, string file2,
            DocumentComparatorOptions comparatorOptions = DocumentComparatorOptions.ReturnNotEqualDocuments, EnumerateElementsOptions enumerateElementsOptions = EnumerateElementsOptions.None,
            IEnumerable<string> elementsToCompare = null, IEnumerable<string> documentReference = null)
        {
            var query = EnumarateTwoBsonDocumentsList(zMongo.BsonRead<BsonDocument>(file1), zMongo.BsonRead<BsonDocument>(file2));

            DocumentComparator comparator = new DocumentComparator();
            comparator.SetComparatorOptions(comparatorOptions);
            comparator.SetEnumerateElementsOptions(enumerateElementsOptions);
            comparator.SetElementsToCompare(elementsToCompare);
            comparator.SetDocumentReference(documentReference);
            CompareDocumentsResult result = comparator.Compare(query);
            result.Source1 = file1;
            result.Source2 = file2;
            return result;
        }

        private static IEnumerable<TwoDocuments> EnumarateTwoBsonDocumentsList(IEnumerable<BsonDocument> documentList1, IEnumerable<BsonDocument> documentList2)
        {
            var enumerator1 = documentList1.GetEnumerator();
            var enumerator2 = documentList2.GetEnumerator();
            while (true)
            {
                bool next1 = enumerator1.MoveNext();
                bool next2 = enumerator2.MoveNext();
                BsonDocument document1 = enumerator1.Current;
                BsonDocument document2 = enumerator2.Current;
                if (next1 || next2)
                {
                    yield return new TwoDocuments { Document1 = document1, Document2 = document2 };
                }
                else
                    break;
            }
        }

        public static CompareDocumentResult CompareBsonDocuments(BsonDocument document1, BsonDocument document2,
            DocumentComparatorOptions comparatorOptions = DocumentComparatorOptions.ReturnNotEqualDocuments, EnumerateElementsOptions enumerateElementsOptions = EnumerateElementsOptions.None,
            IEnumerable<string> elementsToCompare = null)
        {
            DocumentComparator comparator = new DocumentComparator();
            comparator.SetComparatorOptions(comparatorOptions);
            comparator.SetEnumerateElementsOptions(enumerateElementsOptions);
            comparator.SetElementsToCompare(elementsToCompare);
            return comparator.CompareBsonDocuments(new TwoDocuments { Document1 = document1, Document2 = document2 });
        }
    }

    //public static partial class GlobalExtension
    //{
    //    public static CompareDocumentsResult zGetResult(this IEnumerable<CompareDocumentResult> compareResults)
    //    {
    //        CompareDocumentsResult resultAggregate = new CompareDocumentsResult();
    //        foreach (CompareDocumentResult result in compareResults)
    //        {
    //            resultAggregate.AddResult(result);
    //        }
    //        return resultAggregate;
    //    }

    //    public static IEnumerable<BsonDocument> zGetResultDocuments(this IEnumerable<CompareDocumentResult> compareResults)
    //    {
    //        CompareDocumentsResult resultAggregate = new CompareDocumentsResult();
    //        foreach (CompareDocumentResult result in compareResults)
    //        {
    //            resultAggregate.AddResult(result);
    //            yield return result.GetResultDocument();
    //        }
    //        yield return resultAggregate.GetResultAggregateDocument();
    //    }
    //}
}
