using System;
using System.Drawing;
using System.Windows.Forms;

namespace pb.Windows.Forms
{
    public class zForm
    {
        public static ToolStripMenuItem CreateMenuItem(string text, bool checkOnClick = false, bool @checked = false, EventHandler onClick = null)
        {
            ToolStripMenuItem menu = new ToolStripMenuItem();
            menu.Text = text;
            menu.CheckOnClick = checkOnClick;
            menu.Checked = @checked;
            if (onClick != null)
                menu.Click += onClick;
            return menu;
        }

        public static Button CreateButton(string text, string name = null, int? x = null, int? y = null, int? width = null, int? height = null, EventHandler onClick = null)
        {
            Button button = new Button();
            //button.Location = new Point(x, y);
            Point? point = GetPoint(x, y);
            if (point != null)
                button.Location = (Point)point;
            //button.Size = new Size(width, height);
            Size? size = GetSize(width, height);
            if (size != null)
                button.Size = (Size)size;
            button.Text = text;
            button.UseVisualStyleBackColor = true;
            if (onClick != null)
                button.Click += onClick;
            //button.Name = "";
            //button.TabIndex = 1;
            return button;
        }

        public static TextBox CreateTextBox(string name = null, DockStyle dockStyle = DockStyle.None, bool multiline = false, bool wordWrap = false, int? x = null, int? y = null, int? width = null, int? height = null)
        {
            TextBox textBox = new TextBox();
            textBox.Name = name;
            textBox.Dock = dockStyle;
            textBox.Font = new Font("Courier New", 9F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
            textBox.Multiline = multiline;
            if (multiline)
                textBox.ScrollBars = ScrollBars.Both;
            Point? point = GetPoint(x, y);
            if (point != null)
                textBox.Location = (Point)point;
            Size? size = GetSize(width, height);
            if (size != null)
                textBox.Size = (Size)size;
            textBox.WordWrap = wordWrap;
            return textBox;
        }

        public static Label CreateLabel(string text, string name = null, ContentAlignment align = ContentAlignment.MiddleCenter, bool autoSize = true, int? x = null, int? y = null, int? width = null, int? height = null)
        {
            Label label = new Label();
            label.Name = name;
            label.AutoSize = autoSize;
            label.TextAlign = align;
            Point? point = GetPoint(x, y);
            if (point != null)
                label.Location = (Point)point;
            Size? size = GetSize(width, height);
            if (size != null)
                label.Size = (Size)size;
            label.Text = text;
            return label;
        }

        public static ProgressBar CreateProgressBar(string name = null, int? x = null, int? y = null, int? width = null, int? height = null)
        {
            ProgressBar progressBar = new ProgressBar();
            Point? point = GetPoint(x, y);
            if (point != null)
                progressBar.Location = (Point)point;
            Size? size = GetSize(width, height);
            if (size != null)
                progressBar.Size = (Size)size;
            return progressBar;
        }

        public static Panel CreatePanel(string name = null, DockStyle dockStyle = DockStyle.None, Color? backColor = null, int? x = null, int? y = null, int? width = null, int? height = null)
        {
            Panel panel = new Panel();
            panel.SuspendLayout();
            panel.Name = name;
            panel.Dock = dockStyle;
            if (backColor != null)
                panel.BackColor = (Color)backColor;
            Point? point = GetPoint(x, y);
            if (point != null)
                panel.Location = (Point)point;
            Size? size = GetSize(width, height);
            if (size != null)
                panel.Size = (Size)size;
            panel.ResumeLayout(false);
            return panel;
        }

        public static Splitter CreateSplitter(DockStyle dockStyle, Color? backColor = null, int? width = null, int? height = null)
        {
            Splitter split = new Splitter();
            split.Dock = dockStyle;
            if (backColor != null)
                split.BackColor = (Color)backColor;

            int w = 0;
            if (width != null)
                w = (int)width;
            else if (dockStyle == DockStyle.Left || dockStyle == DockStyle.Right)
                w = 3;
            int h = 0;
            if (height != null)
                h = (int)height;
            else if (dockStyle == DockStyle.Top || dockStyle == DockStyle.Bottom)
                h = 3;
            split.Size = new Size(w, h);

            split.TabStop = false;
            return split;
        }

        public static ToolStripButton CreateToolStripButton(string text, EventHandler onClick = null)
        {
            ToolStripButton button = new ToolStripButton();
            button.Text = text;
            if (onClick != null)
                button.Click += onClick;
            return button;
        }

        public static ToolStripLabel CreateToolStripLabel(string text = null, ContentAlignment? textAlign = null, Color? backColor = null, int? width = null, int? height = null)
        {
            ToolStripLabel label = new ToolStripLabel();
            label.Text = text;
            if (backColor != null)
                label.BackColor = (Color)backColor;
            //Size? size = GetSize(width, height);
            //if (size != null)
            //{
            //    label.AutoSize = false;
            //    label.Size = (Size)size;
            //}
            label.Size = GetSize(width, height, label.Size);
            label.AutoSize = width == null && height == null;
            if (textAlign != null)
            label.TextAlign = (ContentAlignment)textAlign;


            //label.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
            //label.Margin
            //label.Padding
            //label.Overflow
            //label.Placement = ToolStripItemPlacement.Main;
            //label.Size
            //label.TextAlign = ContentAlignment.
            //label.ToolTipText
            return label;
        }

        public static ToolStripLabel CreateToolStripLabelSeparator(int width)
        {
            ToolStripLabel label = new ToolStripLabel();
            label.AutoSize = false;
            label.Size = new Size(width, 0);
            return label;
        }

        public static ToolStripTextBox CreateToolStripTextBox(string text = null, Color? backColor = null, int? width = null, int? height = null)
        {
            ToolStripTextBox textBox = new ToolStripTextBox();
            textBox.Text = text;
            if (backColor != null)
                textBox.BackColor = (Color)backColor;
            Size? size = GetSize(width, height);
            if (size != null)
                textBox.Size = (Size)size;
            return textBox;
        }

        public static DataGrid CreateDataGrid(string name = null, DockStyle dockStyle = DockStyle.None, int? x = null, int? y = null, int? width = null, int? height = null)
        {
            DataGrid grid = new DataGrid();
            ((System.ComponentModel.ISupportInitialize)grid).BeginInit();
            grid.Name = name;
            grid.Dock = dockStyle;
            //grid.DataMember = "";
            grid.Font = new Font("Courier New", 8.25F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
            grid.HeaderForeColor = SystemColors.ControlText;
            Point? point = GetPoint(x, y);
            if (point != null)
                grid.Location = (Point)point;
            Size? size = GetSize(width, height);
            if (size != null)
                grid.Size = (Size)size;
            //grid.TabIndex = 0;
            ((System.ComponentModel.ISupportInitialize)grid).EndInit();
            return grid;
        }

        public static Point? GetPoint(int? x, int? y)
        {
            if (x != null || y != null)
            {
                int x2 = 0;
                if (x != null)
                    x2 = (int)x;
                int y2 = 0;
                if (y != null)
                    y2 = (int)y;
                return new Point(x2, y2);
            }
            else
                return null;
        }

        public static Size? GetSize(int? width, int? height)
        {
            if (width != null || height != null)
            {
                int w = 0;
                if (width != null)
                    w = (int)width;
                int h = 0;
                if (height != null)
                    h = (int)height;
                return new Size(w, h);
            }
            else
                return null;
        }

        public static Size GetSize(int? width, int? height, Size size)
        {
            if (width == null && height == null)
                return size;
            int w = size.Width;
            if (width != null)
                w = (int)width;
            int h = size.Height;
            if (height != null)
                h = (int)height;
            return new Size(w, h);
        }
    }
}
