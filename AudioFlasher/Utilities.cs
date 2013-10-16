#region --- License ---
/* Licensed under the MIT/X11 license.
 * Copyright (c) 2006-2008 the OpenTK Team.
 * This notice may not be removed from any source distribution.
 * See license.txt for licensing details.
 */
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

using OpenTK;
using OpenTK.Input;
using System.IO;

namespace AudioFlasher
{
    public static class Utilities
    {
        /// <summary>
        /// Converts a System.Drawing.Color to a System.Int32.
        /// </summary>
        /// <param name="c">The System.Drawing.Color to convert.</param>
        /// <returns>A System.Int32 containing the R, G, B, A values of the
        /// given System.Drawing.Color in the Rbga32 format.</returns>
        public static int ColorToRgba32( Color c )
        {
            return (int) ((c.A << 24) | (c.B << 16) | (c.G << 8) | c.R);
        }

        public static double HertzToSeconds( int freq )
        {
            if ( freq <= 0 ) return 0.0;
 
            return (double) (1.0 / (freq));
        }

        // Does not work
        public static string TwosComplimentMath( string value1, string value2 )
        {
            
            char[] binary1 = value1.ToCharArray();
            char[] binary2 = value2.ToCharArray();
            bool carry = false;
            char[] calcResult = new char[16]; // For 16-bit numbers

            for ( int i = 15; i >= 0; i-- )
            {
                if ( binary1[i] == binary2[i] )
                {
                    if ( binary1[i] == '1' )
                    {
                        if ( carry )
                        {
                            calcResult[i] = '1';
                            carry = true;
                        }
                        else
                        {
                            calcResult[i] = '0';
                            carry = true;
                        }
                    }
                    else
                    {
                        if ( carry )
                        {
                            calcResult[i] = '1';
                            carry = false;
                        }
                        else
                        {
                            calcResult[i] = '0';
                            carry = false;
                        }
                    }
                }
                else
                {
                    if ( carry )
                    {
                        calcResult[i] = '0';
                        carry = true;
                    }
                    else
                    {
                        calcResult[i] = '1';
                        carry = false;
                    }
                }
            }

            return new string( calcResult );
        }
    
        public static void Debug_WriteBufferToFile(string filename, int[] buffer)
        {
            using ( StreamWriter fileStream = new StreamWriter( filename ) )
            {
                foreach (byte b in buffer)
                {
                    fileStream.WriteLine(b);
                }
            }
        }
    
    }
}
