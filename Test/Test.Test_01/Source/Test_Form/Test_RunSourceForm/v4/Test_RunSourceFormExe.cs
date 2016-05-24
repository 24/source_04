using pb;
using pb.Windows.Forms;
using System.Data;

namespace Test.Test_Form.Test_RunSourceForm.v4
{
    public class Test_RunSourceFormExe : RunSourceForm
    {
        public Test_RunSourceFormExe()
        {
            Test_Form.InitSource(_source);
            Test_Form.InitTextValues(_logTextBox);
            DataTable dt = Test_Form.CreateDataTable();
            _gridResult1.DataSource = dt;
            _gridResult2.DataSource = dt;
            _gridResult3.DataSource = dt;
            Test_Form.InitTreeValues(_treeResult);

            this.InitializeForm();
            this.KeyPreview = true;
            SelectMessageResultTab();
            ActiveControl = _source;

            //_source.UpdateUI += _source_UpdateUI;
        }

        private void _source_UpdateUI(object sender, ScintillaNET.UpdateUIEventArgs e)
        {
            Trace.WriteLine("UpdateUI : CurrentPosition {0} Line {1} Column {2} Overtype {3}", _source.CurrentPosition, _source.LineFromPosition(_source.CurrentPosition), _source.GetColumn(_source.CurrentPosition), _source.Overtype);
        }
    }
}
