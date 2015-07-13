import System;
import System.Reflection;

[assembly: AssemblyTitle("Test_JScript_01")]                       // ok
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Pierre")]
[assembly: AssemblyProduct("Test_JScript_01")]                     // ok
[assembly: AssemblyCopyright("Copyright © Pierre Beuzart 2013")]   // ok
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]                                    // bad
[assembly: AssemblyVersion("1.0.0.0")]                             // bad
[assembly: AssemblyFileVersion("1.0.0.0")]                         // ok


package Test_JScript_01 {
    class test {
        var text = "This is JScript";
        public static function test_01() {
            return "toto";
        }
    }
}
