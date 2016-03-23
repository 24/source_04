netsh http add urlacl http://+:8019/DownloadManagerService/ user=\Everyone
@echo off
rem netsh http delete urlacl http://+:8019/DownloadManagerService/
rem netsh http show urlacl
