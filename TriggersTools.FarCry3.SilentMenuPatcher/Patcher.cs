using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace TriggersTools.FarCry3.SilentMenuPatcher
{
	public class PatchException : Exception
	{
		public string FileName { get; }

		public PatchException(string fileName) : base()
		{
			FileName = fileName;
		}
		public PatchException(string fileName, string message) : base(message)
		{
			FileName = fileName;
		}
		public PatchException(string fileName, string message, Exception innerException) : base(message, innerException)
		{
			FileName = fileName;
		}
	}

	public class AlreadyPatchedException : PatchException
	{
		public AlreadyPatchedException(string fileName) : base(fileName) { }
		public AlreadyPatchedException(string fileName, string message) : base(fileName, message) { }
		public AlreadyPatchedException(string fileName, string message, Exception innerException) : base(fileName, message, innerException) { }
	}

	public class Patcher
	{
		// CREDITS: Patch method described by koorashi from reddit post in December 2012.
		// LINK: <https://old.reddit.com/r/farcry/comments/15q4en/goddamn_that_bassy_repetitive_noise_on_the_pause/c7ozuna/>
		// ARCHIVED: <http://web.archive.org/web/20220411160830/https://old.reddit.com/r/farcry/comments/15q4en/goddamn_that_bassy_repetitive_noise_on_the_pause/>
		private static readonly string[] SilentMenu_ImportNames = {
			"sndPauseMenuStopId",
			"sndPauseMenuStartId",
			"sndLoadingScreenStopId",
			"sndLoadingScreenStartId",
		};

		public static void ApplySilentMenuPatch(string dllPath, bool backup, string backupPostfix, bool test = false)
		{
			//if (!File.Exists(dllPath))
			//	throw new FileNotFoundException("DLL file path does not exist.");
			
			List<string> missingNames = new();

			byte[] data = File.ReadAllBytes(dllPath);
			foreach (string importName in SilentMenu_ImportNames) {
				int removedCount = RemoveImportName(data, Encoding.ASCII.GetBytes(importName));
				if (removedCount == 0) {
					missingNames.Add(importName);
				}
			}

			if (missingNames.Count == SilentMenu_ImportNames.Length) {
				// No names were found, assume the file was already been patched.
				throw new AlreadyPatchedException(dllPath, "DLL file is already patched (Expected data not found. All import names are missing).");
			}
			else if (missingNames.Count > 0) {
				// All 4 names are expected to exist.
				// It's possible the patch would still function otherwise, but this is unknown territory.
				throw new PatchException(dllPath, "Patcher could not find expected data. Missing import names: " + string.Join(", ", missingNames));
			}

			if (backup) {
				BackupFile(dllPath, backupPostfix, false);
			}

			if (!test) {
				File.WriteAllBytes(dllPath, data);
			}
			//return true;
		}

		// Zeroes out occurrences of ASCII text in the data.
		static int RemoveImportName(byte[] data, byte[] importName)
		{
			int importLength = importName.Length;
			int length = data.Length - importLength;

			int count = 0;
			for (int i = 0; i < length; i++) {
				if (data.AsSpan(i, importLength).SequenceEqual(importName)) {
					// Zero-out bytes containing the import name.
					for (int j = 0; j < importLength; j++) {
						data[i + j] = 0;
					}
					count++;
					i += importLength - 1; // skip remainder of name
				}
			}
			return count;
		}

		public static string GetBackupPath(string filePath, string backupPostfix) {
			return Path.Combine(
				Path.GetDirectoryName(filePath),
				Path.GetFileNameWithoutExtension(filePath) + backupPostfix + Path.GetExtension(filePath)
			);
		}

		public static bool BackupFile(string filePath, string backupPostfix, bool overwrite) {
			//if (!File.Exists(filePath))
			//	throw new FileNotFoundException("File path does not exist.");

			string backupPath = GetBackupPath(filePath, backupPostfix);

			if (overwrite || !File.Exists(backupPath)) {
				File.Copy(filePath, backupPath, overwrite);
				return true;
			}
			return false;
		}
	}
}
