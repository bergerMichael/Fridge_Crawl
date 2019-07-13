echo Cleaning Up Build Directory
rm -rf ../TestProject/build/

echo Starting Build Process
'/c/Program Files/Unity/Hub/Editor/2019.1.8f1/Editor/Unity.exe' -quit -bachmode -projectPath ../Assets/Scripts/Editor -executeMethod BuildScript.PerformBuild
echo Ended Build Process