using System;
using System.IO;

namespace TriggersTools.FarCry3.SilentMenuPatcher
{
	class Program
	{
		const string BackupPostfix = "_orig";


		static int Main(string[] args)
		{
			PrintHeader();

			if (args.Length == 0) {
				PrintHelp(false);
				WaitForEnter(); // Be helpful for less tech-literate users.
				return 0;
			}

			string dllPath = null;
			bool test = false; // If true, run the patching process without writing to the DLL file.
			for (int i = 0; i < args.Length; i++) {
				string arg = args[i];

				if (dllPath != null) {
					Console.WriteLine($"Error: Expected DLL filename to be last command line argument. Got \"{arg}\" afterwards.");
					return 1;
				}

				if (arg.Length > 0 && arg[0] == '-') {
					switch (arg) {
					case "-h":
					case "--help":
						PrintHelp(true);
						return 0; // Ignore remaining arguments.

					case "-T":
					case "--test":
						test = true;
						break;

					default:
						Console.WriteLine($"Error: Unknown command line option: \"{arg}\"");
						return 1;
					}
				}
				else {
					/*if (i + 1 != args.Length) {
						Console.WriteLine($"Error: Expected DLL filename to be last command line argument. Got \"{args[i + 1]}\" afterwards.");
						return 1;
					}*/

					dllPath = arg;
				}
			}

			if (dllPath == null) {
				Console.WriteLine($"Error: Expected DLL filename argument after command line options.");
				return 1;
			}

			try {
				string backupPath = Patcher.GetBackupPath(dllPath, BackupPostfix);
				bool backupExists = File.Exists(backupPath);

				Patcher.ApplySilentMenuPatch(dllPath, true, BackupPostfix, test);

				string testString = (test ? " (TEST: changes not written)" : "");
				Console.WriteLine($"Patch applied to: \"{dllPath}\"{testString}");
				if (backupExists) {
					Console.WriteLine($"Backup file already exists: \"{backupPath}\"");
				}
				else if (File.Exists(backupPath)) {
					Console.WriteLine($"Backup file created: \"{backupPath}\"");
				}
			}
			catch (FileNotFoundException ex) {
				Console.WriteLine($"Error: Could not find DLL file: \"{ex.FileName}\"");
				return 1;
			}
			catch (AlreadyPatchedException ex) {
				Console.WriteLine($"Notice: DLL file is already assumed to be patched: \"{ex.FileName}\"");
				return 1;
			}
			catch (PatchException ex) {
				Console.WriteLine($"Error: {ex.Message}: \"{ex.FileName}\"");
				return 1;
			}

			return 0;
		}

		static void WaitForEnter()
		{
			Console.Write("Press enter to continue...");
			Console.ReadLine();
		}

		static void PrintHeader()
		{
			Console.WriteLine("==== Far Cry 3: Silent Menu patcher ====");
		}

		static void PrintHelp(bool fullHelp)
		{
			Console.WriteLine("Patch to remove loud electronic humming noise that plays in pause screens.");
			Console.WriteLine();
			if (fullHelp) {
				PrintUsage();
				Console.WriteLine();
			}
			Console.WriteLine("instructions:");
			Console.WriteLine("Drag FC3.dll or FC3_d3d11.dll onto this program to apply the patch. A backup copy will be created with \"_orig\" added to the end of the file name.");
			Console.WriteLine("The DLL file can be found in: <FC3 INSTALL FOLDER>/bin/");
			Console.WriteLine();
			Console.WriteLine("Which DLL to patch depends on your video settings in-game:");
			Console.WriteLine("  FC3.dll if you are using DirectX9");
			Console.WriteLine("  FC3_d3d11.dll if you are using DirectX11 (default)");
			Console.WriteLine("  (If you're unsure, then just patch both DLLs)");
		}

		static void PrintUsage()
		{
			Console.WriteLine("usage: TriggersTools.FarCry3.SilentMenuPatcher.exe [-h|--help] [-T|--test] <FC3DLL>");
			Console.WriteLine();
			Console.WriteLine("arguments:");
			Console.WriteLine("  FC3DLL     FC3.dll or FC3_d3d11.dll file to patch.");
			Console.WriteLine();
			Console.WriteLine("optional arguments:");
			Console.WriteLine("  -h/--help  Show this help message.");
			Console.WriteLine("  -T/--test  Test program execution without writing to the DLL file.");
		}
	}
}
