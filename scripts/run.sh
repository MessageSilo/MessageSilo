cd /root/www/Silo
lsof nohup.out
kill -9 123
cp -R /home/temp/silo/* /root/www/Silo
nohup dotnet MessageSilo.Silo.dll &
cat nohup.out

cd /root/www/API
lsof nohup.out
kill -9 123
cp -R /home/temp/api/* /root/www/API
nohup dotnet MessageSilo.API.dll --urls "https://*:443" &
cat nohup.out

----

git tag v1.0.XX-beta
git push origin --tags