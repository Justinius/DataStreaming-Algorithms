using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StreamingAlgorithms
{
    /// <summary>
    /// Class that will compute the slope, intercept, and correlation
    /// between two variables in one pass
    /// </summary>
    public class RunningRegression
    {
        RunningStats x_stats;
        RunningStats y_stats;
        public double S_xy;
        long n;

        /// <summary>
        /// Constructor
        /// </summary>
        public RunningRegression()
        {
            x_stats = new RunningStats();
            y_stats = new RunningStats();
            clear();
        }
 
        /// <summary>
        /// Clears internal variables for reuse
        /// </summary>
        public void clear()
        {
            x_stats.clear();
            y_stats.clear();
            S_xy = 0.0;
            n = 0;
        }

        /// <summary>
        /// Add new data pair to the computation
        /// </summary>
        /// <param name="x">Independent variable value</param>
        /// <param name="y">Dependent variable value</param>
        public void push(double x, double y)
        {
            S_xy += (x_stats.mean - x)*(y_stats.mean - y)*((double)n/(double)(n+1));
     
            x_stats.push(x);
            y_stats.push(y);
            n++;
        }

        /// <summary>
        /// Add all elements of x into the calculation
        /// This is a convenience around push(double, double)
        /// </summary>
        /// <param name="x">Array of independent values to add to calculation</param>
        /// <param name="y">Array of dependent values to add to calculation</param>
        public void push(double[] x, double[] y)
        {
            if(x.Length != y.Length)
            {
                throw new ArgumentException("Arrays should have the same number of elements.");
            }

            for(int i = 0; i < x.Length; i++)
            {
                push(x[i], y[i]);
            }
        }

        /// <summary>
        /// Add all elements of x into the calculation
        /// This is a convenience around push(double, double)
        /// </summary>
        /// <param name="x">Array of independent values to add to calculation</param>
        /// <param name="y">Array of dependent values to add to calculation</param>
        public void push(List<double> x, List<double> y)
        {
            if (x.Count != y.Count)
            {
                throw new ArgumentException("Arrays should have the same number of elements.");
            }

            for (int i = 0; i < x.Count; i++)
            {
                push(x[i], y[i]);
            }
        }

        /// <summary>
        /// Add all elements of x into the calculation
        /// This is a convenience around push(double, double)
        /// Does not error check sizes, so will throw exception if not at least an n by 2 array
        /// </summary>
        /// <param name="x">2D jagged array of values to add.</param>
        public void push(double[][] x)
        {
            for (int i = 0; i < x.Length; i++)
            {
                push(x[i][0], x[i][1]);
            }
        }

        /// <summary>
        /// Add all elements of x into the calculation
        /// This is a convenience around push(double, double)
        /// Does not error check sizes, so will throw exception if not at least an n by 2 array
        /// </summary>
        /// <param name="x">List<List>> representing 2D data values to add.</param>
        public void push(List<List<double>> x)
        {
            for (int i = 0; i < x.Count; i++)
            {
                push(x[i][0], x[i][1]);
            }
        }

        public long numDataValues
        {
            get
            {
                return n;
            }
        }
 
        public double slope
        {
            get
            {
                double S_xx = x_stats.variance*(n - 1.0);
                return S_xy / S_xx;
            }
        }
 
        public double intercept
        {
            get
            {
                return y_stats.mean - slope*x_stats.mean;
            }
        }
 
        public double correlation
        {
            get
            {
                double t = x_stats.stdDev * y_stats.stdDev;
                return S_xy / ( (n-1) * t );
            }
        }
 
        public static RunningRegression operator+(RunningRegression a, RunningRegression b)
        {
            RunningRegression combined = new RunningRegression();
     
            combined.x_stats = a.x_stats + b.x_stats;
            combined.y_stats = a.y_stats + b.y_stats;
            combined.n       = a.n       + b.n;
     
            double delta_x = b.x_stats.mean - a.x_stats.mean;
            double delta_y = b.y_stats.mean - a.y_stats.mean;
            combined.S_xy = a.S_xy + b.S_xy + 
                ((double)(a.n*b.n))*delta_x*delta_y/((double)combined.n);
     
            return combined;
        }
    }
}
