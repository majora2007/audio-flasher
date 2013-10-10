using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Audio;
using OpenTK.Audio.OpenAL;

namespace AudioFlasher
{
    class Program
    {
        const string FILE_NAME = "C:/Users/jvmilazz/Desktop/zero-min.mp3";
        const int BUFFER_SIZE = (int) (0.5 * 44100);
        const int BUFFER_COUNT = 4;

        static object openal_lock = new object();


        [STAThread]
        public static void Main( string[] args )
        {
            // Load audio file here and pass reference to FlasherWindow.

            /*using ( AudioContext audioContext = new AudioContext() )
            {
                int source = AL.GenSource();
                int[] buffers = AL.GenBuffers(BUFFER_COUNT);
                int state;

                Console.WriteLine("Testing WaveReader({0}).ReadSamples()", FILE_NAME);
                
                Console.WriteLine("Playing");

                AL.SourceQueueBuffers(source, buffers.Length, buffers);
                AL.SourcePlay(source);

                int processedCount, queuedCount;

                do {
                    do {
                        AL.GetSource(source, ALGetSourcei.BuffersProcessed, out processedCount);
                    } while (processedCount == 0);

                    AL.GetSource(source, ALGetSourcei.BuffersQueued, out queuedCount);
                    if (queuedCount > 0)
                    {
                        AL.GetSource(source, ALGetSourcei.SourceState, out state);
                        if ((ALSourceState) state != ALSourceState.Playing)
                        {
                            AL.SourcePlay(source);
                            Console.WriteLine("r");
                        }
                    } else {
                        break;
                    }
                } while (true);

                AL.SourceStop(source);
                AL.DeleteSource(source);
                AL.DeleteBuffers(buffers);

            }*/
 
            using ( FlasherWindow flashWindow = new FlasherWindow() )
            {
                //Utilities.SetWindowTitle( flashWindow );
                flashWindow.Run( 30.0, 0.0 );
            }
        }
    }
}
