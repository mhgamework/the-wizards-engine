using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace MHGameWork.TheWizards.ServerClient.Terrain.Rendering
{
    public class TerrainShader : BasicShader
    {
        private EffectParameter paramWeightmap;

        private TWTexture weightMap;

        public TWTexture WeightMap
        {
            get { return weightMap; }
            set { SetValue( paramWeightmap, ref weightMap, value ); }
        }


        private TerrainShader( IXNAGame game )
            : base( game )
        {
        }



        protected override void LoadParameters()
        {
            base.LoadParameters();

            paramWeightmap = effect.Parameters[ "WeightMap1" ];
            SetTechnique( "DrawTexturedPreprocessed" );

        }

        public static TerrainShader CreateNew( IXNAGame game )
        {
            TerrainShader s = new TerrainShader( game );
            s.LoadFromFXFile( new GameFile( System.Windows.Forms.Application.StartupPath + @"\Content\TerrainHeightMap.fx" ) );

            return s;

        }

        public static TerrainShader CreateFromServerClientMain( ServerClientMain main )
        {
            TerrainShader s = new TerrainShader( main.XNAGame );
            s.LoadFromFXFile( main.Files.TerrainShader );

            return s;
        }

    }
}