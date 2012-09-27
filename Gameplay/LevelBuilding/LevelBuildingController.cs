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
            updateQuickSlots();

            if (activeObjectType != null)
                activeObjectType.ProcessInput(levelBuildingFactory, levelBuildingInfo);

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
                        ILevelBuildingObjectType selected = null;
                        var rayCastResult = raycaster.Raycast(levelBuildingInfo.Camera.GetCenterScreenRay());
                        if(rayCastResult is ILevelBuildingObjectType)
                            selected = (ILevelBuildingObjectType) rayCastResult;

                        if (selected == null)
                            return;

                        quickslots.SetValue(selected, i);
                        return; //Make sure no 2 keys are pressed and recorded at the same time
                    }

                    if(TW.Game.Keyboard.IsKeyDown(Key.NumberPadPlus) && !TW.Game.Keyboard.IsKeyPressed(Key.NumberPadPlus))
                    {
                        quickslots.SetValue(levelBuildingFactory.GetNextType((ILevelBuildingObjectType)quickslots.GetValue(i)), i);
                    }
                    if (TW.Game.Keyboard.IsKeyDown(Key.NumberPadMinus) && !TW.Game.Keyboard.IsKeyPressed(Key.NumberPadMinus))
                    {
                        quickslots.SetValue(levelBuildingFactory.GetPreviousType((ILevelBuildingObjectType)quickslots.GetValue(i)), i);
                    }
                }
                else if(TW.Game.Keyboard.IsKeyReleased((Key)quickslotKeys.GetValue(i)))
                {
                    activeObjectType = (ILevelBuildingObjectType)quickslots.GetValue(i);
                    return;
                }
            }

            if (TW.Game.Keyboard.IsKeyPressed(Key.D0))
                activeObjectType = null;
        }

        /* TODO: Move to something that implements ILevelBuildingObjectType
         * Ray ray = camera.GetCenterScreenRay();
            Vector3 cPos = grid.GetSelectedPosition(ray);

            if (activeObjectType != null)
            {
                activeObjectType.Position = cPos;
            }

            if (TW.Game.Mouse.LeftMouseJustPressed && activeObjectType != null)
            {
                buildingData.AddLevelBuildingObject(activeObjectType);
                activeObjectType = null;
            }

            if (TW.Game.Mouse.RightMouseJustPressed && activeObjectType != null)
            {
                activeObjectType.Rotation = activeObjectType.Rotation *
                                        Quaternion.RotationAxis(Vector3.UnitY, (float)Math.PI * 0.5f);
            }
         */
    }
}
