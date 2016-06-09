using pb;
using pb.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;

namespace runsourced
{
    partial class RunSourceFormExe
    {
        private bool _disableMessage = false;
        private DataTable _dataTableResult = null;
        private DataSet _dataSetResult = null;
        private string _xmlResultFormat = null;
        private bool _newDataTableResult = false;       // true si il y a un nouveau résultat DataTable ou DataSet à afficher
        private bool _newTreeViewResult = false;        // true si il y a un nouveau résultat TreeView à afficher
        private bool _selectTreeViewResult = false;     // true si il faut sélectionner le résultat du TreeView

        private bool _resizeDataTableImages = true;
        private int _dataTableMaxImageWidth = 0;
        private int _dataTableMaxImageHeight = 0;

        private void InitResult()
        {
            _gridResult2.MaxWidth = _config.Get("GridMaxWidth").zParseAs<int>();
            _gridResult2.MaxHeight = _config.Get("GridMaxHeight").zParseAs<int>();
            _dataTableMaxImageWidth = _config.Get("DataTableMaxImageWidth").zParseAs<int>();
            _dataTableMaxImageHeight = _config.Get("DataTableMaxImageHeight").zParseAs<int>();

            SelectMessageResultTab();
        }

        private void TraceWrited(string msg)
        {
            if (InvokeRequired)
            {
                Invoke((Action)(() => TraceWrited(msg)));
            }
            else
            {
                if (!_newDataTableResult)
                {
                    SelectMessageResultTab();
                }
                _logTextBox.WriteMessage(msg);
            }
        }

        private void RazResult()
        {
            // le résultat est conservé d'un appel sur l'autre
            _dataTableResult = null;
            _dataSetResult = null;
            ViewDataTableResult();

            RazTreeViewResult();
            _newDataTableResult = false;
            _newTreeViewResult = false;
            _selectTreeViewResult = false;
        }

        private void ViewDataTableResult()
        {
            if (_dataTableResult != null)
            {
                Try(() => ResizeDataTableImages(_dataTableResult));
                Try(() => _gridResult1.SetDataSource(_dataTableResult, _xmlResultFormat));
                Try(() => _gridResult2.SetDataSource(_dataTableResult));
                Try(() => _gridResult3.DataSource = _dataTableResult.DefaultView);
            }
            else if (_dataSetResult != null)
            {
                Try(() => _gridResult1.SetDataSource(_dataSetResult.Tables[0], _xmlResultFormat));
                Try(() => _gridResult2.SetDataSource(_dataSetResult.Tables[0]));
                Try(() => _gridResult3.DataSource = _dataSetResult);
            }
            else
            {
                _gridResult1.ClearDataSource();
                _gridResult2.DataSource = null;
                _gridResult3.DataSource = null;
            }
        }

        private bool GetGridMaxColumnsWidthAndRowsHeight()
        {
            return _gridResult2.GridMaxColumnsWidthAndRowsHeight;
        }

        private void SetGridMaxColumnsWidthAndRowsHeight(bool setGridMaxColumnsWidthAndRowsHeight)
        {
            _gridResult2.GridMaxColumnsWidthAndRowsHeight = setGridMaxColumnsWidthAndRowsHeight;
        }

        private bool GetResizeDataTableImages()
        {
            return _resizeDataTableImages;
        }

        private void SetResizeDataTableImages(bool resizeDataTableImages)
        {
            _resizeDataTableImages = resizeDataTableImages;
        }

        private void ResizeDataTableImages(DataTable dataTable)
        {
            if (!_resizeDataTableImages || (_dataTableMaxImageWidth == 0 && _dataTableMaxImageHeight == 0))
                return;
            List<int> imageColumns = new List<int>();
            foreach (DataColumn column in dataTable.Columns)
            {
                if (column.DataType == typeof(Image) || column.DataType.BaseType == typeof(Image))
                {
                    imageColumns.Add(column.Ordinal);
                }
            }

            foreach (DataRow row in dataTable.Rows)
            {
                foreach (int indexColumn in imageColumns)
                {
                    object value = row[indexColumn];
                    if (value is Image || value.GetType().BaseType == typeof(Image))
                    //if (value is Bitmap)
                    {
                        Image image = (Image)value;
                        if (_dataTableMaxImageHeight != 0 && image.Height > _dataTableMaxImageHeight) // 200
                        {
                            row[indexColumn] = image.zResize(height: _dataTableMaxImageHeight); // 200
                        }
                        else if (_dataTableMaxImageWidth != 0 && image.Width > _dataTableMaxImageWidth)
                        {
                            row[indexColumn] = image.zResize(width: _dataTableMaxImageWidth);
                        }
                    }
                }
            }
        }

        private void RazTreeViewResult()
        {
            //foreach (Control control in _treeResultPanel.Controls)
            //    control.Dispose();
            //_treeResultPanel.Controls.Clear();
            //_treeResult = new TreeView();
            //_treeResult.Dock = DockStyle.Fill;
            //_treeResult.Location = new System.Drawing.Point(0, 0);
            //_treeResult.Name = "tree_result";
            //_treeResult.Size = new System.Drawing.Size(1048, 364);
            //_treeResult.TabIndex = 0;
            _treeResult.Nodes.Clear();
        }

        private void ViewTreeViewResult()
        {
            //_treeResult.ResumeLayout();
            //_treeResultPanel.Controls.Add(_treeResult);
        }

        private void SetResult(DataTable dt, string xmlFormat)
        {
            _newDataTableResult = true;
            _dataTableResult = dt;
            _xmlResultFormat = xmlFormat;
            _dataSetResult = null;
        }

        private void SetResult(DataSet ds, string xmlFormat)
        {
            _newDataTableResult = true;
            _dataSetResult = ds;
            _xmlResultFormat = xmlFormat;
            _dataTableResult = null;
        }

        private void RazProgress()
        {
            _progressText = null;
            _progressLabel.Text = "";
            _progressBar.Value = 0;
        }

        private void Progress(int current, int total, string message, params object[] prm)
        {
            if (message != null)
            {
                if (prm.Length > 0)
                    message = string.Format(message, prm);
                _progressText = message;
                _progressLabel.Text = _progressText;
                if (_runSource.Progress_PutProgressMessageToWindowsTitle)
                    SetFormTitle();
            }
            if (total != 0)
                _progressBar.Value = (int)((float)current / total * 100 + 0.5);
            else
                _progressBar.Value = 0;
        }

        private void EventDisableMessageChanged(bool disableMessage)
        {
            _disableMessage = disableMessage;
        }

        private void EventGridResultSetDataTable(DataTable dt, string xmlFormat)
        {
            if (InvokeRequired)
            {
                Invoke((Action)(() => EventGridResultSetDataTable(dt, xmlFormat)));
            }
            else
            {
                SetResult(dt, xmlFormat);
            }
        }

        private void EventGridResultSetDataSet(DataSet ds, string xmlFormat)
        {
            if (InvokeRequired)
            {
                Invoke((Action)(() => EventGridResultSetDataSet(ds, xmlFormat)));
            }
            else
            {
                SetResult(ds, xmlFormat);
            }
        }

        private void EventProgressChange(int current, int total, string message, params object[] prm)
        {
            if (InvokeRequired)
            {
                Invoke((Action)(() => EventProgressChange(current, total, message, prm)));
            }
            else
            {
                Progress(current, total, message, prm);
            }
        }
    }
}
