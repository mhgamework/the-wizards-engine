// Project: SkinningWithColladaModelsInXna, File: FileHelper.cs
// Namespace: SkinningWithColladaModelsInXna.Helpers, Class: FileHelper
// Path: C:\code\SkinningWithColladaModelsInXna\Helpers, Author: Abi
// Code lines: 137, Size of file: 4,13 KB
// Creation date: 15.10.2006 09:08
// Last modified: 22.10.2006 18:03
// Generated with Commenter by abi.exDream.com

#region Using directives

using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Xna.Framework.Storage;

#endregion

namespace MHGameWork.TheWizards.Graphics.Xna.Graphics.TODO
{
	/// <summary>
	/// File helper class to get text lines, number of text lines, etc.
	/// Update: Now also supports the XNA Storage classes :)
	/// </summary>
	public class FileHelper
	{
		#region CreateGameContentFile
		/// <summary>
		/// Create game content file, will create file if it does not exist.
		/// Else the existing file is just loaded.
		/// </summary>
		/// <param name="relativeFilename">Relative filename</param>
		/// <returns>File stream</returns>
		public static FileStream CreateGameContentFile(string relativeFilename,
			bool createNew)
		{
			string fullPath = Path.Combine(
				StorageContainer.TitleLocation, relativeFilename);
			return File.Open(fullPath,
				createNew ? FileMode.Create : FileMode.OpenOrCreate,
				FileAccess.Write, FileShare.ReadWrite);
		} // CreateGameContentFile(relativeFilename)
		#endregion

		#region LoadGameContentFile
		/// <summary>
		/// Load game content file, returns null if file was not found.
		/// </summary>
		/// <param name="relativeFilename">Relative filename</param>
		/// <returns>File stream</returns>
		public static FileStream LoadGameContentFile(string relativeFilename)
		{
			string fullPath = Path.Combine(
				StorageContainer.TitleLocation, relativeFilename);
			if (File.Exists(fullPath) == false)
				return null;
			else
				return File.Open(fullPath,
					FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
		} // LoadGameContentFile(relativeFilename)
		#endregion

		#region SaveGameContentFile
		public static FileStream SaveGameContentFile(string relativeFilename)
		{
			string fullPath = Path.Combine(
				StorageContainer.TitleLocation, relativeFilename);
			return File.Open(fullPath,
				FileMode.Create, FileAccess.Write);
		} // SaveGameContentFile(relativeFilename)
		#endregion

		#region OpenOrCreateFileForCurrentPlayer
		/*obs, never used
		/// <summary>
		/// XNA user device, asks for the saving location on the Xbox360,
		/// theirfore remember this device for the time we run the game.
		/// </summary>
		static StorageDevice xnaUserDevice = null;

		/// <summary>
		/// Xna user device
		/// </summary>
		/// <returns>Storage device</returns>
		public static StorageDevice XnaUserDevice
		{
			get
			{
				// Create if not created yet.
				if (xnaUserDevice == null)
					xnaUserDevice =
						StorageDevice.ShowStorageDeviceGuide(PlayerIndex.One);
				return xnaUserDevice;
			} // get
		} // XnaUserDevice

		/// <summary>
		/// Open or create file for current player. Basically just creates a
		/// FileStream using the specified FileMode flag, but on the Xbox360
		/// we have to ask the user first where he wants to.
		/// Basically used for the GameSettings and the Log class.
		/// </summary>
		/// <param name="filename">Filename</param>
		/// <param name="mode">Mode</param>
		/// <param name="access">Access</param>
		/// <returns>File stream</returns>
		public static FileStream OpenFileForCurrentPlayer(
			string filename, FileMode mode, FileAccess access)
		{
			// Open a storage container
			StorageContainer container = XnaUserDevice.OpenContainer("SkinningWithColladaModelsInXna");

			// Add the container path to our filename
			string fullFilename = Path.Combine(container.Path, filename);

			// Opens or creates the requested file
			return new FileStream(
				fullFilename, mode, access, FileShare.ReadWrite);
		} // OpenFileForCurrentPlayer(filename, mode, access)
		 */
    #endregion

		#region Get text lines
		/// <summary>
		/// Returns the number of text lines we got in a file.
		/// </summary>
		static public string[] GetLines(string filename)
		{
			try
			{
				StreamReader reader = new StreamReader(
					new FileStream(filename, FileMode.Open, FileAccess.Read),
					System.Text.Encoding.UTF8);
				// Generic version
				List<string> lines = new List<string>();
				do
				{
					lines.Add(reader.ReadLine());
				} while (reader.Peek() > -1);
				reader.Close();
				return lines.ToArray();
			} // try
			catch
			{
				// Failed to read, just return null!
				return null;
			} // catch
		} // GetLines(filename)
		#endregion

		#region Create text file
		/// <summary>
		/// Create text file
		/// </summary>
		/// <param name="filename">Filename</param>
		/// <param name="textForFile">Text for file</param>
		/// <exception cref="IOException">
		/// Will be thrown if file already exists.
		/// </exception>
		public static void CreateTextFile(
			string filename, string textForFile,
			Encoding fileEncoding)
		{
			StreamWriter textWriter = new StreamWriter(
				new FileStream(filename, FileMode.Create, FileAccess.Write),
				fileEncoding);//System.Text.Encoding.UTF8);

			string[] textLines = StringHelper.SplitMultilineText(textForFile);
			foreach (string line in textLines)
				textWriter.WriteLine(line);
			textWriter.Close();
		} // CreateTextFile(filename, textForFile)
		#endregion
	}	// class FileHelper
} // namespace SkinningWithColladaModelsInXna.Helpers
