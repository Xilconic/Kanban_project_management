#addin nuget:?package=Cake.VersionReader&version=5.1.0

var target = Argument("target", "UnitTests");
var configuration = Argument("configuration", "Release");

var solutionFolder = "./";
var kanbanProjectManagementAppFolder = "./src/Kanban Project Management App";
var publishedArtifactsFolder = "./artifacts";

var cleanTask = Task("Clean")
    .Does(() => {
        CleanDirectory(publishedArtifactsFolder);
    });

var restoreTask = Task("Restore")
    .Does(() => {
        DotNetCoreRestore(solutionFolder);
    });

var buildTask = Task("Build")
    .IsDependentOn(cleanTask)
    .IsDependentOn(restoreTask)
    .Does(() => {
        DotNetCoreBuild(solutionFolder, new DotNetCoreBuildSettings{
            NoRestore = true,
            Configuration = configuration
        });
    });

var unitTestsTask = Task("UnitTests")
    .IsDependentOn(buildTask)
    .Does(() => {
        DotNetCoreTest(solutionFolder, new DotNetCoreTestSettings{
            NoRestore = true,
            Configuration = configuration,
            NoBuild = true,
        });
    });

var publishAppTask = Task("PublishKanbanProjectManagementApp")
    .IsDependentOn(unitTestsTask)
    .Does(() => {
        DotNetCorePublish(kanbanProjectManagementAppFolder, new DotNetCorePublishSettings{
            NoRestore = true,
            Configuration = configuration,
            NoBuild = true,
            OutputDirectory = publishedArtifactsFolder
        });
    });

Task("CreateKanbanProjectManagementAppReleaseZip")
    .IsDependentOn(publishAppTask)
    .Does(() => {
        var files = GetFiles($"{publishedArtifactsFolder}/*.*");
        var version = GetFullVersionNumber($"{publishedArtifactsFolder}/Kanban Project Management App.exe");
        Zip(publishedArtifactsFolder, $"{publishedArtifactsFolder}/Kanban.Project.Management.App-{version}-pre-release.zip", files);
    });

RunTarget(target);