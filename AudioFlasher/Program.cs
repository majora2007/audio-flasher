using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Audio;
using OpenTK.Audio.OpenAL;
using System.IO;
using System.Threading;
using System.Diagnostics;

namespace AudioFlasher
{
    class Program
    {
        const string filename = "C:/Users/jvmilazz/Desktop/zero-ten.wav";
        //static readonly string filename = Path.Combine( Path.Combine( "Data", "Audio" ), "the_ring_that_fell.wav" );
        const int BUFFER_SIZE = (int) (0.5 * 44100);
        const int BUFFER_COUNT = 4;

        static object openal_lock = new object();


        [STAThread]
        public static void Main( string[] args )
        {
            // Load audio file here and pass reference to FlasherWindow.
            using ( AudioContext context = new AudioContext() )
            {
                int buffer = AL.GenBuffer();
                int source = AL.GenSource();
                int state;

                int channels, bits_per_sample, sample_rate;
                byte[] sound_data = Playback.LoadWave( File.Open( filename, FileMode.Open ), out channels, out bits_per_sample, out sample_rate );
                AL.BufferData( buffer, Playback.GetSoundFormat( channels, bits_per_sample ), sound_data, sound_data.Length, sample_rate );

                AL.Source( source, ALSourcei.Buffer, buffer );
                AL.SourcePlay( source );

                Trace.Write( "Playing" );

                // Query the source to find out when it stops playing.
                do
                {
                    Thread.Sleep( 250 );
                    Trace.Write( "." );
                    AL.GetSource( source, ALGetSourcei.SourceState, out state );
                }
                while ( (ALSourceState) state == ALSourceState.Playing );

                Trace.WriteLine( "" );

                AL.SourceStop( source );
                AL.DeleteSource( source );
                AL.DeleteBuffer( buffer );
            }      
 
            using ( FlasherWindow flashWindow = new FlasherWindow() )
            {
                flashWindow.Run( 30.0, 0.0 );
            }
        }
    }
}
