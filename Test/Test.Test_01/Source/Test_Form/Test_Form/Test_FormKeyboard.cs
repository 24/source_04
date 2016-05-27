using System.Windows.Forms;
using pb;
using pb.Windows.Forms;

namespace Test.Test_Form.Test_FormKeyboard
{
    public partial class Test_FormKeyboard : Form
    {
        //private FormKeyboard _formKeyboard;

        public Test_FormKeyboard()
        {
            InitializeComponent();
            FormKeyboard formKeyboard = new FormKeyboard(this);
            formKeyboard.SetSimpleKey(Keys.Control | Keys.N, () => Trace.WriteLine("action ctrl-N"));
            formKeyboard.SetFirstKey(Keys.Control | Keys.K, () => Trace.WriteLine("action first key ctrl-K"));
            formKeyboard.SetMultipleKey(Keys.Control | Keys.K, Keys.Control | Keys.N, () => Trace.WriteLine("action ctrl-K + ctrl-N"));
            formKeyboard.SetMultipleKey(Keys.Control | Keys.K, Keys.Control | Keys.K, () => Trace.WriteLine("action ctrl-K + ctrl-K"));
        }
    }
}
