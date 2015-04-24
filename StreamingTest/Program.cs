using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StreamingAlgorithms;

namespace StreamingTest
{
    class Program
    {
        static void Main(string[] args)
        {

            double[] x = new double[] { -5, -4, -3, -2, -1, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            double[] y = new double[16];
            double[] z = new double[16];

            for(int i = 0; i < x.Length; i++)
            {
                y[i] = 2 * x[i] + 5;
                z[i] = -3.2 * y[i] - 3;

                //z = -3.2(2 * x + 5) - 3
                //z = -6.4 * x - 16 - 3
                //z = -6.4 * x - 19
            }

            RunningStats x_stats = new RunningStats();
            RunningStats y_stats = new RunningStats();
            RunningRegression xy = new RunningRegression();
            RunningRegression xz = new RunningRegression();
            RunningRegression yx = new RunningRegression();
            RunningRegression yz = new RunningRegression();
            RunningRegression zx = new RunningRegression();
            RunningRegression zy = new RunningRegression();

            RunningPercentile rp = new RunningPercentile(new double[]{.25, .5, .75});

            RunningStatsDS xyz = new RunningStatsDS(3);

            for(int i = 0; i < x.Length; i++)
            {
                x_stats.push(x[i]);
                y_stats.push(y[i]);

                xy.push(x[i], y[i]);
                xz.push(x[i], z[i]);
                
                zx.push(z[i], x[i]);
                zy.push(z[i], y[i]);
                
                yx.push(y[i], x[i]);
                yz.push(y[i], z[i]);

                xyz.push(new double[] { x[i], y[i], z[i] });

            }

            Console.WriteLine("X: Min: {0}, Max: {1}, Mean: {2}, StdDev: {3}, Var: {4}, Skew: {5}, Kurtosis: {6}",
                                  x_stats.min, x_stats.max, x_stats.mean, x_stats.stdDev, x_stats.variance, x_stats.skewness, x_stats.kurtosis);

            Console.WriteLine("Y: Min: {0}, Max: {1}, Mean: {2}, StdDev: {3}, Var: {4}, Skew: {5}, Kurtosis: {6}",
                                  y_stats.min, y_stats.max, y_stats.mean, y_stats.stdDev, y_stats.variance, y_stats.skewness, y_stats.kurtosis);

            Console.WriteLine("XY: Slope: {0}, Intercept: {1}, Corr: {2}", xy.slope, xy.intercept, xy.correlation);

            Console.WriteLine("XYZ:");
            Console.WriteLine(" Slopes:");
            double[][] slopes = xyz.slopes;
            for (int i = 0; i < 3; i++ )
            {
                for(int j = 0; j < 3; j++)
                {
                    Console.Write("{0}:", slopes[i][j]);
                }
                Console.WriteLine();
            }

            Console.WriteLine(" Intercepts:");
            double[][] inter = xyz.intercepts;
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    Console.Write("{0}:", inter[i][j]);
                }
                Console.WriteLine();
            }

            Console.WriteLine(" Correlations:");
            double[][] corr = xyz.correlations;
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    Console.Write("{0}:", corr[i][j]);
                }
                Console.WriteLine();
            }

            Console.WriteLine(" Covariances:");
            double[][] cov = xyz.covariances;
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    Console.Write("{0}:", cov[i][j]);
                }
                Console.WriteLine();
            }

            Random rnd = new Random();
            for (int i = 0; i < 10000; i++ )
            {
                rp.push(rnd.Next(101));
            }


            Console.WriteLine("RP: {0}, {1}, {2}", rp.percentileValues()[0], rp.percentileValues()[1], rp.percentileValues()[2]);

            Console.ReadLine();

        }
    }
}
