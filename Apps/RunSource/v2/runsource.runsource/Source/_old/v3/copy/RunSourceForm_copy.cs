using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Xml.Linq;
using System.Windows.Forms;
using pb;
using pb.Data;
using pb.IO;
using pb.Data.Xml;
using pb.Compiler;
using pb.Windows.Forms;

namespace runsourced
{
    public partial class RunSourceForm_copy : Form
    {
        public delegate void fExe();
        // $$todo à supprimer
        //private bool _errorResult = false;              // true si le résultat est une liste d'erreurs
        //private bool _newMessage = false;             // true si il y a un nouveau message à afficher
        //private string _sourceFile = null;

        private TabPage _tabResultMessage = null;
        private TabPage _tabResultGrid = null;
        private TabPage _tabResultTree = null;

        private ScintillaForm _scintillaForm = null;

        private ITrace _trace = null;

        public RunSourceForm_copy(IRunSource runSource, ITrace trace, XmlConfig config, RunSourceRestartParameters runSourceParameters)
        {
            _trace = trace;

            try
            {
                Initialize();

                _tabResultMessage = tab_message;
                _tabResultGrid = tab_result2;
                _tabResultTree = tab_result4;

                //this.Icon = Properties.Resources.app;
                //string title = config.Get("RunsourceTitle");
                //if (title != null)
                //    __title = title;

                //tc_result.SelectedTab = _tabResultMessage;

                //cGrid.Culture = CultureInfo.CurrentUICulture;

                //_gridMaxWidth = _config.Get("GridMaxWidth").zParseAs<int>();
                //_gridMaxHeight = _config.Get("GridMaxHeight").zParseAs<int>();
                //_dataTableMaxImageWidth = _config.Get("DataTableMaxImageWidth").zParseAs<int>();
                //_dataTableMaxImageHeight = _config.Get("DataTableMaxImageHeight").zParseAs<int>();

                //initRunSource();
                //SetFileSaved();
            }
            catch (Exception ex)
            {
                _trace.WriteError(ex);
                zerrf.ErrorMessageBox(ex);
            }
        }

        public void Exe(fExe f)
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;
                f();
            }
            catch (Exception ex)
            {
                //_runSource.Trace.WriteError(ex);
                _trace.WriteError(ex);
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }
    }
}
