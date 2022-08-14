// Copyright Lance Wallis, 2022, All Rights Reserved.

using System.IO;
using System.Diagnostics;
using UnrealBuildTool;
using EpicGames.Core;

public class FastNoise2 : ModuleRules
{
    public FastNoise2(ReadOnlyTargetRules Target) : base(Target)
    {
        Type = ModuleType.External;
        PublicIncludePaths.Add(Path.Combine(ModuleDirectory, "include"));

		string CMakeBuildDir = Path.Combine(GetIntermediateDir(), "FastNoise2Build");
		string CMake = SetupCMake(Target);
		string CMakeBuildCommand = CreateCMakeBuildCommand(Target, CMakeBuildDir);
		string CMakeInstallCommand = CreateCMakeInstallCommand(Target, CMakeBuildDir);

		Log.TraceInformation("Building FastNoise2: Delete Intermediate Folder to re-fetch CMake");
		Log.TraceInformation("TARGET PLATFORM: {0}", Target.Platform);
		Log.TraceInformation("{0}", (Target.Platform == UnrealTargetPlatform.Win64));
		Log.TraceInformation("Building with: \n {0} \n {1}", CMakeBuildCommand, CMakeInstallCommand);

		string platform = "";
		string StaticFileEnding = ".lib";
		string SharedFileEnding = "";

		if (Target.Platform == UnrealTargetPlatform.Win64)
		{
			SharedFileEnding = ".dll";
			platform = "Win64";
		}
		else if (Target.Platform == UnrealTargetPlatform.Linux)
		{
			SharedFileEnding = ".so";
		}
		else if ((Target.Platform == UnrealTargetPlatform.Mac) || (Target.Platform == UnrealTargetPlatform.IOS))
		{
			SharedFileEnding = ".dylib";
		}

		string FNDebugLib = Path.Combine(CMakeBuildDir, "Debug", "lib", "FastNoiseD" + StaticFileEnding);
		string FNDebugDLL = Path.Combine(CMakeBuildDir, "Debug", "bin", "FastNoiseD" + SharedFileEnding);
		string PdbFile = Path.Combine(CMakeBuildDir, "pdb-files", "Debug", "FastNoiseD.pdb");
		string FNReleaseLib = Path.Combine(CMakeBuildDir, "Release", "lib", "FastNoise" + StaticFileEnding);
		string FNReleaseDLL = Path.Combine(CMakeBuildDir, "Release", "bin", "FastNoise" + SharedFileEnding);

		string PrimaryBuildDirectory = Path.Combine(PluginDirectory, "Binaries", "ThirdParty", "FastNoise2", platform);
		Directory.CreateDirectory(PrimaryBuildDirectory);

		if ((Target.Configuration == UnrealTargetConfiguration.Debug) || (Target.Configuration == UnrealTargetConfiguration.DebugGame))
		{
			if ((!File.Exists(Path.Combine(PrimaryBuildDirectory, Path.GetFileName(FNDebugLib)))))
			{
				BuildModel(Target, CMake, CMakeBuildCommand);
				BuildModel(Target, CMake, CMakeInstallCommand);
			}

			Log.TraceInformation("MOVING DEBUG FILES:");
			Log.TraceInformation("{0}\n{1}", PdbFile, Path.Combine(PrimaryBuildDirectory, Path.GetFileName(PdbFile)));
			File.Copy(FNDebugLib, Path.Combine(PrimaryBuildDirectory, Path.GetFileName(FNDebugLib)), true);
			File.Copy(FNDebugDLL, Path.Combine(PrimaryBuildDirectory, Path.GetFileName(FNDebugDLL)), true);
			File.Copy(PdbFile, Path.Combine(PrimaryBuildDirectory, Path.GetFileName(PdbFile)), true);


			PublicAdditionalLibraries.Add(Path.Combine(PrimaryBuildDirectory, Path.GetFileName(FNDebugLib)));
			PublicDelayLoadDLLs.Add("FastNoiseD.dll");
			RuntimeDependencies.Add("$(PluginDir)/Binaries/ThirdParty/FastNoise2/Win64/FastNoiseD.dll");
		}
		else
		{
			if (!File.Exists(Path.Combine(PrimaryBuildDirectory, Path.GetFileName(FNReleaseDLL))))
			{
				BuildModel(Target, CMake, CMakeBuildCommand);
				BuildModel(Target, CMake, CMakeInstallCommand);
			}

			Log.TraceInformation("MOVING RELEASE FILES");
			File.Copy(FNReleaseLib, Path.Combine(PrimaryBuildDirectory, Path.GetFileName(FNReleaseLib)), true);
			File.Copy(FNReleaseDLL, Path.Combine(PrimaryBuildDirectory, Path.GetFileName(FNReleaseDLL)), true);

			PublicAdditionalLibraries.Add(Path.Combine(PrimaryBuildDirectory, Path.GetFileName(FNReleaseLib)));
			PublicDelayLoadDLLs.Add("FastNoise.dll");
			RuntimeDependencies.Add("$(PluginDir)/Binaries/ThirdParty/FastNoise2/Win64/FastNoise.dll");
		}


	}

	private string SetupCMake(ReadOnlyTargetRules Target)
	{
		string CMakeDir = Path.Combine(GetIntermediateDir(), "CMake");
		Directory.CreateDirectory(CMakeDir);
		string CMakeZipPath = Path.Combine(CMakeDir, "CMake.zip");
		string CMakeInstallPath = Path.Combine(CMakeDir, "Install");
		Directory.CreateDirectory(CMakeInstallPath);

		string BaseUrl = "https://github.com/Kitware/CMake/releases/download/v";
		string CMakeVersion = "3.24.0/";

		string Win64CMake = "cmake-3.24.0-windows-x86_64";
		string Linux64CMake = "cmake-3.24.0-linux-x86_64";
		string Mac64CMake = "cmake-3.24.0-macos10.10-universal";

		string GetFileBatch = Path.Combine(GetThirdPartyDir(), "BatchFiles", "GetFile.bat");
		string GetUnzipBatch = Path.Combine(GetThirdPartyDir(), "BatchFiles", "Unzip.bat");

		string GetFileShell = Path.Combine(GetThirdPartyDir(), "BatchFiles", "GetFile.sh");
		string GetUnzipShell = Path.Combine(GetThirdPartyDir(), "BatchFiles", "Unzip.sh");

		string FinalPath = "";
		string exe = "cmake.exe";

		
		if (Target.Platform == UnrealTargetPlatform.Win64)
		{
			if (!File.Exists(CMakeZipPath))
			{
				string url = BaseUrl + CMakeVersion + Win64CMake + ".zip";
				ExecuteBatch(Target, GetFileBatch, url, CMakeZipPath);
				ExecuteBatch(Target, GetUnzipBatch, CMakeZipPath, CMakeInstallPath);
			}
			FinalPath = Win64CMake;
		}
		else if (Target.Platform == UnrealTargetPlatform.Linux)
		{
			if (!File.Exists(CMakeZipPath))
			{
				string url = BaseUrl + CMakeVersion + Linux64CMake + ".tar.gz";
				ExecuteBatch(Target, GetFileBatch, url, CMakeZipPath);
				ExecuteBatch(Target, GetUnzipBatch, CMakeZipPath, CMakeInstallPath);
			}
			FinalPath = Linux64CMake;
			exe = "cmake";
		}
		else if ((Target.Platform == UnrealTargetPlatform.Mac) || (Target.Platform == UnrealTargetPlatform.IOS))
		{
			if (!File.Exists(CMakeZipPath))
			{
				string url = BaseUrl + CMakeVersion + Mac64CMake + ".tar.gz";
				ExecuteBatch(Target, GetFileBatch, url, CMakeZipPath);
				ExecuteBatch(Target, GetUnzipBatch, CMakeZipPath, CMakeInstallPath);
			}
			FinalPath = "CMake.app";
			exe = "cmake";
		}
		else
		{
			throw new BuildException("Cannot build FastNoise2, unsupported platform: {0}", Target.Platform);
		}

		if ((Target.Platform == UnrealTargetPlatform.Mac) || (Target.Platform == UnrealTargetPlatform.IOS))
		{
			return Path.Combine(CMakeInstallPath, FinalPath, "Contents", "bin", exe);
		}

		return Path.Combine(CMakeInstallPath, FinalPath, "bin", exe);
	}

	private string CreateCMakeBuildCommand(ReadOnlyTargetRules Target, string CMakeBuildDir)
	{
		Directory.CreateDirectory(CMakeBuildDir);

		string Generator = "";
		if (Target.Platform == UnrealTargetPlatform.Win64)
		{
			Generator = " -G \"Visual Studio 17 2022\"";
		}

		string Arguments = Generator +
			" -S " + GetThirdPartyDir() +
			" -B " + CMakeBuildDir +
			" -D" + " BUILD_SHARED_LIBS=ON ";

		return Arguments;
	}

	private string CreateCMakeInstallCommand(ReadOnlyTargetRules Target, string CMakeBuildDir)
	{
		string BuildType;
		if ((Target.Configuration == UnrealTargetConfiguration.Debug) || (Target.Configuration == UnrealTargetConfiguration.DebugGame))
		{
			BuildType = "Debug";
		}
		else
		{
			BuildType = "Release";
		}

		return " --build " + CMakeBuildDir + " --config " + BuildType;
	}

	private void BuildModel(ReadOnlyTargetRules Target, string CMake, string Command)
	{
		var processInfo = new ProcessStartInfo();
		processInfo.FileName = CMake;
		processInfo.Arguments = Command;

		processInfo.CreateNoWindow = true;
		processInfo.UseShellExecute = false;
		processInfo.RedirectStandardError = true;
		processInfo.RedirectStandardOutput = true;

		var process = Process.Start(processInfo);

		process.OutputDataReceived += (object sender, DataReceivedEventArgs e) =>
			Log.TraceInformation("FastNoise2:" + e.Data);
		process.BeginOutputReadLine();

		// Right now everything is read as error data. Bug.
		process.ErrorDataReceived += (object sender, DataReceivedEventArgs e) =>
			Log.TraceInformation("FastNoise2:" + e.Data);
		process.BeginErrorReadLine();

		process.WaitForExit();

		if (process.ExitCode != 0)
		{
			Log.TraceError("ExitCode: {0}", process.ExitCode);
		}
		Log.TraceInformation("ExitCode: {0}", process.ExitCode);

		process.Close();
	}

	private void ExecuteBatch(ReadOnlyTargetRules Target, string file, string arg1, string arg2)
	{
		string Command;
		var processInfo = new ProcessStartInfo();
		if (Target.Platform == UnrealTargetPlatform.Win64)
		{
			processInfo.FileName = "cmd.exe";
			Command = " /c ";
		}
		else
		{
			processInfo.FileName = "/bin/bash";
			Command = " ";
		}
		processInfo.Arguments = Command + file + " " + arg1 + " " + arg2;
		processInfo.CreateNoWindow = true;
		processInfo.UseShellExecute = false;
		processInfo.RedirectStandardError = true;
		processInfo.RedirectStandardOutput = true;

		var process = Process.Start(processInfo);

		process.OutputDataReceived += (object sender, DataReceivedEventArgs e) =>
			Log.TraceInformation("FastNoise2:" + e.Data);
		process.BeginOutputReadLine();

		process.ErrorDataReceived += (object sender, DataReceivedEventArgs e) =>
			Log.TraceInformation("FastNoise2:" + e.Data);
		process.BeginErrorReadLine();

		process.WaitForExit();

		if (process.ExitCode != 0)
		{
			Log.TraceError("ExitCode: {0}", process.ExitCode);
		}
		Log.TraceInformation("ExitCode: {0}", process.ExitCode);

		process.Close();
	}

	private string GetThirdPartyDir()
	{
		string pluginDir = PluginDirectory;
		string dir = Path.Combine(pluginDir, "Source", "ThirdParty", "FastNoise2");
		return dir;
	}

	private string GetIntermediateDir()
	{
		string pluginDir = PluginDirectory;
		string dir = Path.Combine(pluginDir, "Intermediate");
		return dir;
	}
}
