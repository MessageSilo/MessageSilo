cd /root/www/Silo
lsof nohup.out
kill -9 123
nohup dotnet MessageSilo.Silo.dll &
cat nohup.out

cd /root/www/API
lsof nohup.out
kill -9 123
nohup dotnet MessageSilo.API.dll --urls "https://*:443" &
cat nohup.out