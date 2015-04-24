using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StreamingAlgorithms
{
    public class RunningPercentile
    {
        /// <summary>
        /// Constuctor taking multiple percentiles to monitor
        /// </summary>
        /// <param name="ptile">Percentile value to estimate out of 1. 50th Percentile -> .5</param>
        public RunningPercentile(double[] ptile)
        {
            percentile = ptile;
            numPercentiles = ptile.Length;
            q = new double[numPercentiles][];
            n = new int[numPercentiles][];
            nPrime = new double[numPercentiles][];
            dn = new double[numPercentiles][];

            for(int i = 0; i < percentile.Length; i++)
            {
                if (percentile[i] < 0 || percentile[i] > 1)
                    throw new ArgumentException("Percentile is out of 1. 50th percentile -> .5");

                q[i] = new double[5];
                n[i] = new int[] { 1, 2, 3, 4, 5 };
                nPrime[i] = new double[] { 1, 1 + 2 * percentile[i], 1 + 4 * percentile[i], 3 + 2 * percentile[i], 5 };
                dn[i] = new double[] { 0, percentile[i] / 2, percentile[i], (1 + percentile[i]) / 2, 1 };

            }
            count = 0;
        }

        /// <summary>
        /// Constructor taking only one perecntile to monitor
        /// </summary>
        /// <param name="ptile">Percentile value to estimate out of 1. 50th Percentile -> .5</param>
        public RunningPercentile(double ptile)
        {
            percentile = new double[1]; 
            percentile[0] = ptile;
            numPercentiles = 1;
            q = new double[numPercentiles][];
            n = new int[numPercentiles][];
            nPrime = new double[numPercentiles][];
            dn = new double[numPercentiles][];

            for (int i = 0; i < percentile.Length; i++)
            {
                if (percentile[i] < 0 || percentile[i] > 1)
                    throw new ArgumentException("Percentile is out of 1. 50th percentile -> .5");

                q[i] = new double[5];
                n[i] = new int[] { 1, 2, 3, 4, 5 };
                nPrime[i] = new double[] { 1, 1 + 2 * percentile[i], 1 + 4 * percentile[i], 3 + 2 * percentile[i], 5 };
                dn[i] = new double[] { 0, percentile[i] / 2, percentile[i], (1 + percentile[i]) / 2, 1 };

            }
            count = 0;
        }

        /// <summary>
        /// Clears internal variables so object can be reused.  Does not reset what percentiles to estimate
        /// </summary>
        public void clear()
        {
            for (int i = 0; i < percentile.Length; i++)
            {
                q[i] = new double[5];
                n[i] = new int[] { 1, 2, 3, 4, 5 };
                nPrime[i] = new double[] { 1, 1 + 2 * percentile[i], 1 + 4 * percentile[i], 3 + 2 * percentile[i], 5 };
                dn[i] = new double[] { 0, percentile[i] / 2, percentile[i], (1 + percentile[i]) / 2, 1 };
            }
            
            count = 0;
        }

        /// <summary>
        /// Push a value through the calculation. This will update the estimated percentiles
        /// </summary>
        /// <param name="x">Value to update estimates with</param>
        public void push(double x)
        {
            if (count < 5) //the first five values start as sigils for the updates below
            {
                for(int i = 0; i < numPercentiles; i++)
                    q[i][count] = x;
            }
            else
            {
                if (count == 5)  //sort the data we have so we start with the actual data in the right order
                {
                    for(int i = 0; i < numPercentiles; i++)
                        Array.Sort(q[i]);
                }

                int[] k = new int[numPercentiles];
                for (int i = 0; i < numPercentiles; i++)
                {
                    k[i] = 1;
                    if (x < q[i][0])  //reset min
                    {
                        q[i][0] = x;
                        k[i] = 1;
                    }
                    else if (q[i][4] < x) //reset max
                    {
                        q[i][4] = x;
                        k[i] = 4;
                    }
                    else if (q[i][0] <= x && x < q[i][1])  //check which bin we are in and save it
                    {
                        k[i] = 1;
                    }
                    else if (q[i][1] <= x && x < q[i][2])
                    {
                        k[i] = 2;
                    }
                    else if (q[i][2] <= x && x < q[i][3])
                    {
                        k[i] = 3;
                    }
                    else if (q[i][3] <= x && x < q[i][4])
                    {
                        k[i] = 4;
                    }
                }

                for (int ind = 0; ind < numPercentiles; ind++)
                {
                    for (int i = k[ind]; i < 5; i++)
                    {
                        n[ind][i] += 1; //update the positions of the actual position 
                    }

                    for (int i = 0; i < 5; i++)
                    {
                        nPrime[ind][i] += dn[ind][i]; //update the desired positions of the bins
                    }

                    for (int i = 1; i <= 3; i++)
                    {
                        double d = nPrime[ind][i] - n[ind][i]; //offset to desired position
                        double dp = n[ind][i + 1] - n[ind][i]; //distance to next position
                        double dm = n[ind][i - 1] - n[ind][i]; //distance to previous position

                        double hp = (q[ind][i + 1] - q[ind][i]) / dp;
                        double hm = (q[ind][i - 1] - q[ind][i]) / dm;

                        if ((d >= 1 && dp > 1) || (d <= -1 && dm < -1))
                        {
                            int sign_d = (int)(d / Math.Abs(d));

                            double h = q[ind][i] + sign_d / (dp - dm) * ((sign_d - dm) * hp + (dp - sign_d) * hm); //quadratic fit 

                            if (q[ind][i - 1] < h && h < q[ind][i + 1])
                            {
                                q[ind][i] = h;
                            }
                            else
                            {
                                if (d > 0)  //linear fit
                                {
                                    q[ind][i] += hp;
                                }
                                else
                                {
                                    q[ind][i] -= hm;
                                }
                            }

                            n[ind][i] += sign_d;
                        }
                    }
                }
            }
            count++;
        }

        /// <summary>
        /// Add all elements of x into the calculation
        /// This is a convenience around push(double)
        /// </summary>
        /// <param name="x">Array of values to add to calculation</param>
        public void push(double[] x)
        {
            foreach (double d in x)
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
        /// Returns the percentile estimates
        /// </summary>
        /// <returns>Array of percentile estimates</returns>
        public double[] percentileValues()
        {
            double[] vals = new double[numPercentiles];
            for (int i = 0; i < numPercentiles; i++)
            {
                vals[i] = q[i][2];
            }
            return vals;
        }


        private double[] percentile;
        private int numPercentiles;

        private double[][] q;
        private double[][] nPrime;
        private int[][] n;
        private double[][] dn;

        private int count;
    }
}
