using System;
using System.Windows.Forms;

namespace pb.Windows.Forms
{
    public class ControlFindForm
    {
        private static bool __trace = false;
        private Action<Form> _result = null;
        private Control _control = null;

        public ControlFindForm(Control control, Action<Form> result)
        {
            _result = result;
            if (__trace)
                Trace.WriteLine("ControlFindForm find form for control {0}", control.GetType().Name);
            _Find(control);
        }

        private void _Find(Control control)
        {
            if (_control != null)
            {
                if (__trace)
                    Trace.WriteLine("ControlFindForm remove ParentChanged event for control {0}", _control.GetType().Name);
                _control.ParentChanged -= Control_ParentChanged;
                _control = null;
            }

            Form form = control.FindForm();
            if (form != null)
            {
                if (__trace)
                    Trace.WriteLine("ControlFindForm found form");
                _result(form);
            }
            else
            {
                while (true)
                {
                    Control parent = control.Parent;
                    if (parent == null)
                    {
                        if (__trace)
                            Trace.WriteLine("ControlFindForm add ParentChanged event for control {0}", control.GetType().Name);
                        control.ParentChanged += Control_ParentChanged;
                        _control = control;
                        break;
                    }
                    control = parent;
                }
            }
        }

        private void Control_ParentChanged(object sender, EventArgs e)
        {
            _Find(_control);
        }

        public static void Find(Control control, Action<Form> result)
        {
            new ControlFindForm(control, result);
        }
    }
}
