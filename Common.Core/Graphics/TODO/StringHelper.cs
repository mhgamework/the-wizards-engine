// Project: SkinningWithColladaModelsInXna, File: StringHelper.cs
// Namespace: SkinningWithColladaModelsInXna.Helpers, Class: StringHelper
// Path: e:\code\SkinningWithColladaModelsInXna\Helpers, Author: abi
// Code lines: 1598, Size of file: 50,82 KB
// Creation date: 25.02.2007 12:36
// Last modified: 25.02.2007 15:06
// Generated with Commenter by abi.exDream.com

#region Using directives
#if DEBUG
//using NUnit.Framework;
#endif
using System;
using System.Text;
using System.Collections;
using System.Collections.Specialized;
using System.Globalization;
using System.Diagnostics;
using System.Collections.Generic;
#endregion

namespace MHGameWork.TheWizards.ServerClient.Engine
{
    /// <summary>
    /// StringHelper: Provides additional or simplified string functions.
    /// This class does also offer a lot of powerful functions and
    /// allows complicated string operations.
    /// Easy functions at the beginning, harder ones later.
    /// </summary>
    public class StringHelper
    {
        #region Constants
        public const string NewLine = "\r\n";
        #endregion

        #region Constructor (it is not allowed to instantiate)
        /// <summary>
        /// Don't allow instantiating this class.
        /// </summary>
        private StringHelper()
        {
        } // StringHelper()
        #endregion

        #region Comparing, Counting and Extraction
        /// <summary>
        /// Check if a string (s1, can be longer as s2) begins with another
        /// string (s2), only if s1 begins with the same string data as s2,
        /// true is returned, else false. The string compare is case insensitive.
        /// </summary>
        static public bool BeginsWith( string s1, string s2 )
        {
            return String.Compare( s1, 0, s2, 0, s2.Length, true,
                CultureInfo.CurrentCulture ) == 0;
        } // BeginsWith(s1, s2)

        /// <summary>
        /// Helps to compare strings, uses case insensitive comparison.
        /// String.Compare is also gay because we have always to check for == 0.
        /// </summary>
        static public bool Compare( string s1, string s2 )
        {
            return String.Compare( s1, s2, true,
                CultureInfo.CurrentCulture ) == 0;
        } // Compare(s1, s2)

        /// <summary>
        /// Helps to compare strings, uses case insensitive comparison.
        /// String.Compare is also gay because we have always to check for == 0.
        /// This overload allows multiple strings to be checked, if any of
        /// them matches we are good to go (e.g. ("hi", {"hey", "hello", "hi"})
        /// will return true).
        /// </summary>
        static public bool Compare( string s1, string[] anyMatch )
        {
            if ( anyMatch == null )
                throw new ArgumentNullException( "anyMatch",
                    "Unable to execute method without valid anyMatch array." );

            foreach ( string match in anyMatch )
                if ( String.Compare( s1, match, true,
                    CultureInfo.CurrentCulture ) == 0 )
                    return true;
            return false;
        } // Compare(s1, anyMatch)

        /// <summary>
        /// Is a specific name in a list of strings?
        /// </summary>
        static public bool IsInList(
            string name,
            ArrayList list,
            bool ignoreCase )
        {
            if ( list == null )
                throw new ArgumentNullException( "list",
                    "Unable to execute method without valid list." );

            foreach ( string listEntry in list )
                if ( String.Compare( name, listEntry, ignoreCase,
                    CultureInfo.CurrentCulture ) == 0 )
                    return true;
            return false;
        } // IsInList(name, list, ignoreCase)

        /// <summary>
        /// Is a specific name in a list of strings?
        /// </summary>
        static public bool IsInList(
            string name,
            string[] list,
            bool ignoreCase )
        {
            if ( list == null )
                throw new ArgumentNullException( "list",
                    "Unable to execute method without valid list." );

            foreach ( string listEntry in list )
                if ( String.Compare( name, listEntry, ignoreCase,
                    CultureInfo.CurrentCulture ) == 0 )
                    return true;
            return false;
        } // IsInList(name, list, ignoreCase)

        /// <summary>
        /// Count words in a text (words are only separated by ' ' (spaces))
        /// </summary>
        static public int CountWords( string text )
        {
            if ( text == null )
                throw new ArgumentNullException( "text",
                    "Unable to execute method without valid text." );

            return text.Split( new char[] { ' ' } ).Length;
        } // CountWords(text)

        /// <summary>
        /// Compare char case insensitive
        /// </summary>
        /// <param name="c1">C 1</param>
        /// <param name="c2">C 2</param>
        /// <returns>Bool</returns>
        public static bool CompareCharCaseInsensitive( char c1, char c2 )
        {
            return char.ToLower( c1 ) == char.ToLower( c2 );
            // Another way (slower):
            // return String.Compare("" + c1, "" + c2, true) == 0;
        } // CompareCharCaseInsensitive(c1, c2)

        /// <summary>
        /// Get last word
        /// </summary>
        /// <param name="text">Text</param>
        /// <returns>String</returns>
        public static string GetLastWord( string text )
        {
            if ( text == null )
                throw new ArgumentNullException( "text",
                    "Unable to execute method without valid text." );

            string[] words = text.Split( new char[] { ' ' } );
            if ( words.Length > 0 )
                return words[ words.Length - 1 ];
            return text;
        } // GetLastWord(text)

        /// <summary>
        /// Remove last word
        /// </summary>
        /// <param name="text">Text</param>
        /// <returns>String</returns>
        public static string RemoveLastWord( string text )
        {
            string lastWord = GetLastWord( text );
            // Fix 2004-10-08: new length can be 0 for killing first word
            if ( text == lastWord )
                return "";
            else if ( lastWord.Length == 0 || text.Length == 0 ||
                text.Length - lastWord.Length - 1 <= 0 )
                return text;
            else
                return text.Substring( 0, text.Length - lastWord.Length - 1 );
        } // RemoveLastWord(text)

        /// <summary>
        /// Get all spaces and tabs at beginning
        /// </summary>
        /// <param name="text">Text</param>
        /// <returns>String</returns>
        static public string GetAllSpacesAndTabsAtBeginning( string text )
        {
            if ( text == null )
                throw new ArgumentNullException( "text",
                    "Unable to execute method without valid text." );

            StringBuilder ret = new StringBuilder();
            for ( int pos = 0; pos < text.Length; pos++ )
            {
                if ( text[ pos ] == ' ' ||
                    text[ pos ] == '\t' )
                    ret.Append( text[ pos ] );
                else
                    break;
            } // for (pos)
            return ret.ToString();
        } // GetAllSpacesAndTabsAtBeginning(text)

        /// <summary>
        /// Get tab depth
        /// </summary>
        /// <param name="text">Text</param>
        /// <returns>Int</returns>
        static public int GetTabDepth( string text )
        {
            for ( int textPos = 0; textPos < text.Length; textPos++ )
                if ( text[ textPos ] != '\t' )
                    return textPos;
            return text.Length;
        } // GetTabDepth(text)

        /// <summary>
        /// Check string word length
        /// </summary>
        public static string CheckStringWordLength(
            string originalText, int maxLength )
        {
            if ( originalText == null )
                throw new ArgumentNullException( "originalText",
                    "Unable to execute method without valid text." );

            string[] splitted = originalText.Split( new char[] { ' ' } );
            string ret = "";
            foreach ( string word in splitted )
            {
                if ( word.Length <= maxLength )
                    ret += word + " ";
                else
                {
                    for ( int i = 0; i < word.Length / maxLength; i++ )
                        ret += word.Substring( i * maxLength, maxLength ) + " ";
                } // else
            } // foreach (word, splitted)
            return ret.TrimEnd();
        } // CheckStringWordLength(originalText, maxLength)
        #endregion

        #region String contains (for case insensitive compares)
        /// <summary>
        /// Is searchName contained in textToCheck, will check case insensitive,
        /// for a normal case sensitive test use textToCheck.Contains(searchName)
        /// </summary>
        /// <param name="textToCheck">Text to check</param>
        /// <param name="searchName">Search name</param>
        /// <returns>Bool</returns>
        public static bool Contains( string textToCheck, string searchName )
        {
            return textToCheck.ToLower().Contains( searchName.ToLower() );
        } // Contains(textToCheck, searchName)

        /// <summary>
        /// Is any of the names in searchNames contained in textToCheck,
        /// will check case insensitive, for a normal case sensitive test
        /// use textToCheck.Contains(searchName).
        /// </summary>
        /// <param name="textToCheck">String to check</param>
        /// <param name="searchNames">Search names</param>
        /// <returns>Bool</returns>
        public static bool Contains( string textToCheck, string[] searchNames )
        {
            string stringToCheckLower = textToCheck.ToLower();
            foreach ( string name in searchNames )
                if ( stringToCheckLower.Contains( name.ToLower() ) )
                    return true;
            // Nothing found, no searchNames is contained in textToCheck
            return false;
        } // Contains(textToCheck, searchNames)
        #endregion

        #region Write data
        /// <summary>
        /// Returns a string with the array data, byte array version.
        /// </summary>
        static public string WriteArrayData( byte[] byteArray )
        {
            StringBuilder ret = new StringBuilder();
            if ( byteArray != null )
                for ( int i = 0; i < byteArray.Length; i++ )
                    ret.Append( ( ret.Length == 0 ? "" : ", " ) +
                        byteArray[ i ].ToString( CultureInfo.InvariantCulture.NumberFormat ) );
            return ret.ToString();
        } // WriteArrayData(byteArray)

        /// <summary>
        /// Returns a string with the array data, int array version.
        /// </summary>
        static public string WriteArrayData( int[] intArray )
        {
            StringBuilder ret = new StringBuilder();
            if ( intArray != null )
                for ( int i = 0; i < intArray.Length; i++ )
                    ret.Append( ( ret.Length == 0 ? "" : ", " ) +
                        intArray[ i ].ToString( CultureInfo.InvariantCulture.NumberFormat ) );
            return ret.ToString();
        } // WriteArrayData(intArray)

        /// <summary>
        /// Returns a string with the array data, general array version.
        /// </summary>
        static public string WriteArrayData( Array array )
        {
            StringBuilder ret = new StringBuilder();
            if ( array != null )
                for ( int i = 0; i < array.Length; i++ )
                    ret.Append( ( ret.Length == 0 ? "" : ", " ) +
                        ( array.GetValue( i ) == null ?
                        "null" : array.GetValue( i ).ToString() ) );
            return ret.ToString();
        } // WriteArrayData(array)

        /// <summary>
        /// Returns a string with the array data, general array version
        /// with maxLength bounding (will return string with max. this
        /// number of entries).
        /// </summary>
        static public string WriteArrayData( Array array, int maxLength )
        {
            StringBuilder ret = new StringBuilder();
            if ( array != null )
                for ( int i = 0; i < array.Length && i < maxLength; i++ )
                    ret.Append( ( ret.Length == 0 ? "" : ", " ) +
                        array.GetValue( i ).ToString() );
            return ret.ToString();
        } // WriteArrayData(array, maxLength)

        /// <summary>
        /// Returns a string with the array data, ArrayList version.
        /// </summary>
        static public string WriteArrayData( ArrayList array )
        {
            StringBuilder ret = new StringBuilder();
            if ( array != null )
                foreach ( object obj in array )
                    ret.Append( ( ret.Length == 0 ? "" : ", " ) + obj.ToString() );
            return ret.ToString();
        } // WriteArrayData(array)

        /// <summary>
        /// Returns a string with the array data, CollectionBase version.
        /// </summary>
        static public string WriteArrayData( CollectionBase collection )
        {
            StringBuilder ret = new StringBuilder();
            if ( collection != null )
                foreach ( object obj in collection )
                    ret.Append( ( ret.Length == 0 ? "" : ", " ) + obj.ToString() );
            return ret.ToString();
        } // WriteArrayData(collection)

#if !XBOX360
        /// <summary>
        /// Returns a string with the array data, StringCollection version.
        /// </summary>
        static public string WriteArrayData( StringCollection textCollection )
        {
            StringBuilder ret = new StringBuilder();
            if ( textCollection != null )
                foreach ( string s in textCollection )
                    ret.Append( ( ret.Length == 0 ? "" : ", " ) + s );
            return ret.ToString();
        } // WriteArrayData(textCollection)
#endif

        /// <summary>
        /// Returns a string with the array data, enumerable class version.
        /// </summary>
        static public string WriteArrayData( IEnumerable enumerableClass )
        {
            StringBuilder ret = new StringBuilder();
            if ( enumerableClass != null )
                foreach ( object obj in enumerableClass )
                    ret.Append( ( ret.Length == 0 ? "" : ", " ) + obj.ToString() );
            return ret.ToString();
        } // WriteArrayData(enumerableClass)

        /// <summary>
        /// Write into space string, useful for writing parameters without
        /// knowing the length of each string, e.g. when writing numbers
        /// (-1, 1.45, etc.). You can use this function to give all strings
        /// the same width in a table. Maybe there is already a string function
        /// for this, but I don't found any useful stuff.
        /// </summary>
        static public string WriteIntoSpaceString( string message, int spaces )
        {
            if ( message == null )
                throw new ArgumentNullException( "message",
                    "Unable to execute method without valid text." );

            // Msg is already that long or longer?
            if ( message.Length >= spaces )
                return message;

            // Create string with number of specified spaces
            char[] ret = new char[ spaces ];

            // Copy data
            int i;
            for ( i = 0; i < message.Length; i++ )
                ret[ i ] = message[ i ];
            // Fill rest with spaces
            for ( i = message.Length; i < spaces; i++ )
                ret[ i ] = ' ';

            // Return result
            return new string( ret );
        } // WriteIntoSpaceString(message, spaces)

        /// <summary>
        /// Write Iso Date (Year-Month-Day)
        /// </summary>
        public static string WriteIsoDate( DateTime date )
        {
            return date.Year + "-" +
                date.Month.ToString( "00" ) + "-" +
                date.Day.ToString( "00" );
        } // WriteIsoDate(date)

        /// <summary>
        /// Write Iso Date and time (Year-Month-Day Hour:Minute)
        /// </summary>
        public static string WriteIsoDateAndTime( DateTime date )
        {
            return date.Year + "-" +
                date.Month.ToString( "00" ) + "-" +
                date.Day.ToString( "00" ) + " " +
                date.Hour.ToString( "00" ) + ":" +
                date.Minute.ToString( "00" );
        } // WriteIsoDateAndTime(date)

        /// <summary>
        /// Write internet time
        /// </summary>
        /// <param name="time">Time</param>
        /// <param name="daylightSaving">Daylight saving</param>
        /// <returns>String</returns>
        public static string WriteInternetTime(
            DateTime time,
            bool daylightSaving )
        {
            return "@" + ( (float)( (int)( time.ToUniversalTime().AddHours(
                daylightSaving ? 1 : 0 ).TimeOfDay.
                TotalSeconds * 100000 / ( 24 * 60 * 60 ) ) ) / 100.0f ).ToString(
                NumberFormatInfo.InvariantInfo );
        } // WriteInternetTime(time, daylightSaving)
        #endregion

        #region Convert methods
        /// <summary>
        /// Convert string data to int array, string must be in the game
        /// "1, 3, 8, 7", etc. WriteArrayData is the complementar function.
        /// </summary>
        /// <returns>int array, will be null if string is invalid!</returns>
        static public int[] ConvertStringToIntArray( string s )
        {
            // Invalid?
            if ( s == null || s.Length == 0 )
                return null;

            string[] splitted = s.Split( new char[] { ' ' } );
            int[] ret = new int[ splitted.Length ];
            for ( int i = 0; i < ret.Length; i++ )
                if ( String.IsNullOrEmpty( splitted[ i ] ) == false )
                {
                    try
                    {
                        ret[ i ] = Convert.ToInt32( splitted[ i ] );
                    } // try
                    catch { } // ignore
                } // for if (String.IsNullOrEmpty)
            return ret;
        } // ConvertStringToIntArray(str)

        /// <summary>
        /// Convert string data to float array, string must be in the game
        /// "1.5, 3.534, 8.76, 7.49", etc. WriteArrayData is the complementar
        /// function.
        /// </summary>
        /// <returns>float array, will be null if string is invalid!</returns>
        static public float[] ConvertStringToFloatArray( string s )
        {
            // Invalid?
            if ( s == null || s.Length == 0 )
                return null;

            string[] splitted = s.Split( new char[] { ' ' } );
            float[] ret = new float[ splitted.Length ];
            for ( int i = 0; i < ret.Length; i++ )
                if ( String.IsNullOrEmpty( splitted[ i ] ) == false )
                {
                    try
                    {
                        ret[ i ] = Convert.ToSingle( splitted[ i ],
                            CultureInfo.InvariantCulture );
                    } // try
                    catch { } // ignore
                } // for if (String.IsNullOrEmpty)
            return ret;
        } // ConvertStringToIntArray(str)
        #endregion

        #region File stuff
        /// <summary>
        /// Extracts filename from full path+filename, cuts of extension
        /// if cutExtension is true. Can be also used to cut of directories
        /// from a path (only last one will remain).
        /// </summary>
        static public string ExtractFilename( string pathFile, bool cutExtension )
        {
            if ( pathFile == null )
                return "";

            // Update 2006-09-29: also checking for normal slashes, needed
            // for support reading 3ds max stuff.
            string[] fileName = pathFile.Split( new char[] { '\\', '/' } );
            if ( fileName.Length == 0 )
            {
                if ( cutExtension )
                    return CutExtension( pathFile );
                return pathFile;
            } // if (fileName.Length)

            if ( cutExtension )
                return CutExtension( fileName[ fileName.Length - 1 ] );
            return fileName[ fileName.Length - 1 ];
        } // ExtractFilename(pathFile, cutExtension)

        /// <summary>
        /// Get directory of path+File, if only a path is given we will cut off
        /// the last sub path!
        /// </summary>
        static public string GetDirectory( string pathFile )
        {
            if ( pathFile == null )
                return "";
            int i = pathFile.LastIndexOf( "\\" );
            if ( i >= 0 && i < pathFile.Length )
                // Return directory
                return pathFile.Substring( 0, i );
            // No sub directory found (parent of some dir is "")
            return "";
        } // GetDirectory(pathFile)

        /// <summary>
        /// Same as GetDirectory(): Get directory of path+File,
        /// if only a path is given we will cut of the last sub path!
        /// </summary>
        static public string CutOneFolderOff( string path )
        {
            // GetDirectory does exactly what we need!
            return GetDirectory( path );
        } // CutOneFolderOff(path)

        /// <summary>
        /// Splits a path into all parts of its directories,
        /// e.g. "maps\\sub\\kekse" becomes
        /// {"maps\\sub\\kekse","maps\\sub","maps"}
        /// </summary>
        static public string[] SplitDirectories( string path )
        {
            ArrayList localList = new ArrayList();
            localList.Add( path );
            do
            {
                path = CutOneFolderOff( path );
                if ( path.Length > 0 )
                    localList.Add( path );
            } while ( path.Length > 0 );

            return (string[])localList.ToArray( typeof( string ) );
        } // SplitDirectories(path)

        /// <summary>
        /// Remove first directory of path (if one exists).
        /// e.g. "maps\\mymaps\\hehe.map" becomes "mymaps\\hehe.map"
        /// Also used to cut first folder off, especially useful for relative
        /// paths. e.g. "maps\\test" becomes "test"
        /// </summary>
        static public string RemoveFirstDirectory( string path )
        {
            int i = path.IndexOf( "\\" );
            if ( i >= 0 && i < path.Length )
                // Return rest of path
                return path.Substring( i + 1 );
            // No first directory found, just return original path
            return path;
        } // RemoveFirstDirectory(path)

        /// <summary>
        /// Check if a folder is a direct sub folder of a main folder.
        /// True is only returned if this is a direct sub folder, not if
        /// it is some sub folder few levels below.
        /// </summary>
        static public bool IsDirectSubfolder( string subfolder, string mainFolder )
        {
            // First check if subFolder is really a sub folder of mainFolder
            if ( subfolder != null &&
                subfolder.StartsWith( mainFolder ) )
            {
                // Same order?
                if ( subfolder.Length < mainFolder.Length + 1 )
                    // Then it ain't a sub folder!
                    return false;
                // Ok, now check if this is direct sub folder or some sub folder
                // of mainFolder sub folder
                string folder = subfolder.Remove( 0, mainFolder.Length + 1 );
                // Check if this is really a direct sub folder
                for ( int i = 0; i < folder.Length; i++ )
                    if ( folder[ i ] == '\\' )
                        // No, this is a sub folder of mainFolder sub folder
                        return false;
                // Ok, this is a direct sub folder of mainFolder!
                return true;
            } // if (subFolder)
            // Not even any sub folder!
            return false;
        } // IsDirectSubFolder(subFolder, mainFolder)

        /// <summary>
        /// Cut of extension, e.g. "hi.txt" becomes "hi"
        /// </summary>
        static public string CutExtension( string file )
        {
            if ( file == null )
                return "";
            int l = file.LastIndexOf( '.' );
            if ( l > 0 )
                return file.Remove( l, file.Length - l );
            return file;
        } // CutExtension(file)

        /// <summary>
        /// Get extension (the stuff behind that '.'),
        /// e.g. "test.bmp" will return "bmp"
        /// </summary>
        static public string GetExtension( string file )
        {
            if ( file == null )
                return "";
            int l = file.LastIndexOf( '.' );
            if ( l > 0 && l < file.Length )
                return file.Remove( 0, l + 1 );
            return "";
        } // GetExtension(file)
        #endregion

        #region String splitting and getting it back together
        /// <summary>
        /// Performs basically the same job as String.Split, but does
        /// trim all parts, no empty parts are returned, e.g.
        /// "hi  there" returns "hi", "there", String.Split would return
        /// "hi", "", "there".
        /// </summary>
        public static string[] SplitAndTrim( string text, char separator )
        {
            ArrayList ret = new ArrayList();
            string[] splitted = text.Split( new char[] { separator } );
            foreach ( string s in splitted )
                if ( s.Length > 0 )
                    ret.Add( s );
            return (string[])ret.ToArray( typeof( string ) );
        } // SplitAndTrim(text, separator)

        /// <summary>
        /// Splits a multi line string to several strings and
        /// returns the result as a string array.
        /// Will also remove any \r, \n or space character
        /// at the end of each line!
        /// </summary>
        public static string[] SplitMultilineText( string text )
        {
            if ( text == null )
                throw new ArgumentNullException( "text",
                    "Unable to execute method without valid text." );

            ArrayList ret = new ArrayList();
            // Supports any format, only \r, only \n, normal \n\r,
            // crazy \r\n or even mixed \n\r with any format
            string[] splitted1 = text.Split( new char[] { '\n' } );
            string[] splitted2 = text.Split( new char[] { '\r' } );
            string[] splitted =
                splitted1.Length >= splitted2.Length ?
            splitted1 : splitted2;

            foreach ( string s in splitted )
            {
                // Never add any \r or \n to the single lines
                if ( s.EndsWith( "\r" ) ||
                    s.EndsWith( "\n" ) )
                    ret.Add( s.Substring( 0, s.Length - 1 ) );
                else if ( s.StartsWith( "\n" ) ||
                    s.StartsWith( "\r" ) )
                    ret.Add( s.Substring( 1 ) );
                else
                    ret.Add( s );
            } // foreach (s, splitted)

            return (string[])ret.ToArray( typeof( string ) );
        } // SplitMultiLineText(text)

        /// <summary>
        /// Build string from lines
        /// </summary>
        /// <param name="lines">Lines</param>
        /// <param name="startLine">Start line</param>
        /// <param name="startOffset">Start offset</param>
        /// <param name="endLine">End line</param>
        /// <param name="endOffset">End offset</param>
        /// <param name="separator">Separator</param>
        /// <returns>String</returns>
        static public string BuildStringFromLines(
            string[] lines,
            int startLine, int startOffset,
            int endLine, int endOffset,
            string separator )
        {
            if ( lines == null )
                throw new ArgumentNullException( "lines",
                    "Unable to execute method without valid lines." );

            // Check if all values are in range (correct if not)
            if ( startLine >= lines.Length )
                startLine = lines.Length - 1;
            if ( endLine >= lines.Length )
                endLine = lines.Length - 1;
            if ( startLine < 0 )
                startLine = 0;
            if ( endLine < 0 )
                endLine = 0;
            if ( startOffset >= lines[ startLine ].Length )
                startOffset = lines[ startLine ].Length - 1;
            if ( endOffset >= lines[ endLine ].Length )
                endOffset = lines[ endLine ].Length - 1;
            if ( startOffset < 0 )
                startOffset = 0;
            if ( endOffset < 0 )
                endOffset = 0;

            StringBuilder builder = new StringBuilder( ( endLine - startLine ) * 80 );
            for ( int lineNumber = startLine; lineNumber <= endLine; lineNumber++ )
            {
                if ( lineNumber == startLine )
                    builder.Append( lines[ lineNumber ].Substring( startOffset ) );
                else if ( lineNumber == endLine )
                    builder.Append( lines[ lineNumber ].Substring( 0, endOffset + 1 ) );
                else
                    builder.Append( lines[ lineNumber ] );

                if ( lineNumber != endLine )
                    builder.Append( separator );
            } // for (lineNumber)
            return builder.ToString();
        } // BuildStringFromLines(lines, startLine, startOffset)

        static public string BuildStringFromLines(
            string[] lines, string separator )
        {
            StringBuilder builder = new StringBuilder( lines.Length * 80 );
            for ( int lineNumber = 0; lineNumber < lines.Length; lineNumber++ )
            {
                builder.Append( lines[ lineNumber ] );
                if ( lineNumber != lines.Length - 1 )
                    builder.Append( separator );
            } // for (lineNumber)
            return builder.ToString();
        } // BuildStringFromLines(lines, separator)

        /// <summary>
        /// Build string from lines
        /// </summary>
        /// <param name="lines">Lines</param>
        /// <returns>String</returns>
        static public string BuildStringFromLines( string[] lines )
        {
            return BuildStringFromLines( lines, NewLine );
        } // BuildStringFromLines(lines)

        /// <summary>
        /// Build string from lines
        /// </summary>
        /// <param name="lines">Lines</param>
        /// <param name="startLine">Start line</param>
        /// <param name="endLine">End line</param>
        /// <param name="separator">Separator</param>
        /// <returns>String</returns>
        static public string BuildStringFromLines(
            string[] lines,
            int startLine,
            int endLine,
            string separator )
        {
            // Check if all values are in range (correct if not)
            if ( startLine < 0 )
                startLine = 0;
            if ( endLine < 0 )
                endLine = 0;
            if ( startLine >= lines.Length )
                startLine = lines.Length - 1;
            if ( endLine >= lines.Length )
                endLine = lines.Length - 1;

            StringBuilder builder = new StringBuilder( ( endLine - startLine ) * 80 );
            for ( int lineNumber = startLine; lineNumber <= endLine; lineNumber++ )
            {
                builder.Append( lines[ lineNumber ] );
                if ( lineNumber != endLine )
                    builder.Append( separator );
            } // for (lineNumber)
            return builder.ToString();
        } // BuildStringFromLines(lines, startLine, endLine)

        /// <summary>
        /// Cut modes
        /// </summary>
        public enum CutMode
        {
            Begin,
            End,
            BothEnds
        } // enum CutMode

        /// <summary>
        /// Maximum string length
        /// </summary>
        /// <param name="originalText">Original text</param>
        /// <param name="maxLength">Maximum length</param>
        /// <param name="cutMode">Cut mode</param>
        /// <returns>String</returns>
        public static string MaxStringLength( string originalText,
            int maxLength, CutMode cutMode )
        {
            if ( originalText.Length <= maxLength )
                return originalText;

            if ( cutMode == CutMode.Begin )
                return originalText.Substring(
                    originalText.Length - maxLength, maxLength );
            else if ( cutMode == CutMode.End )
                return originalText.Substring( 0, maxLength );
            else // logic: if ( cutMode == CutModes.BothEnds )
                return originalText.Substring(
                    ( originalText.Length - maxLength ) / 2, maxLength );
        } // MaxStringLength(originalText, maxLength, cutMode)

        /// <summary>
        /// Get left part of everything to the left of the first
        /// occurrence of a character.
        /// </summary>
        public static string GetLeftPartAtFirstOccurence(
            string sourceText, char ch )
        {
            if ( sourceText == null )
                throw new ArgumentNullException( "sourceText",
                    "Unable to execute this method without valid string." );

            int index = sourceText.IndexOf( ch );
            if ( index == -1 )
                return sourceText;

            return sourceText.Substring( 0, index );
        } // GetLeftPartAtFirstOccurence(sourceText, ch)

        /// <summary>
        /// Get right part of everything to the right of the first
        /// occurrence of a character.
        /// </summary>
        public static string GetRightPartAtFirstOccurrence(
            string sourceText, char ch )
        {
            if ( sourceText == null )
                throw new ArgumentNullException( "sourceText",
                    "Unable to execute this method without valid string." );

            int index = sourceText.IndexOf( ch );
            if ( index == -1 )
                return "";

            return sourceText.Substring( index + 1 );
        } // GetRightPartAtFirstOccurrence(sourceText, ch)

        /// <summary>
        /// Get left part of everything to the left of the last
        /// occurrence of a character.
        /// </summary>
        public static string GetLeftPartAtLastOccurrence(
            string sourceText, char ch )
        {
            if ( sourceText == null )
                throw new ArgumentNullException( "sourceText",
                    "Unable to execute this method without valid string." );

            int index = sourceText.LastIndexOf( ch );
            if ( index == -1 )
                return sourceText;

            return sourceText.Substring( 0, index );
        } // GetLeftPartAtLastOccurrence(sourceText, ch)

        /// <summary>
        /// Get right part of everything to the right of the last
        /// occurrence of a character.
        /// </summary>
        public static string GetRightPartAtLastOccurrence(
            string sourceText, char ch )
        {
            if ( sourceText == null )
                throw new ArgumentNullException( "sourceText",
                    "Unable to execute this method without valid string." );

            int index = sourceText.LastIndexOf( ch );
            if ( index == -1 )
                return sourceText;

            return sourceText.Substring( index + 1 );
        } // GetRightPartAtLastOccurrence(sourceText, ch)

        /// <summary>
        /// Create password string
        /// </summary>
        /// <param name="originalText">Original text</param>
        /// <returns>String</returns>
        public static string CreatePasswordString( string originalText )
        {
            if ( originalText == null )
                throw new ArgumentNullException( "originalText",
                    "Unable to execute this method without valid string." );

            string passwordString = "";
            for ( int i = 0; i < originalText.Length; i++ )
                passwordString += "*";
            return passwordString;
        } // CreatePasswordString(originalText)

        /// <summary>
        /// Helper function to convert letter to lowercase. Could someone
        /// tell me the reason why there is no function for that in char?
        /// </summary>
        public static char ToLower( char letter )
        {
            return (char)letter.ToString().ToLower(
                CultureInfo.InvariantCulture )[ 0 ];
        } // ToLower(letter)

        /// <summary>
        /// Helper function to convert letter to uppercase. Could someone
        /// tell me the reason why there is no function for that in char?
        /// </summary>
        public static char ToUpper( char letter )
        {
            return (char)letter.ToString().ToUpper(
                CultureInfo.InvariantCulture )[ 0 ];
        } // ToUpper(letter)

        /// <summary>
        /// Helper function to check if this is an lowercase letter.
        /// </summary>
        public static bool IsLowercaseLetter( char letter )
        {
            return letter == ToLower( letter );
        } // IsLowercaseLetter(letter)

        /// <summary>
        /// Helper function to check if this is an uppercase letter.
        /// </summary>
        public static bool IsUppercaseLetter( char letter )
        {
            return letter == ToUpper( letter );
        } // IsUppercaseLetter(letter)

        /// <summary>
        /// Helper function for SplitFunctionNameToWordString to detect
        /// abbreviations in the function name
        /// </summary>
        private static int GetAbbreviationLengthInFunctionName(
            string functionName, int startPos )
        {
            StringBuilder abbreviation = new StringBuilder();
            // Go through string until we reach a lower letter or it ends
            for ( int pos = startPos; pos < functionName.Length; pos++ )
            {
                // Quit if its not an uppercase letter
                if ( StringHelper.IsUppercaseLetter( functionName[ pos ] ) == false )
                    break;
                // Else just add letter
                abbreviation.Append( functionName[ pos ] );
            } // for (pos)

            // Abbreviation has to be at least 2 letters long.
            if ( abbreviation.Length >= 2 )
            {
                // If not at end of functionName, last letter belongs to next name,
                // e.g. "TW" is not a abbreviation in "HiMrTWhatsUp",
                // "AB" isn't one either in "IsABall",
                // but "PQ" is in "PQList" and "AB" is in "MyAB"
                if ( startPos + abbreviation.Length >= functionName.Length )
                    // Ok, then return full abbreviation length
                    return abbreviation.Length;
                // Else return length - 1 because of next word
                return abbreviation.Length - 1;
            } // if (abbreviation.Length)

            // No Abbreviation, just return 1
            return 1;
        } // GetAbbreviationLengthInFunctionName(functionName, startPos)

        /// <summary>
        /// Checks if letter is space ' ' or any punctuation (. , : ; ' " ! ?)
        /// </summary>
        public static bool IsSpaceOrPunctuation( char letter )
        {
            return
                letter == ' ' ||
                letter == '.' ||
                letter == ',' ||
                letter == ':' ||
                letter == ';' ||
                letter == '\'' ||
                letter == '\"' ||
                letter == '!' ||
                letter == '?' ||
                letter == '*';
        } // IsSpaceOrPunctuation(letter)

        /// <summary>
        /// Splits a function name to words, e.g.
        /// "MakeDamageOnUnit" gets "Make damage on unit".
        /// Will also detect abbreviation like TCP and leave them
        /// intact, e.g. "CreateTCPListener" gets "Create TCP listener".
        /// </summary>
        public static string SplitFunctionNameToWordString( string functionName )
        {
            if ( functionName == null ||
                functionName.Length == 0 )
                return "";

            string ret = "";
            // Go through functionName and find big letters!
            for ( int pos = 0; pos < functionName.Length; pos++ )
            {
                char letter = functionName[ pos ];
                // First letter is always big!
                if ( pos == 0 ||
                    pos == 1 && StringHelper.IsUppercaseLetter( functionName[ 1 ] ) &&
                    StringHelper.IsUppercaseLetter( functionName[ 0 ] ) ||
                    pos == 2 && StringHelper.IsUppercaseLetter( functionName[ 2 ] ) &&
                    StringHelper.IsUppercaseLetter( functionName[ 1 ] ) &&
                    StringHelper.IsUppercaseLetter( functionName[ 0 ] ) )
                    ret += StringHelper.ToUpper( letter );
                // Found uppercase letter?
                else if ( StringHelper.IsUppercaseLetter( letter ) &&
                    //also support numbers and other symbols not lower/upper letter:
                    //StringHelper.IsLowercaseLetter(letter) == false &&
                    // But don't allow space or any punctuation (. , : ; ' " ! ?)
                    StringHelper.IsSpaceOrPunctuation( letter ) == false &&
                    ret.EndsWith( " " ) == false )
                {
                    // Could be new word, but we have to check if its an abbreviation
                    int abbreviationLength = GetAbbreviationLengthInFunctionName(
                        functionName, pos );
                    // Found valid abbreviation?
                    if ( abbreviationLength > 1 )
                    {
                        // Then add it
                        ret += " " + functionName.Substring( pos, abbreviationLength );
                        // And advance pos (abbreviation is longer than 1 letter)
                        pos += abbreviationLength - 1;
                    } // if (abbreviationLength)
                    // Else just add new word (in lower letter)
                    else
                        ret += " " + StringHelper.ToLower( letter );
                } // else if
                else
                    // Just add letter
                    ret += letter;
            } // for (pos)
            return ret;
        } // SplitFunctionNameToWordString(functionName)
        #endregion

        #region Remove character
        /// <summary>
        /// Remove character from text.
        /// </summary>
        /// <param name="text">Text</param>
        /// <param name="characterToBeRemoved">Character to be removed</param>
        public static void RemoveCharacter( ref string text,
            char characterToBeRemoved )
        {
            if ( text == null )
                throw new ArgumentNullException( "text",
                    "Unable to execute method without valid text." );

            if ( text.Contains( characterToBeRemoved.ToString() ) )
                text = text.Replace( characterToBeRemoved.ToString(), "" );
        } // RemoveCharacter(text, characterToBeRemoved)
        #endregion

        #region Kb/mb name generator
        /// <summary>
        /// Write bytes, KB, MB, GB, TB message.
        /// 1 KB = 1024 Bytes
        /// 1 MB = 1024 KB = 1048576 Bytes
        /// 1 GB = 1024 MB = 1073741824 Bytes
        /// 1 TB = 1024 GB = 1099511627776 Bytes
        /// E.g. 100 will return "100 Bytes"
        /// 2048 will return "2.00 KB"
        /// 2500 will return "2.44 KB"
        /// 1534905 will return "1.46 MB"
        /// 23045904850904 will return "20.96 TB"
        /// </summary>
        public static string WriteBigByteNumber(
            long bigByteNumber, string decimalSeperator )
        {
            if ( bigByteNumber < 0 )
                return "-" + WriteBigByteNumber( -bigByteNumber );

            if ( bigByteNumber <= 999 )
                return bigByteNumber + " Bytes";
            if ( bigByteNumber <= 999 * 1024 )
            {
                double fKB = (double)bigByteNumber / 1024.0;
                return (int)fKB + decimalSeperator +
                    ( (int)( fKB * 100.0f ) % 100 ).ToString( "00" ) + " KB";
            } // if
            if ( bigByteNumber <= 999 * 1024 * 1024 )
            {
                double fMB = (double)bigByteNumber / ( 1024.0 * 1024.0 );
                return (int)fMB + decimalSeperator +
                    ( (int)( fMB * 100.0f ) % 100 ).ToString( "00" ) + " MB";
            } // if
            // this is very big ^^ will not fit into int
            if ( bigByteNumber <= 999L * 1024L * 1024L * 1024L )
            {
                double fGB = (double)bigByteNumber / ( 1024.0 * 1024.0 * 1024.0 );
                return (int)fGB + decimalSeperator +
                    ( (int)( fGB * 100.0f ) % 100 ).ToString( "00" ) + " GB";
            } // if
            //if ( num <= 999*1024*1024*1024*1024 )
            //{
            double fTB = (double)bigByteNumber / ( 1024.0 * 1024.0 * 1024.0 * 1024.0 );
            return (int)fTB + decimalSeperator +
                ( (int)( fTB * 100.0f ) % 100 ).ToString( "00" ) + " TB";
            //} // if
        } // WriteBigByteNumber(num, decimalSeperator)

        /// <summary>
        /// Write bytes, KB, MB, GB, TB message.
        /// 1 KB = 1024 Bytes
        /// 1 MB = 1024 KB = 1048576 Bytes
        /// 1 GB = 1024 MB = 1073741824 Bytes
        /// 1 TB = 1024 GB = 1099511627776 Bytes
        /// E.g. 100 will return "100 Bytes"
        /// 2048 will return "2.00 KB"
        /// 2500 will return "2.44 KB"
        /// 1534905 will return "1.46 MB"
        /// 23045904850904 will return "20.96 TB"
        /// </summary>
        public static string WriteBigByteNumber( long bigByteNumber )
        {
            string decimalSeperator = CultureInfo.CurrentCulture.
                NumberFormat.CurrencyDecimalSeparator;
            return WriteBigByteNumber( bigByteNumber, decimalSeperator );
        } // WriteBigByteNumber(num)
        #endregion

        #region Try parse methods that are not available on the XBox360!
        /// <summary>
        /// Is numeric float
        /// </summary>
        /// <param name="str">Str</param>
        /// <returns>Bool</returns>
        public static bool IsNumericFloat( string str )
        {
            return IsNumericFloat( str, CultureInfo.InvariantCulture.NumberFormat );
        } // IsNumericFloat(str)

        /// <summary>
        /// Allow only one decimal point, used for IsNumericFloat.
        /// </summary>
        /// <param name="str">Input string to check</param>
        /// <param name="numberFormat">Used number format, e.g.
        /// CultureInfo.InvariantCulture.NumberFormat</param>
        /// <return>True if check succeeded, false otherwise</return>
        private static bool AllowOnlyOneDecimalPoint( string str,
            NumberFormatInfo numberFormat )
        {
            char[] strInChars = str.ToCharArray();
            bool hasGroupSeperator = false;
            int decimalSeperatorCount = 0;
            for ( int i = 0; i < strInChars.Length; i++ )
            {
                if ( numberFormat.CurrencyDecimalSeparator.IndexOf( strInChars[ i ] ) == 0 )
                {
                    decimalSeperatorCount++;
                } // if (numberFormat.CurrencyDecimalSeparator.IndexOf)

                // has float group seperators  ?
                if ( numberFormat.CurrencyGroupSeparator.IndexOf( strInChars[ i ] ) == 0 )
                {
                    hasGroupSeperator = true;
                } // if (numberFormat.CurrencyGroupSeparator.IndexOf)
            } // for (int)

            if ( hasGroupSeperator )
            {
                // If first digit is the group seperator or begins with 0,
                // there is something wrong, the group seperator is used as a comma.
                if ( str.StartsWith( numberFormat.CurrencyGroupSeparator ) ||
                    strInChars[ 0 ] == '0' )
                    return false;

                // look only at the digits in front of the decimal point
                string[] splittedByDecimalSeperator = str.Split(
                    numberFormat.CurrencyDecimalSeparator.ToCharArray() );

                #region Invert the digits for modulo check
                //   ==> 1.000 -> 000.1  ==> only after 3 digits 
                char[] firstSplittedInChars = splittedByDecimalSeperator[ 0 ].ToCharArray();
                int arrayLength = firstSplittedInChars.Length;
                char[] firstSplittedInCharsInverted = new char[ arrayLength ];
                for ( int i = 0; i < arrayLength; i++ )
                {
                    firstSplittedInCharsInverted[ i ] =
                        firstSplittedInChars[ arrayLength - 1 - i ];
                } // for (int)
                #endregion

                // group seperators are only allowed between 3 digits -> 1.000.000
                for ( int i = 0; i < arrayLength; i++ )
                {
                    if ( i % 3 != 0 && numberFormat.CurrencyGroupSeparator.IndexOf(
                        firstSplittedInCharsInverted[ i ] ) == 0 )
                    {
                        return false;
                    } // if (i)
                } // for (int)
            } // if (hasGroupSeperator)
            if ( decimalSeperatorCount > 1 )
                return false;

            return true;
        } // AllowOnlyOneDecimalPoint(str, numberFormat)

        /// <summary>
        /// Checks if string is numeric float value
        /// </summary>
        /// <param name="str">Input string</param>
        /// <param name="numberFormat">Used number format, e.g.
        /// CultureInfo.InvariantCulture.NumberFormat</param>
        /// <returns>True if str can be converted to a float,
        /// false otherwise</returns>
        public static bool IsNumericFloat( string str,
            NumberFormatInfo numberFormat )
        {
            // Can't be a float if string is not valid!
            if ( String.IsNullOrEmpty( str ) )
                return false;

            //not supported by Convert.ToSingle:
            //if (str.EndsWith("f"))
            //	str = str.Substring(0, str.Length - 1);

            // Only 1 decimal point is allowed
            if ( AllowOnlyOneDecimalPoint( str, numberFormat ) == false )
                return false;

            // + allows in the first,last,don't allow in middle of the string
            // - allows in the first,last,don't allow in middle of the string
            // $ allows in the first,last,don't allow in middle of the string
            // , allows in the last,middle,don't allow in first char of the string
            // . allows in the first,last,middle, allows in all the indexs
            bool retVal = false;

            // If string is just 1 letter, don't allow it to be a sign
            if ( str.Length == 1 &&
                "+-$.,".IndexOf( str[ 0 ] ) >= 0 )
                return false;

            for ( int i = 0; i < str.Length; i++ )
            {
                // For first indexchar
                char pChar =
                    //char.Parse(str.Substring(i, 1));
                    Convert.ToChar( str.Substring( i, 1 ) );

                if ( retVal )
                    retVal = false;

                if ( ( !retVal ) && ( str.IndexOf( pChar ) == 0 ) )
                {
                    retVal = ( "+-$.0123456789".IndexOf( pChar ) >= 0 ) ? true : false;
                } // if ()
                // For middle characters
                if ( ( !retVal ) && ( str.IndexOf( pChar ) > 0 ) &&
                    ( str.IndexOf( pChar ) < ( str.Length - 1 ) ) )
                {
                    retVal = ( ",.0123456789".IndexOf( pChar ) >= 0 ) ? true : false;
                } // if ()
                // For last characters
                if ( ( !retVal ) && ( str.IndexOf( pChar ) == ( str.Length - 1 ) ) )
                {
                    retVal = ( "+-$,.0123456789".IndexOf( pChar ) >= 0 ) ? true : false;
                } // if ()

                if ( !retVal )
                    break;
            } // for (int)

            return retVal;
        } // IsNumericFloat(str, numberFormat)

        /// <summary>
        /// Try to convert to float. Will not modify value if that does not work. 
        /// This uses also always the invariant culture.
        /// </summary>
        /// <param name="value">Value</param>
        /// <param name="textToConvert">Text to convert</param>
        public static void TryToConvertToFloat(
            ref float value, string textToConvert )
        {
            TryToConvertToFloat( ref value, textToConvert,
                System.Globalization.NumberFormatInfo.InvariantInfo );
        } // TryToConvertToFloat(value, textToConvert)

        /// <summary>
        /// Try to convert to float. Will not modify value if that does not work.
        /// </summary>
        /// <param name="value">Value</param>
        /// <param name="textToConvert">Text to convert</param>
        /// <param name="format">Format for converting</param>
        public static void TryToConvertToFloat(
            ref float value, string textToConvert, NumberFormatInfo format )
        {
            // Basically the same as float.TryParse(), but faster!
            if ( IsNumericFloat( textToConvert, format ) )
            {
                value = Convert.ToSingle( textToConvert, format );
            } // if (IsNumericFloat)
        } // TryToConvertToFloat(value, textToConvert, format)

        /// <summary>
        /// Check if string is numeric integer. A decimal point is not accepted.
        /// </summary>
        /// <param name="str">String to check</param>
        public static bool IsNumericInt( string str )
        {
            // Can't be an int if string is not valid!
            if ( String.IsNullOrEmpty( str ) )
                return false;

            // Go through every letter in str
            int strPos = 0;
            foreach ( char ch in str )
            {
                // Only 0-9 are allowed
                if ( "0123456789".IndexOf( ch ) < 0 &&
                    // Allow +/- for first char
                    ( strPos > 0 || ( ch != '-' && ch != '+' ) ) )
                    return false;
                strPos++;
            } // foreach (ch in str)

            // All fine, return true, this is a number!
            return true;
        } // IsNumericInt(str)
        #endregion


        public static float StringToFloat( string text )
        {
            return Convert.ToSingle( text, CultureInfo.InvariantCulture );
        }


        #region Unit Testing
#if DEBUG
        /*include NUnit.Framework for this
		/// <summary>
		/// String helper tests
		/// </summary>
		//[TestFixture]
		public class StringHelperTests
		{
			/// <summary>
			/// Test is in list
			/// </summary>
			//[Test]
			public void TestIsInList()
			{
				Assert.IsTrue(IsInList("whats",
					new string[] { "hi", "whats", "up?" }, false));
				Assert.IsFalse(IsInList("no way",
					new string[] { "omg", "no no", "there is no way!" }, false));
			} // TestIsInList()

			/// <summary>
			/// Test write array
			/// </summary>
			//[Test]
			public void TestWriteArray()
			{
				Assert.AreEqual(
					"3, 5, 10",
					WriteArrayData(new int[] { 3, 5, 10 }));
				Assert.AreEqual(
					"one, after, another",
					WriteArrayData(new string[] { "one", "after", "another" }));
				List<string> genericList = new List<string>();
				genericList.Add("whats");
				genericList.AddRange(new string[] { "going", "on" });
				Assert.AreEqual(
					"whats, going, on",
					WriteArrayData(genericList));
			} // TestWriteArray()

			/// <summary>
			/// Test split string and combine again
			/// </summary>
			//[Test]
			public void TestSplitStringAndCombineAgain()
			{
				string testText =
					@"using System;

namespace TestVs2003
{
	public class Form1 : System.Windows.Forms.Form
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() 
		{
			Application.Run(new Form1());
		}
	}
}";
				// Test splitting lines and combine them back together
				string[] lines = SplitMultilineText(testText);
				Assert.IsTrue(lines.Length > 0, "no lines were returned");
				string reconstructedText = BuildStringFromLines(lines);
				Assert.AreEqual(testText, reconstructedText,
					"testText is not equal to reconstructedText");

				// Also test other functions
				string[] words = SplitAndTrim(testText, ' ');
				Assert.AreEqual(words.Length, CountWords(testText), "words.Length");
				Assert.AreEqual(testText, BuildStringFromLines(words, " "));
				// Call with some invalid values
				BuildStringFromLines(words, 1, -3, 4, 6, " ");
				BuildStringFromLines(words, 1, 3, 400, 6, " ");
				BuildStringFromLines(words, 1, 3, 4, 600, " ");
				// Check is we can build a part back together
				// Check if we can build a part back together
				Assert.AreEqual(
					testText.Substring(10, 20),
					BuildStringFromLines(lines, 0, 10, 2, 12, NewLine));

				// Get section code and check if we got everything right
				Assert.AreEqual(
					@"static void Main() 
		{
			Application.Run(new Form1());
		}",
					BuildStringFromLines(lines, 10, 2, 13, 3, NewLine));

				// Also check if we change some line we still got the same
				// number of resulting lines.
				lines[13] += " // Test comment";
				Assert.AreEqual(lines.Length,
					SplitMultilineText(BuildStringFromLines(lines, NewLine)).Length);
			} // TestSplitStringAndCombineAgain()

			/// <summary>
			/// Test split multiline text
			/// </summary>
			//[Test]
			public void TestSplitMultilineText()
			{
				Assert.AreEqual(4,
					StringHelper.SplitMultilineText(
					@"test
now an empty lie

thats it").Length);
				// Test few empty lines
				Assert.AreEqual(6,
					StringHelper.SplitMultilineText(
					"hi\n\n\n\n\rthere\n").Length);
				// Test all combination of '\r' and '\n'
				Assert.AreEqual(2,
					StringHelper.SplitMultilineText(
					"test1" + '\r' + "test2").Length);
				Assert.AreEqual(2,
					StringHelper.SplitMultilineText(
					"test1" + '\n' + "test2").Length);
				Assert.AreEqual(2,
					StringHelper.SplitMultilineText(
					"test1" + "\n\r" + "test2").Length);
				Assert.AreEqual(2,
					StringHelper.SplitMultilineText(
					"test1" + "\r\n" + "test2").Length);
				// Also test if mixing normal "\n\r" with '\r' or '\n'
				Assert.AreEqual(3,
					StringHelper.SplitMultilineText(
					"test1" + '\n' + "" + '\r' + "test2" +
					'\r' + "test3").Length);
				Assert.AreEqual(3,
					StringHelper.SplitMultilineText(
					"test1" + '\n' + "" + '\r' + "test2" +
					'\n' + "test3").Length);
			} // TestSplitMultilineText()

			/// <summary>
			/// Test get tab depth
			/// </summary>
			//[Test]
			public void TestGetTabDepth()
			{
				Assert.AreEqual(4,
					StringHelper.GetTabDepth("\t\t\t\tWhats up?"));
			} // TestGetTabDepth()

			/// <summary>
			/// Test is lowercase or uppercase letter
			/// </summary>
			//[Test]
			public void TestIsLowercaseOrUppercaseLetter()
			{
				Assert.AreEqual(true, IsLowercaseLetter('u'));
				Assert.AreEqual(true, IsUppercaseLetter('U'));
				// Well, numbers don't care if they upper or lower letters.
				Assert.AreEqual(true, IsLowercaseLetter('3'));
				Assert.AreEqual(true, IsUppercaseLetter('3'));
			} // TestIsLowercaseOrUppercaseLetter()

			/// <summary>
			/// Test split function name to word string
			/// </summary>
			//[Test]
			public void TestSplitFunctionNameToWordString()
			{
				Assert.AreEqual(
					"Make damage on unit",
					SplitFunctionNameToWordString("MakeDamageOnUnit"));
				Assert.AreEqual(
					"This is a test",
					SplitFunctionNameToWordString("ThisIsATest"));
				Assert.AreEqual(
					"Create TCP listener",
					SplitFunctionNameToWordString("CreateTCPListener"));
				Assert.AreEqual(
					"Lower letter test",
					SplitFunctionNameToWordString("lowerLetterTest"));
				Assert.AreEqual(
					"Test 3D render function",
					SplitFunctionNameToWordString("Test3DRenderFunction"));
				Assert.AreEqual(
					"Check if CD is inserted",
					SplitFunctionNameToWordString("CheckIfCDIsInserted"));

				// Test with text that already got spaces (we expect same stuff
				// returned)
				Assert.AreEqual(
					"Ha ha, I have spaces already",
					SplitFunctionNameToWordString("ha ha, I have spaces already"));
				// Test mixed text
				Assert.AreEqual(
					"Hey what means What the hell?",
					SplitFunctionNameToWordString("Hey what means WhatTheHell?"));

				// Test some crazy strings to test if we get no exceptions
				SplitFunctionNameToWordString("\0\t\n\raoeu\b\t");
				SplitFunctionNameToWordString("@#%#@$<RRROEU<Y$G SINDTBRPI\"'45453'");
			} // TestSplitFunctionNameToWordString()

			#region Test get extension
			/// <summary>
			/// Test the GetExtension method.
			/// </summary>
			//[Test]
			public void TestGetExtension()
			{
				Assert.AreEqual("bmp",
					StringHelper.GetExtension("SomeBitmap.bmp"));
				Assert.AreEqual("xml",
					StringHelper.GetExtension("SomeApplication.config.xml"));
			} // TestGetExtension()
			#endregion
		} // class StringHelperTests
		 */
#endif
        #endregion
    } // class StringHelper
} // namespace SkinningWithColladaModelsInXna.Helpers
