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

		string CMake = SetupCMake(Target);
		string CMakeBuildCommand = CreateCMakeBuildCommand(Target);
		string CMakeInstallCommand = CreateCMakeInstallCommand();

		Log.TraceInformation("TARGET PLATFORM: {0}", Target.Platform);
		Log.TraceInformation("{0}", (Target.Platform == UnrealTargetPlatform.Win64));
		Log.TraceInformation("Building with: {0}", CMakeBuildCommand);
		BuildModel(Target, CMake, CMakeBuildCommand);
		BuildModel(Target, CMake, CMakeInstallCommand);
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
		//string Linux64CMake = "cmake-3.24.0-linux-x86_64";
		//string Mac64CMake = "cmake-3.24.0-macos10.10-universal";

		string GetFileBatch = Path.Combine(GetThirdPartyDir(), "BatchFiles", "GetFile.bat");
		string GetUnzipBatch = Path.Combine(GetThirdPartyDir(), "BatchFiles", "Unzip.bat");

		//string GetFileShell = Path.Combine(GetThirdPartyDir(), "BatchFiles", "GetFile.sh");
		//string GetUnzipShell = Path.Combine(GetThirdPartyDir(), "BatchFiles", "Unzip.sh");

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
		/*
		else if (Target.Platform == UnrealTargetPlatform.Linux)
		{
			throw new BuildException("Cannot build FastNoise2, unimplimented platform: {0}", Target.Platform);
			string url = BaseUrl + CMakeVersion + Linux64CMake + ".tar.gz";
			ExecuteBatch(Target, GetFileBatch, url, CMakeZipPath);
			ExecuteBatch(Target, GetUnzipBatch, CMakeZipPath, CMakeInstallPath);
			FinalPath = Linux64CMake;
			exe = "cmake";
		}
		else if ((Target.Platform == UnrealTargetPlatform.Mac) || (Target.Platform == UnrealTargetPlatform.IOS))
		{
			throw new BuildException("Cannot build FastNoise2, unimplimented platform: {0}", Target.Platform);
			string url = BaseUrl + CMakeVersion + Mac64CMake + ".tar.gz";
			ExecuteBatch(Target, GetFileBatch, url, CMakeZipPath);
			ExecuteBatch(Target, GetUnzipBatch, CMakeZipPath, CMakeInstallPath);
			FinalPath = "CMake.app";
			exe = "cmake";
		}
		*/
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

	private string CreateCMakeBuildCommand(ReadOnlyTargetRules Target)
	{
		string BuildDir = Path.Combine(GetIntermediateDir(), "CMakeBuild");
		Directory.CreateDirectory(BuildDir);

		string Generator = "";
		if (Target.Platform == UnrealTargetPlatform.Win64)
		{
			Generator = " -G \"Visual Studio 17 2022\"";
		}

		string Arguments = Generator +
			" BUILD_SHARED_LIBS=ON " +
			" -S " + GetThirdPartyDir() +
			" -B " + BuildDir;

		return Arguments;
	}

	private string CreateCMakeInstallCommand()
	{
		return " --build " + Path.Combine(GetIntermediateDir(), "CMakeBuild");
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
			Log.TraceInformation("output>>" + e.Data);
		process.BeginOutputReadLine();

		process.ErrorDataReceived += (object sender, DataReceivedEventArgs e) =>
			Log.TraceInformation("output>>" + e.Data);
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
		if (Target.Platform == UnrealTargetPlatform.Win64)
		{
			Command = "cmd.exe /c ";
		}
		else
		{
			Command = "/bin/bash ";
		}
		var processInfo = new ProcessStartInfo(Command + file + " " + arg1 + " " + arg2);
		processInfo.CreateNoWindow = true;
		processInfo.UseShellExecute = false;
		processInfo.RedirectStandardError = true;
		processInfo.RedirectStandardOutput = true;

		var process = Process.Start(processInfo);

		process.OutputDataReceived += (object sender, DataReceivedEventArgs e) =>
			Log.TraceInformation("output>>" + e.Data);
		process.BeginOutputReadLine();

		process.ErrorDataReceived += (object sender, DataReceivedEventArgs e) =>
			Log.TraceInformation("output>>" + e.Data);
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
