using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
//using Ionic.Zip;
//using Win32;

namespace pb.old
{
    public partial class cu
	{
		static string gsPathIni;

        #region Deserialize(byte[] bytes)
        public static object Deserialize(byte[] bytes)
        {
            MemoryStream ms = new MemoryStream(bytes);
            try
            {
                BinaryFormatter bf = new BinaryFormatter();
                object o = bf.Deserialize(ms);
                return o;
            }
            finally
            {
                ms.Close();
            }
        }
        #endregion

        #region Deserialize(string sPath)
        public static object Deserialize(string sPath)
        {
            FileStream fs = new FileStream(sPath, FileMode.Open, FileAccess.Read, FileShare.Read);
            try
            {
                BinaryFormatter bf = new BinaryFormatter();
                object o = bf.Deserialize(fs);
                return o;
            }
            finally
            {
                fs.Close();
            }
        }
        #endregion

        #region Util ini

        public class IniException : Exception
		{
			public IniException(string message) : base(message){}
			public IniException(Exception innerException, string message) : base(message,innerException){}
		}

		public static void SetPathIni(string sPathIni)
		{
			gsPathIni = sPathIni;
		}

		public static string GetPathIni()
		{
			return gsPathIni;
		}

		public static string GetPrivateProfileStringErr(string sSection, string sItem)
		{
			return GetPrivateProfileStringErr(sSection, sItem, null);
		}

		public static string GetPrivateProfileStringErr(string sSection, string sItem,
			string sPathIni)
		{
			string s, sErr;

			s = GetPrivateProfileString(sSection, sItem, "", sPathIni);
			if (s == "")
			{
				sErr = string.Format("Error no value for item '{0}' section [{1}] in '{2}'",
					sItem, sSection, Path.GetFileName(sPathIni));
				throw new IniException(sErr);
			}
			return s;
		}

		public static string GetPrivateProfileString(string sSection, string sItem,
			string sDefault)
		{
			return GetPrivateProfileString(sSection, sItem, sDefault, null);
		}

		public static string GetPrivateProfileString(string sSection, string sItem, 
			string sDefault, string sPathIni)
		{
			StringBuilder sBuffer;
			int iNb;

			if (sPathIni == null)  sPathIni = gsPathIni;
			sBuffer = new StringBuilder(32000);
			iNb = cWin32Ini.GetPrivateProfileString(sSection, sItem, sDefault, sBuffer, sBuffer.Capacity, sPathIni);
			return sBuffer.ToString();
		}

		public static int GetPrivateProfileInt(string sSection, string sItem, int iDefault)
		{
			return GetPrivateProfileInt(sSection, sItem, iDefault, null);
		}

		public static int GetPrivateProfileInt(string sSection, string sItem, int iDefault, string sPathIni)
		{
			if (sPathIni == null)  sPathIni = gsPathIni;
			return cWin32Ini.GetPrivateProfileInt(sSection, sItem, iDefault, sPathIni);
		}

		public static void WritePrivateProfileString(string sSection, string sItem, string sVal)
		{
			WritePrivateProfileString(sSection, sItem, sVal, null);
		}

		public static void WritePrivateProfileString(string sSection, string sItem, string sVal, string sPathIni)
		{
			if (sPathIni == null)  sPathIni = gsPathIni;
			cWin32Ini.WritePrivateProfileString(sSection, sItem, sVal, sPathIni);
		}

		public static string[] GetPrivateProfileListSection()
		{
			return GetPrivateProfileListSection(null);
		}

		public static string[] GetPrivateProfileListSection(string sPathIni)
		{
			int iBuf, iNb;
			string[] sList;
			char[] cBuf;

			iBuf = 32000;
			cBuf = new char[iBuf];

			if (sPathIni == null)  sPathIni = gsPathIni;
			iNb = cWin32Ini.GetPrivateProfileString(null, null, "", cBuf, iBuf, sPathIni);
			sList = TabCharToTabString(cBuf);
			return sList;
		}

		public static string[] GetPrivateProfileListItem(string sSection)
		{
			return GetPrivateProfileListItem(sSection, null);
		}

		public static string[] GetPrivateProfileListItem(string sSection, string sPathIni)
		{
			int iBuf, iNb;
			string[] sList;
			char[] cBuf;

			iBuf = 32000;
			cBuf = new char[iBuf];

			if (sPathIni == null)  sPathIni = gsPathIni;
			iNb = cWin32Ini.GetPrivateProfileString(sSection, null, "", cBuf, iBuf, sPathIni);
			sList = TabCharToTabString(cBuf);
			return sList;
		}

		public static string[] TabCharToTabString(char[] cBuf)
		{
			int i1, i2, nb;
			string s;
			string[] sTab;
			ArrayList aList;

			nb = cBuf.GetLength(0);
			aList = new ArrayList();
			
			i1 = 0;
			for(;;)
			{
				i2 = Array.IndexOf(cBuf, (char)0, i1);
				if (i2 == i1 || i2 == -1)  break;
				s = new string(cBuf, i1, i2 - i1);
				aList.Add(s);
				i1 = i2 + 1;
			}

			sTab = new string[aList.Count];
			for(i1 = 0; i1 < aList.Count; i1++)
				sTab[i1] = aList[i1].ToString();
			return sTab;
		}

		#endregion Util ini
	
		#region Util BinaryReader
        #region ReadString(BinaryReader br, char cEndString)
        public static string ReadString(BinaryReader br, char cEndString)
		{
			byte by;
			string s;

			s = "";
			while ((by = br.ReadByte())!= cEndString)  s += (char)by;
			return s;
        }
        #endregion

        #region ReadString(BinaryReader br, int nbCar)
        public static string ReadString(BinaryReader br, int nbCar)
		{
			char[] tc;
			byte[] tby;
			string s;

			tby = br.ReadBytes(nbCar);
			tc = ArrayByteToChar(tby);
			s = new string(tc);
			return s;
        }
        #endregion

        #region ReadInt
        public static int ReadInt(BinaryReader br, int nbCar)
		{
			string s;

			s = ReadString(br, nbCar);
			return Int(s);
        }
        #endregion

        #region ReadDateTime(BinaryReader br, string sFormat)
        public static object ReadDateTime(BinaryReader br, string sFormat)
		{
			return ReadDateTime(br, sFormat, sFormat.Length);
        }
        #endregion

        #region ReadDateTime(BinaryReader br, string sFormat, int nbCar)
        public static object ReadDateTime(BinaryReader br, string sFormat, int nbCar)
		{
			string s;

			s = ReadString(br, nbCar);
			return Date(s, sFormat);
        }
        #endregion
        #endregion

        #region Util DateTime
        #endregion

        #region Util Regular expression
        #region MatchFileName
        public static bool MatchFileName(string sPattern, string sFileName)
		{
			return Match(FilePatternToRegularPattern(sPattern), sFileName);
        }
        #endregion

        #region FilePatternToRegularPattern
        public static string FilePatternToRegularPattern(string sFilePattern)
		{
			int i;
			string[] sCharSpec = { ".", "$", "^", "{", "[", "(", "|", ")", "+" };
			string sRegularPattern;

			sRegularPattern = sFilePattern;

			// remplacement de \ par \\
			sRegularPattern = sRegularPattern.Replace(@"\", @"\\");

			// ajout de \ devant les caractères spéciaux : . $ ^ { [ ( | ) +
			for (i = 0; i < sCharSpec.Length; i++) sRegularPattern = sRegularPattern.Replace(sCharSpec[i], @"\" + sCharSpec[i]);

			// remplacement des caractères * par .* (plusieurs caractères quelconques)
			sRegularPattern = sRegularPattern.Replace("*", ".*");

			// remplacement des caractères ? par . (un caractère quelconque)
			sRegularPattern = sRegularPattern.Replace("?", ".");

			// ajout de ^ (début de ligne) et $ (fin de ligne)
			sRegularPattern = "^" + sRegularPattern + "$";

			return sRegularPattern;
        }
        #endregion

        #region Match
        public static bool Match(string sPattern, string s)
        {
			Regex r;

			// caractère spéciaux :   . $ ^ { [ ( | ) * + ? \
			r = new Regex(sPattern, RegexOptions.IgnoreCase); 
			return r.Match(s).Success;
        }
        #endregion
        #endregion

        #region print ...
        #region print
        public static void print(DataTable dt)
        {
            int[] iColsWidth = GetMaxColsLength(dt);
            string[] sFmtCols = new string[iColsWidth.Length];
            for (int i = 0; i < iColsWidth.Length; i++)
                sFmtCols[i] = "{0,-" + iColsWidth [i].ToString() + "}";

            foreach (DataRow row in dt.Rows)
            {
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    Console.Write(sFmtCols[i], row[i]);
                    Console.Write("  ");
                }
                Console.WriteLine();
            }

        }
        #endregion
        #endregion

        #region GetMaxColsLength
        private static int[] GetMaxColsLength(DataTable dt)
        {
            int[] iColsWidth = new int[dt.Columns.Count];
            for (int i = 0; i < dt.Columns.Count; i++) iColsWidth[i] = GetMaxColLength(dt.Rows, i);
            return iColsWidth;
        }
        #endregion

        #region GetMaxColLength
        private static int GetMaxColLength(DataRowCollection rows, int iCol)
        {
            int iMaxLength = 0;
            foreach (DataRow row in rows)
            {
                object o = row[iCol];
                if (o is string)
                {
                    int l = ((string)o).Length;
                    if (l > iMaxLength) iMaxLength = l;
                }
            }
            return iMaxLength;
        }
        #endregion
    }

    #region class ConException
    public class ConException : Exception
    {
        public ConException(string sMessage) : base(sMessage) { }
        public ConException(string sMessage, params object[] oPrm) : base(string.Format(sMessage, oPrm)) { }
        public ConException(Exception InnerException, string sMessage) : base(sMessage, InnerException) { }
        public ConException(Exception InnerException, string sMessage, params object[] oPrm) : base(string.Format(sMessage, oPrm), InnerException) { }
    }
    #endregion

    #region class con
    public class con
    {
        #region variable
        private static bool gbOpen;
		private static bool gbAllocConsole;
		private static IntPtr ghOut;
		private static IntPtr ghIn;
		private static FileStream gfsOut;
		private static StreamWriter gswOut;
		private static TextWriter gtwOutOld;

		public delegate bool ConsoleInputEvent(cWin32Con.INPUT_RECORD input_record, ref string strBuildup);
        #endregion

        #region constructor
        static con()
		{
			gbOpen = false;
			gbAllocConsole = false;
			ghOut = (IntPtr)0;
			ghIn = (IntPtr)0;
			gfsOut = null;
			gswOut = null;
			gtwOutOld = null;
        }
        #endregion

        #region Test
        public static void Test()
		{
			char[] cBuf;
			int i, iRet;
			uint ui, uil, uiProId, uiErr;
			IntPtr hIn, hOut, ptrNull;
			string s;
			string[] sList;
			Process[] prProList;

			hIn = cWin32Con.GetStdHandle(cWin32Con.STD_INPUT_HANDLE);
			hOut = cWin32Con.GetStdHandle(cWin32Con.STD_OUTPUT_HANDLE);

			s = "test $$$$$";
			ptrNull = new IntPtr(0);
			ui = 0;
			cWin32Con.WriteConsole(hOut, s, (uint)s.Length, ref ui, ptrNull);

			uil = cWin32Con.GetConsoleAliasExesLength();
			cBuf = new char[uil];
			iRet = cWin32Con.GetConsoleAliasExes(cBuf, uil);
			sList = cu.TabCharToTabString(cBuf);

			prProList = Process.GetProcesses();

			for (i = 0; i < prProList.Length; i++)
			{
				uiProId = (uint)prProList[i].Id;

				iRet = cWin32Con.AttachConsole(uiProId);
				uiErr = 0;
				if (iRet == 0) uiErr = cWin32Con.GetLastError();
				Debug.WriteLine(string.Format("Process {0}, Id {1}, AttachConsole iRet {2} err {3}", prProList[i].ProcessName, uiProId, iRet, uiErr));
			}

			WriteLine("StdIn  = {0}", hIn);
			WriteLine("StdOut = {0}", hOut);
			WriteLine("ghIn   = {0}", ghIn);
			WriteLine("ghOut  = {0}", ghOut);
        }
        #endregion

        #region Open
        #pragma warning disable 618
        public static void Open()
		{
			int iRet;
			uint uiErr;
			IntPtr ptrNull;

			if (gbOpen) return;
			iRet = cWin32Con.AllocConsole();
			ghIn = cWin32Con.GetStdHandle(cWin32Con.STD_INPUT_HANDLE);
			if (ghIn == cWin32Con.INVALID_HANDLE_VALUE) ghIn = (IntPtr)0;
			if (iRet == 0)
			{
				ghOut = cWin32Con.GetStdHandle(cWin32Con.STD_OUTPUT_HANDLE);
				if (ghOut == cWin32Con.INVALID_HANDLE_VALUE) ghOut = (IntPtr)0;
				goto fin;
			}
			gbAllocConsole = true;

			ptrNull = new IntPtr(0);
			ghOut = cWin32Con.CreateConsoleScreenBuffer(cWin32Con.GENERIC_READ + cWin32Con.GENERIC_WRITE,
				cWin32Con.FILE_SHARE_READ + cWin32Con.FILE_SHARE_WRITE, ptrNull, cWin32Con.CONSOLE_TEXTMODE_BUFFER, ptrNull);
			if (ghOut == cWin32Con.INVALID_HANDLE_VALUE)
			{
				uiErr = cWin32Con.GetLastError();
				throw new ConException("error no {0} calling CreateConsoleScreenBuffer()", uiErr);
			}
			iRet = cWin32Con.SetConsoleActiveScreenBuffer(ghOut);
			if (iRet == 0)
			{
				uiErr = cWin32Con.GetLastError();
				throw new ConException("error no {0} calling SetConsoleActiveScreenBuffer()", uiErr);
			}

			iRet = cWin32Con.SetStdHandle(cWin32Con.STD_OUTPUT_HANDLE, ghOut);
			if (iRet == 0)
			{
				uiErr = cWin32Con.GetLastError();
				throw new ConException("error no {0} calling SetStdHandle()", uiErr);
			}

			gtwOutOld = Console.Out;
			gfsOut = new FileStream(ghOut, FileAccess.Write, false);
            gswOut = new StreamWriter(gfsOut, Encoding.Default);
			gswOut.AutoFlush = true;
			Console.SetOut(gswOut);

		fin:
			gbOpen = true;
        }
        #pragma warning restore
        #endregion

        #region Close
        public static void Close()
		{
			if (gbAllocConsole)
			{
				if (gtwOutOld != null) { Console.SetOut(gtwOutOld); gtwOutOld = null; }
				if (gswOut != null) { gswOut.Close(); gswOut = null; }
				if (gfsOut != null) { gfsOut.Close(); gfsOut = null; }
				cWin32Con.CloseHandle(ghOut);
				ghOut = (IntPtr)0;
				cWin32Con.FreeConsole();
				gbAllocConsole = false;
				gbOpen = false;
			}
        }
        #endregion

        #region GetConsoleModeOutput
        public static int GetConsoleModeOutput()
		{
			int iMode, iRet;
			uint uiErr;

			iMode = 0;
			iRet = cWin32Con.GetConsoleMode(ghOut, ref iMode);
			if (iRet == 0)
			{
				uiErr = cWin32Con.GetLastError();
				throw new ConException("error no {0} calling GetConsoleMode()", uiErr);
			}
			return iMode;
        }
        #endregion

        #region SetConsoleModeOutput
        public static int SetConsoleModeOutput(int iMode)
		{
			int iRet;
			uint uiErr;

			iRet = cWin32Con.SetConsoleMode(ghOut, iMode);
			if (iRet == 0)
			{
				uiErr = cWin32Con.GetLastError();
				throw new ConException("error no {0} calling SetConsoleMode()", uiErr);
			}
			return iRet;
        }
        #endregion

        #region Write
        public static void Write(string sFormat, params object[] oPrm)
		{
			uint ui;
			IntPtr ptrNull;
			string sMsg;

			Open();
			ptrNull = new IntPtr(0);
			ui = 0;
			sMsg = string.Format(sFormat, oPrm);
			if (ghOut != (IntPtr)0) cWin32Con.WriteConsole(ghOut, sMsg, (uint)sMsg.Length, ref ui, ptrNull);
        }
        #endregion

        #region WriteLine
        public static void WriteLine(string sFormat, params object[] oPrm)
		{
			Write(sFormat, oPrm);
			WriteLine();
        }
        #endregion

        #region WriteLine
        public static void WriteLine()
		{
			Write("\r\n");
        }
        #endregion

        #region Read
        public static string Read()
		{
			char c;
			int iRet;
			uint i, uiErr, cNumRead;
			string s;
			cWin32Con.INPUT_RECORD[] irInBuf;
			cWin32Con.KEY_EVENT_RECORD ker;

			Open();
			irInBuf = new cWin32Con.INPUT_RECORD[128];
			cNumRead = 0;

			s = "";
			while (s == "")
			{
				iRet = cWin32Con.ReadConsoleInput(ghIn, irInBuf, 128, out cNumRead);
				if (iRet == 0)
				{
					uiErr = cWin32Con.GetLastError();
					throw new ConException("error no {0} calling ReadConsoleInput()", uiErr);
				}
				for (i = 0; i < cNumRead; i++) 
				{
					if (irInBuf[i].EventType == cWin32Con.KEY_EVENT)
					{
						ker = irInBuf[i].Event.KeyEvent;
						if (ker.bKeyDown != 0)
						{
							c = (char)(ker.uchar.UnicodeChar);  // Get the current character pressed.
							if (c != '\0')
								s += c.ToString();
						}
					}
				}
			}
			return s;
        }
        #endregion
    }
	#endregion

	#region class cWin32Ini
	public class cWin32Ini
	{

		[DllImport("kernel32.dll", CharSet=CharSet.Auto)]
		public static extern int GetPrivateProfileString(string sSection, string sItem, string sDefault, StringBuilder sBuffer, int lSize, string sPathIni);
		[DllImport("kernel32.dll", CharSet=CharSet.Auto)]
		public static extern int GetPrivateProfileString(string sSection, string sItem, string sDefault, char[] sBuffer, int lSize, string sPathIni);
		[DllImport("kernel32.dll", CharSet=CharSet.Auto)]
		public static extern int GetPrivateProfileInt(string sSection, string sItem, int iDefault, string sPathIni);
		[DllImport("kernel32.dll", CharSet=CharSet.Auto)]
		public static extern int WritePrivateProfileString(string sSection, string sItem, string sVal, string sPathIni);
	}
	#endregion

	#region class cWin32Con
	public class cWin32Con
	{
		#region const
		public const uint GENERIC_READ     = 0x80000000;
		public const uint GENERIC_WRITE    = 0x40000000;
		public const uint FILE_SHARE_READ  = 0x00000001;
		public const uint FILE_SHARE_WRITE = 0x00000002;
		public const uint CONSOLE_TEXTMODE_BUFFER = 1;

		// Standard input, output, and error
		public const int STD_INPUT_HANDLE  = -10;
		public const int STD_OUTPUT_HANDLE = -11;
		public const int STD_ERROR_HANDLE  = -12;

		//  Input Mode flags.
		public const int ENABLE_PROCESSED_INPUT = 0x0001;
		public const int ENABLE_LINE_INPUT      = 0x0002;
		public const int ENABLE_ECHO_INPUT      = 0x0004;
		public const int ENABLE_WINDOW_INPUT    = 0x0008;
		public const int ENABLE_MOUSE_INPUT     = 0x0010;

		// Output Mode flags:
		public const int ENABLE_PROCESSED_OUTPUT   = 0x0001;
		public const int ENABLE_WRAP_AT_EOL_OUTPUT = 0x0002;

		//  EventType flags.
		public const int KEY_EVENT                  = 0x0001; // Event contains key event record
		public const int MOUSE_EVENT                = 0x0002; // Event contains mouse event record
		public const int WINDOW_BUFFER_SIZE_EVENT   = 0x0004; // Event contains window change event record
		public const int MENU_EVENT                 = 0x0008; // Event contains menu event record
		public const int FOCUS_EVENT                = 0x0010; // event contains focus change

		// Returned by GetStdHandle when an error occurs
		public static readonly IntPtr INVALID_HANDLE_VALUE = new IntPtr(-1);
		#endregion

		#region struct
		[StructLayout(LayoutKind.Explicit)]
		public struct uCharUnion
		{
			[FieldOffset(0)] public ushort UnicodeChar;
			[FieldOffset(0)] public byte AsciiChar;
		}

		[StructLayout(LayoutKind.Sequential, Pack=8)]
		public struct KEY_EVENT_RECORD
		{
			public int bKeyDown;
			public ushort wRepeatCount;
			public ushort wVirtualKeyCode;
			public ushort wVirtualScanCode;
			public uCharUnion uchar;
			public uint dwControlKeyState;
		}

		public struct COORD
		{
			public short X;
			public short Y;
		}

		public struct MOUSE_EVENT_RECORD
		{
			public COORD dwMousePosition;
			public uint dwButtonState;
			public uint dwControlKeyState;
			public uint dwEventFlags;
		}

		public struct WINDOW_BUFFER_SIZE_RECORD
		{
			public COORD dwSize;
		}

		public struct MENU_EVENT_RECORD
		{
			public uint dwCommandId;
		}

		public struct FOCUS_EVENT_RECORD
		{
			public bool bSetFocus;
		}

		[StructLayout(LayoutKind.Explicit)]
		public struct EventUnion
		{
			[FieldOffset(0)] public KEY_EVENT_RECORD KeyEvent;
			[FieldOffset(0)] public MOUSE_EVENT_RECORD MouseEvent;
			[FieldOffset(0)] public WINDOW_BUFFER_SIZE_RECORD WindowBufferSizeEvent;
			[FieldOffset(0)] public MENU_EVENT_RECORD MenuEvent;
			[FieldOffset(0)] public FOCUS_EVENT_RECORD FocusEvent;
		}

		public struct INPUT_RECORD 
		{
			public ushort EventType;
			public EventUnion Event;
		}
		#endregion

		#region function
		#region ___WarningCompile
		// cette fonction n'est là que pour éviter d'avoir des warnings à la compilation
		private static void ___WarningCompile()
		{
			COORD coord;
			MOUSE_EVENT_RECORD mer;
			WINDOW_BUFFER_SIZE_RECORD wbsr;
			MENU_EVENT_RECORD mer2;
			FOCUS_EVENT_RECORD fer;
			INPUT_RECORD ir;

			coord.X = 1;
			coord.Y = 1;
			mer.dwMousePosition.X = 1;
			mer.dwButtonState = 1;
			mer.dwControlKeyState = 1;
			mer.dwEventFlags = 1;
			wbsr.dwSize.X = 1;
			mer2.dwCommandId = 1;
			fer.bSetFocus = false;
			ir.Event.KeyEvent.bKeyDown = 1;
			ir.EventType = 1;
		}
		#endregion

		//[DllImport("kernel32.dll", SetLastError=true)]
		[DllImport("kernel32.dll", CallingConvention=CallingConvention.StdCall)]
		public static extern int AllocConsole();

		[DllImport("kernel32.dll", CallingConvention=CallingConvention.StdCall)]
		public static extern int FreeConsole();

		//[DllImport("Kernel32.DLL", EntryPoint="GetStdHandle", CallingConvention=CallingConvention.StdCall)]
		[DllImport("Kernel32.DLL", CallingConvention=CallingConvention.StdCall)]
		public static extern IntPtr GetStdHandle(int nStdHandle);

		[DllImport("Kernel32.DLL", CallingConvention=CallingConvention.StdCall)]
		public static extern int SetStdHandle(int nStdHandle, IntPtr hHandle);

		[DllImport("Kernel32.DLL", CallingConvention=CallingConvention.StdCall, CharSet=CharSet.Auto)]
		public static extern int WriteConsole(IntPtr hConsoleOutput, string lpBuffer, uint  nNumberOfCharsToWrite, ref uint lpNumberOfCharsWritten, IntPtr lpReserved );

		// ReadConsoleInput() is used to read data from a console input buffer and then remove it from the buffer.
		// We will be relying heavily on this function.
		//[DllImport("Kernel32.DLL", EntryPoint="ReadConsoleInputW", CallingConvention=CallingConvention.StdCall)]
		[DllImport("Kernel32.DLL", CallingConvention=CallingConvention.StdCall, CharSet=CharSet.Auto)]
		public static extern int ReadConsoleInput(IntPtr hConsoleInput, [Out] INPUT_RECORD [] lpBuffer, uint nLength, out uint lpNumberOfEventsRead);

		[DllImport("Kernel32.DLL", CallingConvention=CallingConvention.StdCall)]
		public static extern int GetConsoleMode(IntPtr hConsoleHandle, ref int Mode);

		[DllImport("Kernel32.DLL", CallingConvention=CallingConvention.StdCall)]
		public static extern int SetConsoleMode(IntPtr hConsoleHandle, int Mode);

		[DllImport("Kernel32.DLL", CallingConvention=CallingConvention.StdCall)]
		public static extern IntPtr CreateConsoleScreenBuffer(uint dwDesiredAccess, uint dwShareMode,
			/*ref SECURITY_ATTRIBUTES*/ IntPtr SecurityAttributes, uint dwFlags, IntPtr ScreenBufferData);

		[DllImport("Kernel32.DLL", CallingConvention=CallingConvention.StdCall)]
		public static extern int SetConsoleActiveScreenBuffer(IntPtr hConsoleOutput);

		[DllImport("Kernel32.DLL", CallingConvention=CallingConvention.StdCall)]
		public static extern int CloseHandle(IntPtr hHandle);

		[DllImport("Kernel32.DLL", CallingConvention=CallingConvention.StdCall)]
		public static extern uint GetLastError();

		[DllImport("Kernel32.DLL", CallingConvention=CallingConvention.StdCall)]
		public static extern uint GetConsoleAliasExesLength();

		[DllImport("Kernel32.DLL", CallingConvention=CallingConvention.StdCall)]
		//public static extern uint GetConsoleAliasExes(StringBuilder lpExeNameBuffer, uint ExeNameBufferLength);
		public static extern int GetConsoleAliasExes(char[] lpExeNameBuffer, uint ExeNameBufferLength);

		[DllImport("Kernel32.DLL", CallingConvention=CallingConvention.StdCall)]
		public static extern int AttachConsole(uint dwProcessId);

		#endregion
	}
	#endregion

    #region class GlobalExtension
    public static partial class GlobalExtension
    {
        #region zToStringQuotedValues ...
        #region zToStringQuotedValues<T>(this IEnumerable<T>, string quote)
        public static string zToStringQuotedValues<T>(this IEnumerable<T> list, string quote)
        {
            return list.zToStringQuotedValues<T>(quote, quote);
        }
        #endregion

        #region zToStringQuotedValues<T>(this IEnumerable<T>, string beginQuote, string endQuote)
        public static string zToStringQuotedValues<T>(this IEnumerable<T> list, string beginQuote, string endQuote)
        {
            if (list == null) return null;
            //string s = null;
            //list.zForEach((T v) => { s = s.zAddValue(beginQuote + v.ToString() + endQuote); });
            //return s;
            StringBuilder sb = new StringBuilder();
            list.zForEach((T v) => { sb.zAddValue(beginQuote + v.ToString() + endQuote); });
            return sb.ToString();
        }
        #endregion
        #endregion

        #region zToString01
        public static string zToString01(this bool v)
        {
            return cu.s(v, true);
        }
        #endregion

        #region //zToString<T>(this T)
        //public static string zToString<T>(this T v)
        //{
        //    //if (v is XElement)
        //    //    return (v as XElement).zGetValue();
        //    //else
        //    return v.ToString();
        //}
        #endregion

        #region zSerialize(this object)
        public static byte[] zSerialize(this object o)
        {
            MemoryStream ms = new MemoryStream();
            try
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(ms, o);
                ms.Position = 0;
                return ms.ToArray();
            }
            finally
            {
                ms.Close();
            }
        }
        #endregion

        #region zSerialize(this object, string sPath)
        public static void zSerialize(this object o, string sPath)
        {
            string sDir = cu.PathGetDir(sPath);
            if (!Directory.Exists(sDir)) Directory.CreateDirectory(sDir);
            FileStream fs = new FileStream(sPath, FileMode.Create, FileAccess.Write, FileShare.Read);
            try
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(fs, o);
            }
            finally
            {
                fs.Close();
            }
        }
        #endregion

        #region zDeserialize(this byte[])
        public static object zDeserialize(this byte[] bytes)
        {
            MemoryStream ms = new MemoryStream(bytes);
            try
            {
                BinaryFormatter bf = new BinaryFormatter();
                object o = bf.Deserialize(ms);
                return o;
            }
            finally
            {
                ms.Close();
            }
        }
        #endregion

        #region zInit<T>(this IList<T>, Func<T> f)
        public static void zInit<T>(this IList<T> list, Func<T> f)
        {
            if (list == null || f == null) return;
            for (int i = 0; i < list.Count; i++) list[i] = f();
        }
        #endregion

        #region zInit<T>(this IList<T>, Func<int, T> f)
        public static void zInit<T>(this IList<T> list, Func<int, T> f)
        {
            if (list == null || f == null) return;
            for (int i = 0; i < list.Count; i++) list[i] = f(i);
        }
        #endregion

        #region zContains(this string, params string[] sFinds)
        public static bool zContains(this string s, params string[] sFinds)
        {
            foreach (string sFind in sFinds)
            {
                Regex rx = new Regex(sFind, RegexOptions.IgnoreCase);
                if (rx.IsMatch(s)) return true;
            }
            return false;
        }
        #endregion

        #region zContains(this string, params Regex[] rxs)
        public static bool zContains(this string s, params Regex[] rxs)
        {
            foreach (Regex rx in rxs)
            {
                if (rx.IsMatch(s)) return true;
            }
            return false;
        }
        #endregion
    }
    #endregion
}
