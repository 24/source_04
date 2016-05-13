using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace pb.Windows.Forms
{
    public class PanelTabControl : Panel
    {
        private int _selectedIndex = -1;
        private Control _selectedControl = null;
        private List<Control> _tabControls = new List<Control>();
        private Dictionary<Control, int> _tabControlsDictionary = new Dictionary<Control, int>();


        public PanelTabControl()
        {
            this.ControlAdded += PanelTabControl_ControlAdded;
            this.ControlRemoved += PanelTabControl_ControlRemoved;
        }

        public int SelectedIndex { get { return _selectedIndex; } set { SelectTab(value); } }
        public Control SelectedControl { get { return _selectedControl; } set { SelectTab(value); } }

        private void SelectTab(int index)
        {
            if (index == _selectedIndex)
                return;
            if (index >= _tabControls.Count)
                throw new PBException("index out of range");
            //if (_selectedIndex != -1)
            //    _childs[_selectedIndex].Visible = false;
            if (_selectedControl != null)
                _selectedControl.Visible = false;
            _selectedControl = _tabControls[index];
            _selectedIndex = index;
            //_childs[index].Visible = true;
            _selectedControl.Visible = true;
        }

        private void SelectTab(Control control)
        {
            if (!_tabControlsDictionary.ContainsKey(control))
                throw new PBException("unknow tab control");
            SelectTab(_tabControlsDictionary[control]);
        }

        private void PanelTabControl_ControlAdded(object sender, ControlEventArgs e)
        {
            Control control = e.Control;
            if (control.GetType() != typeof(Panel))
                throw new PBException("only Panel can be add to PanelTabControl");
            if (_tabControls.Count == 0)
            {
                _selectedIndex = 0;
                _selectedControl = control;
                control.Visible = true;
            }
            else if (_tabControls.Count > 0)
                control.Visible = false;
            _tabControlsDictionary.Add(control, _tabControls.Count);
            _tabControls.Add(control);
        }

        private void PanelTabControl_ControlRemoved(object sender, ControlEventArgs e)
        {
            throw new PBException("remove control is not possible");
        }

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
