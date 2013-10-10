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
        private int FLASH_RATE = 1;

        private Color4 renderColor = Color4.Black;
        double deltaTime = 0.0;
        private bool isFlashing = false;

        private double timeBetweenUpdates = 0.0;

        // Frequency in seconds
        double flashFreq = 0.0;

        #region Constructor
        public FlasherWindow()
            : base( 800, 600, new GraphicsMode(16, 16))
        {
            flashFreq = Utilities.HertzToSeconds( FLASH_RATE );
            Console.WriteLine( "Flashing Frequency in seconds: {0}", flashFreq );
        }
        #endregion Constructor

        #region OnLoad
        protected override void OnLoad( EventArgs e )
        {
            base.OnLoad( e );

            this.VSync = VSyncMode.Off;

            #region GL State
            Color4 color = Color4.MidnightBlue;
            GL.ClearColor( color.R, color.G, color.B, color.A );
            GL.Enable( EnableCap.DepthTest );
            GL.Enable( EnableCap.CullFace );
            GL.FrontFace( FrontFaceDirection.Ccw );
            GL.PolygonMode( MaterialFace.Front, PolygonMode.Fill );
            #endregion GL State
        }
        #endregion OnLoad

        protected override void OnResize( EventArgs e )
        {
            GL.Viewport( 0, 0, Width, Height );
        }

        protected override void OnUpdateFrame( OpenTK.FrameEventArgs e )
        {
            timeBetweenUpdates += e.Time;

            deltaTime += e.Time;
            if ( deltaTime >= flashFreq )
            {
                toggleColor();
                deltaTime = 0.0;
            }

            if ( Keyboard[OpenTK.Input.Key.Escape] )
            {
                this.Exit();
                return;
            }
            else if ( Mouse[OpenTK.Input.MouseButton.Left] ) // && deltaTime >= 0.5
            {
                Console.WriteLine( "Left click occured at ({0}, {1}).", Mouse.X, Mouse.Y );
            }
            else if ( Keyboard[OpenTK.Input.Key.Space] && timeBetweenUpdates  >= 0.5)
            {
                timeBetweenUpdates = 0.0;
                bool temp = isFlashing;
                isFlashing = !isFlashing;

                Console.WriteLine( "Is Flashing: {0} -> {1}", temp, isFlashing );
            }
            else if ( Keyboard[OpenTK.Input.Key.Up] && timeBetweenUpdates >= 0.5 )
            {
                FLASH_RATE++;
                flashFreq = Utilities.HertzToSeconds( FLASH_RATE );
                Console.WriteLine( "Flashing increased to {0}", FLASH_RATE );
            }
            else if ( Keyboard[OpenTK.Input.Key.Down] && timeBetweenUpdates >= 0.5 )
            {
                if ( FLASH_RATE - 1 < 0 ) FLASH_RATE = 0;
                else FLASH_RATE--;
                
                flashFreq = Utilities.HertzToSeconds( FLASH_RATE );
                Console.WriteLine( "Flashing decreased to {0}", FLASH_RATE );
            }
        }

        protected override void OnRenderFrame( OpenTK.FrameEventArgs e )
        {
            this.Title = "FPS: " + (1 / e.Time).ToString( "0." );

            GL.Clear( ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit );
            
            if ( isFlashing )
            {
                GL.ClearColor( this.renderColor );
            }

            this.SwapBuffers();
        }


        private void toggleColor()
        {
            if ( this.renderColor == Color4.Black )
            {
                this.renderColor = Color4.Purple;
            }
            else
            {
                this.renderColor = Color4.Black;
            }
        }


    }
}
