// Copyright Lance Wallis, 2022, All Rights Reserved.

using System.IO;
using System.IO.Compression;
using System.Net.Http;
using UnrealBuildTool;

public class FastNoise2 : ModuleRules
{
    public FastNoise2(ReadOnlyTargetRules Target) : base(Target)
    {
        Type = ModuleType.External;
        PublicIncludePaths.Add(Path.Combine(ModuleDirectory, "include"));

		FetchCMakePortable(Target);
    }

	private void FetchCMakePortable(ReadOnlyTargetRules Target)
	{
		string CMakeDir = Path.Combine(GetIntermediateDir(), "CMake");
		Directory.CreateDirectory(CMakeDir);
		string CMakeZipPath = Path.Combine(CMakeDir, "CMake.zip");

		string Win64CMake = "https://github.com/Kitware/CMake/releases/download/v3.24.0/cmake-3.24.0-windows-x86_64.zip";

		if (Target.Platform == UnrealTargetPlatform.Win64)
		{
			GetFile(Win64CMake, CMakeZipPath);
		} 
		else
		{
			throw new BuildException("Cannot build FastNoise2, unsupported platform: {0}", Target.Platform);
		}

		ZipFile.ExtractToDirectory(CMakeZipPath, CMakeDir);
	}

	private void GetFile(string Url, string TargetPath)
	{
		using (var client = new HttpClient())
		{
			using (var file = client.GetStreamAsync(Url))
			{
				using (var fs = new FileStream(TargetPath, FileMode.OpenOrCreate))
				{
					file.Result.CopyTo(fs);
				}
			}
		}
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
