//----------------------------------------
// Copyright (c) 2010, Dmitry Zamkov
// Open source under the new BSD License
//----------------------------------------
using System;
using System.Collections.Generic;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Input;
using OpenTK.Graphics.OpenGL;

namespace DHTW
{
    /// <summary>
    /// Main window for the applications
    /// </summary>
    public class Window : GameWindow
    {
        public Window() : base(640, 480, GraphicsMode.Default, "XSharp")
        {
            ISharpShape shape = new MandelBox(10);
            this.Tree = OctoTree.Create(shape, 5);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            GL.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            Matrix4d proj = Matrix4d.Perspective(0.7, (double)this.ClientSize.Width / (double)this.ClientSize.Height, 0.1, 10.0);
            Matrix4d lookat = Matrix4d.LookAt(new Vector3d(2.0, 2.0, 1.8), new Vector3d(0.0, 0.0, 0.0), new Vector3d(0.0, 0.0, 1.0));

            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.MultMatrix(ref proj);
            GL.MultMatrix(ref lookat);

            GL.PushMatrix();
            GL.Rotate((float)Rot, new Vector3(0.0f, 0.0f, 1.0f));
            GL.PointSize(4.0f);
            GL.Begin(BeginMode.Points);
            this._DrawOctoTree(new Vector(0.0, 0.0, 0.0), new Vector(1.0, 1.0, 1.0), this.Tree);
            GL.End();
            GL.PopMatrix();

            this.SwapBuffers();
        }

        private void _DrawOctoTree(Vector Offset, Vector Scale, OctoTree Tree)
        {
            if (Tree == OctoTree.Full)
            {
                GL.Color3(1.0f, 0.0f, 0.0f);
                GL.Vertex3(Offset);
            }
            else if (Tree == OctoTree.Empty)
            {

            }
            else
            {
                OctoTree[] children = Tree.Children;
                for (int t = 0; t < 8; t++)
                {
                    Vector offset = OctoTree.Offset(t);
                    offset.Scale(Scale);
                    _DrawOctoTree(Offset + offset, Scale * 0.5, children[t]);
                }
            }
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            this.Rot += 0.05;
        }

        /// <summary>
        /// Main entry point of the application, if you didnt already get that.
        /// </summary>
        public static void Main(string[] Args)
        {
            Window win = new Window();
            win.Run(60.0);
        }

        public double Rot = 0.0;
        public OctoTree Tree;
    }
}