using System;
using System.Drawing;
using System.Windows.Forms;

namespace pb.Windows.Forms
{
    public class zForm
    {
        public static ToolStripMenuItem CreateMenuItem(string text, bool checkOnClick, EventHandler clickEventHandler, params ToolStripItem[] childsMenuItems)
        {
            ToolStripMenuItem menu = new ToolStripMenuItem();
            menu.Text = text;
            menu.CheckOnClick = checkOnClick;
            if (childsMenuItems.Length > 0) menu.DropDownItems.AddRange(childsMenuItems);
            if (clickEventHandler != null) menu.Click += clickEventHandler;
            return menu;
        }

        public static Button CreateButton(string text, int x, int y, int width, int height, EventHandler onClick = null)
        {
            Button button = new Button();
            button.Location = new Point(x, y);
            button.Size = new Size(width, height);
            button.Text = text;
            button.UseVisualStyleBackColor = true;
            if (onClick != null)
                button.Click += onClick;
            //button.Name = "";
            //button.TabIndex = 1;
            return button;
        }
    }
}
