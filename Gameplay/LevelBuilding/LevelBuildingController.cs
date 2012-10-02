using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Raycasting;
using MHGameWork.TheWizards.WorldRendering;
using SlimDX;
using SlimDX.DirectInput;

namespace MHGameWork.TheWizards.LevelBuilding
{


    public class LevelBuildingController
    {
        private LevelBuildingInfo levelBuildingInfo;
        private readonly LevelBuildingObjectFactory levelBuildingFactory;

        private Array quickslots;
        private Array quickslotKeys;

        private ILevelBuildingObjectType activeObjectType;
        private WorldRaycaster raycaster = new WorldRaycaster();

        public LevelBuildingController(CameraInfo camera, LevelBuildingObjectFactory levelBuildingFactory)
        {
            this.levelBuildingFactory = levelBuildingFactory;
            levelBuildingInfo = new LevelBuildingInfo(camera, new ScalableGrid());
            quickslots = new ILevelBuildingObjectType[9];
            quickslotKeys = new Key[9] {Key.D1, Key.D2, Key.D3, Key.D4, Key.D5, Key.D6, Key.D7, Key.D8, Key.D9};
        }

        public void Update()
        {
            updateText();
            updateGrid();

            updateQuickSlots();

            if (activeObjectType != null)
                activeObjectType.ProcessInput(levelBuildingFactory, levelBuildingInfo);
            else if(TW.Game.Mouse.LeftMouseJustPressed)
            {
                Object o = null;
                ILevelBuildingObjectType type = null;
                getRaycastedObject(out type, out o);

                if(type != null && o != null)
                {
                    levelBuildingInfo.SelectedObject = o;
                    activeObjectType = type;
                }
            }

            if (TW.Game.Keyboard.IsKeyPressed(Key.X))
                activeObjectType = null;
        }

        private void updateText()
        {
            levelBuildingInfo.Textarea.Text = "CONTROLLER \nG + H: decrease grid spacing \nG + J: increase grid spacing \nG + '+/-': adjust grid height";
        }

        private void updateGrid()
        {
            float gridIncrease = 0.5f;

            if(TW.Game.Keyboard.IsKeyDown(Key.G))
            {
                if(TW.Game.Keyboard.IsKeyPressed(Key.NumberPadPlus))
                {
                    levelBuildingInfo.Grid.AdjustHeight(true);
                }
                if (TW.Game.Keyboard.IsKeyPressed(Key.NumberPadMinus))
                {
                    levelBuildingInfo.Grid.AdjustHeight(false);
                }
                if (TW.Game.Keyboard.IsKeyPressed(Key.H))
                {
                    levelBuildingInfo.Grid.SetNodeXSize(levelBuildingInfo.Grid.NodeXSize - gridIncrease);
                    levelBuildingInfo.Grid.SetNodeZSize(levelBuildingInfo.Grid.NodeZSize - gridIncrease);
                }
                if (TW.Game.Keyboard.IsKeyPressed(Key.J))
                {
                    levelBuildingInfo.Grid.SetNodeXSize(levelBuildingInfo.Grid.NodeXSize + gridIncrease);
                    levelBuildingInfo.Grid.SetNodeZSize(levelBuildingInfo.Grid.NodeZSize + gridIncrease);
                }
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
                if (TW.Game.Keyboard.IsKeyDown((Key)quickslotKeys.GetValue(i)))
                {
                    activeObjectType = null;

                    if(TW.Game.Mouse.LeftMouseJustPressed)
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

                    if(TW.Game.Keyboard.IsKeyPressed(Key.NumberPadPlus))
                    {
                        quickslots.SetValue(levelBuildingFactory.GetNextType((ILevelBuildingObjectType)quickslots.GetValue(i)), i);
                    }
                    if (TW.Game.Keyboard.IsKeyPressed(Key.NumberPadMinus))
                    {
                        quickslots.SetValue(levelBuildingFactory.GetPreviousType((ILevelBuildingObjectType)quickslots.GetValue(i)), i);
                    }
                }
                else if(TW.Game.Keyboard.IsKeyReleased((Key)quickslotKeys.GetValue(i)))
                {
                    activeObjectType = (ILevelBuildingObjectType)quickslots.GetValue(i);
                    levelBuildingInfo.SelectedObject = null;
                    return;
                }
            }

            if (TW.Game.Keyboard.IsKeyPressed(Key.D0))
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
