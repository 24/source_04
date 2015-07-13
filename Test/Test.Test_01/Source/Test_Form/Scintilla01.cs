using System.Windows.Forms;
using ScintillaNET.Configuration;

namespace Test.Test_Form
{
    public partial class Scintilla01 : Form
    {
        public Scintilla01()
        {
            InitializeComponent();
            //ConfigurationManager configurationManager = new ConfigurationManager();
            //configurationManager.Language = "c#";
            //ScintillaNET.xml
            scintilla1.ConfigurationManager.CustomLocation = @"c:\pib\dropbox\pbeuz\Dropbox\dev\project\Source\Source_01\Source\Test\Test_Form\ScintillaNET.xml";
            scintilla1.ConfigurationManager.Language = "cs";
        }
    }
}
