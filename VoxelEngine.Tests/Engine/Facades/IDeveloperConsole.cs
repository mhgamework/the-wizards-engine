using System;

namespace VoxelEngine.Tests.Engine
{
    /// <summary>
    /// Developer console service, provides ability to add commands and to write to console
    /// </summary>
    public interface IDeveloperConsole
    {
        void WriteLine( string line );
        void AddCommand( string name, int numArgs, Func<string[], string> action );

    }
}