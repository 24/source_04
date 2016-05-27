using pb;
using System.Windows.Forms;

namespace Test.Test_Form.Test_FormKeyDown
{
    public partial class Test_FormKeyDown : Form
    {
        public Test_FormKeyDown()
        {
            InitializeComponent();
        }

        private void Test_FormKeyDown_KeyDown(object sender, KeyEventArgs e)
        {
            // KeyCode et KeyValue sont identique
            // KeyData = KeyCode & Modifiers
            Trace.WriteLine("KeyCode {0,-20} - {1} - {2,-3} KeyValue {3} {4} KeyData {5} Modifiers {6} {7} KeyCode {8,-30} KeyData {9}",
                e.KeyCode, ((int)e.KeyCode).zToHex(), (int)e.KeyCode, e.KeyValue.zToHex(), (int)e.KeyCode == e.KeyValue ? "ok" : "NOT OK",
                ((int)e.KeyData).zToHex(), ((int)e.Modifiers).zToHex(), e.KeyData == (e.KeyCode | e.Modifiers) ? "ok" : "NOT OK", e.KeyCode, e.KeyData);
        }
    }
}
