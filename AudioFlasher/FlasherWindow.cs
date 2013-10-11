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
        
        // A schedule of frequencies to flash at. Each entry stores the start time of a new flash freq and the frequency itself.
        PriorityQueue<FlashChange> flashQueue = new PriorityQueue<FlashChange>();


        // Frequency in Hertz
        private int flashRate = 1;
        // Frequency in seconds
        double flashFreq = 0.0;

        private Color4 renderColor = Color4.Black;
        
        private bool isFlashing = false;

        private double timeBetweenUpdates = 0.0;
        double deltaTime = 0.0;

        double deltaFlashTime = 0.0;
        

        #region Constructor
        public FlasherWindow()
            : base( 800, 600, new GraphicsMode(16, 16))
        {

            flashFreq = Utilities.HertzToSeconds( flashRate );
            Console.WriteLine( "Flashing Frequency in seconds: {0}", flashFreq );
        }
        #endregion Constructor

        #region OnLoad
        protected override void OnLoad( EventArgs e )
        {
            base.OnLoad( e );

            const double duration = 2.0; // duration is 1 second long
            for ( int i = 1; i <= 10; i++ )
            {
                flashQueue.Enqueue( new FlashChange( Utilities.HertzToSeconds( i+2 ), i * duration ) ); // startTime should be an offset to current time, thus we can start at 0.0
                Console.WriteLine( "Flash Queue's Count: {0}", flashQueue.Count );
            }


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
            deltaFlashTime += e.Time;

            if ( isFlashing && flashQueue.Count != 0 && deltaTime >= flashQueue.Peek().StartTime )
            {
                flashFreq = flashQueue.Peek().FreqInSec; // this is not working
                deltaTime = flashQueue.Dequeue().StartTime;
                Console.WriteLine( "New Flash Freq: {0}. Next update in {1} secs.", flashFreq, flashQueue.Peek().StartTime - deltaTime );
            }

            if ( deltaFlashTime >= flashFreq )
            {
                toggleColor();
                deltaFlashTime = 0.0;

            }

            if ( Keyboard[OpenTK.Input.Key.Escape] )
            {
                Exit();
            }
            else if ( Keyboard[OpenTK.Input.Key.Space] && timeBetweenUpdates >= 0.5 )
            {
                timeBetweenUpdates = 0.0;
                isFlashing = !isFlashing;
            }
            /*else if ( Mouse[OpenTK.Input.MouseButton.Left] ) // && deltaTime >= 0.5
            {
                Console.WriteLine( "Left click occured at ({0}, {1}).", Mouse.X, Mouse.Y );
            }
            else if ( Keyboard[OpenTK.Input.Key.Up] && timeBetweenUpdates >= 0.7 )
            {
                flashRate++;
                flashFreq = Utilities.HertzToSeconds( flashRate );
                Console.WriteLine( "Flashing increased to {0}", flashRate );
            }
            else if ( Keyboard[OpenTK.Input.Key.Down] && timeBetweenUpdates >= 0.7 )
            {
                if ( flashRate - 1 < 0 ) flashRate = 0;
                else flashRate--;
                
                flashFreq = Utilities.HertzToSeconds( flashRate );
                Console.WriteLine( "Flashing decreased to {0}", flashRate );
            }*/
        }

        protected override void OnRenderFrame( OpenTK.FrameEventArgs e )
        {
            this.Title = "AudioFlasher - FPS: " + (1 / e.Time).ToString( "0." );

            GL.Clear( ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit );
            
            if ( isFlashing )
            {
                GL.ClearColor( this.renderColor );
            }

            this.SwapBuffers();
        }

        #region Private Methods

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

        private int SecondsToHertz( double secs )
        {
            return (int) (secs / 60.0);
        }

        private double HertzToSeconds( int freq )
        {
            if ( freq <= 0.0 ) return 0.0;

            return (double) (1.0 / (freq));
        }

        #endregion


    }
}
