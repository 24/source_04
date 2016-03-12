using System;

namespace pb.Web
{
    public abstract class HtmlReaderBase : IDisposable
    {
        public abstract bool ReadCommentInText { get; set; }
        public abstract int Line { get; }
        public abstract int Column { get; }
        public abstract bool IsMarkBegin { get; }
        public abstract bool IsMarkEnd { get; }
        public abstract bool IsMarkBeginEnd { get; }
        public abstract bool IsMarkInProgress { get; }
        public abstract bool IsProperty { get; }
        public abstract bool IsText { get; }
        public abstract bool IsTextSeparator { get; }
        public abstract bool IsComment { get; }
        public abstract bool IsDocType { get; }
        public abstract bool IsScript { get; }
        public abstract string MarkName { get; }
        public abstract string PropertyName { get; }
        public abstract string PropertyValue { get; }
        public abstract string PropertyQuote { get; }
        public abstract string Text { get; }
        public abstract string Comment { get; }
        public abstract string DocType { get; }
        public abstract string Separator { get; }
        public abstract bool HasValue { get; }
        public abstract string Value { get; }

        public abstract void Dispose();
        public abstract bool Read();
        public abstract void Close();
    }
}
