var target = Argument("target", "UnitTests");
var configuration = Argument("configuration", "Release");

var solutionFolder = "./";

var restoreTask = Task("Restore")
    .Does(() => {
        DotNetCoreRestore(solutionFolder);
    });

var buildTask = Task("Build")
    .IsDependentOn(restoreTask)
    .Does(() => {
        DotNetCoreBuild(solutionFolder, new DotNetCoreBuildSettings{
            NoRestore = true,
            Configuration = configuration
        });
    });

Task("UnitTests")
    .IsDependentOn(buildTask)
    .Does(() => {
        DotNetCoreTest(solutionFolder, new DotNetCoreTestSettings{
            NoRestore = true,
            Configuration = configuration,
            NoBuild = true,
        });
    });

RunTarget(target);