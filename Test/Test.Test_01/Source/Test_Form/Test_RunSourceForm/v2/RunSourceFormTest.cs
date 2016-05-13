using System.Data;
using System.Windows.Forms;

namespace Test.Test_Form.Test_RunSourceForm.v2
{
    public partial class RunSourceForm : Form
    {
        public RunSourceForm(int version)
        {
            _version = version;

            Initialize();

            if (_version == 1)
            {
                Test_Form.InitTextValues(tb_message);
                DataTable dt = Test_Form.CreateDataTable();
                _gridResult1.DataSource = dt;
                _gridResult2.DataSource = dt;
                grid_result3.DataSource = dt;
                Test_Form.InitTreeValues(tree_result);
            }
            else if (_version == 2)
            {
                Test_Form.InitTextValues(tb_message);
                DataTable dt = Test_Form.CreateDataTable();
                _gridResult1.DataSource = dt;
                _gridResult2.DataSource = dt;
                grid_result3.DataSource = dt;
                Test_Form.InitTreeValues(tree_result);
            }
        }

//        private void InitTextValues()
//        {
//            tb_message.AppendText(
//@"Message :
//Message 1
//Message 2
//Message 3
//Message 4
//Message 5
//Message 6
//");
//        }

        //private void InitDataValues()
        //{
        //    DataTable dt = CreateDataTable();
        //    _gridResult1.DataSource = dt;
        //    _gridResult2.DataSource = dt;
        //    grid_result3.DataSource = dt;
        //}

        //private void InitTreeValues()
        //{
        //    tree_result.BeginUpdate();
        //    tree_result.Nodes.Clear();
        //    TreeNode treeNode = new TreeNode("toto 1");
        //    treeNode.Nodes.Add("toto 1 child 1");
        //    treeNode.Nodes.Add("toto 1 child 2");
        //    treeNode.Nodes.Add("toto 1 child 3");
        //    tree_result.Nodes.Add(treeNode);
        //    treeNode = new TreeNode("toto 2");
        //    treeNode.Nodes.Add("toto 2 child 1");
        //    treeNode.Nodes.Add("toto 2 child 2");
        //    treeNode.Nodes.Add("toto 2 child 3");
        //    tree_result.Nodes.Add(treeNode);
        //    tree_result.EndUpdate();
        //}
    }
}
