using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vagin
{
    internal class SLAE
    {
        public int n;
        public int[] ia;
        public int[] ja;
        public double[] di;
        public double[] al;
        public double[] b;

        public SLAE(int n, int[] ia, int[] ja, double[] di, double[] al, double[] b)
        {
            this.n = n;
            this.ia = ia;
            this.ja = ja;
            this.di = di;
            this.al = al;
            this.b = b;
        }

        public void Clear()
        {
            for (int i = 0; i < n; i++)
            {
                di[i] = 0;
                b[i] = 0;
            }
            int m = al.Length;
            for (int i = 0; i < m; i++)
            {
                al[i] = 0;
            }
        }
        public double[] MSG(IReadOnlyList<double> x0, double eps, int maxiter)
        {
            double bnorm = Math.Sqrt(DotProduct(b, b));
            double[] r = MatrixMult(x0);
            double[] z = new double[n];
            double[] res = new double[n];
            for (int i = 0; i < n; i++)
            {
                res[i] =(x0[i]);
                r[i] = b[i] - r[i];
                z[i] = (r[i]);
            }
            double relresidual = 1;
            int k = 0;
            while (relresidual > eps && k < maxiter)
            {
                var Az = MatrixMult(z);
                double rsuqared = DotProduct(r, r);
                double alpha = rsuqared / DotProduct(Az, z);
                for (int i = 0; i < n; i++)
                {
                    res[i] += alpha * z[i];
                    r[i] -= alpha * Az[i];
                }
                double betta = DotProduct(r, r) / rsuqared;
                for (int i = 0; i < n; i++)
                {
                    z[i] = r[i] + betta * z[i];
                }
                relresidual = Math.Sqrt(DotProduct(r, r)) / bnorm;
                k++;
            }
            Console.WriteLine($"{relresidual} {k}");
            return res;
        }
        public double[] MSG( double eps, int maxiter)
        {
            double[] x0 = new double[n];
            double bnorm = Math.Sqrt(DotProduct(b, b));
            double[] r = MatrixMult(x0);
            double[] z = new double[n];
            double[] res = new double[n];
            for (int i = 0; i < n; i++)
            {
                res[i] = (x0[i]);
                r[i] = b[i] - r[i];
                z[i] = (r[i]);
            }
            double relresidual = 1;
            int k = 0;
            while (relresidual > eps && k < maxiter)
            {
                var Az = MatrixMult(z);
                double rsuqared = DotProduct(r, r);
                double alpha = rsuqared / DotProduct(Az, z);
                for (int i = 0; i < n; i++)
                {
                    res[i] += alpha * z[i];
                    r[i] -= alpha * Az[i];
                }
                double betta = DotProduct(r, r) / rsuqared;
                for (int i = 0; i < n; i++)
                {
                    z[i] = r[i] + betta * z[i];
                }
                relresidual = Math.Sqrt(DotProduct(r, r)) / bnorm;
                k++;
            }
            Console.WriteLine($"{relresidual} {k}");
            return res;
        }
        double DotProduct(IReadOnlyList<double> vec1, IReadOnlyList<double> vec2)
        {
            if (vec1.Count != vec2.Count)
                throw new Exception();
            double res = 0;
            int m = vec1.Count;
            for (int i = 0; i < m; i++)
            {
                res += vec1[i] * vec2[i];
            }
            return res;
        }
        double[] MatrixMult(IReadOnlyList<double> x)
        {
            if (x.Count != n)
                throw new Exception();
            double[] res = new double[n];
            for (int i = 0; i < n; i++)
            {
                res[i] = x[i] * di[i];
                for (int k = ia[i]; k < ia[i + 1]; k++)
                {
                    int j = ja[k];
                    res[i] += al[k] * x[j];
                    res[j] += al[k] * x[i];
                }
            }
            return res;
        }
    }
}
