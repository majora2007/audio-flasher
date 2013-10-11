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

        #region Flashing Variables
        // A schedule of frequencies to flash at. Each entry stores the start time of a new flash freq and the frequency itself.
        PriorityQueue<FlashChange> flashQueue = new PriorityQueue<FlashChange>();

        // Frequency in Hertz
        private int flashRate = 1;
        // Frequency in seconds
        double flashFreq = 0.0;
        #endregion Flashing Variables

        #region Rendering Variables
        private Color4 renderColor = Color4.Black;
        // The index which dictates what the pulse color is.
        private int colorIndex = 0;
        private Color4[] colors = new Color4[10] { Color4.Blue, Color4.Green, Color4.Red, Color4.Purple, Color4.RosyBrown,
                                                    Color4.Salmon, Color4.ForestGreen, Color4.Fuchsia, Color4.DarkRed, Color4.DarkSlateGray
                                                 };
        #endregion Rendering Variables

        #region Control Variables
        private bool isFlashing = false;
        //private bool isPaused = false;
        #endregion Control Variables

        #region Time Variables
        private double timeBetweenUpdates = 0.0;
        double deltaTime = 0.0;
        double deltaFlashTime = 0.0;
        #endregion Time Variables

        
        public FlasherWindow()
            : base( 800, 600, new GraphicsMode(16, 16))
        {

            flashFreq = Utilities.HertzToSeconds( flashRate );
            Console.WriteLine( "Flashing Frequency in seconds: {0}", flashFreq );
        }

        #region OnLoad
        protected override void OnLoad( EventArgs e )
        {
            base.OnLoad( e );

            /*const double duration = 2.0; // duration is 1 second long
            for ( int i = 1; i <= 10; i++ )
            {
                flashQueue.Enqueue( new FlashChange( Utilities.HertzToSeconds( i+2 ), i * duration ) ); // startTime should be an offset to current time, thus we can start at 0.0
                Console.WriteLine( "Flash Queue's Count: {0}", flashQueue.Count );
            }*/

            GenerateAlphaFlashSchedule( ref flashQueue );
            Console.WriteLine( "Flash Queue's Count: {0}", flashQueue.Count );


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
                
                FlashChange change = flashQueue.Peek();
                if ( change == null )
                {
                    Console.WriteLine( "New Flash Freq: {0}. No more updates", flashFreq );
                }
                else
                {
                    Console.WriteLine( "New Flash Freq: {0}. Next update in {1} secs.", flashFreq, flashQueue.Peek().StartTime - deltaTime );
                }
                
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
            else if ( Keyboard[OpenTK.Input.Key.ControlLeft] || Keyboard[OpenTK.Input.Key.ControlRight] )
            {
                if ( Keyboard[OpenTK.Input.Key.Number0] ) this.colorIndex = 0;
                else if ( Keyboard[OpenTK.Input.Key.Number1] ) this.colorIndex = 1;
                else if ( Keyboard[OpenTK.Input.Key.Number2] ) this.colorIndex = 2;
                else if ( Keyboard[OpenTK.Input.Key.Number3] ) this.colorIndex = 3;
                else if ( Keyboard[OpenTK.Input.Key.Number4] ) this.colorIndex = 4;
                else if ( Keyboard[OpenTK.Input.Key.Number5] ) this.colorIndex = 5;
                else if ( Keyboard[OpenTK.Input.Key.Number6] ) this.colorIndex = 6;
                else if ( Keyboard[OpenTK.Input.Key.Number7] ) this.colorIndex = 7;
                else if ( Keyboard[OpenTK.Input.Key.Number8] ) this.colorIndex = 8;
                else if ( Keyboard[OpenTK.Input.Key.Number9] ) this.colorIndex = 9;

                //else if ( Keyboard[OpenTK.Input.Key.P] ) this.isPaused = !this.isPaused;
            }

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
                this.renderColor = colors[colorIndex];
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

        private void GenerateAlphaFlashSchedule( ref PriorityQueue<FlashChange> queue )
        {
            const int alphaLow = 8;
            const int alphaHigh = 13;
            Random rnd = new Random();

            for ( int i = 1; i <= 10; i++ )
            {
                flashQueue.Enqueue( new FlashChange( Utilities.HertzToSeconds( rnd.Next( alphaLow, alphaHigh ) ), rnd.NextDouble() * 10.0 + 1.0) ); // startTime should be an offset to current time, thus we can start at 0.0
                Console.WriteLine( "Flash Queue's Count: {0}", flashQueue.Count );
            }
        }

        #endregion


    }
}
