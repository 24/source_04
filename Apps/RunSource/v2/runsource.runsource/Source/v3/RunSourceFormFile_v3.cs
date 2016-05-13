using pb;
using pb.IO;
using System;
using System.Windows.Forms;

namespace runsourced
{
    partial class RunSourceForm_v3
    {
        private bool _fileSaved = true;
        private string _sourceFilter = "source files (*.cs)|*.cs|All files (*.*)|*.*";
        private string _settingsProjectDirectory = null;

        private void New()
        {
            bool cancel;
            ControlSave(out cancel);
            if (cancel)
                return;
            OpenSourceFile(null);
        }

        private void Open()
        {
            bool cancel;
            ControlSave(out cancel);
            if (cancel)
                return;
            string path = SelectOpenFile(_runSource.SourceFile);
            if (path != null)
                OpenSourceFile(path);
        }

        private void Save()
        {
            bool cancel;
            Save(out cancel);
        }

        private void Save(out bool cancel)
        {
            cancel = false;
            if (_runSource.SourceFile == null)
                SetSourceFile(SelectSaveFile(_runSource.SourceFile));
            if (_runSource.SourceFile == null)
            {
                cancel = true;
                return;
            }
            if (!WriteSourceFile())
                cancel = true;
        }

        private void SaveAs()
        {
            string path = SelectSaveFile(_runSource.SourceFile);
            if (path != null)
            {
                SetSourceFile(path);
                WriteSourceFile();
            }
        }

        //public
        private void OpenSourceFile(string path)
        {
            SetSourceFile(path);
            Try(ReadSourceFile);
        }

        private void SetFileSaved()
        {
            _fileSaved = true;
            SetFormTitle();
        }

        private void SetFileNotSaved()
        {
            _fileSaved = false;
            SetFormTitle();
        }

        private void ControlSave(out bool cancel)
        {
            cancel = false;
            if (!_fileSaved)
            {
                DialogResult dr = MessageBox.Show("Le source a été modifié.\r\nVoulez vous le sauvegarder ?", "Sauvegarde du source",
                    MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                if (dr == DialogResult.Cancel)
                    cancel = true;
                else if (dr == DialogResult.Yes)
                {
                    Save(out cancel);
                }
            }
        }

        private string SelectOpenFile(string defaultFile)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = _runSource.ProjectDirectory ?? _settingsProjectDirectory;
            openFileDialog.Filter = _sourceFilter;
            openFileDialog.FilterIndex = 1;
            openFileDialog.RestoreDirectory = true;
            openFileDialog.FileName = defaultFile;
            openFileDialog.CheckFileExists = false;
            DialogResult dr = openFileDialog.ShowDialog();

            string file = null;
            if (dr == DialogResult.OK)
                file = openFileDialog.FileName;
            return file;
        }

        private string SelectSaveFile(string defaultFile)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.InitialDirectory = _runSource.ProjectDirectory ?? _settingsProjectDirectory;
            saveFileDialog.Filter = _sourceFilter;
            saveFileDialog.FilterIndex = 1;
            saveFileDialog.RestoreDirectory = true;
            saveFileDialog.FileName = defaultFile;
            DialogResult dr = saveFileDialog.ShowDialog();

            string file = null;
            if (dr == DialogResult.OK)
                file = saveFileDialog.FileName;
            return file;
        }

        private void ReadSourceFile()
        {
            //try
            //{
            if (_runSource.SourceFile != null && zFile.Exists(_runSource.SourceFile))
                _source.Text = zfile.ReadAllText(_runSource.SourceFile);
            else
                _source.Text = "";
            _source.SelectionStart = 0;
            _source.SelectionEnd = 0;
            SetFileSaved();
            //}
            //catch (Exception ex)
            //{
            //    _trace.WriteError(ex);
            //    zerrf.ErrorMessageBox(ex);
            //}
        }

        private bool WriteSourceFile()
        {
            if (_fileSaved)
                return true;
            try
            {
                zfile.WriteFile(_runSource.SourceFile, _source.Text);
                SetFileSaved();
                return true;
            }
            catch (Exception ex)
            {
                _trace.WriteError(ex);
                MessageBox.Show("Error saving file : \r\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }
    }
}
