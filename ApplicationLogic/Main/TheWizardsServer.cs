using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using DirectX11;
using MHGameWork.TheWizards.Assets;
using MHGameWork.TheWizards.Entity;
using MHGameWork.TheWizards.Gameplay;
using MHGameWork.TheWizards.Graphics;
using MHGameWork.TheWizards.Model;
using MHGameWork.TheWizards.ModelContainer;
using MHGameWork.TheWizards.Networking;
using MHGameWork.TheWizards.Networking.Packets;
using MHGameWork.TheWizards.Networking.Server;
using MHGameWork.TheWizards.OBJParser;
using MHGameWork.TheWizards.Physics;
using MHGameWork.TheWizards.Player;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.Scripting;
using MHGameWork.TheWizards.Simulation;
using MHGameWork.TheWizards.Simulation.Synchronization;
using MHGameWork.TheWizards.World;
using MHGameWork.TheWizards.World.Static;
using MHGameWork.TheWizards.XML;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StillDesign.PhysX;
using TreeGenerator.EngineSynchronisation;
using TreeGenerator.LodEngine;
using TreeGenerator.TreeEngine;
using PlayerController = MHGameWork.TheWizards.GamePlay.PlayerController;

namespace MHGameWork.TheWizards.Main
{
    /// <summary>
    /// This is the main class, the starting point of the server, and delegates all functionality to the functionality classes, but decides which functionalities are used and when they are used.
    /// This class will call other Application Logic classes that implemented some more complicated but specific functionality.
    /// </summary>
    public class TheWizardsServer
    {
        public PhysicsEngine PhysicsEngine { get; private set; }

        //private Physics.PhysicsDebugRenderer physicsDebugRenderer;

        public DX11Game Game { get; private set; }


        public TheWizardsServer()
        {
        }




        private TheWizardsServerCore core = new TheWizardsServerCore();

        public void Start()
        {
            Game = new DX11Game();
            Game.InputDisabled = true;

            Game.GameLoopEvent += delegate
                                  {
                                      core.Tick(Game.Elapsed);
                                  };

            core.Start();

            Game.Run();

            core.Stop();

        }


    }
}
