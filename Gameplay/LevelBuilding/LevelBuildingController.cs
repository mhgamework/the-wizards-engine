﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Engine.Raycasting;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.Player;
using MHGameWork.TheWizards.Raycasting;
using MHGameWork.TheWizards.WorldRendering;
using SlimDX;
using SlimDX.DirectInput;

namespace MHGameWork.TheWizards.LevelBuilding
{
    public class LevelBuildingController
    {
        private LevelBuildingInfo levelBuildingInfo;
        private readonly PlayerData player;
        private readonly LevelBuildingObjectFactory levelBuildingFactory;

        private ILevelBuildingObjectType[] quickslots;
        private Key[] quickslotKeys;

        private ILevelBuildingObjectType activeObjectType;
        private WorldRaycaster raycaster = new WorldRaycaster();

        public LevelBuildingController(PlayerData player, CameraInfo camera, LevelBuildingObjectFactory levelBuildingFactory)
        {
            this.player = player;
            this.levelBuildingFactory = levelBuildingFactory;
            levelBuildingInfo = new LevelBuildingInfo(camera, new ScalableGrid());
            quickslots = new ILevelBuildingObjectType[9];
            quickslotKeys = new Key[9] { Key.D1, Key.D2, Key.D3, Key.D4, Key.D5, Key.D6, Key.D7, Key.D8, Key.D9 };
        }

        public void Update()
        {
            updateText();
            updateGrid();

            if (TW.Graphics.Keyboard.IsKeyPressed(Key.I))
            {
                player.DisableGravity = !player.DisableGravity;
            }

            updateQuickSlots();

            if (activeObjectType != null)
                activeObjectType.ProcessInput(levelBuildingFactory, levelBuildingInfo);
            else if (TW.Graphics.Mouse.LeftMouseJustPressed)
            {
                Object o = null;
                ILevelBuildingObjectType type = null;
                getRaycastedObject(out type, out o);

                if (type != null && o != null)
                {
                    levelBuildingInfo.SelectedObject = o;
                    activeObjectType = type;
                }
            }

            if (TW.Graphics.Keyboard.IsKeyPressed(Key.X))
                activeObjectType = null;
        }

        private void updateText()
        {
            levelBuildingInfo.Textarea.Text = "CONTROLLER \nK: decrease grid spacing \nM: increase grid spacing \nO: increase grid height \nL: decrease grid height";
        }

        private void updateGrid()
        {
            float gridIncrease = 0.5f;

            if (TW.Graphics.Keyboard.IsKeyPressed(Key.O))
            {
                levelBuildingInfo.Grid.AdjustHeight(true);
                player.GroundHeight = levelBuildingInfo.Grid.Height;
            }
            if (TW.Graphics.Keyboard.IsKeyPressed(Key.L))
            {
                levelBuildingInfo.Grid.AdjustHeight(false);
                player.GroundHeight = levelBuildingInfo.Grid.Height;
            }
            if (TW.Graphics.Keyboard.IsKeyPressed(Key.K))
            {
                levelBuildingInfo.Grid.SetNodeXSize(levelBuildingInfo.Grid.NodeXSize - gridIncrease);
                levelBuildingInfo.Grid.SetNodeZSize(levelBuildingInfo.Grid.NodeZSize - gridIncrease);
            }
            if (TW.Graphics.Keyboard.IsKeyPressed(Key.Semicolon))
            {
                levelBuildingInfo.Grid.SetNodeXSize(levelBuildingInfo.Grid.NodeXSize + gridIncrease);
                levelBuildingInfo.Grid.SetNodeZSize(levelBuildingInfo.Grid.NodeZSize + gridIncrease);
            }

            levelBuildingInfo.Grid.UpdateDraw(
                levelBuildingInfo.Grid.GetSelectedPosition(levelBuildingInfo.Camera.GetCenterScreenRay()));
        }

        /// <summary>
        /// BEHAVIOUR: 
        /// - When a quickslot key is pressed: 
        /// The activeObjectType won't be updated and when during this time the user clicks on an object, 
        /// this object's type is set to the quickslot.
        /// '+' and '-' will scroll through available objectTypes.
        /// - When a quickslot key is released:
        ///  The activeObjectType will be set to the type in the quickslot
        /// - The key '0' sets the activeObjectType to null
        /// </summary>
        private void updateQuickSlots()
        {
            for (int i = 0; i < quickslotKeys.Length; i++)
            {
                if (TW.Graphics.Keyboard.IsKeyDown((Key)quickslotKeys.GetValue(i)))
                {
                    //activeObjectType = null;

                    if (TW.Graphics.Mouse.LeftMouseJustPressed)
                    {
                        ILevelBuildingObjectType selectedType = null;
                        Object selectedObject = null;
                        getRaycastedObject(out selectedType, out selectedObject);

                        if (selectedType != null && selectedObject != null)
                        {
                            quickslots.SetValue(selectedType, i);
                            levelBuildingInfo.SelectedObject = selectedObject;
                            return; //Make sure no 2 keys are pressed and recorded at the same time
                        }
                    }

                    if (TW.Graphics.Keyboard.IsKeyPressed(Key.NumberPadPlus))
                    {
                        quickslots.SetValue(levelBuildingFactory.GetNextType((ILevelBuildingObjectType)quickslots.GetValue(i)), i);

                        if (levelBuildingInfo.SelectedObject != null)
                            levelBuildingFactory.DeleteObject(levelBuildingInfo.SelectedObject);

                        activeObjectType = (ILevelBuildingObjectType)quickslots.GetValue(i);
                        return;
                    }
                    if (TW.Graphics.Keyboard.IsKeyPressed(Key.NumberPadMinus))
                    {
                        quickslots.SetValue(levelBuildingFactory.GetPreviousType((ILevelBuildingObjectType)quickslots.GetValue(i)), i);

                        if (levelBuildingInfo.SelectedObject != null)
                            levelBuildingFactory.DeleteObject(levelBuildingInfo.SelectedObject);

                        activeObjectType = (ILevelBuildingObjectType)quickslots.GetValue(i);
                        return;
                    }
                }

                if (TW.Graphics.Keyboard.IsKeyPressed((Key)quickslotKeys.GetValue(i)))
                {
                    activeObjectType = (ILevelBuildingObjectType)quickslots.GetValue(i);
                    levelBuildingInfo.SelectedObject = null;
                    return;
                }
            }

            if (TW.Graphics.Keyboard.IsKeyPressed(Key.D0))
            {
                activeObjectType = null;
                levelBuildingInfo.SelectedObject = null;
            }
        }

        private void getRaycastedObject(out ILevelBuildingObjectType type, out Object o)
        {
            o = raycaster.Raycast(levelBuildingInfo.Camera.GetCenterScreenRay()).Object;
            type = levelBuildingFactory.GetLevelBuildingTypeFromObject(o);
        }

    }
}
