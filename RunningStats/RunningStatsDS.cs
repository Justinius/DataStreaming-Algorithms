using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StreamingAlgorithms
{
    public class RunningStatsDS
    {
        long n;
        int numDims;
        List<RunningStats> varStats;
        public double[][] S; //this should be symmetric so I'm going to try to utilize that fact as much as possible

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="numVars">Number of variables in data set</param>
        public RunningStatsDS(int numVars)
        {
            numDims = numVars;
            varStats = new List<RunningStats>(numDims);
            S = new double[numDims][];
            for(int i = 0; i < numDims; i++)
            {
                varStats.Add(new RunningStats());
                S[i] = new double[numDims];
            }

            clear();
        }
 
        /// <summary>
        /// Clears internal variables for reuse
        /// </summary>
        public void clear()
        {
            for(int i = 0; i < numDims; i++)
            {
                varStats[i].clear();
                for(int j = 0; j < numDims; j++)
                {
                    S[i][j] = 0.0;
                }
            }
            n = 0;
        }

        /// <summary>
        /// Clears internal variables for reuse, but also resizes for different number of dims
        /// </summary>
        /// <param name="newDim">New number of dims to reinitialize with</param>
        public void clear(int newDim)
        {
            numDims = newDim;
            varStats = new List<RunningStats>(numDims);
            S = new double[numDims][];
            for(int i = 0; i < numDims; i++)
            {
                varStats.Add(new RunningStats());
                S[i] = new double[numDims];
            }
            clear();
        }
 

        /// <summary>
        /// Add the new data point to the computation
        /// </summary>
        /// <param name="pt">Array of values to be added. This does not do error checking. Must be at least numDim elements</param>
        public void push(double[] pt)
        {
            for(int x = 0; x < numDims; x++)
            {
                for(int y = 0; y < numDims; y++)
                {
                    S[x][y] += (varStats[x].mean - pt[x])*(varStats[y].mean-pt[y])*((double)n/(double)(n+1));
                }
            }

            for(int i = 0; i < numDims; i++)
            {
                varStats[i].push(pt[i]);
            }
            n++;
        }

        /// <summary>
        /// Add the new data point to the computation
        /// </summary>
        /// <param name="pt">Array of values to be added. This does not do error checking. Must be at least numDim elements</param>
        public void push(List<double> pt)
        {
            for(int x = 0; x < numDims; x++)
            {
                for(int y = 0; y < numDims; y++)
                {
                    S[x][y] += (varStats[x].mean - pt[x])*(varStats[y].mean-pt[y])*((double)n/(double)(n+1));
                }
            }

            for(int i = 0; i < numDims; i++)
            {
                varStats[i].push(pt[i]);
            }
            n++;
        }

        
        /// <summary>
        /// Add all elements of x into the calculation
        /// This is a convenience around push(double[])
        /// Does not error check sizes, so will throw exception if not at least an n by 2 array
        /// </summary>
        /// <param name="x">2D jagged array of values to add.</param>
        public void push(double[][] x)
        {
            for (int i = 0; i < x.Length; i++)
            {
                push(x[i]);
            }
        }

        /// <summary>
        /// Add all elements of x into the calculation
        /// This is a convenience around push(List<double>)
        /// Does not error check sizes, so will throw exception if not at least an n by 2 array
        /// </summary>
        /// <param name="x">List<List>> representing 2D data values to add.</param>
        public void push(List<List<double>> x)
        {
            for (int i = 0; i < x.Count; i++)
            {
                push(x[i]);
            }
        }

        public long numDataValues
        {
            get
            {
                return n;
            }
        }

        public int numDimensions
        {
            get
            {
                return numDims;
            }
        }
 
        /// <summary>
        /// This returns slope of best fit line between data variables
        /// Independent variable is by row, Dependent is column
        /// </summary>
        public double[][] slopes
        {
            get
            {
                double[][] currSlopes = new double[numDims][];
                for(int x = 0; x < numDims; x++)
                {
                    currSlopes[x] = new double[numDims];
                    for(int y = 0; y < numDims; y++)
                    {
                        double S_xx = varStats[x].variance*(n-1.0);
                        currSlopes[x][y] = S[x][y] / S_xx;
                    }
                }

                //double S_xx = x_stats.variance*(n - 1.0);
                //return S_xy / S_xx;
                return currSlopes;
            }
        }
 
        /// <summary>
        /// Returns the intercept of hte best fit line between data variables
        /// Independent variable is by row, dependent is column
        /// </summary>
        public double[][] intercepts
        {
            get
            {
                double[][] currentSlopes = this.slopes;
                
                double[][] currIntercepts = new double[numDims][];
                for(int x = 0; x < numDims; x++)
                {
                    currIntercepts[x] = new double[numDims];
                    for(int y = 0; y < numDims; y++)
                    {
                        currIntercepts[x][y] = varStats[y].mean - currentSlopes[x][y]*varStats[x].mean;
                    }
                }
                
                //return y_stats.mean - slope*x_stats.mean;
                return currIntercepts;
            }
        }
 
        /// <summary>
        /// Returns correlation between the variables, this should be a symmetric matrix
        /// </summary>
        public double[][] correlations
        {
            get
            {
                double[][] currCorrs = new double[numDims][];
                for(int x = 0; x < numDims; x++)
                {
                    currCorrs[x] = new double[numDims];
                    for(int y = 0; y < numDims; y++)
                    {
                        double t = varStats[x].stdDev * varStats[y].stdDev;
                        currCorrs[x][y] =  S[x][y] / ( (n-1) * t );
                    }
                }
                //double t = x_stats.stdDev * y_stats.stdDev;
                //return S_xy / ( (n-1) * t );
                return currCorrs;
            }
        }

        /// <summary>
        /// Returns covariances between the variables, this should be a symmetric matrix
        /// </summary>
        public double[][] covariances
        {
            get
            {
                //double[][] currCorrs = this.correlations;

                double[][] currCov = new double[numDims][];
                for(int x = 0; x < numDims; x++)
                {
                    currCov[x] = new double[numDims];
                    for(int y = 0; y < numDims; y++)
                    {
                        currCov[x][y] = S[x][y] / (n-1);//currCorrs[x][y] * varStats[x].stdDev * varStats[y].stdDev;
                    }
                }
                return currCov;
            }
        }

        public double[] maxVals
        {
            get
            {
                double[] maxs = new double[numDims];
                for(int i = 0; i < numDims; i++)
                {
                    maxs[i] = varStats[i].max;
                }
                return maxs;
            }
        }
        
        public double[] means
        {
            get
            {
                double[] avgs = new double[numDims];
                for(int i = 0; i < numDims; i++)
                {
                    avgs[i] = varStats[i].mean;
                }
                return avgs;
            }
        }

        public double[] minVals
        {
            get
            {
                double[] mins = new double[numDims];
                for (int i = 0; i < numDims; i++)
                {
                    mins[i] = varStats[i].min;
                }
                return mins;
            }
        }

        public double[] stdDevs
        {
            get
            {
                double[] stddevs = new double[numDims];
                for (int i = 0; i < numDims; i++)
                {
                    stddevs[i] = varStats[i].stdDev;
                }
                return stddevs;
            }
        }

        public double[] skewness
        {
            get
            {
                double[] skews = new double[numDims];
                for (int i = 0; i < numDims; i++)
                {
                    skews[i] = varStats[i].skewness;
                }
                return skews;
            }
        }

        public double[] kurtosis
        {
            get
            {
                double[] kurs = new double[numDims];
                for (int i = 0; i < numDims; i++)
                {
                    kurs[i] = varStats[i].kurtosis;
                }
                return kurs;
            }
        }

        public double[] variances
        {
            get
            {
                double[] vars = new double[numDims];
                for (int i = 0; i < numDims; i++)
                {
                    vars[i] = varStats[i].variance;
                }
                return vars;
            }
        }
        public static RunningStatsDS operator+(RunningStatsDS a, RunningStatsDS b)
        {
            if(a.numDimensions != b.numDimensions)
                throw new ArgumentException("Number of dimensions must agree.");
           
            int dims = a.numDimensions;

            RunningStatsDS combined = new RunningStatsDS(a.numDimensions);
            for(int i = 0; i < dims; i++)
            {
                combined.varStats[i] = a.varStats[i] + b.varStats[i];
            }
            combined.n       = a.n       + b.n;
     
            double[] delta = new double[dims];
            for(int i = 0; i < dims; i++)
            {
                delta[i] = a.varStats[i].mean - b.varStats[i].mean;
            }

            for(int x = 0; x < dims; x++)
            {
                for(int y = 0; y < dims; y++)
                {
                    combined.S[x][y] = a.S[x][y] + b.S[x][y] + ((double)(a.n*b.n))*delta[x]*delta[y]/((double)combined.n);
                }
            }

            //double delta_x = b.x_stats.mean - a.x_stats.mean;
            //double delta_y = b.y_stats.mean - a.y_stats.mean;
            //combined.S_xy = a.S_xy + b.S_xy + 
            //    ((double)(a.n*b.n))*delta_x*delta_y/((double)combined.n);
     
            return combined;
        }

        /// <summary>
        /// Returns instance of RunningStats for the individual variable
        /// </summary>
        /// <param name="key">Index of variable to get</param>
        /// <returns>RunningStats object of the requested variable</returns>
        public RunningStats this[int key]
        {
            get
            {
                return varStats[key];
            }
        }
    
    }
}
