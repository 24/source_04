using System.Drawing;
using System.Windows.Forms;
using pb.Windows.Forms;

namespace pb.Windows.Forms
{
    public partial class RunSourceForm : RunSourceFormBase
    {
        protected ToolStripMenuItem _menuFile;
        protected ToolStripMenuItem _menuOptions;

        public RunSourceForm()
        {
            CreateMenu();
            CreateScintillaControl();
            CreateResultControls();
            CreateStatusBar();
            InitMenuScintilla();
            this.ClientSize = new Size(1060, 650);
            //this.InitializeForm();
            //this.KeyPreview = true;
        }

        private void CreateMenu()
        {
            _menuFile = zForm.CreateMenuItem("&File");
            _menuOptions = zForm.CreateMenuItem("&Options");
            this.MainMenuStrip.Items.AddRange(new ToolStripItem[] { _menuFile, _menuOptions });
        }
    }
}
