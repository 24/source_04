import System;
import System.Reflection;

[assembly: AssemblyTitleAttribute("Test_JScript_01")]
[assembly: AssemblyDescriptionAttribute("")]
[assembly: AssemblyConfigurationAttribute("")]
[assembly: AssemblyCompanyAttribute("Pierre")]
[assembly: AssemblyProductAttribute("Test_JScript_01")]
[assembly: AssemblyCopyrightAttribute("Copyright © Pierre Beuzart 2013")]
[assembly: AssemblyTrademarkAttribute("")]
[assembly: AssemblyCultureAttribute("")]

[assembly: AssemblyVersionAttribute("1.0.0.0")]
[assembly: AssemblyFileVersionAttribute("1.0.0.0")]


package Test_JScript_01 {
    class test {
        var text = "This is JScript";
        public static function test_01() {
            return "toto";
        }
    }
}
