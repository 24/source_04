(function(){ var g=void 0,h=!0,i=null,l=!1,m=Error,aa=parseInt,n=String,p=Math,q=Array,r="push",t="getMonth",v="length",x="propertyIsEnumerable",y="prototype",z="replace",ba="split",A="floor",B="charAt",C="indexOf",D="call",F="fromCharCode",ca="charCodeAt",G="type",H="name",I="getHours",J="join",K="getTimezoneOffset",L=this,da=function(a,c,b){a=a[ba](".");b=b||L;!(a[0]in b)&&b.execScript&&b.execScript("var "+a[0]);for(var d;a[v]&&(d=a.shift());)!a[v]&&c!==g?b[d]=c:b=b[d]?b[d]:b[d]={}},ea=function(a){var c=typeof a;
if("object"==c)if(a){if(a instanceof q)return"array";if(a instanceof Object)return c;var b=Object[y].toString[D](a);if("[object Window]"==b)return"object";if("[object Array]"==b||"number"==typeof a[v]&&"undefined"!=typeof a.splice&&"undefined"!=typeof a[x]&&!a[x]("splice"))return"array";if("[object Function]"==b||"undefined"!=typeof a[D]&&"undefined"!=typeof a[x]&&!a[x]("call"))return"function"}else return"null";else if("function"==c&&"undefined"==typeof a[D])return"object";return c},fa=function(a){var c=
ea(a);return"array"==c||"object"==c&&"number"==typeof a[v]},N=function(a){return"string"==typeof a},ga=function(a){var c=typeof a;return"object"==c&&a!=i||"function"==c};p[A](2147483648*p.random()).toString(36);var O=function(a,c,b){da(a,c,b)},ha=function(a,c){function b(){}b.prototype=c[y];a.Ca=c[y];a.prototype=new b;a[y].constructor=a};var P=function(a){m.captureStackTrace?m.captureStackTrace(this,P):this.stack=m().stack||"";a&&(this.message=n(a))};ha(P,m);P[y].name="CustomError";var ia=function(a,c){for(var b=1;b<arguments[v];b++){var d=n(arguments[b])[z](/\$/g,"$$$$");a=a[z](/\%s/,d)}return a},oa=function(a,c){if(c)return a[z](ja,"&amp;")[z](ka,"&lt;")[z](la,"&gt;")[z](ma,"&quot;");if(!na.test(a))return a;-1!=a[C]("&")&&(a=a[z](ja,"&amp;"));-1!=a[C]("<")&&(a=a[z](ka,"&lt;"));-1!=a[C](">")&&(a=a[z](la,"&gt;"));-1!=a[C]('"')&&(a=a[z](ma,"&quot;"));return a},ja=/&/g,ka=/</g,la=/>/g,ma=/\"/g,na=/[&<>\"]/,pa=function(a,c){return q(c+1)[J](a)},Q=function(a,c,b){a=b!==g?a.toFixed(b):
n(a);b=a[C](".");-1==b&&(b=a[v]);return pa("0",p.max(0,c-b))+a},qa=function(a,c){for(var b=0,d=n(a)[z](/^[\s\xa0]+|[\s\xa0]+$/g,"")[ba]("."),f=n(c)[z](/^[\s\xa0]+|[\s\xa0]+$/g,"")[ba]("."),e=p.max(d[v],f[v]),j=0;0==b&&j<e;j++){var k=d[j]||"",s=f[j]||"",u=RegExp("(\\d*)(\\D*)","g"),w=RegExp("(\\d*)(\\D*)","g");do{var E=u.exec(k)||["","",""],M=w.exec(s)||["","",""];if(0==E[0][v]&&0==M[0][v])break;var b=0==E[1][v]?0:aa(E[1],10),ya=0==M[1][v]?0:aa(M[1],10),b=(b<ya?-1:b>ya?1:0)||((0==E[2][v])<(0==M[2][v])?
-1:(0==E[2][v])>(0==M[2][v])?1:0)||(E[2]<M[2]?-1:E[2]>M[2]?1:0)}while(0==b)}return b};p.random();var ra=function(a,c){c.unshift(a);P[D](this,ia.apply(i,c));c.shift();this.messagePattern=a};ha(ra,P);ra[y].name="AssertionError";var sa=function(a,c,b,d){var f="Assertion failed";if(b)var f=f+(": "+b),e=d;else a&&(f+=": "+a,e=c);throw new ra(""+f,e||[]);},ta=function(a,c,b){a||sa("",i,c,q[y].slice[D](arguments,2));return a};var R,ua={ERAS:["av. J.-C.","ap. J.-C."],ERANAMES:["avant J\u00e9sus-Christ","apr\u00e8s J\u00e9sus-Christ"],NARROWMONTHS:"JFMAMJJASOND".split(""),STANDALONENARROWMONTHS:"JFMAMJJASOND".split(""),MONTHS:"janvier f\u00e9vrier mars avril mai juin juillet ao\u00fbt septembre octobre novembre d\u00e9cembre".split(" "),STANDALONEMONTHS:"janvier f\u00e9vrier mars avril mai juin juillet ao\u00fbt septembre octobre novembre d\u00e9cembre".split(" "),SHORTMONTHS:"janv. f\u00e9vr. mars avr. mai juin juil. ao\u00fbt sept. oct. nov. d\u00e9c.".split(" "),
STANDALONESHORTMONTHS:"janv. f\u00e9vr. mars avr. mai juin juil. ao\u00fbt sept. oct. nov. d\u00e9c.".split(" "),WEEKDAYS:"dimanche lundi mardi mercredi jeudi vendredi samedi".split(" "),STANDALONEWEEKDAYS:"dimanche lundi mardi mercredi jeudi vendredi samedi".split(" "),SHORTWEEKDAYS:"dim. lun. mar. mer. jeu. ven. sam.".split(" "),STANDALONESHORTWEEKDAYS:"dim. lun. mar. mer. jeu. ven. sam.".split(" "),NARROWWEEKDAYS:"DLMMJVS".split(""),STANDALONENARROWWEEKDAYS:"DLMMJVS".split(""),SHORTQUARTERS:["T1",
"T2","T3","T4"],QUARTERS:["1er trimestre","2e trimestre","3e trimestre","4e trimestre"],AMPMS:["AM","PM"],DATEFORMATS:["EEEE d MMMM y","d MMMM y","d MMM y","dd/MM/yy"],TIMEFORMATS:["HH:mm:ss zzzz","HH:mm:ss z","HH:mm:ss","HH:mm"],FIRSTDAYOFWEEK:0,WEEKENDRANGE:[5,6],FIRSTWEEKCUTOFFDAY:3};R=ua;var va=function(a,c){var b=a+(" "+c);return b};var S=q[y],wa=S[C]?function(a,c,b){ta(a[v]!=i);return S[C][D](a,c,b)}:function(a,c,b){b=b==i?0:0>b?p.max(0,a[v]+b):b;if(N(a))return!N(c)||1!=c[v]?-1:a[C](c,b);for(;b<a[v];b++)if(b in a&&a[b]===c)return b;return-1},xa=S.forEach?function(a,c,b){ta(a[v]!=i);S.forEach[D](a,c,b)}:function(a,c,b){for(var d=a[v],f=N(a)?a[ba](""):a,e=0;e<d;e++)e in f&&c[D](b,f[e],e,a)},za=function(a){var c=a[v];if(0<c){for(var b=q(c),d=0;d<c;d++)b[d]=a[d];return b}return[]},Aa=function(a,c,b){ta(a[v]!=i);return 2>=arguments[v]?
S.slice[D](a,c):S.slice[D](a,c,b)};var Ba=function(a,c,b){for(var d in a)c[D](b,a[d],d,a)},Ca="constructor hasOwnProperty isPrototypeOf propertyIsEnumerable toLocaleString toString valueOf".split(" "),Da=function(a,c){for(var b,d,f=1;f<arguments[v];f++){d=arguments[f];for(b in d)a[b]=d[b];for(var e=0;e<Ca[v];e++)b=Ca[e],Object[y].hasOwnProperty[D](d,b)&&(a[b]=d[b])}};var T,Ea,Fa,Ga,Ha=function(){return L.navigator?L.navigator.userAgent:i},Ia=function(){return L.navigator},Ja=function(){Ga=Fa=Ea=T=l;var a;if(a=Ha()){var c=Ia();T=0==a[C]("Opera");Ea=!T&&-1!=a[C]("MSIE");(Fa=!T&&-1!=a[C]("WebKit"))&&a[C]("Mobile");Ga=!T&&!Fa&&"Gecko"==c.product}};Ja();var Ka=T,U=Ea,La=Ga,Ma=Fa,Na=function(){var a=Ia();return a&&a.platform||""},Oa=Na(),Pa=function(){Oa[C]("Mac");Oa[C]("Win");Oa[C]("Linux");Ia()&&(Ia().appVersion||"")[C]("X11")};Pa();
var Ra=function(){var a="",c;Ka&&L.opera?(a=L.opera.version,a="function"==typeof a?a():a):(La?c=/rv\:([^\);]+)(\)|;)/:U?c=/MSIE\s+([^\);]+)(\)|;)/:Ma&&(c=/WebKit\/(\S+)/),c&&(a=(a=c.exec(Ha()))?a[1]:""));return U&&(c=Qa(),c>parseFloat(a))?n(c):a},Qa=function(){var a=L.document;return a?a.documentMode:g},Sa=Ra(),Ta={},Ua=function(){var a=L.document;if(a&&U){var c=Qa();return c||("CSS1Compat"==a.compatMode?aa(Sa,10):5)}}();var Va=!U||U&&9<=Ua;!La&&!U||U&&U&&9<=Ua||La&&(Ta["1.9.1"]||(Ta["1.9.1"]=0<=qa(Sa,"1.9.1")));U&&(Ta["9"]||(Ta["9"]=0<=qa(Sa,"9")));var Wa=function(a,c){a.className=c},Xa=function(a){a=a.className;return N(a)&&a.match(/\S+/g)||[]},Za=function(a,c){var b=Xa(a),d=Aa(arguments,1),f=b[v]+d[v];Ya(b,d);Wa(a,b[J](" "));return b[v]==f},Ya=function(a,c){for(var b=0;b<c[v];b++)0<=wa(a,c[b])||a[r](c[b])};var ab=function(a,c){Ba(c,function(b,c){"style"==c?a.style.cssText=b:"class"==c?a.className=b:"for"==c?a.htmlFor=b:c in $a?a.setAttribute($a[c],b):0==c.lastIndexOf("aria-",0)||0==c.lastIndexOf("data-",0)?a.setAttribute(c,b):a[c]=b})},$a={cellpadding:"cellPadding",cellspacing:"cellSpacing",colspan:"colSpan",frameborder:"frameBorder",height:"height",maxlength:"maxLength",role:"role",rowspan:"rowSpan",type:"type",usemap:"useMap",valign:"vAlign",width:"width"},cb=function(a,c,b){return bb(document,arguments)},
bb=function(a,c){var b=c[0],d=c[1];if(!Va&&d&&(d[H]||d[G])){b=["<",b];d[H]&&b[r](' name="',oa(d[H]),'"');if(d[G]){b[r](' type="',oa(d[G]),'"');var f={};Da(f,d);delete f[G];d=f}b[r](">");b=b[J]("")}b=a.createElement(b);d&&(N(d)?b.className=d:"array"==ea(d)?Za.apply(i,[b].concat(d)):ab(b,d));2<c[v]&&db(a,b,c,2);return b},db=function(a,c,b,d){function f(b){b&&c.appendChild(N(b)?a.createTextNode(b):b)}for(;d<b[v];d++){var e=b[d];fa(e)&&!(ga(e)&&0<e.nodeType)?xa(eb(e)?za(e):e,f):f(e)}},eb=function(a){if(a&&
"number"==typeof a[v]){if(ga(a))return"function"==typeof a.item||"string"==typeof a.item;if("function"==ea(a))return"function"==typeof a.item}return l};var V=function(){},gb=function(a){if("number"==typeof a)return fb(a);var c=new V;c.xa=a.id;c.ua=-a.std_offset;c.C=a.names;c.s=a.transitions;return c},fb=function(a){var c=new V;c.ua=a;c.xa=hb(a);a=ib(a);c.C=[a,a];c.s=[];return c},jb=function(a){var c=["GMT"];c[r](0>=a?"+":"-");a=p.abs(a);c[r](Q(p[A](a/60)%100,2),":",Q(a%60,2));return c[J]("")},hb=function(a){if(0==a)return"Etc/GMT";var c=["Etc/GMT",0>a?"-":"+"];a=p.abs(a);c[r](p[A](a/60)%100);a%=60;0!=a&&c[r](":",Q(a,2));return c[J]("")},ib=function(a){if(0==
a)return"UTC";var c=["UTC",0>a?"+":"-"];a=p.abs(a);c[r](p[A](a/60)%100);a%=60;0!=a&&c[r](":",a);return c[J]("")};V[y].v=function(a){a=Date.UTC(a.getUTCFullYear(),a.getUTCMonth(),a.getUTCDate(),a.getUTCHours(),a.getUTCMinutes());a/=36E5;for(var c=0;c<this.s[v]&&a>=this.s[c];)c+=2;return 0==c?0:this.s[c-1]};V[y].Z=function(a){return jb(this.n(a))};V[y].$=function(a){return this.C[this.A(a)?3:1]};V[y].n=function(a){return this.ua-this.v(a)};
V[y].ea=function(a){a=-this.n(a);var c=[0>a?"-":"+"];a=p.abs(a);c[r](Q(p[A](a/60)%100,2),Q(a%60,2));return c[J]("")};V[y].ma=function(a){return this.C[this.A(a)?2:0]};V[y].oa=function(){return this.xa};V[y].A=function(a){return 0<this.v(a)};var W=function(a){ta(a!==g,"Pattern must be defined");this.q=[];"number"==typeof a?this.f(a):this.c(a)},kb=[/^\'(?:[^\']|\'\')*\'/,/^(?:G+|y+|M+|k+|S+|E+|a+|h+|K+|H+|c+|L+|Q+|d+|m+|s+|v+|z+|Z+)/,/^[^\'GyMkSEahKHcLQdmsvzZ]+/];W[y].c=function(a){for(;a;)for(var c=0;c<kb[v];++c){var b=a.match(kb[c]);if(b){b=b[0];a=a.substring(b[v]);0==c&&("''"==b?b="'":(b=b.substring(1,b[v]-1),b=b[z](/\'\'/,"'")));this.q[r]({text:b,type:c});break}}};
W[y].h=function(a,c){var b=c?6E4*(a[K]()-c.n(a)):0,d=b?new Date(a.getTime()+b):a,f=d;c&&d[K]()!=a[K]()&&(b+=0<b?-864E5:864E5,f=new Date(a.getTime()+b));for(var b=[],e=0;e<this.q[v];++e){var j=this.q[e].text;1==this.q[e][G]?b[r](this.N(j,a,d,f,c)):b[r](j)}return b[J]("")};W[y].f=function(a){if(4>a)a=R.DATEFORMATS[a];else if(8>a)a=R.TIMEFORMATS[a-4];else if(12>a)a=R.DATEFORMATS[a-8]+" "+R.TIMEFORMATS[a-8];else{this.f(10);return}this.c(a)};
W[y].a=function(a){if(R.ZERODIGIT===g)return a;for(var c=[],b=0;b<a[v];b++){var d=a[ca](b);c[r](48<=d&&57>=d?n[F](R.ZERODIGIT+d-48):a[B](b))}return c[J]("")};W[y].M=function(a,c){var b=0<c.getFullYear()?1:0;return 4<=a?R.ERANAMES[b]:R.ERAS[b]};W[y].Y=function(a,c){var b=c.getFullYear();0>b&&(b=-b);return this.a(2==a?Q(b%100,2):n(b))};W[y].Q=function(a,c){var b=c[t]();switch(a){case 5:return R.NARROWMONTHS[b];case 4:return R.MONTHS[b];case 3:return R.SHORTMONTHS[b];default:return this.a(Q(b+1,a))}};
W[y].I=function(a,c){return this.a(Q(c[I]()||24,a))};W[y].O=function(a,c){var b=c.getTime()%1E3/1E3;return this.a(b.toFixed(p.min(3,a)).substr(2)+(3<a?Q(0,a-3):""))};W[y].L=function(a,c){var b=c.getDay();return 4<=a?R.WEEKDAYS[b]:R.SHORTWEEKDAYS[b]};W[y].J=function(a,c){var b=c[I]();return R.AMPMS[12<=b&&24>b?1:0]};W[y].H=function(a,c){return this.a(Q(c[I]()%12||12,a))};W[y].F=function(a,c){return this.a(Q(c[I]()%12,a))};W[y].G=function(a,c){return this.a(Q(c[I](),a))};
W[y].T=function(a,c){var b=c.getDay();switch(a){case 5:return R.STANDALONENARROWWEEKDAYS[b];case 4:return R.STANDALONEWEEKDAYS[b];case 3:return R.STANDALONESHORTWEEKDAYS[b];default:return this.a(Q(b,1))}};W[y].U=function(a,c){var b=c[t]();switch(a){case 5:return R.STANDALONENARROWMONTHS[b];case 4:return R.STANDALONEMONTHS[b];case 3:return R.STANDALONESHORTMONTHS[b];default:return this.a(Q(b+1,a))}};W[y].R=function(a,c){var b=p[A](c[t]()/3);return 4>a?R.SHORTQUARTERS[b]:R.QUARTERS[b]};
W[y].K=function(a,c){return this.a(Q(c.getDate(),a))};W[y].P=function(a,c){return this.a(Q(c.getMinutes(),a))};W[y].S=function(a,c){return this.a(Q(c.getSeconds(),a))};W[y].W=function(a,c,b){b=b||gb(c[K]());return 4>a?b.ea(c):this.a(b.Z(c))};W[y].X=function(a,c,b){b=b||gb(c[K]());return 4>a?b.ma(c):b.$(c)};W[y].V=function(a,c){c=c||gb(a[K]());return c.oa()};
W[y].N=function(a,c,b,d,f){var e=a[v];switch(a[B](0)){case "G":return this.M(e,b);case "y":return this.Y(e,b);case "M":return this.Q(e,b);case "k":return this.I(e,d);case "S":return this.O(e,d);case "E":return this.L(e,b);case "a":return this.J(e,d);case "h":return this.H(e,d);case "K":return this.F(e,d);case "H":return this.G(e,d);case "c":return this.T(e,b);case "L":return this.U(e,b);case "Q":return this.R(e,b);case "d":return this.K(e,b);case "m":return this.P(e,d);case "s":return this.S(e,d);
case "v":return this.V(c,f);case "z":return this.X(e,c,f);case "Z":return this.W(e,c,f);default:return""}};var mb=function(a){var c=lb[a];return a==c[1]?a:a+" "+c[1]},nb=function(a,c){var b=["0"],d=lb[c],d=d[0]&7;if(0<d){b[r](".");for(var f=0;f<d;f++)b[r]("0")}return a[z](/0.00/g,b[J](""))},lb={AED:[2,"dh","\u062f.\u0625.","DH"],AUD:[2,"$","AU$"],BDT:[2,"\u09f3","Tk"],BRL:[2,"R$","R$"],CAD:[2,"$","C$"],CHF:[2,"CHF","CHF"],CLP:[0,"$","CL$"],CNY:[2,"\u00a5","RMB\u00a5"],COP:[0,"$","COL$"],CRC:[0,"\u20a1","CR\u20a1"],CZK:[2,"K\u010d","K\u010d"],DKK:[18,"kr","kr"],DOP:[2,"$","RD$"],EGP:[2,"\u00a3","LE"],EUR:[18,
"\u20ac","\u20ac"],GBP:[2,"\u00a3","GB\u00a3"],HKD:[2,"$","HK$"],ILS:[2,"\u20aa","IL\u20aa"],INR:[2,"\u20b9","Rs"],ISK:[0,"kr","kr"],JMD:[2,"$","JA$"],JPY:[0,"\u00a5","JP\u00a5"],KRW:[0,"\u20a9","KR\u20a9"],LKR:[2,"Rs","SLRs"],MNT:[0,"\u20ae","MN\u20ae"],MXN:[2,"$","Mex$"],MYR:[2,"RM","RM"],NOK:[18,"kr","NOkr"],PAB:[2,"B/.","B/."],PEN:[2,"S/.","S/."],PHP:[2,"\u20b1","Php"],PKR:[0,"Rs","PKRs."],RUB:[42,"\u0440\u0443\u0431.","\u0440\u0443\u0431."],SAR:[2,"Rial","Rial"],SEK:[2,"kr","kr"],SGD:[2,"$",
"S$"],THB:[2,"\u0e3f","THB"],TRY:[2,"TL","YTL"],TWD:[2,"NT$","NT$"],USD:[2,"$","US$"],UYU:[2,"$","UY$"],VND:[0,"\u20ab","VN\u20ab"],YER:[0,"Rial","Rial"],ZAR:[2,"R","ZAR"]};var ob={DECIMAL_SEP:".",GROUP_SEP:",",PERCENT:"%",ZERO_DIGIT:"0",PLUS_SIGN:"+",MINUS_SIGN:"-",EXP_SYMBOL:"E",PERMILL:"\u2030",INFINITY:"\u221e",NAN:"NaN",DECIMAL_PATTERN:"#,##0.###",SCIENTIFIC_PATTERN:"#E0",PERCENT_PATTERN:"#,##0%",CURRENCY_PATTERN:"\u00a4#,##0.00;(\u00a4#,##0.00)",DEF_CURRENCY_CODE:"USD"},pb={DECIMAL_SEP:",",GROUP_SEP:"\u00a0",PERCENT:"%",ZERO_DIGIT:"0",PLUS_SIGN:"+",MINUS_SIGN:"-",EXP_SYMBOL:"E",PERMILL:"\u2030",INFINITY:"\u221e",NAN:"NaN",DECIMAL_PATTERN:"#,##0.###",SCIENTIFIC_PATTERN:"#E0",
PERCENT_PATTERN:"#,##0\u00a0%",CURRENCY_PATTERN:"#,##0.00\u00a0\u00a4;(#,##0.00\u00a0\u00a4)",DEF_CURRENCY_CODE:"EUR"},X=ob,X=pb;var Y=function(a,c,b){this.i=c||X.DEF_CURRENCY_CODE;this.Aa=b||0;this.o=40;this.b=1;this.B=3;this.p=this.j=0;this.ya=l;this.m=this.e="";this.d="-";this.g="";this.k=1;this.z=3;this.t=this.D=l;"number"==typeof a?this.f(a):this.c(a)},qb=l;Y[y].c=function(a){this.Ba=a[z](/ /g,"\u00a0");var c=[0];this.e=this.l(a,c);var b=c[0];this.ra(a,c);b=c[0]-b;this.m=this.l(a,c);c[0]<a[v]&&";"==a[B](c[0])?(c[0]++,this.d=this.l(a,c),c[0]+=b,this.g=this.l(a,c)):(this.d=this.e+this.d,this.g+=this.m)};
Y[y].f=function(a){switch(a){case 1:this.c(X.DECIMAL_PATTERN);break;case 2:this.c(X.SCIENTIFIC_PATTERN);break;case 3:this.c(X.PERCENT_PATTERN);break;case 4:this.c(nb(X.CURRENCY_PATTERN,this.i));break;default:throw m("Unsupported pattern type.");}};
Y[y].parse=function(a,c){var b=c||[0],d=NaN;a=a[z](/ /g,"\u00a0");var f=a[C](this.e,b[0])==b[0],e=a[C](this.d,b[0])==b[0];f&&e&&(this.e[v]>this.d[v]?e=l:this.e[v]<this.d[v]&&(f=l));f?b[0]+=this.e[v]:e&&(b[0]+=this.d[v]);a[C](X.INFINITY,b[0])==b[0]?(b[0]+=X.INFINITY[v],d=Infinity):d=this.qa(a,b);if(f){if(a[C](this.m,b[0])!=b[0])return NaN;b[0]+=this.m[v]}else if(e){if(a[C](this.g,b[0])!=b[0])return NaN;b[0]+=this.g[v]}return e?-d:d};
Y[y].qa=function(a,c){for(var b=l,d=l,f=l,e=1,j=X.DECIMAL_SEP,k=X.GROUP_SEP,s=X.EXP_SYMBOL,u="";c[0]<a[v];c[0]++){var w=a[B](c[0]),E=this.w(w);if(0<=E&&9>=E)u+=E,f=h;else if(w==j[B](0)){if(b||d)break;u+=".";b=h}else if(w==k[B](0)&&("\u00a0"!=k[B](0)||c[0]+1<a[v]&&0<=this.w(a[B](c[0]+1)))){if(b||d)break}else if(w==s[B](0)){if(d)break;u+="E";d=h}else if("+"==w||"-"==w)u+=w;else if(w==X.PERCENT[B](0)){if(1!=e)break;e=100;if(f){c[0]++;break}}else if(w==X.PERMILL[B](0)){if(1!=e)break;e=1E3;if(f){c[0]++;
break}}else break}return parseFloat(u)/e};Y[y].h=function(a){if(isNaN(a))return X.NAN;var c=[],b=0>a||0==a&&0>1/a;c[r](b?this.d:this.e);if(isFinite(a))a*=b?-1:1,a*=this.k,this.t?this.va(a,c):this.r(a,this.b,c);else c[r](X.INFINITY);c[r](b?this.g:this.m);return c[J]("")};
Y[y].r=function(a,c,b){var d=p.pow(10,this.B),f=p.round(a*d),e;isFinite(f)?(a=p[A](f/d),e=p[A](f-a*d)):e=0;for(var j=0<this.j||0<e,k="",f=a;1E20<f;)k="0"+k,f=p.round(f/10);var k=f+k,s=X.DECIMAL_SEP,u=X.GROUP_SEP,f=qb?48:X.ZERO_DIGIT[ca](0),w=k[v];if(0<a||0<c){for(a=w;a<c;a++)b[r](n[F](f));for(a=0;a<w;a++)b[r](n[F](f+1*k[B](a))),1<w-a&&(0<this.z&&1==(w-a)%this.z)&&b[r](u)}else j||b[r](n[F](f));(this.D||j)&&b[r](s);c=""+(e+d);for(d=c[v];"0"==c[B](d-1)&&d>this.j+1;)d--;for(a=1;a<d;a++)b[r](n[F](f+1*
c[B](a)))};Y[y].u=function(a,c){c[r](X.EXP_SYMBOL);0>a?(a=-a,c[r](X.MINUS_SIGN)):this.ya&&c[r](X.PLUS_SIGN);for(var b=""+a,d=qb?"0":X.ZERO_DIGIT,f=b[v];f<this.p;f++)c[r](d);c[r](b)};Y[y].va=function(a,c){if(0==a)this.r(a,this.b,c),this.u(0,c);else{var b=p[A](p.log(a)/p.log(10));a/=p.pow(10,b);var d=this.b;if(1<this.o&&this.o>this.b){for(;0!=b%this.o;)a*=10,b--;d=1}else 1>this.b?(b++,a/=10):(b-=this.b-1,a*=p.pow(10,this.b-1));this.r(a,d,c);this.u(b,c)}};
Y[y].w=function(a){a=a[ca](0);if(48<=a&&58>a)return a-48;var c=X.ZERO_DIGIT[ca](0);return c<=a&&a<c+10?a-c:-1};
Y[y].l=function(a,c){for(var b="",d=l,f=a[v];c[0]<f;c[0]++){var e=a[B](c[0]);if("'"==e)c[0]+1<f&&"'"==a[B](c[0]+1)?(c[0]++,b+="'"):d=!d;else if(d)b+=e;else switch(e){case "#":case "0":case ",":case ".":case ";":return b;case "\u00a4":if(c[0]+1<f&&"\u00a4"==a[B](c[0]+1))c[0]++,b+=this.i;else switch(this.Aa){case 0:b+=lb[this.i][1];break;case 2:b+=mb(this.i);break;case 1:b+=lb[this.i][2]}break;case "%":if(1!=this.k)throw m("Too many percent/permill");this.k=100;b+=X.PERCENT;break;case "\u2030":if(1!=
this.k)throw m("Too many percent/permill");this.k=1E3;b+=X.PERMILL;break;default:b+=e}}return b};
Y[y].ra=function(a,c){for(var b=-1,d=0,f=0,e=0,j=-1,k=a[v],s=h;c[0]<k&&s;c[0]++){var u=a[B](c[0]);switch(u){case "#":0<f?e++:d++;0<=j&&0>b&&j++;break;case "0":if(0<e)throw m('Unexpected "0" in pattern "'+a+'"');f++;0<=j&&0>b&&j++;break;case ",":j=0;break;case ".":if(0<=b)throw m('Multiple decimal separators in pattern "'+a+'"');b=d+f+e;break;case "E":if(this.t)throw m('Multiple exponential symbols in pattern "'+a+'"');this.t=h;this.p=0;c[0]+1<k&&"+"==a[B](c[0]+1)&&(c[0]++,this.ya=h);for(;c[0]+1<k&&
"0"==a[B](c[0]+1);)c[0]++,this.p++;if(1>d+f||1>this.p)throw m('Malformed exponential pattern "'+a+'"');s=l;break;default:c[0]--,s=l}}0==f&&(0<d&&0<=b)&&(f=b,0==f&&f++,e=d-f,d=f-1,f=1);if(0>b&&0<e||0<=b&&(b<d||b>d+f)||0==j)throw m('Malformed pattern "'+a+'"');e=d+f+e;this.B=0<=b?e-b:0;0<=b&&(this.j=d+f-b,0>this.j&&(this.j=0));f=0<=b?b:e;this.b=f-d;this.t&&(this.o=d+this.b,0==this.B&&0==this.b&&(this.b=1));this.z=p.max(0,j);this.D=0==b||b==e};var Z,rb="AD AE AF AG AI AL AM AN AO AQ AR AS AT AU AW AX AZ BA BB BD BE BF BG BH BI BJ BL BM BN BO BR BS BT BV BW BY BZ CA CC CD CF CG CH CI CK CL CM CN CO CR CU CV CX CY CZ DE DJ DK DM DO DZ EC EE EG EH ER ES ET FI FJ FK FM FO FR GA GB GD GE GF GG GH GI GL GM GN GP GQ GR GS GT GU GW GY HK HM HN HR HT HU ID IE IL IM IN IO IQ IR IS IT JE JM JO JP KE KG KH KI KM KN KP KR KW KY KZ LA LB LC LI LK LR LS LT LU LV LY MA MC MD ME MF MG MH MK ML MM MN MO MP MQ MR MS MT MU MV MW MX MY MZ NA NC NE NF NG NI NL NO NP NR NU NZ OM PA PE PF PG PH PK PL PM PN PR PS PT PW PY QA RE RO RS RU RW SA SB SC SD SE SG SH SI SJ SK SL SM SN SO SR ST SV SY SZ TC TD TF TG TH TJ TK TL TM TN TO TR TT TV TW TZ UA UG UM US UY UZ VA VC VE VG VI VN VU WF WS YE YT ZA ZM ZW".split(" ");var sb=function(){Z||(Z="en");return Z},tb=function(a){return(a=a.match(/^\w{2,3}([-_]|$)/))?a[0][z](/[_-]/g,""):""},vb=function(a,c){c||(c=ub("LocaleNameConstants",sb()));if(a in c.LANGUAGE)return c.LANGUAGE[a];var b=tb(a);return b in c.LANGUAGE?c.LANGUAGE[b]:a},wb=function(a,c,b){$[c]||($[c]={});$[c][b]=a;Z||(Z=b)},$={},xb=function(a,c){wb(a,"LocaleNameConstants",c)},ub=function(a,c){var b=c?c:sb();return!(a in $)?g:$[a][b]},yb=xb;var zb="ar bg ca cs da de el en es et fi fil fr he hi hr hu id it ja ko lt lv nl no pl pt ro ru sk sl sr sv th tr uk vi zh_Hans zh_Hant".split(" "),Ab={et:h};
function Bb(a,c,b,d){for(var f=[],e=ub("LocaleNameConstants"),j=0;j<zb[v];j++){var k=zb[j];if(d||!Ab[k]){var s;s=k in e.LANGUAGE?e.LANGUAGE[k]:vb(k,i);var k=k[z]("_","-","g"),u={value:k,name:s};c&&k[v]<=c[v]&&c.substr(0,k[v])==k&&(u.selected=h);s=cb("option",u,s);b&&s.setAttribute("jsvalues",".defaultSelected:this.value == cse.language");f[r](s)}}f.sort(function(a,b){return a[H]<b[H]?-1:1});for(j=0;j<f[v];j++)a.appendChild(f[j])}
function Cb(a,c){for(var b=[],d=rb,f=ub("LocaleNameConstants"),e=0;e<d[v];e++){var j=d[e],k=f.COUNTRY[j];if(k!=i){var s={value:j,name:k};c==j&&(s.defaultSelected=h,s.selected=h);b[r](cb("option",s,k))}}b.sort(function(a,b){return a[H]<b[H]?-1:1});for(e=0;e<b[v];e++)a.appendChild(b[e])}function Db(a){return va(R.MONTHS[a[t]()],a.getFullYear())}function Eb(a,c){var b=new W(a);return b.h(c)}function Fb(a,c){var b=new Y(a);return b.h(c)}O("addCSELanguageOptions",Bb);O("addCountryOptions",Cb);
O("formatDate",Eb);O("customFormatDate",Eb);O("formatMonthAndYear",Db);O("formatNumber",Fb);O("getResource",ub);var Gb={COUNTRY:{"001":"Monde","002":"Afrique","003":"Am\u00e9rique du Nord","005":"Am\u00e9rique du Sud","009":"Oc\u00e9anie","011":"Afrique occidentale","013":"Am\u00e9rique centrale","014":"Afrique orientale","015":"Afrique septentrionale","017":"Afrique centrale","018":"Afrique australe","019":"Am\u00e9riques","021":"Am\u00e9rique septentrionale","029":"Cara\u00efbes","030":"Asie orientale","034":"Asie du Sud","035":"Asie du Sud-Est","039":"Europe m\u00e9ridionale","053":"Australie et Nouvelle-Z\u00e9lande",
"054":"M\u00e9lan\u00e9sie","057":"r\u00e9gion micron\u00e9sienne","061":"Polyn\u00e9sie","062":"Asie centrale et du Sud",142:"Asie",143:"Asie centrale",145:"Asie occidentale",150:"Europe",151:"Europe orientale",154:"Europe septentrionale",155:"Europe occidentale",172:"Communaut\u00e9 des \u00c9tats ind\u00e9pendants",200:"Czechoslovakia",419:"Am\u00e9rique latine",830:"\u00celes Anglo-normandes",AC:"\u00cele de l'Ascension",AD:"Andorre",AE:"\u00c9mirats arabes unis",AF:"Afghanistan",AG:"Antigua-et-Barbuda",
AI:"Anguilla",AL:"Albanie",AM:"Arm\u00e9nie",AN:"Antilles n\u00e9erlandaises",AO:"Angola",AQ:"Antarctique",AR:"Argentine",AS:"Samoa am\u00e9ricaines",AT:"Autriche",AU:"Australie",AW:"Aruba",AX:"\u00celes \u00c5land",AZ:"Azerba\u00efdjan",BA:"Bosnie-Herz\u00e9govine",BB:"Barbade",BD:"Bangladesh",BE:"Belgique",BF:"Burkina Faso",BG:"Bulgarie",BH:"Bahre\u00efn",BI:"Burundi",BJ:"B\u00e9nin",BL:"Saint-Barth\u00e9l\u00e9my",BM:"Bermudes",BN:"Brun\u00e9i Darussalam",BO:"Bolivie",BQ:"Bonaire, Saint Eustatius, and Saba",
BR:"Br\u00e9sil",BS:"Bahamas",BT:"Bhoutan",BV:"\u00cele Bouvet",BW:"Botswana",BY:"B\u00e9larus",BZ:"Belize",CA:"Canada",CC:"\u00celes Cocos - Keeling",CD:"R\u00e9publique d\u00e9mocratique du Congo",CF:"R\u00e9publique centrafricaine",CG:"Congo-Brazzaville",CH:"Suisse",CI:"C\u00f4te d\u2019Ivoire",CK:"\u00celes Cook",CL:"Chili",CM:"Cameroun",CN:"Chine",CO:"Colombie",CP:"\u00cele Clipperton",CR:"Costa Rica",CS:"Serbie-et-Mont\u00e9n\u00e9gro",CT:"Canton and Enderbury Islands",CU:"Cuba",CV:"Cap-Vert",
CW:"Cura\u00e7ao",CX:"\u00cele Christmas",CY:"Chypre",CZ:"R\u00e9publique tch\u00e8que",DD:"East Germany",DE:"Allemagne",DG:"Diego Garcia",DJ:"Djibouti",DK:"Danemark",DM:"Dominique",DO:"R\u00e9publique dominicaine",DZ:"Alg\u00e9rie",EA:"Ceuta et Melilla",EC:"\u00c9quateur",EE:"Estonie",EG:"\u00c9gypte",EH:"Sahara occidental",ER:"\u00c9rythr\u00e9e",ES:"Espagne",ET:"\u00c9thiopie",EU:"Union europ\u00e9enne",FI:"Finlande",FJ:"Fidji",FK:"\u00celes Malouines",FM:"\u00c9tats f\u00e9d\u00e9r\u00e9s de Micron\u00e9sie",
FO:"\u00celes F\u00e9ro\u00e9",FQ:"French Southern and Antarctic Territories",FR:"France",FX:"France m\u00e9tropolitaine",GA:"Gabon",GB:"Royaume-Uni",GD:"Grenade",GE:"G\u00e9orgie",GF:"Guyane fran\u00e7aise",GG:"Guernesey",GH:"Ghana",GI:"Gibraltar",GL:"Groenland",GM:"Gambie",GN:"Guin\u00e9e",GP:"Guadeloupe",GQ:"Guin\u00e9e \u00e9quatoriale",GR:"Gr\u00e8ce",GS:"G\u00e9orgie du Sud et les \u00eeles Sandwich du Sud",GT:"Guatemala",GU:"Guam",GW:"Guin\u00e9e-Bissau",GY:"Guyana",HK:"Hong Kong",HM:"\u00celes Heard et MacDonald",
HN:"Honduras",HR:"Croatie",HT:"Ha\u00efti",HU:"Hongrie",IC:"\u00celes Canaries",ID:"Indon\u00e9sie",IE:"Irlande",IL:"Isra\u00ebl",IM:"\u00cele de Man",IN:"Inde",IO:"Territoire britannique de l'oc\u00e9an Indien",IQ:"Irak",IR:"Iran",IS:"Islande",IT:"Italie",JE:"Jersey",JM:"Jama\u00efque",JO:"Jordanie",JP:"Japon",JT:"Johnston Island",KE:"Kenya",KG:"Kirghizistan",KH:"Cambodge",KI:"Kiribati",KM:"Comores",KN:"Saint-Kitts-et-Nevis",KP:"Cor\u00e9e du Nord",KR:"Cor\u00e9e du Sud",KW:"Kowe\u00eft",KY:"\u00celes Ca\u00efmans",
KZ:"Kazakhstan",LA:"Laos",LB:"Liban",LC:"Sainte-Lucie",LI:"Liechtenstein",LK:"Sri Lanka",LR:"Lib\u00e9ria",LS:"Lesotho",LT:"Lituanie",LU:"Luxembourg",LV:"Lettonie",LY:"Libye",MA:"Maroc",MC:"Monaco",MD:"Moldavie",ME:"Mont\u00e9n\u00e9gro",MF:"Saint-Martin",MG:"Madagascar",MH:"\u00celes Marshall",MI:"Midway Islands",MK:"Mac\u00e9doine",ML:"Mali",MM:"Myanmar",MN:"Mongolie",MO:"Macao",MP:"\u00celes Mariannes du Nord",MQ:"Martinique",MR:"Mauritanie",MS:"Montserrat",MT:"Malte",MU:"Maurice",MV:"Maldives",
MW:"Malawi",MX:"Mexique",MY:"Malaisie",MZ:"Mozambique",NA:"Namibie",NC:"Nouvelle-Cal\u00e9donie",NE:"Niger",NF:"\u00cele Norfolk",NG:"Nig\u00e9ria",NI:"Nicaragua",NL:"Pays-Bas",NO:"Norv\u00e8ge",NP:"N\u00e9pal",NQ:"Dronning Maud Land",NR:"Nauru",NT:"Neutral Zone",NU:"Niue",NZ:"Nouvelle-Z\u00e9lande",OM:"Oman",PA:"Panama",PC:"Pacific Islands Trust Territory",PE:"P\u00e9rou",PF:"Polyn\u00e9sie fran\u00e7aise",PG:"Papouasie-Nouvelle-Guin\u00e9e",PH:"Philippines",PK:"Pakistan",PL:"Pologne",PM:"Saint-Pierre-et-Miquelon",
PN:"Pitcairn",PR:"Porto Rico",PS:"Territoire palestinien",PT:"Portugal",PU:"U.S. Miscellaneous Pacific Islands",PW:"Palaos",PY:"Paraguay",PZ:"Panama Canal Zone",QA:"Qatar",QO:"r\u00e9gions \u00e9loign\u00e9es de l\u2019Oc\u00e9anie",RE:"R\u00e9union",RO:"Roumanie",RS:"Serbie",RU:"Russie",RW:"Rwanda",SA:"Arabie saoudite",SB:"\u00celes Salomon",SC:"Seychelles",SD:"Soudan",SE:"Su\u00e8de",SG:"Singapour",SH:"Sainte-H\u00e9l\u00e8ne",SI:"Slov\u00e9nie",SJ:"Svalbard et \u00cele Jan Mayen",SK:"Slovaquie",
SL:"Sierra Leone",SM:"Saint-Marin",SN:"S\u00e9n\u00e9gal",SO:"Somalie",SR:"Suriname",SS:"Soudan du Sud",ST:"Sao Tom\u00e9-et-Principe",SU:"Union of Soviet Socialist Republics",SV:"Salvador",SX:"Sint Maarten",SY:"Syrie",SZ:"Swaziland",TA:"Tristan da Cunha",TC:"\u00celes Turks et Ca\u00efques",TD:"Tchad",TF:"Terres australes fran\u00e7aises",TG:"Togo",TH:"Tha\u00eflande",TJ:"Tadjikistan",TK:"Tokelau",TL:"Timor oriental",TM:"Turkm\u00e9nistan",TN:"Tunisie",TO:"Tonga",TR:"Turquie",TT:"Trinit\u00e9-et-Tobago",
TV:"Tuvalu",TW:"Ta\u00efwan",TZ:"Tanzanie",UA:"Ukraine",UG:"Ouganda",UM:"\u00celes Mineures \u00c9loign\u00e9es des \u00c9tats-Unis",US:"\u00c9tats-Unis",UY:"Uruguay",UZ:"Ouzb\u00e9kistan",VA:"\u00c9tat de la Cit\u00e9 du Vatican",VC:"Saint-Vincent-et-les Grenadines",VD:"North Vietnam",VE:"Venezuela",VG:"\u00celes Vierges britanniques",VI:"\u00celes Vierges des \u00c9tats-Unis",VN:"Vi\u00eat Nam",VU:"Vanuatu",WF:"Wallis-et-Futuna",WK:"Wake Island",WS:"Samoa",YD:"People\u2019s Democratic Republic of Yemen",
YE:"Y\u00e9men",YT:"Mayotte",ZA:"Afrique du Sud",ZM:"Zambie",ZW:"Zimbabwe",ZZ:"r\u00e9gion ind\u00e9termin\u00e9e"},LANGUAGE:{aa:"afar",ab:"abkhaze",ace:"aceh",ach:"acoli",ada:"adangme",ady:"adygh\u00e9en",ae:"avestique",af:"afrikaans",afa:"langue afro-asiatique",afh:"afrihili",agq:"Aghem",ain:"a\u00efnou",ak:"akan",akk:"akkadien",ale:"al\u00e9oute",alg:"langue algonquienne",alt:"alta\u00ef du Sud",am:"amharique",an:"aragonais",ang:"ancien anglais",anp:"angika",apa:"langue apache",ar:"arabe",arc:"aram\u00e9en",
arn:"araukan",arp:"arapaho",art:"langue artificielle",arw:"arawak",as:"assamais",asa:"Asu",ast:"asturien",ath:"langue athapascane",aus:"langue australienne",av:"avar",awa:"awadhi",ay:"aymara",az:"az\u00e9ri",ba:"bachkir",bad:"banda",bai:"langue bamil\u00e9k\u00e9e",bal:"baloutchi",ban:"balinais",bas:"bassa",bat:"langue balte",be:"bi\u00e9lorusse",bej:"bedja",bem:"bemba",ber:"berb\u00e8re",bez:"Bena",bg:"bulgare",bh:"bihari",bho:"bhojpuri",bi:"bichelamar",bik:"bikol",bin:"bini",bla:"siksika",bm:"bambara",
bn:"bengali",bnt:"bantou",bo:"tib\u00e9tain",br:"breton",bra:"braj",brx:"Bodo",bs:"bosniaque",btk:"batak",bua:"bouriate",bug:"bugi",byn:"blin",ca:"catalan",cad:"caddo",cai:"langue am\u00e9rindienne centrale",car:"caribe",cau:"langue caucasienne",cay:"Cayuga",cch:"atsam",ce:"tch\u00e9tch\u00e8ne",ceb:"cebuano",cel:"langue celtique",cgg:"Chiga",ch:"chamorro",chb:"chibcha",chg:"tchaghata\u00ef",chk:"chuuk",chm:"mari",chn:"jargon chinook",cho:"choctaw",chp:"chipewyan",chr:"cherokee",chy:"cheyenne",ckb:"Sorani Kurdish",
cmc:"langue chame",co:"corse",cop:"copte",cpe:"cr\u00e9ole ou pidgin anglais",cpf:"cr\u00e9ole ou pidgin fran\u00e7ais",cpp:"cr\u00e9ole ou pidgin portugais",cr:"cree",crh:"turc de Crim\u00e9e",crp:"cr\u00e9ole ou pidgin",cs:"tch\u00e8que",csb:"kachoube",cu:"slavon d\u2019\u00e9glise",cus:"langue couchitique",cv:"tchouvache",cy:"gallois",da:"danois",dak:"dakota",dar:"dargwa",dav:"Taita",day:"dayak",de:"allemand",de_AT:"allemand autrichien",de_CH:"allemand suisse",del:"delaware",den:"slavey",dgr:"dogrib",
din:"dinka",dje:"Zarma",doi:"dogri",dra:"langue dravidienne",dsb:"bas-sorabe",dua:"douala",dum:"moyen n\u00e9erlandais",dv:"maldivien",dyo:"Jola-Fonyi",dyu:"dioula",dz:"dzongkha",ebu:"Embu",ee:"\u00e9w\u00e9",efi:"efik",egy:"\u00e9gyptien ancien",eka:"ekajuk",el:"grec",elx:"\u00e9lamite",en:"anglais",en_AU:"anglais australien",en_CA:"anglais canadien",en_GB:"anglais britannique",en_US:"anglais am\u00e9ricain",enm:"moyen anglais",eo:"esp\u00e9ranto",es:"espagnol",es_419:"espagnol latino-am\u00e9ricain",
es_ES:"espagnol ib\u00e9rique",et:"estonien",eu:"basque",ewo:"\u00e9wondo",fa:"persan",fan:"fang",fat:"fanti",ff:"peul",fi:"finnois",fil:"filipino",fiu:"langue finno-ougrienne",fj:"fidjien",fo:"f\u00e9ro\u00efen",fon:"fon",fr:"fran\u00e7ais",fr_CA:"fran\u00e7ais canadien",fr_CH:"fran\u00e7ais suisse",frm:"moyen fran\u00e7ais",fro:"ancien fran\u00e7ais",frr:"frison du Nord",frs:"frison oriental",fur:"frioulan",fy:"frison",ga:"irlandais",gaa:"ga",gay:"gayo",gba:"gbaya",gd:"ga\u00e9lique \u00e9cossais",
gem:"langue germanique",gez:"gu\u00e8ze",gil:"gilbertais",gl:"galicien",gmh:"moyen haut-allemand",gn:"guarani",goh:"ancien haut allemand",gon:"gondi",gor:"gorontalo",got:"gotique",grb:"grebo",grc:"grec ancien",gsw:"al\u00e9manique",gu:"goudjar\u00e2t\u00ee",guz:"Gusii",gv:"manx",gwi:"gwich\u02bcin",ha:"haoussa",hai:"haida",haw:"hawa\u00efen",he:"h\u00e9breu",hi:"hindi",hil:"hiligaynon",him:"himachali",hit:"hittite",hmn:"hmong",ho:"hiri motu",hr:"croate",hsb:"haut-sorabe",ht:"ha\u00eftien",hu:"hongrois",
hup:"hupa",hy:"arm\u00e9nien",hz:"h\u00e9r\u00e9ro",ia:"interlingua",iba:"iban",id:"indon\u00e9sien",ie:"interlingue",ig:"igbo",ii:"yi de Sichuan",ijo:"ijo",ik:"inupiaq",ilo:"ilokano",inc:"langue indo-aryenne",ine:"langue indo-europ\u00e9enne",inh:"ingouche",io:"ido",ira:"langue iranienne",iro:"langue iroquoienne",is:"islandais",it:"italien",iu:"inuktitut",ja:"japonais",jbo:"lojban",jmc:"Machame",jpr:"jud\u00e9o-persan",jrb:"jud\u00e9o-arabe",jv:"javanais",ka:"g\u00e9orgien",kaa:"karakalpak",kab:"kabyle",
kac:"kachin",kaj:"jju",kam:"kamba",kar:"karen",kaw:"kawi",kbd:"kabardin",kcg:"tyap",kde:"Makonde",kea:"Kabuverdianu",kfo:"koro",kg:"kongo",kha:"khasi",khi:"langue kho\u00efsan",kho:"khotanais",khq:"Koyra Chiini",ki:"kikuyu",kj:"kuanyama",kk:"kazakh",kl:"groenlandais",kln:"Kalenjin",km:"khmer",kmb:"kiMboundou",kn:"kannada",ko:"cor\u00e9en",kok:"konkani",kos:"kusaien",kpe:"kpell\u00e9",kr:"kanouri",krc:"karatcha\u00ef balkar",krl:"car\u00e9lien",kro:"krou",kru:"kurukh",ks:"k\u00e2shm\u00eer\u00ee",
ksb:"Shambala",ksf:"Bafia",ksh:"Colognian",ku:"kurde",kum:"koumyk",kut:"kutenai",kv:"komi",kw:"cornique",ky:"kirghize",la:"latin",lad:"ladino",lag:"Langi",lah:"lahnda",lam:"lamba",lb:"luxembourgeois",lez:"lezghien",lg:"ganda",li:"limbourgeois",ln:"lingala",lo:"lao",lol:"mongo",loz:"lozi",lt:"lituanien",lu:"luba-katanga",lua:"luba-lulua",lui:"luiseno",lun:"lunda",luo:"luo",lus:"lushai",luy:"Luyia",lv:"letton",mad:"madurais",mag:"magahi",mai:"maithili",mak:"makassar",man:"mandingue",map:"malayo-polyn\u00e9sien",
mas:"masai",mdf:"moksa",mdr:"mandar",men:"mend\u00e9",mer:"Meru",mfe:"Morisyen",mg:"malgache",mga:"moyen irlandais",mgh:"Makhuwa-Meetto",mh:"marshall",mi:"maori",mic:"micmac",min:"minangkabau",mis:"langue diverse",mk:"mac\u00e9donien",mkh:"langue mon-khm\u00e8re",ml:"malayalam",mn:"mongol",mnc:"mandchou",mni:"manipuri",mno:"langue manobo",mo:"moldave",moh:"mohawk",mos:"mor\u00e9",mr:"marathe",ms:"malais",mt:"maltais",mua:"Mundang",mul:"multilingue",mun:"langue mounda",mus:"creek",mwl:"mirandais",
mwr:"marwar\u00ee",my:"birman",myn:"langue maya",myv:"erzya",na:"nauruan",nah:"nahuatl",nai:"langue am\u00e9rindienne du Nord",nap:"napolitain",naq:"Nama",nb:"norv\u00e9gien bokm\u00e5l",nd:"nd\u00e9b\u00e9l\u00e9 du Nord",nds:"bas-allemand",ne:"n\u00e9palais","new":"newari",ng:"ndonga",nia:"nias",nic:"langue nig\u00e9ro-congolaise",niu:"niu\u00e9",nl:"n\u00e9erlandais",nl_BE:"n\u00e9erlandais belge",nmg:"Kwasio",nn:"norv\u00e9gien nynorsk",no:"norv\u00e9gien",nog:"noga\u00ef",non:"vieux norrois",
nqo:"n\u2019ko",nr:"nd\u00e9b\u00e9l\u00e9 du Sud",nso:"sotho du Nord",nub:"langue nubienne",nus:"Nuer",nv:"navaho",nwc:"newar\u00ee classique",ny:"nyanja",nym:"nyamwezi",nyn:"nyankol\u00e9",nyo:"nyoro",nzi:"nzema",oc:"occitan",oj:"ojibwa",om:"oromo",or:"oriya",os:"oss\u00e8te",osa:"osage",ota:"turc ottoman",oto:"langue otomangue",pa:"pendjabi",paa:"langue papoue",pag:"pangasinan",pal:"pahlavi",pam:"pampangan",pap:"papiamento",pau:"palau",peo:"persan ancien",phi:"langue philippine",phn:"ph\u00e9nicien",
pi:"pali",pl:"polonais",pon:"pohnpei",pra:"langues pr\u00e2krit",pro:"proven\u00e7al ancien",ps:"pachto",pt:"portugais",pt_BR:"portugais br\u00e9silien",pt_PT:"portugais ib\u00e9rique",qu:"langue quechua",raj:"rajasthani",rap:"rapanui",rar:"rarotongien",rm:"rh\u00e9to-roman",rn:"roundi",ro:"roumain",roa:"langue romane",rof:"Rombo",rom:"tzigane",root:"racine",ru:"russe",rup:"valaque",rw:"rwanda",rwk:"Rwa",sa:"sanskrit",sad:"sandawe",sah:"iakoute",sai:"langue am\u00e9rindienne du Sud",sal:"langue salishenne",
sam:"aram\u00e9en samaritain",saq:"Samburu",sas:"sasak",sat:"santal",sbp:"Sangu",sc:"sarde",scn:"sicilien",sco:"\u00e9cossais",sd:"sindh\u00ee",se:"sami du Nord",see:"Seneca",seh:"Sena",sel:"selkoupe",sem:"langue s\u00e9mitique",ses:"Koyraboro Senni",sg:"sangho",sga:"ancien irlandais",sgn:"langue des signes",sh:"serbo-croate",shi:"Tachelhit",shn:"shan",si:"singhalais",sid:"sidamo",sio:"langue sioux",sit:"langue sino-tib\u00e9taine",sk:"slovaque",sl:"slov\u00e8ne",sla:"langue slave",sm:"samoan",sma:"sami du Sud",
smi:"langue samie",smj:"sami de Lule",smn:"sami d\u2019Inari",sms:"sami skolt",sn:"shona",snk:"sonink\u00e9",so:"somali",sog:"sogdien",son:"songhai",sq:"albanais",sr:"serbe",srn:"sranan tongo",srr:"s\u00e9r\u00e8re",ss:"swati",ssa:"langue nilo-saharienne",ssy:"Saho",st:"sesotho",su:"soundanais",suk:"sukuma",sus:"soussou",sux:"sum\u00e9rien",sv:"su\u00e9dois",sw:"swahili",swb:"comorien",swc:"Congo Swahili",syc:"syriaque classique",syr:"syriaque",ta:"tamoul",tai:"langue ta\u00ef",te:"t\u00e9lougou",
tem:"temne",teo:"Teso",ter:"tereno",tet:"tetum",tg:"tadjik",th:"tha\u00ef",ti:"tigrigna",tig:"tigr\u00e9",tiv:"tiv",tk:"turkm\u00e8ne",tkl:"tokelau",tl:"tagalog",tlh:"klingon",tli:"tlingit",tmh:"tamacheq",tn:"tswana",to:"tongan",tog:"tonga nyasa",tpi:"tok pisin",tr:"turc",trv:"Taroko",ts:"tsonga",tsi:"tsimshian",tt:"tatar",tum:"tumbuka",tup:"langue tupi",tut:"langue alta\u00efque",tvl:"tuvalu",tw:"twi",twq:"Tasawaq",ty:"tahitien",tyv:"touva",tzm:"Central Morocco Tamazight",udm:"oudmourte",ug:"ou\u00efgour",
uga:"ougaritique",uk:"ukrainien",umb:"umbundu",und:"ind\u00e9termin\u00e9",ur:"ourdou",uz:"ouzbek",vai:"va\u00ef",ve:"venda",vi:"vietnamien",vo:"volapuk",vot:"vote",vun:"Vunjo",wa:"wallon",wae:"Walser",wak:"langues wakashennes",wal:"walamo",war:"waray",was:"washo",wen:"langue sorabe",wo:"wolof",xal:"kalmouk",xh:"xhosa",xog:"Soga",yao:"yao",yap:"yapois",yav:"Yangben",yi:"yiddish",yo:"yoruba",ypk:"langues yupik",yue:"cantonais",za:"zhuang",zap:"zapot\u00e8que",zbl:"symboles Bliss",zen:"zenaga",zh:"chinois",
zh_Hans:"chinois simplifi\u00e9",zh_Hant:"chinois traditionnel",znd:"zand\u00e9",zu:"zoulou",zun:"zuni",zxx:"sans contenu linguistique",zza:"zazaki"}};yb(Gb,"fr"); })()
