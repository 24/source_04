using System.Data;
using System.Windows.Forms;
using ScintillaNET;
using pb.Windows.Forms;

namespace Test.Test_Form.Test_RunSourceForm.v4
{
    public static class Test_Form
    {
        public static void Test_RunSourceForm_v4_01()
        {
            //RunSourceForm form = new RunSourceForm();
            //InitSource(form.Source);
            //InitTextValues(form.LogTextBox);
            //DataTable dt = CreateDataTable();
            //form.GridResult1.DataSource = dt;
            //form.GridResult2.DataSource = dt;
            //form.GridResult3.DataSource = dt;
            //InitTreeValues(form.TreeResult);
            //form.Show();
            Test_RunSourceFormExe form = new Test_RunSourceFormExe();
            form.Show();
        }

        private static void CreateMenu(MenuStrip menu)
        {
            ToolStripMenuItem m_file = zForm.CreateMenuItem("&File");
            m_file.DropDownItems.AddRange(new ToolStripItem[] {
                zForm.CreateMenuItem("&New (Ctrl-N)"),
                zForm.CreateMenuItem("&Open (Ctrl-O)"),
                zForm.CreateMenuItem("&Save (Ctrl-S)"),
                zForm.CreateMenuItem("Save &as (Ctrl-A)"),
                //new ToolStripSeparator(),
                //zForm.CreateMenuItem("&Execute (F5)", onClick: m_execute_Click),
                //zForm.CreateMenuItem("Execute on &main thread (shift + F5)", onClick: m_execute_on_main_thread_Click),
                //zForm.CreateMenuItem("Execute on &main thread (shift + F5)", onClick: m_execute_on_main_thread_Click),
                //zForm.CreateMenuItem("Execute &without project (ctrl + F5)", onClick: m_execute_without_project_Click),
                //zForm.CreateMenuItem("&Compile (Shift-Ctrl-B)", onClick: m_compile_Click),
                //new ToolStripSeparator(),
                //zForm.CreateMenuItem("Compile and &restart \"Run source\" (Shift-Ctrl-U)", onClick: m_update_runsource_Click),
                //zForm.CreateMenuItem("C&ompile \"Run source\"  (Shift-Ctrl-C)", onClick: m_compile_runsource_Click),
                //zForm.CreateMenuItem("&Restart \"Run source\"  (Shift-Ctrl-R)", onClick:  m_restart_runsource_Click),
                //new ToolStripSeparator(),
                //zForm.CreateMenuItem("&Quit", onClick: m_quit_Click),
            });

            ToolStripMenuItem m_options = zForm.CreateMenuItem("&Options");

            //m_view_source_line_number = zForm.CreateMenuItem("View source line number", checkOnClick: true, @checked: true, onClick: m_view_source_line_number_Click);
            //m_run_init = zForm.CreateMenuItem("Run &init", checkOnClick: true, @checked: true, onClick: m_run_init_Click);
            //m_allow_multiple_execution = zForm.CreateMenuItem("&Allow multiple execution", checkOnClick: true, @checked: true, onClick: m_allow_multiple_execution_Click);

            m_options.DropDownItems.AddRange(new ToolStripItem[] {
                zForm.CreateMenuItem("Set grid &max width height", checkOnClick: true, @checked: true),
                zForm.CreateMenuItem("Resize data table images", checkOnClick: true, @checked: true)
                //m_view_source_line_number,
                //new ToolStripSeparator(),
                //m_run_init,
                //m_allow_multiple_execution
            });

            menu.Items.AddRange(new ToolStripItem[] { m_file, m_options });
        }

        public static void InitSource(Scintilla scintilla)
        {
            scintilla.Text = @"using System;
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

        public static void InitTextValues(TextBox textBox)
        {
            textBox.AppendText(
@"Message :
Message 1
Message 2
Message 3
Message 4
Message 5
Message 6
");
        }

        public static DataTable CreateDataTable()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("col1");
            dt.Columns.Add("col2");
            dt.Columns.Add("col3");
            DataRow row = dt.NewRow();
            dt.Rows.Add("toto", "tata", "tutu");
            dt.Rows.Add("toto", "tata", "tutu");
            dt.Rows.Add("toto", "tata", "tutu");
            dt.Rows.Add("toto", "tata", "tutu");
            dt.Rows.Add("toto", "tata", "tutu");
            dt.Rows.Add("toto", "tata", "tutu");
            dt.Rows.Add("toto", "tata", "tutu");
            dt.Rows.Add("toto", "tata", "tutu");
            dt.Rows.Add("toto", "tata", "tutu");
            dt.Rows.Add("toto", "tata", "tutu");
            dt.Rows.Add("toto", "tata", "tutu");
            dt.Rows.Add("toto", "tata", "tutu");
            dt.Rows.Add("toto", "tata", "tutu");
            dt.Rows.Add("toto", "tata", "tutu");
            dt.Rows.Add("toto", "tata", "tutu");
            dt.Rows.Add("toto", "tata", "tutu");
            dt.Rows.Add("toto", "tata", "tutu");
            return dt;
        }

        public static void InitTreeValues(TreeView tree_result)
        {
            tree_result.BeginUpdate();
            tree_result.Nodes.Clear();
            TreeNode treeNode = new TreeNode("toto 1");
            treeNode.Nodes.Add("toto 1 child 1");
            treeNode.Nodes.Add("toto 1 child 2");
            treeNode.Nodes.Add("toto 1 child 3");
            tree_result.Nodes.Add(treeNode);
            treeNode = new TreeNode("toto 2");
            treeNode.Nodes.Add("toto 2 child 1");
            treeNode.Nodes.Add("toto 2 child 2");
            treeNode.Nodes.Add("toto 2 child 3");
            tree_result.Nodes.Add(treeNode);
            tree_result.EndUpdate();
        }
    }
}
