using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace pb.Windows.Forms
{
    public class PanelTabElement
    {
        public int Index;
        public Panel Panel;
        public ToolStripItem Button;
    }

    public class PanelTabControl : Panel
    {
        //private int _selectedIndex = -1;
        //private Control _selectedControl = null;
        private PanelTabElement _selectedElement = null;
        //private List<Control> _tabControls = new List<Control>();
        private List<PanelTabElement> _tabControls = new List<PanelTabElement>();
        private Dictionary<Control, int> _tabControlsDictionary = new Dictionary<Control, int>();
        private Color _buttonSelectedColor = SystemColors.ControlLightLight;
        private Color _buttonUnselectedColor = SystemColors.ControlLight;

        public PanelTabControl()
        {
            //this.ControlAdded += PanelTabControl_ControlAdded;
            //this.ControlRemoved += PanelTabControl_ControlRemoved;
        }

        public int SelectedIndex { get { return _selectedElement != null ? _selectedElement.Index : -1; } set { SelectTab(value); } }
        public Control SelectedControl { get { return _selectedElement != null ? _selectedElement.Panel : null; } set { SelectTab(value); } }
        public Color ButtonSelectedColor { get { return _buttonSelectedColor; } set { _buttonSelectedColor = value; } }
        public Color ButtonUnselectedColor { get { return _buttonUnselectedColor; } set { _buttonUnselectedColor = value; } }

        private void SelectTab(int index)
        {
            //if (index == _selectedIndex)
            if (_selectedElement != null && _selectedElement.Index == index)
                return;
            if (index < 0 || index >= _tabControls.Count)
                throw new PBException("index out of range");
            ////if (_selectedControl != null)
            ////    _selectedControl.Visible = false;
            //if (_selectedElement != null)
            //    _selectedElement.Panel.Visible = false;
            ////_selectedControl = _tabControls[index];
            //_selectedElement = _tabControls[index];
            ////_selectedIndex = index;
            ////_selectedControl.Visible = true;
            //_selectedElement.Panel.Visible = true;
            SelectTab(_tabControls[index]);
        }

        private void SelectTab(PanelTabElement element)
        {
            if (_selectedElement != null)
            {
                _selectedElement.Panel.Visible = false;
                if (_selectedElement.Button != null)
                    _selectedElement.Button.BackColor = _buttonUnselectedColor;
            }
            _selectedElement = element;
            _selectedElement.Panel.Visible = true;
            if (_selectedElement.Button != null)
                _selectedElement.Button.BackColor = _buttonSelectedColor;
        }

        private void SelectTab(Control control)
        {
            if (!_tabControlsDictionary.ContainsKey(control))
                throw new PBException("unknow tab control");
            SelectTab(_tabControlsDictionary[control]);
        }

        public PanelTabElement CreateTabPanel(string buttonText, Color? backColor = null)
        {
            Panel panel = zForm.CreatePanel(dockStyle: DockStyle.Fill, backColor: backColor);
            ToolStripButton button = zForm.CreateToolStripButton(buttonText);
            PanelTabElement tabElement = AddTabPanel(panel, button);
            button.Tag = tabElement.Index;
            button.Click += (sender, eventArgs) => SelectTab((int)button.Tag);
            return tabElement;
        }

        public PanelTabElement AddTabPanel(Panel panel, ToolStripItem button = null)
        {
            //if (control.GetType() != typeof(Panel))
            //    throw new PBException("only Panel can be add to PanelTabControl");
            PanelTabElement tabElement = new PanelTabElement();
            tabElement.Index = _tabControls.Count;
            tabElement.Panel = panel;
            tabElement.Button = button;
            //if (_tabControls.Count == 0)
            if (_selectedElement == null)
            {
                //_selectedIndex = 0;
                //_selectedControl = control;
                _selectedElement = tabElement;
                panel.Visible = true;
                if (button != null)
                    button.BackColor = _buttonSelectedColor;
            }
            else // if (_tabControls.Count > 0)
            {
                panel.Visible = false;
                if (button != null)
                    button.BackColor = _buttonUnselectedColor;
            }
            _tabControlsDictionary.Add(panel, _tabControls.Count);
            //_tabControls.Add(control);
            _tabControls.Add(tabElement);
            this.Controls.Add(panel);
            return tabElement;
        }

        //private void PanelTabControl_ControlAdded(object sender, ControlEventArgs e)
        //{
        //    //Control control = e.Control;
        //    AddTab(e.Control);
        //}

        //private void PanelTabControl_ControlRemoved(object sender, ControlEventArgs e)
        //{
        //    throw new PBException("remove control is not possible");
        //}

        public static PanelTabControl Create(DockStyle dockStyle, Color? backColor = null, int? width = null, int? height = null)
        {
            PanelTabControl tabControl = new PanelTabControl();
            tabControl.SuspendLayout();
            tabControl.Dock = dockStyle;
            if (backColor != null)
                tabControl.BackColor = (Color)backColor;
            if (width != null || height != null)
            {
                int w = 0;
                if (width != null)
                    w = (int)width;
                int h = 0;
                if (height != null)
                    h = (int)height;
                tabControl.Size = new Size(w, h);
            }
            tabControl.ResumeLayout(false);
            return tabControl;
        }
    }
}
