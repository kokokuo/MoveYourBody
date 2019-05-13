#region File Description
//-----------------------------------------------------------------------------
// SpherePrimitive.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace KinectProby.Primitives3D
{
    /// <summary>
    /// Geometric primitive class for drawing spheres.
    /// </summary>
    public class SpherePrimitive : GeometricPrimitive
    {
        GraphicsDevice graphicsDevice;
        /// <summary>
        /// Constructs a new sphere primitive, using default settings.
        /// </summary>
        public SpherePrimitive(GraphicsDevice graphicsDevice)
            : this(graphicsDevice, 1, 16)
        {
        }


        /// <summary>
        /// Constructs a new sphere primitive,
        /// with the specified size and tessellation level.
        /// </summary>
        public SpherePrimitive(GraphicsDevice graphicsDevice,
                               float diameter, int tessellation)
        {
            this.graphicsDevice = graphicsDevice;

            //AddVertex(Vector3.Transform(new Vector3(0, radius, 2000), Matrix.CreateRotationZ(2 * (float)Math.PI * 0 / 3)), Vector3.Forward);
            //AddVertex(Vector3.Transform(new Vector3(0, radius, 2000), Matrix.CreateRotationZ(2 * (float)Math.PI * 1 / 3)), Vector3.Forward);
            //AddVertex(Vector3.Transform(new Vector3(0, radius, 2000), Matrix.CreateRotationZ(2 * (float)Math.PI * 2 / 3)), Vector3.Forward);
            /*
            for (int i = 0; i < 5; i++)
                for (int j = 0; j < 5; j++)
                {
                    float dX = i / 5f - .5f;
                    float dY = j / 5f - .5f;
                    
                    dX =  dX * 100;
                    dY = dY * 100;
                    AddVertex(new Vector3(dX, dY, 2000), Vector3.Forward);
                }


            AddIndex(0);
            AddIndex(5);
            AddIndex(1);
             */


            for (int i = 0; i < width; i++)
                for (int j = 0; j < height; j++)
                {
                    float dX = i / width - .5f; ;
                    float dY = j / height - .5f; ;
                    AddVertex(new Vector3(dX * 1000, dY * 1000, 2000), Vector3.Forward);
                }


            for (int i = 0; i < width - 1; i++)
                for (int j = 0; j < height - 1; j++)
                {
                    AddIndex((int)(i * height) + j);
                    AddIndex((int)(i * (int)height + j + 1));
                    AddIndex((int)((i + 1) * (int)height + j));
                }

            InitializePrimitive(graphicsDevice);
        }

        float width = 640;
        float height = 480;

        public void Update(float[] data)
        {
            int p = 0;

            Random r = new Random();
            for (int j = 0; j < height; j++)
                for (int i = 0; i < width; i++)
                
                {
                    if (data[p] < 0)
                        data[p] = 1;
                    p = j * (int)width+ i;
                    VertexPositionNormal t = vertices[p];
                    t.Position.Z = data[p] * 10000f;
                    vertices[p] = t;
                }
            InitializePrimitive(graphicsDevice);
        }
    }
}
