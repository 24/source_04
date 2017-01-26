using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using pb.Data;

namespace pb.IO
{
    #region class FileReaderException
    public class FileReaderException : Exception
    {
        public FileReaderException(string sMessage) : base(sMessage) { }
        public FileReaderException(string sMessage, params object[] oPrm) : base(string.Format(sMessage, oPrm)) { }
        public FileReaderException(Exception InnerException, string sMessage) : base(sMessage, InnerException) { }
        public FileReaderException(Exception InnerException, string sMessage, params object[] oPrm) : base(string.Format(sMessage, oPrm), InnerException) { }
    }
    #endregion

    #region class FileReader
    //public class FileReader : IEnumerable, IEnumerator, IDisposable
    public class FileReader : IEnumerable<FileReader>, IEnumerator<FileReader>, IDisposable
    {
		#region enum FieldType
//		public enum FieldType
//		{
//			Unknown,
//			String,
//			DateTime,
//			Int,
//			Long,
//			Double,
//			Bool
//		}
		#endregion

		#region variable
		private string gsPath = null;
		private FileStream gfs = null;
		private int giBufferSize = 32000;  // taille buffer de FileStream
		private StreamReader gsr = null;
        private bool gbOpen = false;
		private Encoding gEncoding = Encoding.Default; // code caractère : Default = ANSI avec le code page du système
		private FieldList gFields = null;
		private bool gbReadAllFields = false;
        private string gsFieldSeparator = "\t";
		private string gsRecordTerminator = "\r\n";
		//private string[] sFormatDatetime = { "y-M-d h:m:s.f" };
		private bool gbDeleteChar0 = false;

		private int giRecordNumber = 1;

        public delegate void FieldAddedEvent(Field field);
        public event FieldAddedEvent FieldAdded = null;
        #endregion

		#region constructeur
        #region FileReader()
        public FileReader()
        {
            gFields = new FieldList();
        }
        #endregion

        #region FileReader(string sPath)
        public FileReader(string sPath)
        {
            gsPath = sPath;
            gFields = new FieldList();
        }
        #endregion

        #region FileReader(string sPath, string sFieldDef)
        public FileReader(string sPath, string sFieldDef)
        {
            gsPath = sPath;
            gFields = new FieldList(sFieldDef, gbReadAllFields);
        }
        #endregion

        #region FileReader(string sPath, string sFieldDef, Encoding encoding)
        public FileReader(string sPath, string sFieldDef, Encoding encoding)
        {
            gsPath = sPath;
            gEncoding = encoding;
            gFields = new FieldList(sFieldDef, gbReadAllFields);
        }
        #endregion

        #region FileReader(string sPath, string sFieldDef, Encoding encoding, string sFieldSeparator, string sRecordTerminator, bool bReadAllField)
        public FileReader(string sPath, string sFieldDef, Encoding encoding, string sFieldSeparator, string sRecordTerminator, bool bReadAllField)
		{
			gsPath = sPath;
			gEncoding = encoding;
			//if (sFieldSeparator != null && sFieldSeparator != "") gsFieldSeparator = sFieldSeparator;
            gsFieldSeparator = sFieldSeparator;
			if (sRecordTerminator != null && sRecordTerminator != "") gsRecordTerminator = sRecordTerminator;
			gbReadAllFields = bReadAllField;
			gFields = new FieldList(sFieldDef, bReadAllField);
        }
        #endregion

        #region FileReader(string sPath, FieldList Field, Encoding encoding, string sFieldSeparator, string sRecordTerminator, bool bReadAllField)
        public FileReader(string sPath, FieldList Field, Encoding encoding, string sFieldSeparator, string sRecordTerminator, bool bReadAllField)
		{
			gsPath = sPath;
			gEncoding = encoding;
			//if (sFieldSeparator != null && sFieldSeparator != "") gsFieldSeparator = sFieldSeparator;
            gsFieldSeparator = sFieldSeparator;
			if (sRecordTerminator != null && sRecordTerminator != "") gsRecordTerminator = sRecordTerminator;
			gbReadAllFields = bReadAllField;
			gFields = Field;
        }
        #endregion
        #endregion

        #region Dispose
        public void Dispose()
        {
            Close();
        }
        #endregion

        #region indexeur
        public Field this[int i]
		{
			get { return this.gFields[i]; }
		}

		public Field this[string sFieldName]
		{
			get { return this.gFields[sFieldName]; }
		}
		#endregion

		#region Path
		public string Path
		{
			get { return gsPath; }
			set { gsPath = value; }
		}
		#endregion

		#region DeleteChar0
		public bool DeleteChar0
		{
			get { return gbDeleteChar0; }
			set { gbDeleteChar0 = value; }
		}
		#endregion

		#region Fields
		public FieldList Fields
		{
			get { return gFields; }
			set { gFields = value; }
		}
		#endregion

        #region ReadAllFields
        public bool ReadAllFields
        {
            get { return gbReadAllFields; }
            set { gbReadAllFields = value; }
        }
        #endregion

        #region FieldSeparator
        public string FieldSeparator
        {
            get { return gsFieldSeparator; }
            set { gsFieldSeparator = value; }
        }
        #endregion

        #region RecordTerminator
        public string RecordTerminator
        {
            get { return gsRecordTerminator; }
            set { gsRecordTerminator = value; }
        }
        #endregion

        #region RecordNumber
        public int RecordNumber
        {
            get { return giRecordNumber; }
        }
        #endregion

		#region Count
		public int Count
		{
			get { return gFields.Count; }
		}
		#endregion

		#region ValueArray
		public object[] ValueArray
		{
			get { return gFields.ValueArray; }
		}
		#endregion

		#region Open
		private void Open()
		{
			Close();
            gfs = new FileStream(gsPath, FileMode.Open, FileAccess.Read, FileShare.Read, giBufferSize);
            gsr = new StreamReader(gfs, gEncoding);
            gbOpen = true;
		}
		#endregion

		#region Close
		public void Close()
		{
			if (gfs != null)
			{
				gfs.Close();
				gfs = null;
			}
			if (gsr != null)
			{
				gsr.Close();
				gsr = null;
			}
            gbOpen = false;
		}
		#endregion

		#region Read
		public bool Read()
		{
            if (!gbOpen) Open();
			if (gsr.Peek() == -1) return false;
            StringBuilder sb = new StringBuilder(1000, 1000000);
            bool bNextField;
            bool bEndOfRecord = false;
            bool bRecordFound = false;
            int iIdxFileField = -1;
            int iField = -1;
            Field f = null;
			while(true)
			{
                string sFieldSeparator = GetFieldSeparator(iField + 1, iIdxFileField + 1);

				bNextField = false;
				if (EndOfFieldRecord(gsRecordTerminator))
				{
					bEndOfRecord = true;
				}
				else if (EndOfFieldRecord(sFieldSeparator))
				{
					bNextField = true;
				}
				else
				{
                    int i = gsr.Read();
					if (i == -1)
					{
                        if (sb.Length == 0) break;
                        bEndOfRecord = true;
                        bNextField = true;
                    }
					else
						sb.Append((char)i);
				}

                if (bNextField || bEndOfRecord)
                {
                    bRecordFound = true;
                    if (bNextField || sb.Length != 0)
					{
						//bRecordFound = true;
						iIdxFileField++;
						if (++iField == gFields.Count)
						{
                            if (gbReadAllFields)
                            {
                                Field newField = gFields.Add();
                                if (FieldAdded != null) FieldAdded(newField);
                            }
                            else
                            {
                                if (!bEndOfRecord)
                                {
                                    SearchEndOfRecord();
                                    bEndOfRecord = true;
                                }
                            }
						}
						if (iField < gFields.Count)
						{
							f = gFields[iField];
							if (f.FieldIndex == iIdxFileField)
							{
								f.TextValue = sb.ToString();
								if (gbDeleteChar0)
									f.TextValue = f.TextValue.Replace(((char)0).ToString(), "");
								FieldConvertion(f);
							}
							else
								iField--;
						}
					}
					if (bEndOfRecord)
					{
						giRecordNumber++;
						break;
					}
					sb.Remove(0, sb.Length);
				}
			}
			for (iField++; iField < gFields.Count; iField++)
			{
				f = gFields[iField];
				f.TextValue = null;
				f.Value = f.DefaultValue;
			}
			if (bRecordFound)
				return true;
			else
				return false;
		}
		#endregion

		#region NextRecord
		public void NextRecord()
		{
			SearchEndOfRecord();
		}
		#endregion

		#region EndOfFieldRecord
		private bool EndOfFieldRecord(string sSeparator)
		{
            if (sSeparator == null || sSeparator == "") return false;
            for (int i = 0; i < sSeparator.Length; i++)
			{
				if (gsr.Peek() != (int)sSeparator[i]) return false;
				gsr.Read();
			}
			return true;
		}
		#endregion

		#region SearchEndOfRecord
		private void SearchEndOfRecord()
		{
			int i, i2;

			i = 0;
			while (i < gsRecordTerminator.Length)
			{
				i2 = gsr.Read();
				if (i2 == -1) break;
				if (i2 != (int)gsRecordTerminator[i]) i = 0;
				i++;
			}
		}
		#endregion

		#region GetFieldSeparator
		private string GetFieldSeparator(int iField, int iIdxFileField)
		{
			Field f;

			if (iField < gFields.Count)
			{
				f = gFields[iField];
				if (f.FieldIndex == iIdxFileField && f.FieldSeparator != null && f.FieldSeparator != "")
					return f.FieldSeparator;
			}
			return gsFieldSeparator;
		}
		#endregion

		#region //GetIndexedField
//		private int GetIndexedField(string sFieldName)
//		{
//			int i;
//			string s;
//
//			s = sFieldName.ToLower();
//			for (i = 0; i < galField.Count; i++)
//			{
//				if (((Field)galField[i]).sName.ToLower() == s) return i;
//			}
//			throw new FileReaderException("le champ \"{0}\" n'existe pas dans le fichier \"{1}\"", sFieldName, gsPath);
//		}
		#endregion

		#region //FieldAdd
//		private void FieldAdd()
//		{
//			Field f;
//
//			f = new Field();
//			f.iIdxFileField = galField.Count;
//			f.eType = FieldType.String;
//			f.sName = "Field" + cu.s(galField.Count + 1);
//			galField.Add(f);
//		}
		#endregion

		#region //GetFieldType
//		private FieldType GetFieldType(string sType)
//		{
//			switch(sType.ToLower())
//			{
//				case "bool":
//					return FieldType.Bool;
//				case "datetime":
//					return FieldType.DateTime;
//				case "double":
//					return FieldType.Double;
//				case "int":
//					return FieldType.Int;
//				case "long":
//					return FieldType.Long;
//				case "string":
//					return FieldType.String;
//			}
//			return FieldType.Unknown;
//		}
		#endregion

		#region FieldConvertion
		private void FieldConvertion(Field f)
		{
			try
			{
				gFields.FieldConvertion(f);
			}
			catch(Exception ex)
			{
                if (ex is FieldException)
                    throw new FileReaderException("erreur de convertion ligne {0} du fichier \"{1}\" : {2}", giRecordNumber, gsPath, ex.Message);
                else
                    throw new FileReaderException(ex, "erreur de convertion ligne {0} du fichier \"{1}\"", giRecordNumber, gsPath);
			}
		}
		#endregion

        #region //IEnumerable member
        //public IEnumerator GetEnumerator()
        //{
        //    return this;
        //}
        #endregion

        #region //IEnumerator member
        #region //Current
        //public object Current
        //{
        //    get { return this; }
        //}
        #endregion

        #region //MoveNext
        //public bool MoveNext()
        //{
        //    return Read();
        //}
        #endregion

        #region //Reset
        //public void Reset()
        //{
        //    Close();
        //}
        #endregion
        #endregion

        #region IEnumerable<FileReader> Members
        public IEnumerator<FileReader> GetEnumerator()
        {
            return this;
        }
        #endregion

        #region IEnumerable Members
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this;
        }
        #endregion

        #region IEnumerator<FileReader> Members
        public FileReader Current
        {
            get { return this; }
        }
        #endregion

        #region IEnumerator Members
        #region Current
        object IEnumerator.Current
        {
            get { return this; }
        }
        #endregion

        #region MoveNext
        public bool MoveNext()
        {
            return Read();
        }
        #endregion

        #region Reset
        public void Reset()
        {
            Close();
        }
        #endregion
        #endregion
    }
	#endregion
}
