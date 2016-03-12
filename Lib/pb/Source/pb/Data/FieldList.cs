using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.Data.OleDb;
using System.Data.SqlTypes;
using System.Globalization;
using pb.Text;

namespace pb.Data
{
	#region comment
	// à faire
	//  - FieldConvertDecimal()
	//  - gérer null, not null et collate database_default ... dans la définition des champs
	//  - remplacer interface par class virtuelle

	// modif du 23/02/2006 PB
	//  - controle du type smalldatetime (entre le 01/01/1900 et le 06/06/2079)
	#endregion

	#region //interface Field
//	public interface Field
//	{
//		string Name { get; set; }
//		int IdxField { get; set; }
//		Type Type { get; set; }
//		DbType dbType { get; set; }
//		OleDbType oleDbType { get; set; }
//		SqlDbType sqlDbType { get; set; }
//		OdbcType odbcType { get; set; }
//		string SqlType { get; }
//		bool IsNullable { get; set; }
//		int Size { get; set; }
//		byte Precision { get; set; }
//		byte Scale { get; set; }
//		string Format { get; set; }
//		string Collate { get; set; }
//		object Value { get; set; }
//		string TextValue { get; set; }
//		string FieldSeparator { get; set; }
//	}
	#endregion

    #region class FieldException
    public class FieldException : Exception
    {
        public FieldException(string sMessage) : base(sMessage) { }
        public FieldException(string sMessage, params object[] oPrm) : base(string.Format(sMessage, oPrm)) { }
        public FieldException(Exception InnerException, string sMessage) : base(sMessage, InnerException) { }
        public FieldException(Exception InnerException, string sMessage, params object[] oPrm) : base(string.Format(sMessage, oPrm), InnerException) { }
    }
    #endregion

    #region class Field
	public class Field
	{
		#region variable
		private string sName = null;
		private int iFieldIndex = -1;
		private ParameterDirection direction = ParameterDirection.Input;
		private Type _Type = null;
		private DbType _dbType = 0;
		private OleDbType _oleDbType = 0;
		private SqlDbType _sqlDbType = 0;
		private OdbcType _odbcType = 0;
        private Type _TypeCastBeforeCreateSqlServerType = null;
		private bool bIsNullable = true;
		private bool bIdentity = false;
		private int iSize = 0;
        private int iMaxSize = 0;            // taille maximum rencontré lors de la conversion d'un champ texte
        private byte byPrecision = 0;
		private byte byScale = 0;
		private string sFormat = null;
		private string sCollate = null;
		private object oValue = null;
		private string sTextValue;
		private string sFieldSeparator = null;
		private string sDefaultValue = null;
		private object oDefaultValue = null;
        private CultureInfo gCulture = new CultureInfo("en-US", true);
        #endregion

		#region constructor
        #region Field()
        public Field()
		{
        }
        #endregion

        #region Field(string sName, Type type)
        public Field(string sName, Type type)
        {
            this.sName = sName;
            this._Type = type;
            this._dbType = GetDbType(type);
            this._sqlDbType = GetSqlDbType(type);
            this._oleDbType = GetOleDbType(type);
            this._odbcType = GetOdbcType(type);
        }
        #endregion
		#endregion

		#region property
		public string Name { get { return sName; } set { sName = value; } }
		public int FieldIndex { get { return iFieldIndex; } set { iFieldIndex = value; } }
		public ParameterDirection Direction { get { return direction; } set { direction = value; } }
		public Type Type { get { return _Type; } set { _Type = value; } }
		public DbType dbType { get { return _dbType; } set { _dbType = value; } }
		public OleDbType oleDbType { get { return _oleDbType; } set { _oleDbType = value; } }
		public SqlDbType sqlDbType { get { return _sqlDbType; } set { _sqlDbType = value; } }
		public OdbcType odbcType { get { return _odbcType; } set { _odbcType = value; } }
        public Type TypeCastBeforeCreateSqlServerType { get { return _TypeCastBeforeCreateSqlServerType; } set { _TypeCastBeforeCreateSqlServerType = value; } }
        public Type SqlServerType { get { return GetSqlServerType(_sqlDbType); } }
        //public string SqlType { get { return GetSqlType(_sqlDbType, iSize, byPrecision, byScale, bIdentity, bIsNullable, sCollate, sDefaultValue); } }
        //public string SqlType { get { return GetSqlType(); } }
        //public string SqlDefinition
        //{
        //    get
        //    {
        //        return GetSqlDefinition();
        //        //int size = iSize;
        //        //if (size == 0 && iMaxSize != 0) size = iMaxSize;
        //        //return sName + " " + GetSqlType(_sqlDbType, size, byPrecision, byScale, bIdentity, bIsNullable, sCollate, sDefaultValue);
        //    }
        //}
		public bool IsNullable { get { return bIsNullable; } set { bIsNullable = value; } }
		public bool Identity { get { return bIdentity; } set { bIdentity = value; } }
		public int Size { get { return iSize; } set { iSize = value; } }
        /// <summary>
        /// taille maximum rencontré lors de la conversion d'un champ texte
        /// </summary>
        public int MaxSize { get { return iMaxSize; } }
        public byte Precision { get { return byPrecision; } set { byPrecision = value; } }
		public byte Scale { get { return byScale; } set { byScale = value; } }
		public string Format { get { return sFormat; } set { sFormat = value; } }
		public string Collate { get { return sCollate; } set { sCollate = value; } }
		public object Value { get { return oValue; } set { oValue = value; } }
		public string TextValue { get { return sTextValue; } set { sTextValue = value; } }
		public string FieldSeparator { get { return sFieldSeparator; } set { sFieldSeparator = value; } }
		public object DefaultValue { get { return oDefaultValue; } set { oDefaultValue = value; } }
		public string DefaultValueText { get { return sDefaultValue; } set { sDefaultValue = value; } }
		#endregion

        #region GetFieldDefinition()
        /// <summary>
        /// string de définition du champ
        /// </summary>
        public string GetFieldDefinition()
        {
            return GetFieldDefinition(false);
        }
        #endregion

        #region GetFieldDefinition(bool bUnicodeChar)
        /// <summary>
        /// string de définition du champ
        /// </summary>
        public string GetFieldDefinition(bool bUnicodeChar)
        {
            // [numéro d'ordre] [nom] [type [identity] [collate ...] [default value] [[not] null] ["format"]], ...
            //string sDefinition = string.Format("{0} {1}", iFieldIndex + 1, GetFieldType(bUnicodeChar));
            string sDefinition = string.Format("{0} {1} {2}", iFieldIndex + 1, sName, GetFieldType(bUnicodeChar));
            if (sFormat != null && sFormat != "") sDefinition += string.Format(" '{0}'", sFormat);
            return sDefinition;
        }
        #endregion

        #region GetSqlServerType(SqlDbType sqlDbType)
        public static Type GetSqlServerType(SqlDbType sqlDbType)
        {
            switch (sqlDbType)
            {
                // boolean type
                case SqlDbType.Bit:
                    return typeof(SqlBoolean);

                // integer type
                case SqlDbType.TinyInt:
                    return typeof(SqlByte);
                case SqlDbType.SmallInt:
                    return typeof(SqlInt16);
                case SqlDbType.Int:
                    return typeof(SqlInt32);
                case SqlDbType.BigInt:
                    return typeof(SqlInt64);

                // float type
                case SqlDbType.Float:
                    return typeof(SqlDouble);
                case SqlDbType.Real:
                    return typeof(SqlSingle);

                // numeric type
                case SqlDbType.Decimal:
                    return typeof(SqlDecimal);

                // money type
                case SqlDbType.Money:
                case SqlDbType.SmallMoney:
                    return typeof(SqlMoney);

                // datetime type
                case SqlDbType.DateTime:
                case SqlDbType.SmallDateTime:
                    return typeof(SqlDateTime);

                // character type
                case SqlDbType.Char:
                case SqlDbType.NChar:
                case SqlDbType.VarChar:
                case SqlDbType.NVarChar:
                case SqlDbType.Text:
                case SqlDbType.NText:
                    //return typeof(SqlChars);
                    return typeof(SqlString);
                default:
                    throw new FieldException("le type SqlDbType {0} n'existe pas", sqlDbType);
            }
        }
        #endregion

        #region GetDefinition()
        public string GetDefinition()
        {
            return GetDefinition(false);
        }
        #endregion

        #region GetDefinition(bool bUnicodeChar)
        public string GetDefinition(bool bUnicodeChar)
        {
            return sName + " " + GetFieldType(bUnicodeChar);
        }
        #endregion

        #region GetSqlDefinition()
        public string GetSqlDefinition()
        {
            return GetSqlDefinition(false);
        }
        #endregion

        #region GetSqlDefinition(bool bUnicodeChar)
        public string GetSqlDefinition(bool bUnicodeChar)
        {
            return sName + " " + GetSqlType(bUnicodeChar);
        }
        #endregion

        #region GetFieldType(bool bUnicodeChar)
        public string GetFieldType(bool bUnicodeChar)
        {
            int size = iSize;
            if (size == 0 && iMaxSize != 0) size = iMaxSize;
            if (size == 0) size = 1;
            return GetFieldType(_Type, _sqlDbType, size, byPrecision, byScale, bIdentity, bIsNullable, sCollate, sDefaultValue, bUnicodeChar);
        }
        #endregion

        #region GetFieldType(SqlDbType sqlDbType, int iSize, byte iPrecision, byte iScale, bool bIdentity, bool bIsNullable, string sCollate, string sDefaultValue, bool bUnicodeChar)
        public static string GetFieldType(Type type, SqlDbType sqlDbType, int iSize, byte iPrecision, byte iScale, bool bIdentity, bool bIsNullable, string sCollate, string sDefaultValue, bool bUnicodeChar)
        {
            string s;

            if (type == typeof(Date))
            {
                s = "date";
            }
            else
            {
                switch (sqlDbType)
                {
                    // boolean type
                    case SqlDbType.Bit:
                        s = "bit";
                        break;

                    // integer type
                    case SqlDbType.TinyInt:
                        s = "tinyint";
                        break;
                    case SqlDbType.SmallInt:
                        s = "smallint";
                        break;
                    case SqlDbType.Int:
                        s = "int";
                        break;
                    case SqlDbType.BigInt:
                        s = "bigint";
                        break;

                    // float type
                    case SqlDbType.Float:
                        s = "float";
                        break;
                    case SqlDbType.Real:
                        s = "real";
                        break;

                    // numeric type
                    case SqlDbType.Decimal:
                        s = string.Format("numeric({0},{1})", iPrecision, iScale);
                        break;

                    // money type
                    case SqlDbType.Money:
                        s = "money";
                        break;
                    case SqlDbType.SmallMoney:
                        s = "smallmoney";
                        break;

                    // datetime type
                    case SqlDbType.DateTime:
                        s = "datetime";
                        break;
                    case SqlDbType.SmallDateTime:
                        s = "smalldatetime";
                        break;

                    // character type
                    case SqlDbType.Char:
                        if (bUnicodeChar)
                            s = string.Format("nchar({0})", iSize);
                        else
                            s = string.Format("char({0})", iSize);
                        break;
                    case SqlDbType.NChar:
                        s = string.Format("nchar({0})", iSize);
                        break;
                    case SqlDbType.VarChar:
                        if (bUnicodeChar)
                            s = string.Format("nvarchar({0})", iSize);
                        else
                            s = string.Format("varchar({0})", iSize);
                        break;
                    case SqlDbType.NVarChar:
                        s = string.Format("nvarchar({0})", iSize);
                        break;
                    case SqlDbType.Text:
                        if (bUnicodeChar)
                            s = "ntext";
                        else
                            s = "text";
                        break;
                    case SqlDbType.NText:
                        s = "ntext";
                        break;
                    default:
                        throw new FieldException("le type SqlDbType {0} n'existe pas", sqlDbType);
                }
            }
            if (bIdentity) s += " identity";
            if (sCollate != null && sCollate != "") s += " collate " + sCollate;
            if (sDefaultValue != null && sDefaultValue != "") s += " default " + sDefaultValue;
            if (!bIsNullable) s += " not";
            s += " null";
            return s;
        }
        #endregion

        #region GetSqlType()
        public string GetSqlType()
        {
            return GetSqlType(false);
        }
        #endregion

        #region GetSqlType(bool bUnicodeChar)
        public string GetSqlType(bool bUnicodeChar)
        {
            int size = iSize;
            if (size == 0 && iMaxSize != 0) size = iMaxSize;
            if (size == 0) size = 1;
            return GetSqlType(_sqlDbType, size, byPrecision, byScale, bIdentity, bIsNullable, sCollate, sDefaultValue, bUnicodeChar);
        }
        #endregion

        #region GetSqlType(SqlDbType sqlDbType, int iSize, byte iPrecision, byte iScale, bool bIdentity, bool bIsNullable, string sCollate, string sDefaultValue)
        public static string GetSqlType(SqlDbType sqlDbType, int iSize, byte iPrecision, byte iScale, bool bIdentity, bool bIsNullable, string sCollate, string sDefaultValue)
        {
            return GetSqlType(sqlDbType, iSize, iPrecision, iScale, bIdentity, bIsNullable, sCollate, sDefaultValue, false);
        }
        #endregion

        #region GetSqlType(SqlDbType sqlDbType, int iSize, byte iPrecision, byte iScale, bool bIdentity, bool bIsNullable, string sCollate, string sDefaultValue, bool bUnicodeChar)
        public static string GetSqlType(SqlDbType sqlDbType, int iSize, byte iPrecision, byte iScale, bool bIdentity, bool bIsNullable, string sCollate, string sDefaultValue, bool bUnicodeChar)
		{
			string s;

			switch (sqlDbType)
			{
					// boolean type
				case SqlDbType.Bit:
					s = "bit";
					break;

					// integer type
				case SqlDbType.TinyInt:
					s = "tinyint";
					break;
				case SqlDbType.SmallInt:
					s = "smallint";
					break;
				case SqlDbType.Int:
					s = "int";
					break;
				case SqlDbType.BigInt:
					s = "bigint";
					break;

					// float type
				case SqlDbType.Float:
					s = "float";
					break;
				case SqlDbType.Real:
					s = "real";
					break;

					// numeric type
				case SqlDbType.Decimal:
					s = string.Format("numeric({0},{1})", iPrecision, iScale);
					break;

					// money type
				case SqlDbType.Money:
					s = "money";
					break;
				case SqlDbType.SmallMoney:
					s = "smallmoney";
					break;

					// datetime type
				case SqlDbType.DateTime:
					s = "datetime";
					break;
				case SqlDbType.SmallDateTime:
					s = "smalldatetime";
					break;

					// character type
				case SqlDbType.Char:
                    if (bUnicodeChar)
                        s = string.Format("nchar({0})", iSize);
                    else
                        s = string.Format("char({0})", iSize);
					break;
				case SqlDbType.NChar:
					s = string.Format("nchar({0})", iSize);
					break;
				case SqlDbType.VarChar:
                    if (bUnicodeChar)
                        s = string.Format("nvarchar({0})", iSize);
                    else
                        s = string.Format("varchar({0})", iSize);
					break;
				case SqlDbType.NVarChar:
					s = string.Format("nvarchar({0})", iSize);
					break;
				case SqlDbType.Text:
                    if (bUnicodeChar)
                        //s = "ntext";
                        s = "nvarchar(max)";
                    else
                        //s = "text";
                        s = "varchar(max)";
                    break;
				case SqlDbType.NText:
					//s = "ntext";
                    s = "nvarchar(max)";
                    break;
				default:
					throw new FieldException("le type SqlDbType {0} n'existe pas", sqlDbType);
			}
			if (bIdentity) s += " identity";
			if (sCollate != null && sCollate != "") s += " collate " + sCollate;
			if (sDefaultValue != null && sDefaultValue != "") s += " default " + sDefaultValue;
			if (!bIsNullable) s += " not";
			s += " null";
			return s;
		}
		#endregion

        #region GetDbType
        public static DbType GetDbType(Type type)
        {
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Boolean:
                    return DbType.Boolean;

                // integer type
                case TypeCode.Byte:
                    return DbType.Byte;
                case TypeCode.Int16:
                    return DbType.Int16;
                case TypeCode.Int32:
                    return DbType.Int32;
                case TypeCode.Int64:
                    return DbType.Int64;

                // float type
                case TypeCode.Single:
                    return DbType.Single;
                case TypeCode.Double:
                    return DbType.Double;

                // numeric type
                case TypeCode.Decimal:
                    return DbType.Decimal;

                // datetime type
                case TypeCode.DateTime:
                    return DbType.DateTime;

                // character type
                case TypeCode.Char:
                    return DbType.StringFixedLength;
                case TypeCode.String:
                    return DbType.String;

                case TypeCode.Object:
                    if (type == typeof(bool?))
                        return DbType.Boolean;
                    else if (type == typeof(byte?))
                        return DbType.Byte;
                    else if (type == typeof(short?))
                        return DbType.Int16;
                    else if (type == typeof(int?))
                        return DbType.Int32;
                    else if (type == typeof(long?))
                        return DbType.Int64;
                    else if (type == typeof(float?))
                        return DbType.Single;
                    else if (type == typeof(double?))
                        return DbType.Double;
                    else if (type == typeof(decimal?))
                        return DbType.Decimal;
                    else if (type == typeof(DateTime?))
                        return DbType.DateTime;
                    else if (type == typeof(char?))
                        return DbType.StringFixedLength;
                    else if (type == typeof(Date))
                        return DbType.DateTime;
                    break;
            }
            throw new FieldException("le type \"{0}\" n'est pas géré par la class Field", type.Name);
        }
        #endregion

        #region GetSqlDbType
        public static SqlDbType GetSqlDbType(Type type)
        {
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Boolean:
                    return SqlDbType.Bit;

                // integer type
                case TypeCode.Byte:
                    return SqlDbType.TinyInt;
                case TypeCode.Int16:
                    return SqlDbType.SmallInt;
                case TypeCode.Int32:
                    return SqlDbType.Int;
                case TypeCode.Int64:
                    return SqlDbType.BigInt;

                // float type
                case TypeCode.Single:
                    return SqlDbType.Real;
                case TypeCode.Double:
                    return SqlDbType.Float;

                // numeric type
                case TypeCode.Decimal:
                    return SqlDbType.Decimal;

                // datetime type
                case TypeCode.DateTime:
                    return SqlDbType.DateTime;

                // character type
                case TypeCode.Char:
                    return SqlDbType.Char;
                case TypeCode.String:
                    return SqlDbType.VarChar;

                case TypeCode.Object:
                    if (type == typeof(bool?))
                        return SqlDbType.Bit;
                    else if (type == typeof(byte?))
                        return SqlDbType.TinyInt;
                    else if (type == typeof(short?))
                        return SqlDbType.SmallInt;
                    else if (type == typeof(int?))
                        return SqlDbType.Int;
                    else if (type == typeof(long?))
                        return SqlDbType.BigInt;
                    else if (type == typeof(float?))
                        return SqlDbType.Real;
                    else if (type == typeof(double?))
                        return SqlDbType.Float;
                    else if (type == typeof(decimal?))
                        return SqlDbType.Decimal;
                    else if (type == typeof(DateTime?))
                        return SqlDbType.DateTime;
                    else if (type == typeof(char?))
                        return SqlDbType.Char;
                    else if (type == typeof(Date))
                        return SqlDbType.DateTime;
                    break;
            }
            throw new FieldException("le type \"{0}\" n'est pas géré par la class Field", type.Name);
        }
        #endregion

        #region GetOleDbType
        public static OleDbType GetOleDbType(Type type)
        {
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Boolean:
                    return OleDbType.Boolean;

                // integer type
                case TypeCode.Byte:
                    return OleDbType.TinyInt;
                case TypeCode.Int16:
                    return OleDbType.SmallInt;
                case TypeCode.Int32:
                    return OleDbType.Integer;
                case TypeCode.Int64:
                    return OleDbType.BigInt;

                // float type
                case TypeCode.Single:
                    return OleDbType.Single;
                case TypeCode.Double:
                    return OleDbType.Double;

                // numeric type
                case TypeCode.Decimal:
                    return OleDbType.Decimal;

                // datetime type
                case TypeCode.DateTime:
                    return OleDbType.DBTimeStamp;

                // character type
                case TypeCode.Char:
                    return OleDbType.Char;
                case TypeCode.String:
                    return OleDbType.VarChar;

                case TypeCode.Object:
                    if (type == typeof(bool?))
                        return OleDbType.Boolean;
                    else if (type == typeof(byte?))
                        return OleDbType.TinyInt;
                    else if (type == typeof(short?))
                        return OleDbType.SmallInt;
                    else if (type == typeof(int?))
                        return OleDbType.Integer;
                    else if (type == typeof(long?))
                        return OleDbType.BigInt;
                    else if (type == typeof(float?))
                        return OleDbType.Single;
                    else if (type == typeof(double?))
                        return OleDbType.Double;
                    else if (type == typeof(decimal?))
                        return OleDbType.Decimal;
                    else if (type == typeof(DateTime?))
                        return OleDbType.DBTimeStamp;
                    else if (type == typeof(char?))
                        return OleDbType.Char;
                    else if (type == typeof(Date))
                        return OleDbType.DBTimeStamp;
                    break;
            }
            throw new FieldException("le type \"{0}\" n'est pas géré par la class Field", type.Name);
        }
        #endregion

        #region GetOdbcType
        public static OdbcType GetOdbcType(Type type)
        {
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Boolean:
                    return OdbcType.Bit;

                // integer type
                case TypeCode.Byte:
                    return OdbcType.TinyInt;
                case TypeCode.Int16:
                    return OdbcType.SmallInt;
                case TypeCode.Int32:
                    return OdbcType.Int;
                case TypeCode.Int64:
                    return OdbcType.BigInt;

                // float type
                case TypeCode.Single:
                    return OdbcType.Real;
                case TypeCode.Double:
                    return OdbcType.Double;

                // numeric type
                case TypeCode.Decimal:
                    return OdbcType.Decimal;

                // datetime type
                case TypeCode.DateTime:
                    return OdbcType.DateTime;

                // character type
                case TypeCode.Char:
                    return OdbcType.Char;
                case TypeCode.String:
                    return OdbcType.VarChar;

                case TypeCode.Object:
                    if (type == typeof(bool?))
                        return OdbcType.Bit;
                    else if (type == typeof(byte?))
                        return OdbcType.TinyInt;
                    else if (type == typeof(short?))
                        return OdbcType.SmallInt;
                    else if (type == typeof(int?))
                        return OdbcType.Int;
                    else if (type == typeof(long?))
                        return OdbcType.BigInt;
                    else if (type == typeof(float?))
                        return OdbcType.Real;
                    else if (type == typeof(double?))
                        return OdbcType.Double;
                    else if (type == typeof(decimal?))
                        return OdbcType.Decimal;
                    else if (type == typeof(DateTime?))
                        return OdbcType.DateTime;
                    else if (type == typeof(char?))
                        return OdbcType.Char;
                    else if (type == typeof(Date))
                        return OdbcType.DateTime;
                    break;
            }
            throw new FieldException("le type \"{0}\" n'est pas géré par la class Field", type.Name);
        }
        #endregion

        #region Convert
        public void Convert()
        {
            Convert(this, gCulture);
        }
        #endregion

        #region Convert(Field f, CultureInfo culture)
        public static void Convert(Field f, CultureInfo culture)
        {
            switch (Type.GetTypeCode(f.Type))
            {
                case TypeCode.Boolean:
                    BoolConvert(f);
                    break;
                case TypeCode.DateTime:
                    DateTimeConvert(f, culture);
                    break;
                case TypeCode.Single:
                    FloatConvert(f, culture);
                    break;
                case TypeCode.Double:
                    DoubleConvert(f, culture);
                    break;
                case TypeCode.Decimal:
                    DecimalConvert(f, culture);
                    break;
                case TypeCode.Byte:
                    ByteConvert(f);
                    break;
                case TypeCode.Int16:
                    Int16Convert(f);
                    break;
                case TypeCode.Int32:
                    Int32Convert(f);
                    break;
                case TypeCode.Int64:
                    Int64Convert(f);
                    break;
                case TypeCode.String:
                    StringConvert(f);
                    break;
                case TypeCode.Object:
                    if (f.Type == typeof(Date))
                        DateConvert(f, culture);
                    break;
            }
            if (f.Value == null) f.Value = f.DefaultValue;
        }
        #endregion

        #region BoolConvert
        private static void BoolConvert(Field f)
        {
            int i;
            string s;

            s = f.TextValue.Trim();
            if (s == "")
            {
                if (!f.IsNullable) throw new FieldException("la valeur du champ \"{0}\" ne peut pas être nulle", f.Name);
                f.Value = null;
            }
            else if (zconvert.IsInt(s))
            {
                //i = zconvert.Int(s);
                i = s.zTryParseAs<int>();
                if (i == 0) f.Value = false; else f.Value = true;
            }
            else
            {
                //try
                //{
                //    f.Value = bool.Parse(f.TextValue);
                //}
                //catch (Exception ex)
                //{
                //    throw new FieldException(ex, "la valeur \"{0}\" du champ \"{1}\" n'est pas un booléen (true, >0, <0, false, 0)", f.TextValue, f.Name);
                //}
                bool bValue;
                if (!bool.TryParse(f.TextValue, out bValue))
                    throw new FieldException("la valeur \"{0}\" du champ \"{1}\" n'est pas un booléen (true, >0, <0, false, 0)", f.TextValue, f.Name);
                f.Value = bValue;
            }
        }
        #endregion

        #region DateTimeConvert
        private static void DateTimeConvert(Field f, CultureInfo culture)
        {
            string s;
            //DateTime dt, dtSmallDatetimeMin, dtSmallDatetimeMax;
            DateTimeStyles dts;

            dts = DateTimeStyles.AllowWhiteSpaces | DateTimeStyles.NoCurrentDateDefault;
            s = f.TextValue.Trim();
            if (s == "")
            {
                if (!f.IsNullable) throw new FieldException("la valeur du champ \"{0}\" ne peut pas être nulle", f.Name);
                f.Value = null;
            }
            else
            {
                //try
                //{
                //    if (f.Format == null || f.Format == "")
                //        f.Value = DateTime.Parse(s, culture, dts);
                //    else
                //        f.Value = DateTime.ParseExact(s, f.Format, culture, dts);
                //}
                //catch (Exception ex)
                //{
                //    throw new FieldException(ex, "la valeur \"{0}\" du champ \"{1}\" n'est pas une date ou une heure dont le format est \"{2}\"", f.TextValue, f.Name, f.Format);
                //}
                DateTime dt;
                bool b;
                if (f.Format == null || f.Format == "")
                    b = DateTime.TryParse(s, culture, dts, out dt);
                else
                    b = DateTime.TryParseExact(s, f.Format, culture, dts, out dt);
                if (!b) throw new FieldException("la valeur \"{0}\" du champ \"{1}\" n'est pas une date ou une heure dont le format est \"{2}\"", f.TextValue, f.Name, f.Format);
                f.Value = dt;

                if (f.sqlDbType == SqlDbType.SmallDateTime)
                {
                    dt = (DateTime)f.Value;
                    DateTime dtSmallDatetimeMin = new DateTime(1900, 1, 1);
                    DateTime dtSmallDatetimeMax = new DateTime(2079, 6, 7);
                    if (dt < dtSmallDatetimeMin || dt >= dtSmallDatetimeMax)
                    {
                        if (!f.IsNullable)
                            throw new FieldException("la valeur \"{0:dd/MM/yyyy}\" du champ \"{1}\" de type smalldatetime doît être compris entre 01/01/1900 et 06/06/2079", f.Value, f.Name);
                        f.Value = null;
                    }
                }
            }
        }
        #endregion

        #region DateConvert
        private static void DateConvert(Field f, CultureInfo culture)
        {
            string s;
            DateTimeStyles dts;

            dts = DateTimeStyles.AllowWhiteSpaces | DateTimeStyles.NoCurrentDateDefault;
            s = f.TextValue.Trim();
            if (s == "")
            {
                if (!f.IsNullable) throw new FieldException("la valeur du champ \"{0}\" ne peut pas être nulle", f.Name);
                f.Value = null;
            }
            else
            {
                //DateTime dt;
                Date dt;
                bool b;
                if (f.Format == null || f.Format == "")
                    b = Date.TryParse(s, culture, dts, out dt);
                else
                    b = Date.TryParseExact(s, f.Format, culture, dts, out dt);
                if (!b) throw new FieldException("la valeur \"{0}\" du champ \"{1}\" n'est pas une date ou une heure dont le format est \"{2}\"", f.TextValue, f.Name, f.Format);
                //f.Value = (Date)dt;
                f.Value = dt;

                if (f.sqlDbType == SqlDbType.SmallDateTime)
                {
                    //dt = (DateTime)f.Value;
                    //DateTime dtSmallDatetimeMin = new DateTime(1900, 1, 1);
                    //DateTime dtSmallDatetimeMax = new DateTime(2079, 6, 7);
                    Date dtSmallDatetimeMin = new Date(1900, 1, 1);
                    Date dtSmallDatetimeMax = new Date(2079, 6, 7);
                    if (dt < dtSmallDatetimeMin || dt >= dtSmallDatetimeMax)
                    {
                        if (!f.IsNullable)
                            throw new FieldException("la valeur \"{0:dd/MM/yyyy}\" du champ \"{1}\" de type smalldatetime doît être compris entre 01/01/1900 et 06/06/2079", f.Value, f.Name);
                        f.Value = null;
                    }
                }
            }
        }
        #endregion

        #region FloatConvert
        private static void FloatConvert(Field f, CultureInfo culture)
        {
            string s;
            NumberStyles ns;

            s = f.TextValue.Trim();
            if (s == "")
            {
                if (!f.IsNullable) throw new FieldException("la valeur du champ \"{0}\" ne peut pas être nulle", f.Name);
                f.Value = null;
            }
            else
            {
                ns = NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign | NumberStyles.AllowLeadingWhite | NumberStyles.AllowThousands;
                //try
                //{
                //    f.Value = Single.Parse(s, ns, culture);
                //}
                //catch (Exception ex)
                //{
                //    throw new FieldException(ex, "la valeur \"{0}\" du champ \"{1}\" n'est pas un float", f.TextValue, f.Name);
                //}
                float fValue;
                if (!float.TryParse(s, ns, culture, out fValue))
                    throw new FieldException("la valeur \"{0}\" du champ \"{1}\" n'est pas un float", f.TextValue, f.Name);
                f.Value = fValue;
            }
        }
        #endregion

        #region DoubleConvert
        private static void DoubleConvert(Field f, CultureInfo culture)
        {
            string s;
            NumberStyles ns;

            s = f.TextValue.Trim();
            if (s == "")
            {
                if (!f.IsNullable) throw new FieldException("la valeur du champ \"{0}\" ne peut pas être nulle", f.Name);
                f.Value = null;
            }
            else
            {
                ns = NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign | NumberStyles.AllowLeadingWhite | NumberStyles.AllowThousands;
                //try
                //{
                //    f.Value = Double.Parse(s, ns, culture);
                //}
                //catch (Exception ex)
                //{
                //    throw new FieldException(ex, "la valeur \"{0}\" du champ \"{1}\" n'est pas un double", f.TextValue, f.Name);
                //}
                double d;
                if (!double.TryParse(s, ns, culture, out d))
                    throw new FieldException("la valeur \"{0}\" du champ \"{1}\" n'est pas un double", f.TextValue, f.Name);
                f.Value = d;
            }
        }
        #endregion

        #region DecimalConvert
        private static void DecimalConvert(Field f, CultureInfo culture)
        {
            string s;
            NumberStyles ns;

            s = f.TextValue.Trim();
            if (s == "")
            {
                if (!f.IsNullable) throw new FieldException("la valeur du champ \"{0}\" ne peut pas être nulle", f.Name);
                f.Value = null;
            }
            else
            {
                ns = NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign | NumberStyles.AllowLeadingWhite | NumberStyles.AllowThousands;
                //try
                //{
                //    f.Value = Decimal.Parse(s, ns, culture);
                //}
                //catch (Exception ex)
                //{
                //    throw new FieldException(ex, "la valeur \"{0}\" du champ \"{1}\" n'est pas de type décimal", f.TextValue, f.Name);
                //}
                decimal d;
                if (!decimal.TryParse(s, ns, culture, out d))
                    throw new FieldException("la valeur \"{0}\" du champ \"{1}\" n'est pas de type décimal", f.TextValue, f.Name);
                f.Value = d;
            }
        }
        #endregion

        #region ByteConvert
        private static void ByteConvert(Field f)
        {
            string s;

            s = f.TextValue.Trim();
            if (s == "")
            {
                if (!f.IsNullable) throw new FieldException("la valeur du champ \"{0}\" ne peut pas être nulle", f.Name);
                f.Value = null;
            }
            else
            {
                //try
                //{
                //    f.Value = Byte.Parse(s);
                //}
                //catch (Exception ex)
                //{
                //    throw new FieldException(ex, "la valeur \"{0}\" du champ \"{1}\" n'est pas un entier (Byte)", f.TextValue, f.Name);
                //}
                byte by;
                if (!byte.TryParse(s, NumberStyles.Any, NumberFormatInfo.InvariantInfo, out by))
                    throw new FieldException("la valeur \"{0}\" du champ \"{1}\" n'est pas un entier (Byte)", f.TextValue, f.Name);
                f.Value = by;
            }
        }
        #endregion

        #region Int16Convert
        private static void Int16Convert(Field f)
        {
            string s;

            s = f.TextValue.Trim();
            if (s == "")
            {
                if (!f.IsNullable) throw new FieldException("la valeur du champ \"{0}\" ne peut pas être nulle", f.Name);
                f.Value = null;
            }
            else
            {
                //try
                //{
                //    f.Value = Int16.Parse(s, NumberStyles.Any);
                //}
                //catch (Exception ex)
                //{
                //    throw new FieldException(ex, "la valeur \"{0}\" du champ \"{1}\" n'est pas un entier (Int16)", f.TextValue, f.Name);
                //}
                short sh;
                if (!short.TryParse(s, NumberStyles.Any, NumberFormatInfo.InvariantInfo, out sh))
                    throw new FieldException("la valeur \"{0}\" du champ \"{1}\" n'est pas un entier (Int16)", f.TextValue, f.Name);
                f.Value = sh;
            }
        }
        #endregion

        #region Int32Convert
        private static void Int32Convert(Field f)
        {
            string s;

            s = f.TextValue.Trim();
            if (s == "")
            {
                if (!f.IsNullable) throw new FieldException("la valeur du champ \"{0}\" ne peut pas être nulle", f.Name);
                f.Value = null;
            }
            else
            {
                //try
                //{
                //    f.Value = Int32.Parse(s, NumberStyles.Any);
                //}
                //catch (Exception ex)
                //{
                //    throw new FieldException(ex, "la valeur \"{0}\" du champ \"{1}\" n'est pas un entier (Int32)", f.TextValue, f.Name);
                //}
                int i;
                if (!int.TryParse(s, NumberStyles.Any, NumberFormatInfo.InvariantInfo, out i))
                    throw new FieldException("la valeur \"{0}\" du champ \"{1}\" n'est pas un entier (Int32)", f.TextValue, f.Name);
                f.Value = i;
            }
        }
        #endregion

        #region Int64Convert
        private static void Int64Convert(Field f)
        {
            string s;

            s = f.TextValue.Trim();
            if (s == "")
            {
                if (!f.IsNullable) throw new FieldException("la valeur du champ \"{0}\" ne peut pas être nulle", f.Name);
                f.Value = null;
            }
            else
            {
                //try
                //{
                //    f.Value = Int64.Parse(s, NumberStyles.Any);
                //}
                //catch (Exception ex)
                //{
                //    throw new FieldException(ex, "la valeur \"{0}\" du champ \"{1}\" n'est pas un entier (Int64)", f.TextValue, f.Name);
                //}
                long l;
                if (!long.TryParse(s, NumberStyles.Any, NumberFormatInfo.InvariantInfo, out l))
                    throw new FieldException("la valeur \"{0}\" du champ \"{1}\" n'est pas un entier (Int64)", f.TextValue, f.Name);
                f.Value = l;
            }
        }
        #endregion

        #region StringConvert
        private static void StringConvert(Field f)
        {
            string s = f.TextValue.TrimEnd();
            f.Value = s;
            if (s.Length > f.iMaxSize) f.iMaxSize = s.Length;
            if (s == "" && f.IsNullable) f.Value = null;
            if (f.Size != 0 && s.Length > f.Size) throw new FieldException("la valeur du champ \"{0}\" a une longueur de {1} caractères qui dépasse la taille du champ ({2})", f.Name, s.Length, f.Size);
        }
        #endregion
    }
	#endregion

    #region class FieldListException
    public class FieldListException : Exception
    {
        public FieldListException(string sMessage) : base(sMessage) { }
        public FieldListException(string sMessage, params object[] oPrm) : base(string.Format(sMessage, oPrm)) { }
        public FieldListException(Exception InnerException, string sMessage) : base(sMessage, InnerException) { }
        public FieldListException(Exception InnerException, string sMessage, params object[] oPrm) : base(string.Format(sMessage, oPrm), InnerException) { }
    }
    #endregion

    public class FieldList : IEnumerable, IEnumerator
	{
		#region variable
        private string gsOriginalFieldsDefinition = null;
        private string gsFieldsSqlDefinition = null;
        private string gsFieldsName = null;
        private List<Field> glField = new List<Field>();
		private CultureInfo gCulture = new CultureInfo("en-US", true);
        private int giEnumerator = -1;

        public delegate void FieldAddedEvent(Field field);
        public event FieldAddedEvent FieldAdded = null;
        #endregion

		#region constructor
		#region FieldList(string sFieldDef, bool bCreateMissingField)
		// exemple de sFieldDef :
		// string, int, date, time, bool
		// nom string, age int, date_naissance date, heure_naissance time, majeur bool
		// nom string, age int, date_naissance date "dd/MM/yyyy", heure_naissance time "hh:mm", majeur bool
		// 1 nom string, age int, 4 date_naissance date "dd/MM/yyyy", 5 heure_naissance time "hh:mm", 6 majeur bool
		/// <summary>
		/// </summary>
        /// <param name="sFieldDef">description des champs du fichier texte : [numéro d'ordre] [nom] [type [identity] [collate ...] [default value] [[not] null] ["format"]], ...</param>
        /// <param name="bCreateMissingField"></param>
        public FieldList(string sFieldDef, bool bCreateMissingField)
		{
            CreateField(sFieldDef, bCreateMissingField);
		}
		#endregion

		#region FieldList(string sFieldDef)
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sFieldDef">description des champs du fichier texte : [numéro d'ordre] [nom] type ["format"], ...</param>
		public FieldList(string sFieldDef)
		{
			CreateField(sFieldDef, false);
		}
		#endregion

		#region FieldList()
		public FieldList()
		{
		}
		#endregion
		#endregion

		#region indexeur
		public Field this[int i]
		{
			get { return this.glField[i]; }
		}

		public Field this[string sFieldName]
		{
			get { return this.glField[GetIndexedField(sFieldName)]; }
		}
		#endregion

        #region property
        #region OriginalFieldsDefinition
        /// <summary>
        /// string utilisée lors de la création de la classe
        /// </summary>
        public string OriginalFieldsDefinition
        {
            get { return gsOriginalFieldsDefinition; }
        }
        #endregion

        #region FieldsSqlDefinition
        /// <summary>
        /// définition sql des champs
        /// </summary>
        public string FieldsSqlDefinition
        {
            get
            {
                if (gsFieldsSqlDefinition == null) gsFieldsSqlDefinition = GetFieldsSqlDefinition();
                return gsFieldsSqlDefinition;
            }
        }
        #endregion

        #region FieldsName
        /// <summary>
        /// liste des nom des champs
        /// </summary>
        public string FieldsName
        {
            get
            {
                if (gsFieldsName == null) gsFieldsName = GetFieldsName();
                return gsFieldsName;
            }
        }
        #endregion

		#region Count
		public int Count
		{
			get { return glField.Count; }
		}
		#endregion

		#region ValueArray
        public object[] ValueArray
		{
			get
			{
                object[] values = new object[glField.Count];
				for (int i = 0; i < glField.Count; i++)
					values[i] = glField[i].Value;
				return values;
			}
		}
		#endregion

        #region Culture
        public CultureInfo Culture
        {
            get { return gCulture; }
            set { gCulture = value; }
        }
        #endregion
        #endregion

        #region CreateField
        // exemple de sFieldDef :
		// string, int, date, time, bool, varchar(10), numeric(10,2), varchar(10) collate database_default not null, ...
		// nom string, age int, date_naissance date, heure_naissance time, majeur bool
		// nom string, age int, date_naissance date "dd/MM/yyyy", heure_naissance time "hh:mm", majeur bool
		// id int identity not null, 1 nom string, age int, 4 date_naissance date "dd/MM/yyyy", 5 heure_naissance time "hh:mm", 6 majeur bool
		/// <summary>
		/// </summary>
		/// <param name="sFieldDef">description des champs du fichier texte : [numéro d'ordre] [nom] [type [identity] [collate ...] [default value] [[not] null] ["format"]], ...</param>
        /// <param name="bCreateMissingField"></param>
        private void CreateField(string sFieldDef, bool bCreateMissingField)
		{
			int i, j, k, i1, i2, iIdxField;
            char[,] cCharZone = { { '"', '"' }, { '\'', '\'' }, { '(', ')' } };
            string s, sDefaultValue;
			string[] sFieldDef2, sFieldDef3;

            gsOriginalFieldsDefinition = sFieldDef;
            sFieldDef2 = zsplit.Split(sFieldDef, ',', cCharZone);
			if (sFieldDef2.Length == 0)
				throw new FieldListException("aucun champ n'est défini \"{0}\"", sFieldDef);
			iIdxField = 0;
			for (i = 0; i < sFieldDef2.Length; i++)
			{
				sDefaultValue = null;

                sFieldDef3 = zsplit.Split(sFieldDef2[i], ' ', cCharZone, true, false, true);
				if (sFieldDef3.Length == 0) continue;
                Field f = new Field();

				j = StringArrayIndexOf(sFieldDef3, "identity");
				if (j != -1)
				{
					f.Identity = true;
					sFieldDef3 = StringArrayRemove(sFieldDef3, j, 1);
				}

				j = StringArrayIndexOf(sFieldDef3, "collate");
				if (j != -1)
				{
					if (sFieldDef3.Length < j + 2)
						throw new FieldListException("le champ no {0} ne contient pas le nom du jeu de caractète (collate) \"{1}\"", i + 1, sFieldDef);
					f.Collate = sFieldDef3[j + 1];
					sFieldDef3 = StringArrayRemove(sFieldDef3, j, 2);
				}

				j = StringArrayIndexOf(sFieldDef3, "default");
				if (j != -1)
				{
					if (sFieldDef3.Length < j + 2)
						throw new FieldListException("le champ no {0} ne contient pas de valeur par défaut (default) \"{1}\"", i + 1, sFieldDef);
					f.DefaultValueText = sFieldDef3[j + 1];
					sFieldDef3 = StringArrayRemove(sFieldDef3, j, 2);
				}

				j = StringArrayIndexOf(sFieldDef3, "null");
				if (j != -1)
				{
					if (j > 0 && sFieldDef3[j -1].ToLower() == "not")
					{
						f.IsNullable = false;
						k = 2; j--;
					}
					else
					{
						f.IsNullable = true;
						k = 1;
					}
					sFieldDef3 = StringArrayRemove(sFieldDef3, j, k);
				}

				// numéro d'ordre du champ
				i1 = 0;
				if (zconvert.IsInt(sFieldDef3[0]))
				{
                    //j = zconvert.Int(sFieldDef3[0]);
                    j = sFieldDef3[0].zTryParseAs<int>();
                    if (j == 0)
						throw new FieldListException("le numéro d'ordre du champ no {0} ne peut pas être 0 \"{1}\"", i + 1, sFieldDef);
					j--; // base 1 en base 0
					if (j < iIdxField)
						throw new FieldListException("le numéro d'ordre du champ no {0} n'est pas dans l'ordre \"{1}\"", i + 1, sFieldDef);
					if (bCreateMissingField && j > iIdxField)
					{
						for(k = iIdxField; k < j; k++) Add();
					}
					iIdxField = j; i1 = 1;
				}
				f.FieldIndex = iIdxField++;

				// format du champ
				i2 = sFieldDef3.Length - 1;
				if (sFieldDef3.Length >= 2)
				{
					s = sFieldDef3[i2];
                    if ((zstr.left(s, 1) == "\"" && zstr.right(s, 1) == "\"") || (zstr.left(s, 1) == "\'" && zstr.right(s, 1) == "\'"))
                    {
						i2--;
						f.Format = s.Substring(1, s.Length - 2);
					}
				}

				// type du champ
                //if (i1 > i2)
                //    throw new FieldListException("pas de type dans la définition du champ no {0} \"{1}\"", i + 1, sFieldDef);
                //SetFieldType(f, sFieldDef3[i2]);
                //if (f.Type == null)
                //    throw new FieldListException("type incorrect dans la définition du champ no {0} \"{1}\"", i + 1, sFieldDef);

                // nom du champ
                //if (i1 < i2 - 1)
                //    throw new FieldListException("erreur dans la définition du champ no {0} \"{1}\"", i + 1, sFieldDef);
                //if (i1 < i2) f.Name = sFieldDef3[i1];


                // i1 indice du nom, i2 = indice du type

                if (i1 < i2 - 1) // si il y a une autre valeur entre le nom et le type : erreur
                    throw new FieldListException("erreur dans la définition du champ no {0} \"{1}\"", i + 1, sFieldDef);

                if (i1 <= i2)
                {
                    // type du champ
                    SetFieldType(f, sFieldDef3[i2]);
                    if (f.Type == null)
                    {
                        if (i1 < i2) // s'il y a un nom et un type : erreur
                            throw new FieldListException("type incorrect dans la définition du champ no {0} \"{1}\"", i + 1, sFieldDef);
                        SetFieldType(f, "string");
                    }
                    else
                        i2--;
                }
                // nom du champ
                if (i1 <= i2) f.Name = sFieldDef3[i1];

                f.FieldSeparator = null;

				if (f.DefaultValueText != null)
				{
					sDefaultValue = f.DefaultValueText;
					if (sDefaultValue.StartsWith("'") && sDefaultValue.EndsWith("'"))
					{
						sDefaultValue = sDefaultValue.Remove(0, 1);
						sDefaultValue = sDefaultValue.Remove(sDefaultValue.Length - 1, 1);
					}
					f.TextValue = sDefaultValue;
					try
					{
						FieldConvertion(f);
					}
					catch(Exception ex)
					{
						throw new FieldListException(ex, "la valeur par défaut du champ no {0} n'est pas correcte \"{1}\"", i + 1, sFieldDef);
					}
					f.DefaultValue = f.Value;
					f.TextValue = null;
					f.Value = null;

				}

				glField.Add(f);
			}
		}
		#endregion

		#region ArrayRemove
		private static string[] StringArrayRemove(string[] s, int i, int nb)
		{
			string[] s2;

			s2 = new string[s.Length - nb];
			if (i > 0) Array.Copy(s, 0, s2, 0, i);
			if (s.Length - i - nb > 0) Array.Copy(s, i + nb, s2, i, s.Length - i - nb);
			return s2;
		}
		#endregion

		#region StringArrayIndexOf
		private static int StringArrayIndexOf(Array a, string s)
		{
			int i;

			s = s.ToLower();
			for (i = 0; i < a.Length; i++)
				if (((string)a.GetValue(i)).ToLower() == s) return i;
			return -1;
		}
		#endregion

		#region Add ...
		#region Add()
        public Field Add()
		{
            Field f = new Field();
			f.FieldIndex = glField.Count;
            //f.Name = "field" + zconvert.s(glField.Count + 1);
            f.Name = "field" + (glField.Count + 1).ToString();
            f.Type = typeof(string);
			f.dbType = DbType.String;
			f.sqlDbType = SqlDbType.VarChar;
			//f.Size = 8000;
			glField.Add(f);
            if (FieldAdded != null) FieldAdded(f);
            return f;
		}
		#endregion

        #region Add(Field f)
        public Field Add(Field f)
		{
			f.FieldIndex = glField.Count;
			glField.Add(f);
            if (FieldAdded != null) FieldAdded(f);
            return f;
        }
		#endregion
		#endregion

		#region SetFieldType
		// type possible : int, string, numeric(10,2), varchar(10)
		private void SetFieldType(Field f, string sType)
		{
            int iPrm1 = 0; int iPrm2 = 0;
            string s = sType.Trim();
            string[]  s2 = zsplit.Split(s, new char[] { '(', ')' }, true, false, true);
			if (s2.Length == 0 || s2.Length > 2) goto err;
			s = s2[0];
			if (s2.Length == 2)
			{
                s2 = zsplit.Split(s2[1], ',', true, false, false);
				if (s2.Length == 0 || s2.Length > 2) goto err;
				if (!zconvert.IsInt(s2[0])) goto err;
                //iPrm1 = zconvert.Int(s2[0]);
                iPrm1 = s2[0].zTryParseAs<int>();
                if (s2.Length == 2)
				{
                    if (!zconvert.IsInt(s2[1])) goto err;
                    //iPrm2 = zconvert.Int(s2[1]);
                    iPrm2 = s2[1].zTryParseAs<int>();
				}
			}

            Type type;
            DbType dbType;
            OleDbType oleDbType;
            SqlDbType sqlDbType;
            OdbcType odbcType;
            Type TypeCastBeforeCreateSqlServerType;
            int iSize;
            byte byPrecision;
            byte byScale;
            GetFieldType(s, iPrm1, iPrm2, out type, out dbType, out oleDbType, out sqlDbType, out odbcType, out TypeCastBeforeCreateSqlServerType, out iSize, out byPrecision, out byScale);
			f.Type = type;
			f.dbType = dbType;
			f.oleDbType = oleDbType;
			f.sqlDbType = sqlDbType;
			f.odbcType = odbcType;
            f.TypeCastBeforeCreateSqlServerType = TypeCastBeforeCreateSqlServerType;
			f.Size = iSize;
			f.Precision = byPrecision;
			f.Scale = byScale;
			return;

			err:
			throw new FieldListException("le type \"{0}\" n'est pas correct", sType);
		}
		#endregion

		#region static GetFieldType
		// les types .net :
		//   bool, int, long, double, float, string, DateTime
		// les types sql server :
		//   bit, tinyint, smallint, integer, bigint
		//   float, real
		//   decimal, numeric
		//   datetime, smalldatetime
		//   char, varchar, text, nchar, nvarchar, ntext
		public static void GetFieldType(string sType, int iPrm1, int iPrm2, out Type Type, out DbType dbType, out OleDbType oleDbType,
            out SqlDbType sqlDbType, out OdbcType odbcType, out Type TypeCastBeforeCreateSqlServerType, out int iSize, out byte iPrecision, out byte iScale)
		{
			Type = null;
			dbType = 0;
			oleDbType = 0;
			sqlDbType = 0;
			odbcType = 0;
            TypeCastBeforeCreateSqlServerType = null;
			iSize = 0;
			iPrecision = 0;
			iScale = 0;
			switch(sType.ToLower())
			{
				// boolean type
				case "bool":
				case "bit":
					Type = typeof(bool);
					//dbType = DbType.Byte;
                    dbType = DbType.Boolean;
                    oleDbType = OleDbType.Boolean;
					sqlDbType = SqlDbType.Bit;
					odbcType = OdbcType.Bit;
					break;

				// integer type
				case "tinyint":
					Type = typeof(byte);
					dbType = DbType.Byte;
					oleDbType = OleDbType.TinyInt;
					sqlDbType = SqlDbType.TinyInt;
					odbcType = OdbcType.TinyInt;
					break;
				case "smallint":
					Type = typeof(Int16);
					dbType = DbType.Int16;
					oleDbType = OleDbType.SmallInt;
					sqlDbType = SqlDbType.SmallInt;
					odbcType = OdbcType.SmallInt;
					break;
				case "integer":
				case "int":
					Type = typeof(int);
					dbType = DbType.Int32;
					oleDbType = OleDbType.Integer;
					sqlDbType = SqlDbType.Int;
					odbcType = OdbcType.Int;
					break;
				case "long":
				case "bigint":
					Type = typeof(long);
					dbType = DbType.Int64;
					oleDbType = OleDbType.BigInt;
					sqlDbType = SqlDbType.BigInt;
					odbcType = OdbcType.BigInt;
					break;

				// float type
				case "single":
				case "real":
					Type = typeof(float);
					dbType = DbType.Single;
					oleDbType = OleDbType.Single;
					sqlDbType = SqlDbType.Real;
					odbcType = OdbcType.Real;
					break;
				case "float":
				case "double":
					Type = typeof(double);
					dbType = DbType.Double;
					oleDbType = OleDbType.Double;
					sqlDbType = SqlDbType.Float;
					odbcType = OdbcType.Double;
					break;

				// numeric type
				case "decimal":
				case "numeric":
					Type = typeof(decimal);
					dbType = DbType.Decimal;
					oleDbType = OleDbType.Decimal;
					sqlDbType = SqlDbType.Decimal;
					odbcType = OdbcType.Decimal;
					iPrecision = (byte)iPrm1;
					iScale = (byte)iPrm2;
					break;

				// money type
				case "money":
				case "currency":
					Type = typeof(decimal);
					dbType = DbType.Currency;
					oleDbType = OleDbType.Currency;
					sqlDbType = SqlDbType.Money;
					odbcType = OdbcType.Double;
					break;
				case "smallmoney":
					Type = typeof(decimal);
					dbType = DbType.Currency;
					oleDbType = OleDbType.Currency;
					sqlDbType = SqlDbType.SmallMoney;
					odbcType = OdbcType.Double;
					break;

					// datetime type
				case "datetime":
					Type = typeof(DateTime);
					dbType = DbType.DateTime;
					oleDbType = OleDbType.DBTimeStamp;
					sqlDbType = SqlDbType.DateTime;
					odbcType = OdbcType.DateTime;
					break;
				case "smalldatetime":
					Type = typeof(DateTime);
					dbType = DbType.DateTime;
					oleDbType = OleDbType.DBTimeStamp;
					sqlDbType = SqlDbType.SmallDateTime;
					odbcType = OdbcType.SmallDateTime;
					break;
                case "date":
                    Type = typeof(Date);
                    dbType = DbType.DateTime;
                    oleDbType = OleDbType.DBTimeStamp;
                    sqlDbType = SqlDbType.DateTime;
                    odbcType = OdbcType.DateTime;
                    TypeCastBeforeCreateSqlServerType = typeof(DateTime);
                    break;

				// character type
				case "char":
					Type = typeof(string);
					dbType = DbType.StringFixedLength;
					oleDbType = OleDbType.Char;
					sqlDbType = SqlDbType.Char;
					odbcType = OdbcType.Char;
					iSize = iPrm1;
					break;
				case "nchar":
					Type = typeof(string);
					dbType = DbType.StringFixedLength;
					oleDbType = OleDbType.WChar;
					sqlDbType = SqlDbType.NChar;
					odbcType = OdbcType.NChar;
					iSize = iPrm1;
					break;
				case "varchar":
				case "varchar2":
					Type = typeof(string);
					dbType = DbType.String;
					oleDbType = OleDbType.VarChar;
					sqlDbType = SqlDbType.VarChar;
					odbcType = OdbcType.VarChar;
					iSize = iPrm1;
					break;
				case "nvarchar":
					Type = typeof(string);
					dbType = DbType.String;
					oleDbType = OleDbType.VarWChar;
					sqlDbType = SqlDbType.NVarChar;
					odbcType = OdbcType.NVarChar;
					iSize = iPrm1;
					break;
				case "text":
					Type = typeof(string);
					dbType = DbType.String;
					oleDbType = OleDbType.LongVarChar;
					sqlDbType = SqlDbType.Text;
					odbcType = OdbcType.Text;
					iSize = 1073741823; // taille maximum du type text de microsoft sql server
					break;
				case "ntext":
					Type = typeof(string);
					dbType = DbType.String;
					oleDbType = OleDbType.LongVarWChar;
					sqlDbType = SqlDbType.NText;
					odbcType = OdbcType.NText;
					iSize = 1073741823; // taille maximum du type text de microsoft sql server
					break;
				case "string":
					Type = typeof(string);
					dbType = DbType.String;
					oleDbType = OleDbType.VarChar;
					sqlDbType = SqlDbType.VarChar;
					odbcType = OdbcType.VarChar;
					//iSize = 1073741823;
					break;
			}
		}
		#endregion

        #region GetIndexedField
        private int GetIndexedField(string sFieldName)
		{
			int i;
			string s;

			s = sFieldName.ToLower();
			for (i = 0; i < glField.Count; i++)
			{
				if ((glField[i]).Name.ToLower() == s) return i;
			}
			throw new FieldListException("le champ \"{0}\" n'existe pas", sFieldName);
		}
		#endregion

        #region GetFieldsDefinition()
        /// <summary>
        /// string de définition des champs construite à partir des champs
        /// </summary>
        public string GetFieldsDefinition()
        {
            return GetFieldsDefinition(false);
        }
        #endregion

        #region GetFieldsDefinition(bool bUnicodeChar)
        /// <summary>
        /// string de définition des champs construite à partir des champs
        /// </summary>
        public string GetFieldsDefinition(bool bUnicodeChar)
        {
            // [numéro d'ordre] [nom] [type [identity] [collate ...] [default value] [[not] null] ["format"]], ...
            string sFieldsDef = "";
            bool bFirst = true;
            foreach (Field field in glField)
            {
                if (!bFirst) sFieldsDef += ", ";
                sFieldsDef += field.GetFieldDefinition(bUnicodeChar);
                bFirst = false;
            }
            return sFieldsDef;
        }
        #endregion

        #region GetFieldsSqlDefinition()
        public string GetFieldsSqlDefinition()
        {
            return GetFieldsSqlDefinition(false);
        }
        #endregion

        #region GetFieldsSqlDefinition(bool bUnicodeChar)
        public string GetFieldsSqlDefinition(bool bUnicodeChar)
        {
            string sSqlDef = "";
            bool bFirst = true;
            foreach (Field field in glField)
            {
                if (!bFirst) sSqlDef += ", ";
                //sSqlDef += field.SqlDefinition;
                sSqlDef += field.GetSqlDefinition(bUnicodeChar);
                bFirst = false;
            }
            return sSqlDef;
        }
        #endregion

        #region GetFieldsName
        public string GetFieldsName()
        {
            string sNames = "";
            bool bFirst = true;
            foreach (Field field in glField)
            {
                if (!bFirst) sNames += ", ";
                sNames += field.Name;
                bFirst = false;
            }
            return sNames;
        }
        #endregion

        #region FieldConvertion(Field f)
        public void FieldConvertion(Field f)
        {
            Field.Convert(f, gCulture);
        }
        #endregion

        #region IEnumerable IEnumerator
        #region GetEnumerator
        public IEnumerator GetEnumerator()
        {
            giEnumerator = -1;
            return this;
        }
        #endregion

        #region Current
        public object Current
        {
            get
            {
                if (giEnumerator < 0 || giEnumerator >= glField.Count) throw new FieldListException("error IEnumerator Current is not valid");
                return this.glField[giEnumerator];
            }
        }
        #endregion

        #region MoveNext
        public bool MoveNext()
        {
            if (giEnumerator == glField.Count) return false;
            if (++giEnumerator == glField.Count) return false;
            return true;
        }
        #endregion

        #region Reset
        public void Reset()
        {
            giEnumerator = -1;
        }
        #endregion
        #endregion
    }
}
