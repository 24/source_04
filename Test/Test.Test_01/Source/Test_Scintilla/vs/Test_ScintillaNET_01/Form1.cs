using System;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using ScintillaNET;
using pb.Windows.Forms;

namespace Test_ScintillaNET_01
{
    public partial class Form1 : Form
    {
        private string _directory = @"c:\pib\drive\google\dev\project\.net\Lib\pb\Source\pb\Compiler";
        private string _fileFilter = "source files (*.cs)|*.cs|All files (*.*)|*.*";
        private ScintillaForm _scintillaForm = null;

        public Form1()
        {
            InitializeComponent();
            InitScintilla();
            InitScintillaText();
        }

        private void InitScintilla()
        {
            scintilla1.InsertCheck += scintilla1_InsertCheck;
            _scintillaForm = new ScintillaForm(scintilla1);
            _scintillaForm.SetFont("Consolas", 10);
            _scintillaForm.ConfigureLexerCpp();
            _scintillaForm.SetTabIndent(2, 0, false);
            _scintillaForm.DisplayLineNumber(4);
        }

        private void InitScintillaText()
        {
            scintilla1.Text = @"using System;
using System.Text;
using System.IO;

// doc
// doc
// doc
// doc

/*
  doc
  doc
  doc
*/

namespace toto
{
  public class toto
  {
toto
  }
toto
}

";
        }

        //private void ScintillaTextModified()
        //{
        //    _findForm.FindText.TextModified();
        //    //WriteMessage("scintilla text modified");
        //}

        private void OpenFile()
        {
            if (InvokeRequired)
            {
                Invoke((Action)OpenFile);
            }
            else
            {
                string file = SelectOpenFile();
                if (file != null)
                {
                    if (File.Exists(file))
                        scintilla1.Text = File.ReadAllText(file, Encoding.UTF8);
                    else
                        scintilla1.Text = "";
                    scintilla1.SelectionStart = 0;
                    scintilla1.SelectionEnd = 0;
                }
            }
        }

        private string SelectOpenFile()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = _directory;
            openFileDialog.Filter = _fileFilter;
            openFileDialog.FilterIndex = 1;
            openFileDialog.RestoreDirectory = true;
            //openFileDialog.FileName = defaultFile;
            openFileDialog.CheckFileExists = false;
            DialogResult dr = openFileDialog.ShowDialog();

            string file = null;
            if (dr == DialogResult.OK)
            {
                file = openFileDialog.FileName;
                _directory = Path.GetDirectoryName(file);
            }
            return file;
        }

        private void bt_previous_bookmark_Click(object sender, System.EventArgs e)
        {
            _scintillaForm.ScintillaBookmark.GotoPreviousBookmark();
            scintilla1.Focus();
        }

        private void bt_next_bookmark_Click(object sender, System.EventArgs e)
        {
            _scintillaForm.ScintillaBookmark.GotoNextBookmark();
            scintilla1.Focus();
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            //WriteMessage("key {0} - {1}", KeyToString(_previousKey), KeyToString(e));
            // ctrl + ...
            if (!e.Alt && e.Control && !e.Shift)
            {
                switch (e.KeyCode)
                {
                    case Keys.O:
                        new Task(OpenFile).Start();
                        break;
                    case Keys.T:
                        Try(Test);
                        break;
                }
            }
        }

        private StringBuilder _keyToString = new StringBuilder();
        private string KeyToString(KeyEventArgs key)
        {
            if (key == null)
                return null;
            _keyToString.Clear();
            bool first = true;
            if (key.Shift)
            {
                _keyToString.Append("shift");
                first = false;
            }
            if (key.Control)
            {
                if (!first)
                    _keyToString.Append('+');
                _keyToString.Append("ctrl");
                first = false;
            }
            if (key.Alt)
            {
                if (!first)
                    _keyToString.Append('+');
                _keyToString.Append("alt");
                first = false;
            }
            if (!first)
                _keyToString.Append('+');
            _keyToString.Append(key.KeyCode.ToString());
            return _keyToString.ToString();
        }

        private void Test()
        {
            //Test_01();
            //Test_02();
            //Test_03();
            //Test_04();
            //Test_05();
            //Test_04();
            Test_06();
            //WriteMessage("");
        }

        private uint _bookmarkMarginMask = 0;
        private void Test_06()
        {
            if (_bookmarkMarginMask == 0)
                _bookmarkMarginMask = 1;
            else
                _bookmarkMarginMask *= 2;
            scintilla1.Margins[1].Mask = _bookmarkMarginMask;
            WriteMessage("set bookmark margin mask to {0}", _bookmarkMarginMask);
        }

        private void Test_04()
        {
            int line = scintilla1.zGetCurrentLineNumber();
            int indent = scintilla1.zGetLineIndent(line);
            WriteMessage("current line {0} indent {1}", line, indent);
        }

        private void Test_05()
        {
            int indent = 12;
            int line = scintilla1.zGetCurrentLineNumber();
            scintilla1.zSetLineIndent(line, indent);
            WriteMessage("set current line {0} indent {1}", line, indent);
        }

        private void Test_02()
        {
            int position = scintilla1.CurrentPosition;
            // position where a word starts, searching backward
            int wordPosition = scintilla1.WordStartPosition(position, true);
            string text = null;
            if (wordPosition != -1)
                text = scintilla1.GetTextRange(wordPosition, position - wordPosition + 1);
            WriteMessage("WordStartPosition onlyWordCharacters true  : current position {0} position found {1} text \"{2}\"", position, wordPosition, text);

            wordPosition = scintilla1.WordStartPosition(position, false);
            text = null;
            if (wordPosition != -1)
                text = scintilla1.GetTextRange(wordPosition, position - wordPosition + 1);
            WriteMessage("WordStartPosition onlyWordCharacters false : current position {0} position found {1} text \"{2}\"", position, wordPosition, text);

            wordPosition = scintilla1.WordEndPosition(position, true);
            text = null;
            if (wordPosition != -1)
                text = scintilla1.GetTextRange(position, wordPosition - position + 1);
            WriteMessage("WordEndPosition   onlyWordCharacters true  : current position {0} position found {1} text \"{2}\"", position, wordPosition, text);

            wordPosition = scintilla1.WordEndPosition(position, false);
            text = null;
            if (wordPosition != -1)
                text = scintilla1.GetTextRange(position, wordPosition - position + 1);
            WriteMessage("WordEndPosition   onlyWordCharacters false : current position {0} position found {1} text \"{2}\"", position, wordPosition, text);
        }

        private void Test_03()
        {
            int position = scintilla1.CurrentPosition;
            // position where a word starts, searching backward
            int wordStart = scintilla1.WordStartPosition(position, true);
            // WordEndPosition return position of first blank character
            int wordEnd = scintilla1.WordEndPosition(position, true);
            string text;
            if (wordStart != wordEnd)
                text = scintilla1.GetTextRange(wordStart, wordEnd - wordStart);
            else
                text = "--no word--";
            WriteMessage("word : \"{0}\" start {1} end {2}", text, wordStart, wordEnd);
        }

        private void Test_01()
        {
            WriteMessage("GetCurrentLineNumber() {0}", scintilla1.zGetCurrentLineNumber());

            Line line = scintilla1.Lines[0];
            WriteMessage("Line 0 :");
            TraceLine(line);

            line = scintilla1.Lines[-1];
            WriteMessage("Line -1 :");
            TraceLine(line);
        }

        private void TraceLine(Line line)
        {
            WriteMessage("  Index              {0}", line.Index);
            WriteMessage("  DisplayIndex       {0}", line.DisplayIndex);
            WriteMessage("  Position           {0}", line.Position);
            WriteMessage("  EndPosition        {0}", line.EndPosition);
            WriteMessage("  Length             {0}", line.Length);
            WriteMessage("  Indentation        {0}", line.Indentation);
            WriteMessage("  Text               {0}", line.Text);
        }

        private void Try(Action action)
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                WriteMessage(ex.Message);
                WriteMessage(ex.StackTrace);
            }
        }

        private void WriteMessage(string msg, params object[] prm)
        {
            if (tb_message.Lines.Length > 1000)
            {
                tb_message.SuspendLayout();
                string[] lines = new string[900];
                Array.Copy(tb_message.Lines, tb_message.Lines.Length - 900, lines, 0, 900);
                tb_message.Lines = lines;
                tb_message.ResumeLayout();
            }
            if (prm.Length != 0)
                msg = string.Format(msg, prm);
            tb_message.AppendText(msg);
            tb_message.AppendText("\r\n");
        }

        private void Status(string message)
        {
            tb_status.Text = message;
        }

        private void m_open_Click(object sender, EventArgs e)
        {
            //Try(OpenFile);
        }

        private void m_quit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void m_find_Click(object sender, EventArgs e)
        {
            _scintillaForm.ScintillaFindText.OpenFindForm();
        }

        private void m_previous_bookmark_Click(object sender, EventArgs e)
        {
            _scintillaForm.ScintillaBookmark.GotoPreviousBookmark();
        }

        private void m_next_bookmark_Click(object sender, EventArgs e)
        {
            _scintillaForm.ScintillaBookmark.GotoNextBookmark();
        }

        private void scintilla1_InsertCheck(object sender, InsertCheckEventArgs e)
        {
            bool removed = false;
            string text = e.Text;
            // remove ctrl-B code 0x02
            if (e.Text == "\u0002")
            {
                e.Text = null;
                removed = true;
            }
            WriteMessage("insert \"{0}\" code {1}{2}", text, (int)text[0], removed ? " removed" : "");
        }
    }
}
