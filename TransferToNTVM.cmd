del jogging.zip
del .\jogging\jogging.tar
docker save -o .\jogging\jogging.tar mysql:latest jogging-jogging.api:latest jogging-jogging-ui:latest
tar -a -cf jogging.zip jogging
REM PWD: Jupiler01
scp jogging.zip u23893@192.168.1.163:/tmp
REM cd /home/wim
REM unzip /tmp/jogging.zip
REM cd jogging
REM chmod 755 StartProjectwerk.sh
REM chmod 755 showlog.sh
REM StartProjectwerk.sh unload
REM StartProjectwerk.sh load
REM StartProjectwerk.sh up
echo off
FOR /L %%i IN (1,1,5) DO (
rundll32 user32.dll,MessageBeep -1
timeout 2 > NUL
)
