using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lightspeed
{
    class MainProgram
    {
        // https://github.com/Osinko/BigFloat

        // http://stackoverflow.com/questions/4124189/performing-math-operations-on-decimal-datatype-in-c
        // x - a number, from which we need to calculate the square root
        // epsilon - an accuracy of calculation of the root from our number.
        // The result of the calculations will differ from an actual value
        // of the root on less than epslion.
        public static decimal Sqrt( decimal x, decimal epsilon = 0.0m, bool preroot = !true )
        {
            if( x < 0m ) throw new OverflowException( "Cannot calculate square root from a negative number" );

            decimal current, previous;
            current = preroot ? (decimal)Math.Sqrt( (double)x ) : x;
            do
            {
                previous = current;
                if( previous == 0m ) { return 0m; }
                current = ( previous + x / previous ) / 2;
            }
            while( Math.Abs( previous - current ) > epsilon );
            return current;
        }


        public static BigFloat VelocityCalculatorRelativistic( BigFloat energy, BigFloat mass )
        {
            bool debug = false;

            try
            {
                /*BigFloat c = 299792458.0m;
                BigFloat p = energy / ( c * c );
                BigFloat m = 1.0m / mass;
                BigFloat b = 1.0m + ( p * m );
                BigFloat a = 1.0m / b;
                return c * BigFloat.Sqrt( 1.0m - a * a );*/

                if( debug ) Console.WriteLine( "mass = {0}", ( mass ).ToString( 100, true ) );
                if( debug ) Console.WriteLine( "energy = {0}", ( energy ).ToString( 100, true ) );

                BigFloat c = 299792458.0m;
                BigFloat c2 = 299792458.0m * 299792458.0m;

                if( debug ) Console.WriteLine( "c = {0}", ( c ).ToString( 100, true ) );

                //BigFloat p = energy / ( c * c );
                BigFloat p = energy / c2;

                if( debug ) Console.WriteLine( "p = {0}", ( p ).ToString( 100, true ) );

                //BigFloat m = 1.0m / mass;
                BigFloat m = BigFloat.Inverse( mass );

                if( debug ) Console.WriteLine( "m = {0}", ( m ).ToString( 100, true ) );

                //BigFloat b = 1.0m + ( p * m );
                BigFloat b = 1 + ( p * m );

                if( debug ) Console.WriteLine( "b = {0}", ( b ).ToString( 100, true ) );

                //BigFloat a = 1.0m / b;
                BigFloat a = BigFloat.Inverse( b );

                if( debug ) Console.WriteLine( "a = {0}", ( a ).ToString( 100, true ) );

                BigFloat ret_a2 = a * a;
                if( debug ) Console.WriteLine( "a2 = {0}", ( ret_a2 ).ToString( 100, true ) );

                BigFloat ret_a21 = 1 - ret_a2;
                if( debug ) Console.WriteLine( "a21 = {0}", ( ret_a21 ).ToString( 100, true ) );

                BigFloat ret_sqrt = BigFloat.Sqrt( ret_a21 );
                if( debug ) Console.WriteLine( "ret_sqrt = {0}", ( ret_sqrt ).ToString( 100, true ) );
                
                //BigFloat ret = c.Multiply( BigFloat.Sqrt( BigFloat.One.Subtract( a.Pow( 2 ) ) ) );
                BigFloat ret = c * ret_sqrt;

                if( debug ) Console.WriteLine( "ret = {0}", ( ret ).ToString( 100, true ) );

                if( ret > c )
                {
                    Console.WriteLine( "ERROR FASTER THAN LIGHT!" );
                }

                //return c * BigFloat.Sqrt( 1.0m - a * a );
                return ret;
            }
            catch( Exception e )
            {
                Console.WriteLine( e.ToString() );
                return BigFloat.MinusOne;
            }
        }

        public static BigFloat VelocityCalculatorNonRelativistic( BigFloat energy, BigFloat mass )
        {
            return BigFloat.Sqrt( ( 2.0m * energy ) / mass );
        }

        public static BigFloat VelocityError( BigFloat energy, BigFloat mass )
        {
            BigFloat a = VelocityCalculatorNonRelativistic( energy, mass );
            BigFloat b = VelocityCalculatorRelativistic( energy, mass );
            return BigFloat.Abs( a - b );
        }

        public static void Main( string[] args )
        {
            Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.BelowNormal;


            //Console.WriteLine( "{0}", BigFloat.Sqrt( BigFloat.One.ShiftDecimalRight(1) ).ToString( 100, true ) );
            Console.WriteLine( "{0}", BigFloat.Sqrt( 2 ).ToString( 1000, true ) );
            //Console.WriteLine( "{0}", BigFloat.One.ShiftDecimalRight(99).ToString( 100, true ) );
            //Console.WriteLine( "{0}",( new BigFloat( 0.000001m ) ).ToString( 100, true ) );

            //BigFloat energy = (new BigFloat(10)).Pow(20);
            //BigFloat mass =   (new BigFloat(10)).Pow(24);
            BigFloat energy = 74000000000;
            BigFloat mass =   0.000001;
            //decimal energy = 1000m;
            //decimal mass   = 0.0001m;

            /*Console.Write( "Energy: {0} E_k\tMass: {1} kg\n", energy.ToString(), mass.ToString() );
            Console.WriteLine( "   Relativistic: {0} m/s", VelocityCalculatorRelativistic( energy, mass ).ToString() );
            Console.WriteLine( "NonRelativistic: {0} m/s", VelocityCalculatorNonRelativistic( energy, mass ).ToString() );
            Console.WriteLine( "          Error: {0} m/s", VelocityError( energy, mass ).ToString() );*/

            //Console.WriteLine( "{0}", ( new BigFloat( 10 ).Pow( 100 ) ).ToString( 100, true ) );

            
            BigFloat[] energies = new BigFloat[50];
            BigFloat[] masses   = new BigFloat[50];

            energies[0] = 1e-25m;
            masses[0]   = 1e-25m;

            for( int i = 1; i < energies.Length; ++i ) { energies[i] = energies[i - 1] * 10.0m; }

            for( int i = 1; i <   masses.Length; ++i ) { masses[i]     = masses[i - 1] * 10.0m; }

            string[][] dataSheet = new string[energies.Length+1][];
            for( int i = 0; i < dataSheet.Length; ++i ) { dataSheet[i] = new string[masses.Length+1]; }

            for( int i = 0; i < dataSheet.Length; ++i ) { for( int c = 0; c < dataSheet[i].Length; ++c ) { dataSheet[i][c] = ""; } }

            for( int i = 0; i < energies.Length; ++i ) { dataSheet[i+1][0] = String.Format( "{0:e} J", energies[i] ); }

            for( int i = 0; i < masses.Length; ++i ) { dataSheet[0][i+1] = String.Format( "{0:e} kg", masses[i] ); }

            BigFloat speedOfLight = new BigFloat( 299792458.0m );

            List<Tuple<int, int>> ll = new List<Tuple<int,int>>();

            for( int i = 0; i < energies.Length; ++i )
            {
                for( int c = 0; c < masses.Length; ++c )
                {
                    /*dataSheet[i + 1][c + 1] = String.Format( "{0}", VelocityCalculatorRelativistic( energies[i], masses[c] ) );
                    dataSheet[i + 1][c + 1] = String.Format( "{0}", VelocityError( energies[i], masses[c] ) );*/

                    //dataSheet[i + 1][c + 1] = String.Format( "{0}", VelocityCalculatorRelativistic( energies[i], masses[c] ) / speedOfLight );
                    //Console.WriteLine( "{0:e} J {1:e} kg ==> {2} m/s", energies[i], masses[c], dataSheet[i + 1][c + 1] );

                    ll.Add( Tuple.Create( i, c ) );
                }
            }

            object _sync = new object();
            int count = 0;

            Parallel.ForEach( ll, new ParallelOptions { MaxDegreeOfParallelism = 8 }, (tuple_l) =>
                {
                    int i = tuple_l.Item1;
                    int c = tuple_l.Item2;
                    String s = String.Format( "{0}", VelocityCalculatorRelativistic( energies[i], masses[c] ) );
                    lock( _sync )
                    {
                        dataSheet[i + 1][c + 1] = s;
                        ++count;
                        Console.WriteLine( "{3:0.00}% {0:e} J {1:e} kg ==> {2} m/s", energies[i], masses[c], dataSheet[i + 1][c + 1], (double)count/(double)ll.Count*100.0 );
                    }
                }
            );


            string text = "";

            for( int i = 0; i < dataSheet.Length; ++i )
            {
                for( int c = 0; c < dataSheet[i].Length; ++c )
                {
                    text += dataSheet[i][c] + ",";
                }
                text += "\n";
            }

            //System.IO.File.WriteAllText( @"C:\Users\Tarpe\Desktop\Dat.csv", text );
            System.IO.File.WriteAllText( Environment.GetFolderPath( Environment.SpecialFolder.Desktop ) + @"\lightspeed_data.csv", text );


        }
    }
}
