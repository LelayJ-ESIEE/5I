@rem obtenir la date

echo off
set CUR_YYYY=%date:~6,4%
set CUR_MM=%date:~3,2%
set CUR_DD=%date:~0,2%

set CUR_HH=%time:~0,2%
if %CUR_HH% lss 10 (set CUR_HH=0%time:~1,1%)

set CUR_MIN=%time:~3,2%
set CUR_SS=%time:~6,2%


@rem preparer la date en 1 gros morceau
set SUBFILENAME=%CUR_YYYY%%CUR_MM%%CUR_DD%-%CUR_HH%%CUR_MIN%%CUR_SS%


@rem windows inclut tar.exe pour faire des zip
tar.exe -a -c -f %SUBFILENAME%_prog_unity.zip Assets Packages ProjectSettings createExercicesIncomplets.jse zipper.bat

