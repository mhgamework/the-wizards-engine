using System;
using System.Collections.Generic;
using System.Text;
using MHGameWork.TheWizards.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
namespace MHGameWork.TheWizards.ServerClient
{
    public class EditorGizmoOud
    {
        public enum Axes
        {
            None = 0,
            X = 1,
            Y = 2,
            Z = 4,
            XZ = X | Z,
            XY = X | Y,
            YZ = Y | Z,
            XYZ = X | Y | Z
        }

        private XNAGame game;

        private Vector3 position;

        public Vector3 Position
        {
            get { return position; }
            set { position = value; changed = true; }
        }

        private float radius;

        public float Radius
        {
            get { return radius; }
            set { radius = value; changed = true; }
        }
        private float thickness;
        /// <summary>
        /// Radius of the center defined as a part of Radius. (radius center = Thickness * Radius)
        /// </summary>
        public float Thickness
        {
            get { return thickness; }
            set { thickness = value; }
        }

        /// <summary>
        /// Size of the squares between 2 axes = BetweenAxesScale * Radius
        /// </summary>
        private float betweenAxesScale;

        public float BetweenAxesScale
        {
            get { return betweenAxesScale; }
            set { betweenAxesScale = value; }
        }


        private BoundingBox xAxis;
        private BoundingBox yAxis;
        private BoundingBox zAxis;

        private BoundingBox xySquare;
        private BoundingBox yzSquare;
        private BoundingBox xzSquare;

        private LineManager3D lineManager;
        private bool changed;

        public EditorGizmoOud( XNAGame nGame )
        {
            game = nGame;
            lineManager = new LineManager3D( game );
            radius = 1;
            thickness = 0.1f;
            betweenAxesScale = 0.7f;
        }



        private void CalculateAxes()
        {
            if ( !changed ) return;

            float centerRadius = radius * thickness;

            Vector3 centerMin = new Vector3( position.X - centerRadius, position.Y - centerRadius, position.Z - centerRadius );
            Vector3 centerMax = new Vector3( position.X + centerRadius, position.Y + centerRadius, position.Z + centerRadius );

            Vector3 xMax = centerMax;
            xMax.X = position.X + radius;
            Vector3 yMax = centerMax;
            yMax.Y = position.Y + radius;
            Vector3 zMax = centerMax;
            zMax.Z = position.Z + radius;


            xAxis = new BoundingBox( centerMin, xMax );
            yAxis = new BoundingBox( centerMin, yMax );
            zAxis = new BoundingBox( centerMin, zMax );

            float squareRadius = radius * betweenAxesScale;
            float squareThickness = 0.01f;
            Vector3 xyMax = new Vector3();
            xyMax.Z = centerMin.Z + squareThickness;
            xyMax.X = position.X + squareRadius;
            xyMax.Y = position.Y + squareRadius;
            Vector3 yzMax = new Vector3();
            yzMax.X = centerMin.X + squareThickness;
            yzMax.Y = position.Y + squareRadius;
            yzMax.Z = position.Z + squareRadius;
            Vector3 xzMax = new Vector3();
            xzMax.Y = centerMin.Y + squareThickness;
            xzMax.X = position.X + squareRadius;
            xzMax.Z = position.Z + squareRadius;

            xySquare = new BoundingBox( centerMin, xyMax );
            xzSquare = new BoundingBox( centerMin, xzMax );
            yzSquare = new BoundingBox( centerMin, yzMax );
        }

        public void Render()
        {
            CalculateAxes();
            lineManager.AddBox( xAxis, Color.Red );
            lineManager.AddBox( yAxis, Color.Green );
            lineManager.AddBox( zAxis, Color.Blue );

            lineManager.AddBox( xySquare, Color.Yellow );
            lineManager.AddBox( yzSquare, Color.SeaGreen );
            lineManager.AddBox( xzSquare, Color.Purple );

            lineManager.Render();
        }

        public EditorRaycastResult<Axes>[] Raycast( Ray ray )
        {
            CalculateAxes();

            EditorRaycastResult<Axes>[] hits = new EditorRaycastResult<Axes>[ 6 ];

            hits[ 0 ] = new EditorRaycastResult<Axes>( ray.Intersects( xAxis ), Axes.X );
            hits[ 1 ] = new EditorRaycastResult<Axes>( ray.Intersects( yAxis ), Axes.Y );
            hits[ 2 ] = new EditorRaycastResult<Axes>( ray.Intersects( zAxis ), Axes.Z );

            hits[ 3 ] = new EditorRaycastResult<Axes>( ray.Intersects( xySquare ), Axes.XY );
            hits[ 4 ] = new EditorRaycastResult<Axes>( ray.Intersects( yzSquare ), Axes.YZ );
            hits[ 5 ] = new EditorRaycastResult<Axes>( ray.Intersects( xzSquare ), Axes.XZ );

            return hits;

        }
    }
}
