Trace.WriteLine("toto");
Trace.WriteLine(zPath.GetDirectoryName(@"..\..\..\..\..\..\RuntimeLibrary\LongPath\LongPath\Pri.LongPath.dll"));
Trace.WriteLine(zPath.GetDirectoryName(@"c:\..\..\..\..\..\..\RuntimeLibrary\LongPath\LongPath\tata.dll"));
zPath.GetDirectoryName(@"c:\toto\RuntimeLibrary\LongPath\LongPath\tata.dll").zTrace();
zPath.GetDirectoryName(@"..\..\..\..\..\..\RuntimeLibrary\LongPath\toto.dll").zTrace();
Path.GetDirectoryName(@"..\..\..\..\..\..\RuntimeLibrary\LongPath\LongPath\Pri.LongPath.dll").zTrace();
