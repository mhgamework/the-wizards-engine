using System;
using System.Collections.Generic;
using System.Text;
using MHGameWork.TheWizards.Common.Core;
using MHGameWork.TheWizards.Editor;
using MHGameWork.TheWizards.Graphics;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
namespace MHGameWork.TheWizards.ServerClient.Editor
{
    public class EditorTerrainHeightMapShader : BasicShader
    {
        private EffectParameter paramMaxHeight;
        private EffectParameter paramDisplacementMap;

        private float maxHeight;

        public float MaxHeight
        {
            get { return maxHeight; }
            set
            {
                SetValue(paramMaxHeight, ref maxHeight, value);
            }
        }

        private EditorTerrainHeightMapShader(IXNAGame game)
            : base(game)
        {
        }

        private TWTexture displacementMap;

        public TWTexture DisplacementMap
        {
            get { return displacementMap; }
            set { SetValue(paramDisplacementMap, ref displacementMap, value); }
        }



        protected override void LoadParameters()
        {
            base.LoadParameters();

            paramMaxHeight = effect.Parameters["maxHeight"];
            paramDisplacementMap = effect.Parameters["displacementMap"];

        }


        public static EditorTerrainHeightMapShader CreateFromEditor(IXNAGame game, WizardsEditor editor)
        {
            EditorTerrainHeightMapShader s = new EditorTerrainHeightMapShader(game);



            System.IO.Stream strm = EmbeddedFile.GetStream(
                "MHGameWork.TheWizards.Editor.FromEditorProject.TODO.Files.TerrainGeomipmap.fx",
                "/TerrainGeomipmap.fx");


            s.LoadFromFXFile(strm, new EffectPool());

            return s;
        }

    }
}
