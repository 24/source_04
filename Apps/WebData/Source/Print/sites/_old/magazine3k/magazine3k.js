﻿import System;
import System.Reflection;

[assembly: AssemblyTitle("magazine3k")]                       // ok
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Pierre Beuzart")]
[assembly: AssemblyProduct("magazine3k")]                     // ok
[assembly: AssemblyCopyright("Copyright © Pierre Beuzart 2013")]   // ok
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]                                    // bad
[assembly: AssemblyVersion("1.0.0.0")]                             // bad
[assembly: AssemblyFileVersion("1.0.0.0")]                         // ok

package Print.download {

    class test {
        var text = "This is JScript";
        public static function test_01() {
            return "toto";
        }
    }

    class Magazine3kEncrypt {
        static var initDone = false;
        static var SSS;
        static var SS7;

        public static function init() {
            if (initDone) return;
            SSS = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/".split("");
            SS7 = {};
            for(var i = 0; i < 64; i++) {
                SS7[SSS[i]] = i;
            }
            initDone = true;
        }

        public static function encrypt(SS) {
            init();
            var x, $, S$7, $SS, S5, S5$, e, S6;
            var S$ = S$S("e") + "b" + "o";
            x = "0938093093803356099";
            for($ = 0; $ < x.length; $++) {
                // JS1205 Assignment creates an expando property that is immediately thrown away
                //x[$] = S$6(x[$]);
                x[$] = x[$];
            }
            S$7 = S$ + S$$("o") + "k" + "3" + "0" + "." + "c";
            S$ = S$7 + S$S("o") + "m";
            x = "557da41f780498";
            S5 = donow(x);
            S6 = S5 + "26885fd6b";
            var S5$, $, start;
            S$ = S$5(S$);
            S6 = S$5(S6);
            if (S6.length == 0 || S$.length == 0 || SS.length == 0) {
                return SS;
            }
            SS = SS.match(/[^0-9A-Za-z+\/]+|[0-9A-Za-z+\/]+/g);
            start = /[0-9A-Za-z+\/]/.test(SS[0]) ? 0 : 1;
            for($ = start; $ < SS.length; $ +=2) {
                S5$ = SS[$].length - 1;
                SS[$] = S7$(SS[$].substring(0, S5$), S$, S6, SS7[SS[$].charAt(S5$)]);
            }
            return SS.join("");
        }

        static function S$S(SS) {
	        var txt = "", $$ = "";
	        $$ = SS.toUpperCase();
	        var S7 = SS.length;
	        var state = [1732584193, -271733879, -1732584194, 271733878];
	        var $;
	        return $$;
        }

        static function S$6(S7) {
            return S7;
        }

        static function S$$(S7) {
            var SS = "", S57 = 0, $$ = "", hex_chr;
            $$ = S7.toUpperCase();
            for(S57 = 0; S57 < 4; S57++) {
                SS +=1;
            }
            return $$;
        }

        static function donow(SS) {
            SS += "5bff2d318";
            return SS;
        }

        static function S$5(SS) {
            var $;
            SS = SS.replace(/[^0-9A-Za-z+\/]+/g,"");
            SS = SS.split("");
            for($ = 0; $ < SS.length; $++) {
                SS[$] = SS7[SS[$]];
            }
            return SS;
        }

        static function S7$(SS,S$,S6,S75) {
            var $, x, $$, S6S;
            SS = SS.split("");
            $ = null;
            S6S = donow($);
            $$ = SS;
            SS = SSS[1] + SSS[2] + SSS[3] + SSS[4] + donow(SSS[3]);
            for($ = 0; $ < SS.length; $++) {
                SS[$] = SS7[SS[$]];
            }
            S6S = SS + S$S("o") + "m";
            x = S$$(S$6(donow(SS) + "557da41f780498"));
            SS = $$;
            for($ = 0; $ < SS.length; $++) {
                SS[$] = SSS[SS7[SS[$]] ^ S$[$%S$.length] ^ S6[$%S6.length] ^ S75];
            }
            return SS.join("");
        }
    }
}
