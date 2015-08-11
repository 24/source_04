using System;
using System.Windows.Forms;

namespace pb.Windows.Forms
{
    public class zmenu
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
    }
}
