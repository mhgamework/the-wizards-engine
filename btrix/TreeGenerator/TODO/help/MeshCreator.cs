using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

namespace TreeGenerator.help
{
    class MeshCreator
    {
        VertexBuffer vb;
        int numVertices;
        int numVerCir;
        int Cirkels;
        int numBranches;
        int maxNumBranches;
        float straal;
        float height;
        Vector4[] branches;
        public VertexPositionNormalTexture[] vertices;
        GraphicsDevice device;
        

        public MeshCreator(float _straal, float _height,int _numVertices,int _maxNumBranches,GraphicsDevice _device)
        {
            this.numVertices = _numVertices;
            this.straal = _straal;
            this.height = _height;
            this.maxNumBranches =_maxNumBranches;
            this.device = _device;
            vertices = new VertexPositionNormalTexture[_numVertices - 1];
        }
        public void CalculateVerticesCircel()
        {
            //aantal cirkels indien minimum vertices in cirkel
            float maxCir;
            maxCir=numVertices/7;
            int minCir;
            minCir = maxNumBranches * 4;
            bool vertOver;
            vertOver=true;
            bool enoughver;
            enoughver=maxCir<3;
            int i = 7;
            if(enoughver||(maxCir<minCir))
            {
                
            }
            while (vertOver)
            {
               
               
                if ((minCir+3)*i<this.numVertices-1)
                {
                    vertOver = true;
                    i++;
                }
                else
                {
                    vertOver = false;
                    numVerCir = i;
                    Cirkels = (numVertices- 1 )/ numVerCir;
                }
                
            }
               


            

        }

        public void CreateVertices()
        {
            double radiansBet = (2 * MathHelper.Pi) / numVerCir;
          for(int j=0;j<Cirkels;j++)
            {
                double r = 0;
                for (int i = 0; i < numVerCir; i++)
                {
                   
                   
                    
                        vertices[numVerCir * j + i].Position.X = (float)(Math.Sin(r)*straal);
                        vertices[numVerCir * j + i].Position.Y = (float)(Math.Cos(r)*straal);
                        vertices[numVerCir * j + i].Position.Z = (float)((height / Cirkels) * j);
                        r += radiansBet;
                 }
            }
            vb = new VertexBuffer(device,numVertices* VertexPositionNormalTexture.SizeInBytes, BufferUsage.None);
            vb.SetData<VertexPositionNormalTexture>(vertices);
        }

        public void CreateIndices()
        {

        }//todo

        public void Drawmesh(GameTime time, Matrix viewmatrix, Matrix worldmatrix, Matrix projectionmatrix)
        {
            BasicEffect effect;
            effect = new BasicEffect(device, null);
            device.Vertices[0].SetSource(vb, 0, VertexPositionNormalTexture.SizeInBytes);
            device.VertexDeclaration = new VertexDeclaration(device, VertexPositionNormalTexture.VertexElements);

            effect.View = viewmatrix;
            effect.World = worldmatrix;
            effect.Projection = projectionmatrix;

            effect.Begin();
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Begin();
                device.DrawPrimitives(PrimitiveType.LineStrip, 1, 999);
                pass.End();
            }
            effect.End();
        }
    }
}
