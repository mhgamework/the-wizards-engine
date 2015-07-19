﻿using System;
using MHGameWork.TheWizards.GodGame.Internal;

namespace VoxelEngine.Tests.Engine.FacadesImplementation
{
    public class DeveloperConsole : IDeveloperConsole
    {
        private DeveloperConsoleUI consoleUi;
        private readonly DelegateCommandProvider commandProvider;

        public DeveloperConsole(DeveloperConsoleUI consoleUi, DelegateCommandProvider commandProvider)
        {
            this.consoleUi = consoleUi;
            this.commandProvider = commandProvider;
        }

        public void WriteLine(string line)
        {
            consoleUi.WriteLine(line);
        }

        public void AddCommand(string name, int numArgs, Func<string[], string> action)
        {
            commandProvider.addCommand(name, numArgs, action);
        }
    }
}