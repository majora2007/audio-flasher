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
        const string filename = @"C:/Users/jvmilazz/Desktop/one.wav";
        //static readonly string filename = Path.Combine( Path.Combine( "Data", "Audio" ), "one.wav" );
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

                /*if ( sound_data.Length >= 2 )
                {
                    int[] sound_buffer = new int[sound_data.Length / 4];
                    int count = 0;
                    for ( int i = 0; i < sound_data.Length; i += 4 )
                    {
                        sound_buffer[count] = ((sound_data[i + 1] * 256 + sound_data[i]) + (sound_data[i+3] * 256 + sound_data[i+2]))/2;
                        count++;
                    }

                    Utilities.Debug_WriteBufferToFile( filename + "_debug.txt", sound_buffer );
                }*/



                Trace.WriteLine( "Bits per Sample: " + bits_per_sample );
                Trace.WriteLine( "Sample Rate: " + sample_rate );

                // Every 2 bytes is equal to one sample. The reason for the 4 is because the audio file has sample for left ear, then sample for right ear. These should count as 1 sample.
                int seconds = (sound_data.Length/ sample_rate) / 4;
                Trace.WriteLine( "There should be " + seconds + " seconds of sound to play." );

                int pulseCount = 0;
                for ( int i = 0; i < sound_data.Length; i+=4 )
                {
                    int currentSecond = calculateSecond( i, sample_rate );
                    int leftSample = sound_data[i + 1] * 256 + sound_data[i];
                    int rightSample = sound_data[i + 2] * 256 + sound_data[i + 3];
                    int sample = leftSample + rightSample;

                    if ( sample > 200 ) // 1020 is always max per second
                    {
                        pulseCount++;
                    }

                    // BUG: TODO: This is executed twice for every second and the second execution is wrong
                    if ( hasSecondElapsed( i, sample_rate ) ) 
                    {
                        Trace.WriteLine( "Second " + currentSecond + " elapsed at index " + i );
                        Console.WriteLine( "There are {0} pulses for the {1}th second.", pulseCount, currentSecond );
                        pulseCount = 0;
                        if ( currentSecond == 10 ) break;
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

        private static int calculateSecond( int i, int sampleRate  )
        {
            if ( i > 0 ) return (i / sampleRate) / 4;
            else return 0;
        }

        // sample rate * 2 because we have 2 bytes per sample
        private static bool hasSecondElapsed( int i, int sampleRate )
        {
            if ( i == 0 ) return false;
            else return (i * 2) % sampleRate == 0;
        }

        
    }
}
