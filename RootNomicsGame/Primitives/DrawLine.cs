using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RootNomics.Primitives
{
    class DrawLine
    {
        private CameraTransforms cameraTransforms;
        private BasicEffect basicEffect;

        public DrawLine(GraphicsDevice graphicsDevice, CameraTransforms cameraTransforms)
        {
            this.cameraTransforms = cameraTransforms;
            this.basicEffect = new BasicEffect(graphicsDevice);
            // -- enable per-polygon vertex colors
            basicEffect.VertexColorEnabled = true;
        }

        public void DrawLinePrimitive(GraphicsDevice graphicsDevice, Vector3[] vertices, Color color)
        {
            VertexPositionColor[] vertexList = new VertexPositionColor[2];
            vertexList[0] = new VertexPositionColor(vertices[0], color);
            vertexList[1] = new VertexPositionColor(vertices[1], color);
            ApplyCameraTransform();
            basicEffect.CurrentTechnique.Passes[0].Apply();
            graphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineList, vertexList, 0, 1);
        }

        private void ApplyCameraTransform()
        {
            basicEffect.World = cameraTransforms.worldMatrix;
            basicEffect.View = cameraTransforms.viewMatrix;
            basicEffect.Projection = cameraTransforms.projectionMatrix;
        }

        public void DrawTestLine(GraphicsDevice graphicsDevice)
        {
            Vector3[] vertices = new Vector3[2];
            vertices[0] = new Vector3(0, 0, 0);
            vertices[1] = new Vector3(0, 2f, 0);
            DrawLinePrimitive(graphicsDevice, vertices, Color.Blue);
        }

        public void DrawAxis(GraphicsDevice graphicsDevice)
        {
            Vector3[] positiveX = new Vector3[2];
            positiveX[0] = new Vector3(0, 0, 0);
            positiveX[1] = new Vector3(2f, 0, 0);
            DrawLinePrimitive(graphicsDevice, positiveX, Color.Red);
            Vector3[] negativeX = new Vector3[2];
            negativeX[0] = new Vector3(0, 0, 0);
            negativeX[1] = new Vector3(-2f, 0, 0);
            DrawLinePrimitive(graphicsDevice, negativeX, Color.Black);

            Vector3[] positiveY = new Vector3[2];
            positiveY[0] = new Vector3(0, 0, 0);
            positiveY[1] = new Vector3(0, 2f, 0);
            DrawLinePrimitive(graphicsDevice, positiveY, Color.Green);
            Vector3[] negativeY = new Vector3[2];
            negativeY[0] = new Vector3(0, 0, 0);
            negativeY[1] = new Vector3(0, -2f, 0);
            DrawLinePrimitive(graphicsDevice, negativeY, Color.Black);

            Vector3[] positiveZ = new Vector3[2];
            positiveZ[0] = new Vector3(0, 0, 0);
            positiveZ[1] = new Vector3(0, 0, 2f);
            DrawLinePrimitive(graphicsDevice, positiveZ, Color.Blue);
            Vector3[] negativeZ = new Vector3[2];
            negativeZ[0] = new Vector3(0, 0, 0);
            negativeZ[1] = new Vector3(0, 0, -2f);
            DrawLinePrimitive(graphicsDevice, negativeZ, Color.Black);

        }
    }
}


