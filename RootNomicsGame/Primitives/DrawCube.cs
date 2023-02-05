using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RootNomics.Primitives
{
    class DrawCube
    {

        private CameraTransforms cameraTransforms;
        private BasicEffect basicEffect;
        private readonly static Color DEFAULT_COLOR = Color.Green;

        public DrawCube(GraphicsDevice graphicsDevice, CameraTransforms cameraTransforms) :
            this(graphicsDevice, cameraTransforms, DEFAULT_COLOR)
        { }

        public DrawCube(GraphicsDevice graphicsDevice, CameraTransforms cameraTransforms, Color color)
        {
            this.cameraTransforms = cameraTransforms;
            this.basicEffect = new BasicEffect(graphicsDevice);
            basicEffect.VertexColorEnabled = true; // enables apply color to the vertices
            this.color = color;
        }

        private Color color;

        private const PrimitiveType TRIANGLE_LIST = PrimitiveType.TriangleList;
        private const int VERTEX_OFFSET = 0;
        public void DrawCubeAsPrimitives(GraphicsDevice graphicsDevice, Vector3 cubeCenter, float side, float depth)
        {
            //VertexPositionColor[] vertexList = new VertexPositionColor[3];
            //vertexList[0] = new VertexPositionColor(vertices[0], color);
            //vertexList[1] = new VertexPositionColor(vertices[1], color);
            //vertexList[2] = new VertexPositionColor(vertices[2], color);

            List<VertexPositionColor> vertexList = new();
            // vertexList.Add(new VertexPositionColor(new Vector3(0, 0, 0), color));


            PopulateVerticesForSide(vertexList, cubeCenter, side / 2, side / 2, depth / 2, 1, 0, 0);
            PopulateVerticesForSide(vertexList, cubeCenter, side / 2, side / 2, depth / 2, -1, 0, 0);
            PopulateVerticesForSide(vertexList, cubeCenter, side / 2, side / 2, depth / 2, 0, 1, 0);
            PopulateVerticesForSide(vertexList, cubeCenter, side / 2, side / 2, depth / 2, 0, -1, 0);
            PopulateVerticesForSide(vertexList, cubeCenter, depth / 2, side / 2, side / 2, 0, 0, 1);
            PopulateVerticesForSide(vertexList, cubeCenter, depth / 2, side / 2, side / 2, 0, 0, -1);

            basicEffect.World = cameraTransforms.worldMatrix;
            basicEffect.View = cameraTransforms.viewMatrix;
            basicEffect.Projection = cameraTransforms.projectionMatrix;
            basicEffect.CurrentTechnique.Passes[0].Apply();
            int primitiveCount = vertexList.Count / 3;
            graphicsDevice.DrawUserPrimitives<VertexPositionColor>(TRIANGLE_LIST, vertexList.ToArray(), VERTEX_OFFSET, primitiveCount);
        }


        private void PopulateVerticesForSide(List<VertexPositionColor> vertexList,
            Vector3 cubeCenter, float disp, float halfside1, float halfside2, int dx, int dy, int dz)
        {
            (Vector3 up, Vector3 s1, Vector3 s2, bool rev) v = (dx, dy, dz) switch
            {
                (1, 0, 0) => (Vector3.UnitX, -Vector3.UnitY, Vector3.UnitZ, false),
                (-1, 0, 0) => (-Vector3.UnitX, Vector3.UnitY, Vector3.UnitZ, false),
                (0, 1, 0) => (Vector3.UnitY, Vector3.UnitX, Vector3.UnitZ, true),
                (0, -1, 0) => (-Vector3.UnitY, -Vector3.UnitX, Vector3.UnitZ, true),
                (0, 0, 1) => (Vector3.UnitZ, -Vector3.UnitX, Vector3.UnitY, false),
                (0, 0, -1) => (-Vector3.UnitZ, -Vector3.UnitX, -Vector3.UnitY, true),
                _ => throw new Exception($"Invalid argument (dx,dy,dz)=({dx},{dy},{dz})")
            };

            Vector3 squareCenter = cubeCenter + Vector3.Multiply(v.up, disp);
            Vector3 v0 = squareCenter + Vector3.Multiply(v.s1, halfside1) + Vector3.Multiply(v.s2, halfside2);
            Vector3 v1 = squareCenter - Vector3.Multiply(v.s1, halfside1) + Vector3.Multiply(v.s2, halfside2);
            Vector3 v2 = squareCenter - Vector3.Multiply(v.s1, halfside1) - Vector3.Multiply(v.s2, halfside2);
            Vector3 v3 = squareCenter + Vector3.Multiply(v.s1, halfside1) - Vector3.Multiply(v.s2, halfside2);


            //Debug.WriteLine($"squareCenter = {squareCenter}");
            Debug.WriteLine($"v0 = {v0}");
            Debug.WriteLine($"v1 = {v1}");
            Debug.WriteLine($"v2 = {v2}");
            Debug.WriteLine($"v3 = {v3}");


            // Using same winding rule
            // [0]   [1]
            // [3]   [2]
            // triangle1  { 0,1,2}
            // triangle2  { 0,2,3}
            vertexList.Add(new VertexPositionColor(v0, color));
            vertexList.Add(new VertexPositionColor(v1, color));
            vertexList.Add(new VertexPositionColor(v2, color));
            vertexList.Add(new VertexPositionColor(v0, color));
            vertexList.Add(new VertexPositionColor(v2, color));
            vertexList.Add(new VertexPositionColor(v3, color));
        }



        /*
        private Vector3 SquareCentre(Vector3 cubeCenter, int dx, int dy, int dz)
        {

        }
        private void PopulateVerticesForSquare(List<VertexPositionColor> vertexList,
            Vector3 center, Vector3 unit1, Vector3 unit2, float side, Color color)
        {

        }
        */






    }
}
