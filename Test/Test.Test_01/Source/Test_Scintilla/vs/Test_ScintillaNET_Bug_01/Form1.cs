using System;
using System.Threading.Tasks;
using System.Windows.Forms;

// bug :
//   problème quand ctrl-o ouvre une boite de dialogue modale, un code est inséré dans le control scintilla
//   la desactivation de la commande ctrl-o ne marche pas dans ce cas (Scintilla.ClearCmdKey)
//   l'ouverture d'une boite de dialogue non modale ne pose pas de problème
//   solution 1
//     - supprimer le code avec l'event Scintilla.InsertCheck
//     - problème l'event TextChanged est quand meme appelé
//   solution 2
//     - faire un postmessage à la reception du ctrl-o et ouvrir la boite de dialogue modale avec le postmessage
//   solution 3
//     - ouvrir la boite de dialogue modale dans un autre thread
//
//  Form1 : show the bug and solution 3
//  Form2 : solution 2



namespace Test_ScintillaNET_Bug_01
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            scintilla1.ClearCmdKey(Keys.Control | Keys.O);
        }

        private string OpenFile()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.RestoreDirectory = true;
            openFileDialog.CheckFileExists = false;
            // problem with ShowDialog()
            DialogResult dr = openFileDialog.ShowDialog();

            string file = null;
            if (dr == DialogResult.OK)
            {
                file = openFileDialog.FileName;
            }
            return file;
        }

        private void OpenDialog()
        {
            Dialog dialog = new Dialog();
            // problem with ShowDialog()
            //dialog.ShowDialog();
            // problem with ShowDialog(this)
            dialog.ShowDialog(this);
            // no problem with Show()
            //dialog.Show();
        }

        private void OpenDialogInvoke()
        {
            if (InvokeRequired)
            {
                Invoke((Action)OpenDialogInvoke);
            }
            else
            {
                Dialog dialog = new Dialog();
                // problem with ShowDialog()
                //dialog.ShowDialog();
                // problem with ShowDialog(this)
                dialog.ShowDialog(this);
                // no problem with Show()
                //dialog.Show();
            }
        }

        private void OpenDialogWithThread()
        {
            // no problem with new Task()
            new Task(OpenDialogInvoke).Start();
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (!e.Alt && e.Control && !e.Shift && e.KeyCode == Keys.O)
            {
                //OpenFile();
                //OpenDialog();
                OpenDialogWithThread();
            }
        }
    }
}
