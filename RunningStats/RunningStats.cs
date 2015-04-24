using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StreamingAlgorithms
{
    /// <summary>
    /// RunningStats is an online algorithm that calculates information about a variable in one pass
    /// It will calculate:  mean, variance, standard deviation, skewness, kurtosis
    /// It overloads the + operator so two RunningStats objects can be added later and still be accurate
    /// </summary>
    public class RunningStats
    {
        long n;
        double M1, M2, M3, M4;
        double minV, maxV;
       

        /// <summary>
        /// Default constructor
        /// </summary>
        public RunningStats() 
        {
            clear();
        }

        /// <summary>
        /// Resets object to initial state so it can be reused
        /// </summary>
        public void clear()
        {
            n = 0;
            M1 = M2 = M3 = M4 = 0.0;
            minV = maxV = 0;

        }
 
        /// <summary>
        /// Add value for calculation
        /// </summary>
        /// <param name="x">Next value to add</param>
        public void push(double x)
        {
            double delta, delta_n, delta_n2, term1;
 
            long n1 = n;
            n++;
            delta = x - M1;
            delta_n = delta / n;
            delta_n2 = delta_n * delta_n;
            term1 = delta * delta_n * n1;
            M1 += delta_n;
            M4 += term1 * delta_n2 * (n*n - 3*n + 3) + 6 * delta_n2 * M2 - 4 * delta_n * M3;
            M3 += term1 * delta_n * (n - 2) - 3 * delta_n * M2;
            M2 += term1;
            if(n == 1) //first value
            {
                maxV = x;
                minV = x;
            }

            if (maxV < x)
                maxV = x;
            if (minV > x)
                minV = x;
            
        }

        /// <summary>
        /// Add all elements of x into the calculation
        /// This is a convenience around push(double)
        /// </summary>
        /// <param name="x">Array of values to add to calculation</param>
        public void push(double[] x)
        {
            foreach(double d in x)
            {
                push(d);
            }
        }

        /// <summary>
        /// Add all elements of x into the calculation
        /// This is a convenience around push(double)
        /// </summary>
        /// <param name="x">List of values to add to calculation</param>
        public void push(List<double> x)
        {
            foreach (double d in x)
            {
                push(d);
            }
        }

        /// <summary>
        /// Number of data points pushed through the calculation
        /// </summary>
        public long numDataValues 
        {
            get
            {
                return n;
            }
        }
 
        /// <summary>
        /// Mean of the data pushed
        /// </summary>
        public double mean
        {
            get
            {
                return M1;
            }
        }
 
        /// <summary>
        /// Variance of the data pushed
        /// </summary>
        public double variance
        {
            get
            {
                return M2/(n-1.0);
            }
        }
 
        /// <summary>
        /// Standard Deviation of the data pushed
        /// </summary>
        public double stdDev
        {
            get
            {
                return Math.Sqrt( variance );
            }
        }
 
        /// <summary>
        /// Skew of the data pushed
        /// </summary>
        public double skewness
        {
            get
            {
                return Math.Sqrt((double)n) * M3/ Math.Pow(M2, 1.5);
            }
        }
 
        /// <summary>
        /// Kurtosis of the data pushed
        /// </summary>
        public double kurtosis
        {
            get
            {
                return ((double)n)*M4 / (M2*M2) - 3.0;
            }
        }
 
        public double max
        {
            get
            {
                return maxV;
            }
        }

        public double min
        {
            get
            {
                return minV;
            }
        }
        
        /// <summary>
        /// Combines two RunningStat objects together such that the result has the same statistics
        /// as if you pushed the data from both through only one RunningStat object
        /// </summary>
        /// <param name="a">RunningStats object</param>
        /// <param name="b">RunningStats object </param>
        /// <returns>Combined RunningStats object</returns>
        public static RunningStats operator+(RunningStats a, RunningStats b)
        {
            RunningStats combined = new RunningStats();
     
            combined.n = a.n + b.n;
     
            double delta = b.M1 - a.M1;
            double delta2 = delta*delta;
            double delta3 = delta*delta2;
            double delta4 = delta2*delta2;
     
            combined.M1 = (a.n*a.M1 + b.n*b.M1) / combined.n;
     
            combined.M2 = a.M2 + b.M2 + delta2 * a.n * b.n / combined.n;
     
            combined.M3 = a.M3 + b.M3 + delta3 * a.n * b.n * (a.n - b.n)/(combined.n*combined.n);
            combined.M3 += 3.0*delta * (a.n*b.M2 - b.n*a.M2) / combined.n;
     
            combined.M4 = a.M4 + b.M4 + delta4*a.n*b.n * (a.n*a.n - a.n*b.n + b.n*b.n) / 
                          (combined.n*combined.n*combined.n);
            combined.M4 += 6.0 * delta2 * (a.n * a.n * b.M2 + b.n * b.n * a.M2) / (combined.n * combined.n) +
                          4.0 * delta * (a.n * b.M3 - b.n * a.M3) / combined.n;

            return combined;
        }
    }
}
