//By Jason Zelsnack, All rights reserved

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Graphics = Microsoft.Xna.Framework.Graphics;


namespace NovodexWrapper
{
	public class NovodexDebugRenderer
	{
		public static readonly int MAX_NUM_POINTS = 1000000;
		public static readonly int MAX_NUM_LINES = 1000000;
		public static readonly int MAX_NUM_TRIANGLES = 1000000;

		private GraphicsDevice renderDevice = null;

		private VertexPositionColor[] pointVert = null;
		private VertexPositionColor[] lineVert = null;
		private VertexPositionColor[] triangleVert = null;
		private int pointCapacity, lineCapacity, triangleCapacity;
		private bool zBufferEnabledFlag = false;
		private bool drawLineShadows = false;
		private Vector3 shadowOffset = new Vector3(0, 0, 0);


		Matrix worldMatrix;

		VertexDeclaration basicEffectVertexDeclaration;
		BasicEffect basicEffect;



		public NovodexDebugRenderer(GraphicsDevice nRenderDevice)
			: this(nRenderDevice, 5000, 5000, 5000)
		{

		}

		protected NovodexDebugRenderer(GraphicsDevice renderDevice, int startNumPoints, int startNumLines, int startNumTriangles)
		{


			setRenderDevice(renderDevice);
			initPrimitiveCapacities(startNumPoints, startNumLines, startNumTriangles);
		}

		public void setRenderDevice(GraphicsDevice newRenderDevice)
		{ 
			renderDevice = newRenderDevice;

			if (renderDevice == null) return;

			worldMatrix = Matrix.Identity;

			basicEffect = new BasicEffect(renderDevice, null);


			//basicEffect.FogEnabled = true;

			//basicEffect.FogColor = Color.CornflowerBlue.ToVector3();

			//basicEffect.FogStart = 50;
			//basicEffect.FogEnd = 100;

			basicEffect.VertexColorEnabled = true;

			basicEffect.LightingEnabled = false;

			basicEffect.World = Matrix.Identity;


			basicEffectVertexDeclaration = new VertexDeclaration(renderDevice, VertexPositionColor.VertexElements);




		}

		private void initPrimitiveCapacities(int numPoints, int numLines, int numTriangles)
		{
			pointVert = new VertexPositionColor[Math.Min(numPoints, MAX_NUM_POINTS)];
			lineVert = new VertexPositionColor[Math.Min(numLines, MAX_NUM_LINES) * 2];
			triangleVert = new VertexPositionColor[Math.Min(numTriangles, MAX_NUM_TRIANGLES) * 3];
			pointCapacity = pointVert.Length;
			lineCapacity = lineVert.Length / 2;
			triangleCapacity = triangleVert.Length / 3;
		}

		protected void setPointCapacity(int numPoints)
		{
			if (numPoints > pointCapacity && pointCapacity < MAX_NUM_POINTS)
			{ pointVert = new VertexPositionColor[Math.Min(numPoints, MAX_NUM_POINTS)]; }
			pointCapacity = pointVert.Length;
		}

		protected void setLineCapacity(int numLines)
		{
			if (numLines > lineCapacity && lineCapacity < MAX_NUM_LINES)
			{ lineVert = new VertexPositionColor[Math.Min(numLines, MAX_NUM_LINES) * 2]; }
			lineCapacity = lineVert.Length / 2;
		}

		protected void setTriangleCapacity(int numTriangles)
		{
			if (numTriangles > triangleCapacity && triangleCapacity < MAX_NUM_TRIANGLES)
			{ triangleVert = new VertexPositionColor[Math.Min(numTriangles, MAX_NUM_TRIANGLES) * 3]; }
			triangleCapacity = triangleVert.Length / 3;
		}

		protected void setPrimitiveCapacities(int numPoints, int numLines, int numTriangles)
		{
			setPointCapacity(numPoints);
			setLineCapacity(numLines);
			setTriangleCapacity(numTriangles);
		}

		public bool DrawLineShadows
		{
			get { return drawLineShadows; }
			set { drawLineShadows = value; }
		}

		public Vector3 ShadowOffset
		{
			get { return shadowOffset; }
			set { shadowOffset = value; }
		}

		public bool ZBufferEnabled
		{
			get { return zBufferEnabledFlag; }
			set { zBufferEnabledFlag = value; }
		}

		public void renderData(NxDebugRenderable data, Matrix viewMatrix, Matrix projMatrix)
		{
			//MHGW
			if (data == null) { return; }

			if (renderDevice == null)
			{ return; }

			if (data.getNbPoints() > pointCapacity)
			{ setPointCapacity(pointCapacity * 2); }
			if (data.getNbLines() > lineCapacity)
			{ setLineCapacity(lineCapacity * 2); }
			if (data.getNbTriangles() > triangleCapacity)
			{ setTriangleCapacity(triangleCapacity * 2); }

			NxDebugPoint[] pointArray = data.getPoints();
			NxDebugLine[] lineArray = data.getLines();
			NxDebugTriangle[] triangleArray = data.getTriangles();

			System.Drawing.Color c;

			//WARNING: het gebruik van system.drawing.Color als conversiemiddel hieronder is 
			//          waarsch nogal traag

			int numPoints = Math.Min(pointArray.Length, pointVert.Length);
			for (int i = 0; i < numPoints; i++)
			{
				pointVert[i].Position = pointArray[i].p;
				c = System.Drawing.Color.FromArgb((int)pointArray[i].color);
				pointVert[i].Color = new Color(c.R, c.G, c.B, c.A);
			}

			int numLines = Math.Min(lineArray.Length, lineVert.Length / 2);
			for (int i = 0; i < numLines; i++)
			{
				lineVert[(i * 2) + 0].Position = lineArray[i].p0;
				c = System.Drawing.Color.FromArgb((int)lineArray[i].color);
				lineVert[(i * 2) + 0].Color = new Color(c.R, c.G, c.B, c.A);
				lineVert[(i * 2) + 1].Position = lineArray[i].p1;
				c = System.Drawing.Color.FromArgb((int)lineArray[i].color);
				lineVert[(i * 2) + 1].Color = new Color(c.R, c.G, c.B, c.A);
			}

			int numTriangles = Math.Min(triangleArray.Length, triangleVert.Length / 3);
			for (int i = 0; i < numTriangles; i++)
			{
				triangleVert[(i * 3) + 0].Position = triangleArray[i].p0;
				c = System.Drawing.Color.FromArgb((int)triangleArray[i].color);
				triangleVert[(i * 3) + 0].Color = new Color(c.R, c.G, c.B, c.A);
				triangleVert[(i * 3) + 1].Position = triangleArray[i].p1;
				c = System.Drawing.Color.FromArgb((int)triangleArray[i].color);
				triangleVert[(i * 3) + 1].Color = new Color(c.R, c.G, c.B, c.A);
				triangleVert[(i * 3) + 2].Position = triangleArray[i].p2;
				c = System.Drawing.Color.FromArgb((int)triangleArray[i].color);
				triangleVert[(i * 3) + 2].Color = new Color(c.R, c.G, c.B, c.A);
			}


			//Cache zBuffer and lighting states
			/*bool last_zBufferEnabled = renderDevice.RenderState.DepthBufferEnable;
			// //bool last_lighting = renderDevice.RenderState.Lighting;

		

			renderDevice.RenderState.DepthBufferEnable = zBufferEnabledFlag;*/
			renderDevice.RenderState.PointSize = 10;
			

			basicEffect.View = viewMatrix;
			basicEffect.Projection = projMatrix;
			
			renderDevice.VertexDeclaration = basicEffectVertexDeclaration;





			/*if (drawLineShadows)
			{
				//Create a squished matrix at the ground
				renderDevice.Transform.World = NovodexUtil.createMatrix(new Vector3(1, 0, 0), new Vector3(0, 0.001f, 0), new Vector3(0, 0, 1), shadowOffset);
				//Setting lighting to true with a squished matrix will make the shadow lines black
				renderDevice.RenderState.Lighting = true;
				renderDevice.VertexFormat = CustomVertex.PositionColored.Format;
				renderDevice.DrawUserPrimitives(PrimitiveType.LineList, numLines, lineVert);
			}*/

			// //renderDevice.RenderState.Lighting = false;

			//Set matrix to identity because the data is in worldspace
			basicEffect.World = Matrix.Identity;
			
			basicEffect.Begin();
			foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes)
			{
				pass.Begin();
				
				//Render triangles first

				if (numTriangles > 0) renderDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.TriangleList, triangleVert, 0, numTriangles);

				//Draw lines over triangles
				if (numLines > 0) renderDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineList, lineVert, 0, numLines);

				//Draw points over lines and triangles	(this needs to be better because 1 pixel points suck)

				//renderDevice.RenderState.PointSize = 5; //MHGW

				if (numPoints > 0) renderDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.PointList, pointVert, 0, numPoints);


				pass.End();
			}
			basicEffect.End();
		
			//put zBuffer and lighting back the way they were
			/*renderDevice.RenderState.DepthBufferEnable = last_zBufferEnabled;
			// //renderDevice.RenderState.Lighting = last_lighting;

			renderDevice.RenderState.DepthBufferEnable = true;*/
		}




		/*public void drawTrianglesFromVertexTriplets(Vector3[] triangleTripletArray, int color)
		{
			int lineVertIndex = 0;
			int numTris = triangleTripletArray.Length / 3;
			numTris = Math.Min(numTris, MAX_NUM_LINES / 3);
			if (numTris * 3 > lineCapacity)
			{ setLineCapacity(numTris * 3 * 2); }

			int numLines = numTris * 3;
			for (int i = 0; i < numTris; i++)
			{
				for (int j = 0; j < 3; j++)
				{
					lineVert[lineVertIndex].Position = triangleTripletArray[(i * 3) + j];
					lineVert[lineVertIndex].Color = color;
					lineVertIndex++;
					lineVert[lineVertIndex].Position = triangleTripletArray[(i * 3) + ((j + 1) % 3)];
					lineVert[lineVertIndex].Color = color;
					lineVertIndex++;
				}
			}


			//Cache zBuffer and lighting states
			bool last_zBufferEnabled = renderDevice.RenderState.ZBufferEnable;
			bool last_lighting = renderDevice.RenderState.Lighting;

			renderDevice.RenderState.ZBufferEnable = zBufferEnabledFlag;

			if (drawLineShadows)
			{
				//Create a squished matrix at the ground
				renderDevice.Transform.World = NovodexUtil.createMatrix(new Vector3(1, 0, 0), new Vector3(0, 0.001f, 0), new Vector3(0, 0, 1), shadowOffset);
				//Setting lighting to true with a squished matrix will make the shadow lines black
				renderDevice.RenderState.Lighting = true;
				renderDevice.VertexFormat = CustomVertex.PositionColored.Format;
				renderDevice.DrawUserPrimitives(PrimitiveType.LineList, numLines, lineVert);
			}

			renderDevice.RenderState.Lighting = false;

			//Set matrix to identity because the data is in worldspace
			renderDevice.Transform.World = Matrix.Identity;

			//Draw lines over triangles
			renderDevice.VertexFormat = CustomVertex.PositionColored.Format;
			renderDevice.DrawUserPrimitives(PrimitiveType.LineList, numLines, lineVert);

			//put zBuffer and lighting back the way they were
			renderDevice.RenderState.ZBufferEnable = last_zBufferEnabled;
			renderDevice.RenderState.Lighting = last_lighting;
		}*/
	}
}


