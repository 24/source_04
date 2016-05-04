using System;
using System.Windows.Forms;
using ScintillaNET;

namespace pb.Windows.Forms
{
    public partial class ScintillaFindForm : Form
    {
        public Action<string, SearchFlags> SetFindParam = null;
        public Action FindNext = null;

        public ScintillaFindForm()
        {
            InitializeComponent();
        }

        private const int CP_NOCLOSE_BUTTON = 0x200;
        protected override CreateParams CreateParams
        {
            // from http://www.codeproject.com/Articles/20379/Disabling-Close-Button-on-Forms
            get
            {
                CreateParams createParams = base.CreateParams;
                createParams.ClassStyle = createParams.ClassStyle | CP_NOCLOSE_BUTTON;
                return createParams;
            }
        }

        public void SetText(string text)
        {
            tb_text.Text = text;
        }

        private void HideForm()
        {
            this.Hide();
            //_Find(false);
        }

        private void _SetFindParam()
        {
            if (SetFindParam != null)
                SetFindParam(tb_text.Text, SearchFlags.None);
        }

        private void _FindNext()
        {
            //if (Find != null)
            //    Find(tb_text.Text, SearchFlags.None);
            if (FindNext != null)
                FindNext();
        }

        private void FindForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (!e.Alt && !e.Control && !e.Shift)
            {
                switch (e.KeyCode)
                {
                    case Keys.Escape:
                        HideForm();
                        break;
                }
            }
        }

        private void bt_find_next_Click(object sender, System.EventArgs e)
        {
            _FindNext();
        }

        private void tb_text_TextChanged(object sender, EventArgs e)
        {
            _SetFindParam();
        }
    }
}
