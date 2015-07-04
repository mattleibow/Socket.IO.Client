echo off

rem build the solution
echo Building the solution
msbuild binding\Socket.IO-Client.sln /p:Configuration=Release /t:Rebuild

rem build the nuget
echo Packaging the NuGets
nuget pack nuget\Engine.IO.Client.nuspec
nuget pack nuget\Socket.IO.Client.nuspec

rem build the components
echo Building the samples
msbuild sample\SocketIOClientJavaSample.sln /p:Configuration=Release /t:Rebuild
echo Packaging the Components
xamarin-component package component\socketio
move component\socketio\*.xam .\
