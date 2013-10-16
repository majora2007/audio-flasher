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

                Trace.WriteLine( "Bits per Sample: " + bits_per_sample );
                Trace.WriteLine( "Sample Rate: " + sample_rate );

                // Every 2 bytes is equal to one sample. The reason for the 4 is because the audio file has sample for left ear, then sample for right ear. These should count as 1 sample.
                int seconds = (sound_data.Length/ sample_rate) / 4;
                Trace.WriteLine( "There should be " + seconds + " seconds of sound to play." );
                int pulseRate = 0, pulseCount = 0;
                for ( int i = 0; i < sound_data.Length; i+=4 )
                {
                    int currentSecond = 0;
                    if (i > 0) currentSecond = (i / sample_rate) / 4;
                    int leftSample = sound_data[i] + sound_data[i + 1];
                    int rightSample = sound_data[i + 2] + sound_data[i + 3];

                    int sample = leftSample + rightSample;
                    //Console.WriteLine( "Byte at second {0} is {1}", currentSecond, sample );

                    // sample rate * 2 because we have 2 bytes per sample
                    /*if ( i > 1 && (i*2) / (sample_rate) == 1 )
                    {
                        Console.WriteLine( "There were {0} bytes for 1 second.", i );
                        break;
                    }*/

                    if ( sample >= 255 )
                    {
                        pulseCount++;
                    }

                    if ( i > 1 && (i*2) % sample_rate == 0 && pulseCount > 0 )
                    {
                        pulseRate = sample_rate / pulseCount;
                        Console.WriteLine( "There are {0} pulses for the {1}th second.", pulseRate, currentSecond );
                        pulseCount = 0;
                    }
                    
                }

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
