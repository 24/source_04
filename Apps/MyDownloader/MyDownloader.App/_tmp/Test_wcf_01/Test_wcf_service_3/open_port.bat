rem runas /env /user:administrator "netsh http add urlacl http://+:8701/ user=\Everyone"
netsh http add urlacl http://+:8701/ user=\Everyone
rem netsh http show urlacl
rem netsh http delete urlacl http://+:8701/
