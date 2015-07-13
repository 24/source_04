Trace.WriteLine("toto");
Trace.WriteLine(Trace.CurrentTrace.GetLogFile());

Compiler.TraceLevel = 1;
Compiler.TraceLevel = 2;

Trace.WriteLine(XmlConfig.CurrentConfig.ConfigFile);
Trace.WriteLine(XmlConfig.CurrentConfig.ConfigLocalFile);
Trace.WriteLine(XmlConfig.CurrentConfig.Get("Log/@LogOptions"));
Trace.WriteLine(XmlConfig.CurrentConfig.Get("Log/@LogOptions").zTextDeserialize(LogOptions.None).ToString());
Trace.WriteLine("None".zTextDeserialize(LogOptions.None).ToString());
Trace.WriteLine(TextSerializer.CurrentTextSerializer.Deserialize("None", LogOptions.None).ToString());
Trace.WriteLine(RunSource.CurrentRunSource.ProjectFile);
Trace.WriteLine(RunSource.CurrentRunSource.SourceDir);

int i = 123;
object i = 123;
int i2 = __refvalue(__makeref(i), int);
Trace.WriteLine("{0} {1}", i, i2);

object i = 123;
Type type = __reftype(__makeref(i));
Trace.WriteLine("{0} {1}", i, type.zGetTypeName());

Trace.WriteLine(XmlConfig.CurrentConfig.Get("Log").zSetRootDirectory());
LogOptions logOptions = XmlConfig.CurrentConfig.Get("Log/@LogOptions").zTextDeserialize(LogOptions.None);
Trace.WriteLine(logOptions.ToString());

Trace.CurrentTrace.SetLogFile(XmlConfig.CurrentConfig.Get("Log").zSetRootDirectory(), XmlConfig.CurrentConfig.Get("Log/@LogOptions").zTextDeserialize(LogOptions.None));
Trace.CurrentTrace.SetLogFile("log2.txt", LogOptions.None);
Trace.WriteLine(Trace.CurrentTrace.GetLogFile());

Trace.WriteLine(System.Reflection.Assembly.GetEntryAssembly().Location);
Trace.WriteLine(System.Reflection.Assembly.GetExecutingAssembly().Location);
Trace.WriteLine(System.Threading.Thread.GetDomain().BaseDirectory);

