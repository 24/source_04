Test_01("tata");
Test_02();
Test_Interface_01();

Test_Form_ViewOpenForms();
Form1_CreateForm();

Form2_CreateForm();
Form2_ChangeFormHeight();
Form2_ChangeTabControlHeight();

RunSourceForm_CreateForm();
RunSourceForm_ChangeTabControlHeight();

RunSourceForm2_CreateForm();
RunSourceForm2_ChangeTabControlHeight();

Trace.WriteLine("toto");
Trace.WriteLine("{0}", Application.OpenForms.Count);
Trace.WriteLine("{0}", Application.OpenForms[0].Text);
Trace.WriteLine("{0}", Application.OpenForms[1].Text);
Trace.WriteLine("{0}", Application.OpenForms[1].Name);
Trace.WriteLine("Width {0} Height {1}", Application.OpenForms[1].Size.Width, Application.OpenForms[1].Size.Height);
Trace.WriteLine("{0}", Application.OpenForms[1]);
Trace.WriteLine("{0}", Application.OpenForms[0].Controls.Count);
Trace.WriteLine("{0}", Application.OpenForms[0].Controls[0].Name);
Trace.WriteLine("{0}", Application.OpenForms[0].Controls["tc_result"].Name);
Trace.WriteLine("Width {0} Height {1}", Application.OpenForms[0].Controls["tc_result"].Size.Width, Application.OpenForms[0].Controls["tc_result"].Size.Height);
Application.OpenForms[0].Controls["tc_result"].Size = new Size(1056, 200);
Trace.WriteLine("Width {0} Height {1}", Application.OpenForms[0].Controls["pan_top"].Size.Width, Application.OpenForms[0].Controls["pan_top"].Size.Height);
Application.OpenForms[0].Controls["pan_top"].Size = new Size(1485, 400);
Trace.WriteLine("me_source {0}", Application.OpenForms[0].Controls["pan_top"].Controls["me_source"] == null ? "null" : Application.OpenForms[0].Controls["pan_top"].Controls["me_source"].ToString());
Trace.WriteLine("me_source.AutoScrollOffset X {0} Y {1}", Application.OpenForms[0].Controls["pan_top"].Controls["me_source"].AutoScrollOffset.X, Application.OpenForms[0].Controls["pan_top"].Controls["me_source"].AutoScrollOffset.Y);
Rectangle r = Application.OpenForms[0].Controls["pan_top"].Controls["me_source"].DisplayRectangle;
Trace.WriteLine("me_source.DisplayRectangle X {0} Y {1}", r.X, r.Y);
Trace.WriteLine("me_source.DisplayRectangle Location.X {0} Location.Y {1}", r.Location.X, r.Location.Y);
Trace.WriteLine("me_source.DisplayRectangle Width {0} Height {1}", r.Width, r.Height);
