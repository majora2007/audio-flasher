using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Platform;

namespace AudioFlasher
{
    class FlasherWindow : GameWindow
    {
        #region Constructor
        public FlasherWindow()
            : base( 800, 600 )
        {

        }
        #endregion Constructor

        #region OnLoad
        protected override void OnLoad( EventArgs e )
        {
            base.OnLoad( e );

            Color4 color = Color4.MidnightBlue;
            GL.ClearColor( color.R, color.G, color.B, color.A );
            GL.Enable( EnableCap.DepthTest );
        }
        #endregion OnLoad

        protected override void OnResize( EventArgs e )
        {
            GL.Viewport( 0, 0, Width, Height );
        }

        protected override void OnUpdateFrame( OpenTK.FrameEventArgs e )
        {
            if ( Keyboard[OpenTK.Input.Key.Escape] )
            {
                this.Exit();
                return;
            }
        }

        protected override void OnRenderFrame( OpenTK.FrameEventArgs e )
        {
            GL.Clear( ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit );

            GL.Color3( 0.0f, 0.0f, 1.0f );

            this.SwapBuffers();
        }


    }
}
