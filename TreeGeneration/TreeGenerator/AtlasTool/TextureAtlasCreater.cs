using System;
using System.Collections.Generic;
using System.Text;
using MHGameWork.TheWizards.Graphics.Xna.Graphics;
using MHGameWork.TheWizards.Graphics.Xna.Graphics.DEPRECATED;
using MHGameWork.TheWizards.Graphics.Xna.XML;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MHGameWork.TheWizards.Graphics;
//using MHGameWork.TheWizards.ServerClient.Editor;
using MHGameWork.TheWizards.ServerClient;

namespace TreeGenerator.AtlasTool
{
    class TextureAtlasCreater
    {
        
        SpriteBatch spriteBatch;
        int selectedTexture=-1;
         List<TextureUnit> Textures = new List<TextureUnit>();
        
        XNAGame game;
         public int Resolution=128;
        RenderTarget2D renderTarget;
        Texture2D endTexture;
        TextureForm form = new TextureForm();
        Texture2D lineTex;

       
        public void Initialize(XNAGame _game)
        {
            game = _game;
            
            form.Show();
            spriteBatch = new SpriteBatch(game.GraphicsDevice);


            //to create lines a pixel texture
            lineTex = new Texture2D(game.GraphicsDevice, 2, 2,1, TextureUsage.None, SurfaceFormat.Color);
            Color[] pixels = new Color[4];
            for (int i  = 0; i < pixels.Length; i++)
            {
                pixels[i] = Color.White;
            }
            pixels[0] = Color.White;
            lineTex.SetData<Color>(pixels);
        }

     
        
        Vector2 offset=new Vector2();
        bool MouseInside = false;
        bool TextureInside = true;
        public void Update()
        {
            if (game.Keyboard.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.F))
            {
                form = new TextureForm();
                form.Show();
            }
            if (form.ResolutionChanged)
            {
                Resolution = form.Resolution;
                game.GraphicsDevice.PresentationParameters.BackBufferHeight = form.Resolution;
                game.GraphicsDevice.PresentationParameters.BackBufferWidth = form.Resolution;

                form.ResolutionChanged = false;
            }

            if (form.FileAdded)
            {
                Texture2D tex;
                tex = Texture2D.FromFile(game.GraphicsDevice, form.FilePath);
                Textures.Add(new TextureUnit(tex, new Vector2(0, 0), 1f, game.GraphicsDevice));
                form.FileAdded=false;
            }
            if (form.scaleChanged&& selectedTexture>-1)
            {
                Textures[selectedTexture].Scale = form.scale;
                form.scaleChanged = false;
            }

            
            if ((game.Mouse.CursorPositionVector.X > 0 && game.Mouse.CursorPositionVector.Y > 0 && game.Mouse.CursorPositionVector.Y < game.Window.ClientBounds.Bottom && game.Mouse.CursorPositionVector.X < game.Window.ClientBounds.Right))
            {
                MouseInside = true;
            }
            else
            { MouseInside = false; }

           
            if (game.Mouse.LeftMouseJustPressed&&MouseInside)
            {

                Vector2 MousePos = new Vector2(game.Mouse.CursorPosition.X, game.Mouse.CursorPosition.Y); ;
                bool texNotChanged = true;
                for (int i = 0; i < Textures.Count; i++)
                {
                    
                    if (((Textures[i].Position.X < MousePos.X) && (MousePos.X <Textures[i].Position.X +Textures[i].Size.X) && ((Textures[i].Position.Y < MousePos.Y)&&(MousePos.Y < Textures[i].Position.Y +Textures[i].Size.Y))))
                    {
                        
                        selectedTexture=i;
                        form.scale = Textures[i].Scale;
                        texNotChanged = false;
                        offset = new Vector2(Textures[i].Position.X - MousePos.X, Textures[i].Position.Y-MousePos.Y);
                    }
                }
                if (texNotChanged)
                {
                    selectedTexture = -1;
                }
                
            }

            if (game.Mouse.LeftMousePressed && Textures.Count > 0 && selectedTexture > -1 && MouseInside)
            {
                
                //Textures[selectedTexture].Position.X += game.Mouse.RelativeX;
                //Textures[selectedTexture].Position.Y += game.Mouse.RelativeY;
              
                    Textures[selectedTexture].Position = game.Mouse.CursorPositionVector + offset;
             

            }
            if (selectedTexture > -1&&game.Mouse.LeftMouseJustReleased)
            {
                if (Textures[selectedTexture].Position.X < 0) { Textures[selectedTexture].Position.X = 0; }
                if (Textures[selectedTexture].Position.Y < 0) { Textures[selectedTexture].Position.Y = 0; }
                if (Textures[selectedTexture].Position.X + Textures[selectedTexture].Size.X > Resolution) { if(Textures[selectedTexture].Size.X<Resolution)Textures[selectedTexture].Position.X = Resolution - Textures[selectedTexture].Size.X; }
                if (Textures[selectedTexture].Position.Y + Textures[selectedTexture].Size.Y > Resolution) { if (Textures[selectedTexture].Size.Y < Resolution)Textures[selectedTexture].Position.Y = Resolution - Textures[selectedTexture].Size.Y; }


            }




            if (form.saveClicked)
            {
                SaveTexture(form.pathname);
                SaveUVToXmlFile(form.pathname);
            }

        }
      
        public void Draw()
        {
            
            spriteBatch.Begin();
            for (int i = 0; i < Textures.Count; i++)
            {
                Textures[i].Draw(spriteBatch);
                //spriteBatch.Draw(Textures[i], Position[i], null, Color.White, 0, new Vector2(0, 0), Scale[i], SpriteEffects.None, 0);
            }
            if (selectedTexture > -1)
            {
                Textures[selectedTexture].Draw(spriteBatch, Color.Red);
            }
            spriteBatch.DrawString(game.SpriteFont, game.Mouse.CursorPosition.ToString(), Vector2.Zero, Color.White);
            spriteBatch.DrawString(game.SpriteFont, MouseInside.ToString(), Vector2.UnitY*20, Color.White);
            
            
            spriteBatch.End();
            DrawLine(new Vector2(0, Resolution), new Vector2(Resolution, Resolution), Color.Black);
            DrawLine(new Vector2(Resolution, 0), new Vector2(Resolution, Resolution+2), Color.Black);


        }

        private void DrawLine(Vector2 pos1, Vector2 pos2, Color color)
        {
           
           
            float length=Vector2.Distance(pos1,pos2);
            Vector2 pos;
            if (pos1.X<=pos2.X)
	            {
		        pos=pos1;
	            }else{pos=pos2;}
               
            float AngleBetweenTwoVectors =(float)Math.Atan((pos2.Y-pos1.Y)/(pos2.X-pos1.X));
         
            spriteBatch.Begin();
            spriteBatch.Draw(lineTex, new Rectangle((int)pos.X,(int) pos.Y, (int)length, 1),new Rectangle(0, 0, 1, 1),color,AngleBetweenTwoVectors,new Vector2(0, 0),SpriteEffects.None, 0);
            
            
            spriteBatch.End();
        }

        public void SaveTexture(string pathName)
        {

            renderTarget = new RenderTarget2D(game.GraphicsDevice, Resolution, Resolution, 1, SurfaceFormat.Color, RenderTargetUsage.DiscardContents);

            game.GraphicsDevice.SetRenderTarget(0, renderTarget);
            spriteBatch.Begin();
            for (int i = 0; i < Textures.Count; i++)
            {
                Textures[i].DrawFinal(spriteBatch);
            }
            spriteBatch.End();
            game.GraphicsDevice.SetRenderTarget(0, null);
            endTexture = renderTarget.GetTexture();
            endTexture.Save(pathName+".dds", ImageFileFormat.Dds);

            form.saveClicked = false;
        }
        public void SaveUVToXmlFile(string pathName)
        {
       
            TWXmlNode node = new TWXmlNode(TWXmlNode.CreateXmlDocument(), "AtlasTextureUV");

            for (int i = 0; i < Textures.Count; i++)
            {
                TWXmlNode levelNode = node.CreateChildNode("Texture");

                float UVStartX, UVStartY, UVEndX, UVEndY;

                UVStartX = Textures[i].Position.X / Resolution;
                UVStartY = Textures[i].Position.Y / Resolution;
                UVEndX = (Textures[i].Position.X+Textures[i].Size.X )/ Resolution;
                UVEndY = (Textures[i].Position.Y + Textures[i].Size.Y) / Resolution;

                

                levelNode.AddChildNode("UVStartX", UVStartX.ToString());
                levelNode.AddChildNode("UVStartY", UVStartY.ToString());
                levelNode.AddChildNode("UVEndX", UVEndX.ToString());
                levelNode.AddChildNode("UVEndY", UVEndY.ToString());
              

               
            }
            node.Document.Save(pathName + ".XML");
            

        
        }

        public static void TestTextureAtlasCreater()
        {
            XNAGame game;
            game = new XNAGame();
            game.DrawFps = true;
            game.IsFixedTimeStep = false;
            game.RenderAxis = false;
            

            TextureAtlasCreater atlas = new TextureAtlasCreater();
            game.InitializeEvent +=
                delegate
                {
                    game.SpectaterCamera.Enabled = false;
                    game.Mouse.CursorEnabled = true;
                    game.IsMouseVisible = true;
                    Cursor cursor = new Cursor(game, new GameFile(game.Content.RootDirectory + @"Content\Cursor001.dds"), Vector2.Zero);
                    game.Cursor = cursor;
                    atlas.Initialize(game);
                };
            game.UpdateEvent +=
                delegate
                {
                    atlas.Update();

                };
            game.DrawEvent +=
                delegate
                {

                    atlas.Draw();

                };
            game.Run();
        }
    }
}
