using System.Drawing;
using System.Windows.Forms;

namespace pb.Windows.Forms
{
    public class LogTextBox : TextBox
    {
        public static LogTextBox Create(string name = null, DockStyle dockStyle = DockStyle.None, bool wordWrap = false, int? x = null, int? y = null, int? width = null, int? height = null)
        {
            LogTextBox textBox = new LogTextBox();
            textBox.Name = name;
            textBox.Dock = dockStyle;
            textBox.Font = new Font("Courier New", 9F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
            textBox.Multiline = true;
            textBox.ScrollBars = ScrollBars.Both;
            Point? point = zForm.GetPoint(x, y);
            if (point != null)
                textBox.Location = (Point)point;
            Size? size = zForm.GetSize(width, height);
            if (size != null)
                textBox.Size = (Size)size;
            textBox.WordWrap = wordWrap;
            return textBox;
        }
    }
}
