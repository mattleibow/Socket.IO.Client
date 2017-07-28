#tool nuget:?package=XamarinComponent

#addin nuget:?package=Cake.Xamarin
#addin nuget:?package=Cake.FileHelpers

//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");

var version = "0.9.0";

//////////////////////////////////////////////////////////////////////
// PREPARATION
//////////////////////////////////////////////////////////////////////

FilePath XamarinComponentPath = "./tools/XamarinComponent/tools/xamarin-component.exe";

DirectoryPath outDir = "./output/";
if (!DirectoryExists(outDir)) {
    CreateDirectory(outDir);
}
    
//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

Task("Clean")
    .Does(() =>
{
});

Task("RestorePackages")
    .IsDependentOn("Clean")
    .Does(() =>
{
    NuGetRestore("./binding/Socket.IO-Client.sln");
    NuGetRestore("./sample/SocketIOClientJavaSample.sln");
});

Task("Externals")
    .Does(() =>
{
    EnsureDirectoryExists("./externals");

    DownloadFile(string.Format("http://search.maven.org/remotecontent?filepath=io/socket/socket.io-client/{0}/socket.io-client-{0}.jar", version), "./externals/socket.io-client.jar");
    DownloadFile(string.Format("http://search.maven.org/remotecontent?filepath=io/socket/engine.io-client/{0}/engine.io-client-{0}.jar", version), "./externals/engine.io-client.jar");
});

Task("Build")
    .IsDependentOn("Externals")
    .IsDependentOn("RestorePackages")
    .Does(() =>
{
    var solution = "./binding/Socket.IO-Client.sln";
    if (IsRunningOnWindows()) {
        MSBuild(solution, s => s.SetConfiguration(configuration).SetMSBuildPlatform(MSBuildPlatform.x86));
    } else {
        XBuild(solution, s => s.SetConfiguration(configuration));
    }
    
    CopyFileToDirectory("./binding/Socket.IO-Client.Java/bin/" + configuration + "/Socket.IO.Client.dll", outDir);
    CopyFileToDirectory("./binding/Engine.IO-Client.Java/bin/" + configuration + "/Engine.IO.Client.dll", outDir);
});

Task("BuildSamples")
    .IsDependentOn("RestorePackages")
    .Does(() =>
{
    var solution = "./sample/SocketIOClientJavaSample.sln";
    if (IsRunningOnWindows()) {
        MSBuild(solution, s => s.SetConfiguration(configuration).SetMSBuildPlatform(MSBuildPlatform.x86));
    } else {
        XBuild(solution, s => s.SetConfiguration(configuration));
    }
});

Task("PackageNuGet")
    .IsDependentOn("Build")
    .Does(() =>
{
    NuGetPack("./nuget/Engine.IO.Client.nuspec", new NuGetPackSettings {
        OutputDirectory = outDir
    });
    NuGetPack("./nuget/Socket.IO.Client.nuspec", new NuGetPackSettings {
        OutputDirectory = outDir
    });
});

Task("PackageComponent")
    .IsDependentOn("Build")
    .IsDependentOn("PackageNuGet")
    .IsDependentOn("BuildSamples")
    .Does(() =>
{
    DeleteFiles("./component/socketio/*.xam");
    PackageComponent("./component/socketio/", new XamarinComponentSettings { ToolPath = XamarinComponentPath });
    
    DeleteFiles("./output/*.xam");
    MoveFiles("./component/socketio/*.xam", outDir);
});

Task("Package")
    .IsDependentOn("PackageNuGet")
    .IsDependentOn("PackageComponent")
    .Does(() =>
{
});

//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////

Task("Default")
    .IsDependentOn("Build")
    .IsDependentOn("Package");

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);
