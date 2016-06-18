@echo off
"C:\Program Files (x86)\Windows Kits\8.1\bin\x86\rc.exe" /fo Resource\test.res Resource\test.rc
..\..\bin\Debug\Test_Roslyn_01.exe Test_Form_01.project.xml
