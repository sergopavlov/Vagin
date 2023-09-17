using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

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
        public double[] di_precond;
        public double[] al_precond;
        public double[] au_precond;

        public SLAE(int n, int[] ia, int[] ja, double[] di, double[] al, double[] b)
        {
            this.n = n;
            this.ia = ia;
            this.ja = ja;
            this.di = di;
            this.al = al;
            this.b = b;
            di_precond = new double[n];
            al_precond = new double[al.Length];
            au_precond = new double[al.Length];
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
        #region LLTPrecond
        public void LLT()
        {

            for (int i = 0; i < n; i++)
            {
                di_precond[i] = di[i];
            }
            for (int i = 0; i < al.Length; i++)
            {
                al_precond[i] = al[i];
            }


            for (int i = 0; i < n; i++)
            {
                double sumdi = 0;
                for (int k = ia[i]; k < ia[i + 1]; k++)
                {
                    int j = ja[k];
                    int j0 = ia[j];
                    int j1 = ia[j + 1];
                    int ik = ia[i];
                    int kj = j0;

                    double suml = 0.0;

                    while (ik < k)
                    {

                        if (ja[ik] == ja[kj])
                        {

                            suml += al_precond[ik] * al_precond[kj];
                            ik++;
                            kj++;
                        }

                        else
                        {
                            if (ja[ik] > ja[kj])
                            {
                                kj++;
                            }
                            else
                            {
                                ik++;
                            }
                        }
                    }

                    al_precond[k] = (al_precond[k] - suml) / di_precond[j];
                    sumdi += al_precond[k] * al_precond[k];
                }
                if (di_precond[i] - sumdi < 0)
                    throw new Exception();
                di_precond[i] = Math.Sqrt(di_precond[i] - sumdi);
            }
        }
        public double[] GaussLLT(IReadOnlyList<double> b)
        {
            double[] res = new double[n];
            for (int i = 0; i < n; i++)
            {
                res[i] = b[i];
            }
            for (int i = 0; i < n; i++)
            {
                double sum = 0.0;
                for (int j = ia[i]; j < ia[i + 1]; j++)
                    sum += al_precond[j] * res[ja[j]];
                res[i] -= sum;
                res[i] /= di_precond[i];
            }
            for (int i = n - 1; i >= 0; i--)
            {
                double summ = 0;
                for (int j = n - 1; j > i; j--)
                {
                    int index = Array.BinarySearch(ja, ia[j], ia[j + 1] - ia[j], i, default);
                    if (index >= 0)
                        summ += al_precond[index] * res[j];
                }
                res[i] = (res[i] - summ) / di_precond[i];
            }

            return res;
        }
        #endregion

        #region LUPrecond
        public void LU()
        {
            for (int i = 0; i < di.Length; i++)
            {
                di_precond[i] = di[i];
            }
            for (int i = 0; i < al.Length; i++)
            {
                al_precond[i] = al[i];
                au_precond[i] = al[i];
            }
            for (int i = 0; i < n; i++)
            {
                double sumdi = 0.0;

                int i0 = ia[i];
                int i1 = ia[i + 1];


                for (int k = i0; k < i1; k++)
                {
                    int j = ja[k];
                    int j0 = ia[j];
                    int j1 = ia[j + 1];
                    int ik = i0;
                    int kj = j0;

                    double suml = 0.0;
                    double sumu = 0.0;

                    while (ik < k)
                    {

                        if (ja[ik] == ja[kj])
                        {

                            suml += al_precond[ik] * au_precond[kj];
                            sumu += au_precond[ik] * al_precond[kj];
                            ik++;
                            kj++;
                        }

                        else
                        {
                            if (ja[ik] > ja[kj])
                            {
                                kj++;
                            }
                            else
                            {
                                ik++;
                            }
                        }
                    }

                    al_precond[k] = al_precond[k] - suml;
                    au_precond[k] = (au_precond[k] - sumu) / di_precond[j];
                    sumdi += al_precond[k] * au_precond[k];
                }

                di_precond[i] = di_precond[i] - sumdi;
            }
        }

        double[] LUDirect(IReadOnlyList<double> rpart)
        {
            double[] res = new double[n];
            for (int i = 0; i < n; i++)
            {
                res[i] = (rpart[i]);
            }

            for (int i = 0; i < n; i++)
            {
                double sum = 0.0;
                for (int j = ia[i]; j < ia[i + 1]; j++)
                    sum += al_precond[j] * res[ja[j]];
                res[i] -= sum;
                res[i] /= di_precond[i];
            }
            return res;
        }
        double[] LUReverse(IReadOnlyList<double> rpart)
        {
            double[] res = new double[n];
            for (int i = 0; i < n; i++)
            {
                res[i] = (rpart[i]);
            }
            for (int i = n - 1; i >= 0; i--)
            {
                for (int j = ia[i]; j < ia[i + 1]; j++)
                    res[ja[j]] -= au_precond[j] * res[i];
            }
            return res;
        }
        #endregion

        #region MSG
        public double[] MSGLLTPreCond(double eps, int maxiter)
        {
            double[] x0 = new double[n];
            return MSGLLTPreCond(x0, eps, maxiter);
        }
        public double[] MSGLLTPreCond(IReadOnlyList<double> x0, double eps, int maxiter)
        {
            double bnorm = Math.Sqrt(DotProduct(b, b));
            double[] r = MatrixMult(x0);
            double[] z;
            double[] res = new double[n];
            for (int i = 0; i < n; i++)
            {
                res[i] = x0[i];
                r[i] = b[i] - r[i];
            }
            z = GaussLLT(r);
            double relresidual = 1;
            int k = 0;
            double[] Mr = GaussLLT(r);
            while (relresidual > eps && k < maxiter)
            {
                var Az = MatrixMult(z);
                double rsuqared = DotProduct(Mr, r);
                double alpha = rsuqared / DotProduct(Az, z);
                for (int i = 0; i < n; i++)
                {
                    res[i] += alpha * z[i];
                    r[i] -= alpha * Az[i];
                }
                Mr = GaussLLT(r);
                double betta = DotProduct(Mr, r) / rsuqared;
                for (int i = 0; i < n; i++)
                {
                    z[i] = Mr[i] + betta * z[i];
                }
                relresidual = Math.Sqrt(DotProduct(r, r)) / bnorm;
                k++;
            }
            return res;
        }

        public double[] MSGLUPreCond(IReadOnlyList<double> x0, double eps, int maxiter)
        {
            double bnorm = Math.Sqrt(DotProduct(b, b));
            double[] r = MatrixMult(x0);
            double[] z;
            double[] res = new double[n];
            for (int i = 0; i < n; i++)
            {
                res[i] = x0[i];
                r[i] = b[i] - r[i];
            }
            z = (r);
            double relresidual = 1;
            int k = 0;
            double[] Mr = LUReverse(LUDirect(r));
            //double[] Mr = GaussLLT(r);
            while (relresidual > eps && k < maxiter)
            {
                var Az = MatrixMult(z);
                double rsuqared = DotProduct(Mr, r);
                double alpha = rsuqared / DotProduct(Az, z);
                for (int i = 0; i < n; i++)
                {
                    res[i] += alpha * z[i];
                    r[i] -= alpha * Az[i];
                }
                Mr = LUReverse(LUDirect(r));
                double betta = DotProduct(Mr, r) / rsuqared;
                for (int i = 0; i < n; i++)
                {
                    z[i] = Mr[i] + betta * z[i];
                }
                relresidual = Math.Sqrt(DotProduct(r, r)) / bnorm;
                k++;
            }
            return res;
        }
        public double[] MSGLUPreCond(double eps, int maxiter)
        {
            double[] x0 = new double[n];
            return MSGLUPreCond(x0, eps, maxiter);
        }
        public double[] MSG(IReadOnlyList<double> x0, double eps, int maxiter)
        {
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
        public double[] MSG(double eps, int maxiter)
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
        #endregion

        #region LOS
        public double[] SolveLosLUPrecond(double eps, int maxiter)
        {
            List<double> x0 = new();
            for (int i = 0; i < n; i++)
            {
                x0.Add(0);
            }
            LU();
            return LoS_precond(x0, eps, maxiter);
        }

        /*public double[] SolveLos(double eps, int maxiter)
        {
            List<double> x0 = new();
            for (int i = 0; i < n; i++)
            {
                x0.Add(0);
            }
            LU();
            return LOS(x0, eps, maxiter);
        }*/
        public double[] SolveLosLUPrecond(List<double> x0, double eps, int maxiter)
        {
            List<double> x = new();
            foreach (var item in x0)
            {
                x.Add(item);
            }
            LU();
            return LoS_precond(x, eps, maxiter);
        }

        /*public double[] LOS(IReadOnlyList<double> x0, double eps, int maxiter)
        {
            double bnorm = Math.Sqrt(DotProduct(b, b));

            List<double> Ar = new();
            List<double> r = MatrixMult(x0);
            List<double> z = new();
            List<double> res = new List<double>();
            for (int i = 0; i < n; i++)
            {
                res.Add(x0[i]);
                r[i] = b[i] - r[i];
                z.Add(r[i]);
            }
            List<double> p = MatrixMult(z);
            int k = 0;
            double alpha, betta, rnorm = Math.Sqrt(DotProduct(r, r));
            while (k < maxiter && rnorm / bnorm > eps)
            {
                alpha = DotProduct(p, r) / DotProduct(p, p);
                for (int i = 0; i < n; i++)
                {
                    res[i] += alpha * z[i];
                    r[i] -= alpha * p[i];
                }
                Ar = MatrixMult(r);
                betta = -DotProduct(p, Ar) / DotProduct(p, p);
                rnorm = Math.Sqrt(DotProduct(r, r));
                for (int i = 0; i < n; i++)
                {
                    z[i] = r[i] + betta * z[i];
                    p[i] = Ar[i] + betta * p[i];
                }
                k++;
            }
            //Console.WriteLine($"{rnorm / bnorm} {k}");
            return res;
        }*/
        public double[] LoS_precond(IReadOnlyList<double> x0, double eps, int maxiter)
        {
            double[] x = new double[x0.Count];
            for (int i = 0; i < x0.Count; i++)
            {
                x[i] = x0[i];
            }
            int k = 1;
            var buf = MatrixMult(x);
            double bnorm = 0;
            for (int i = 0; i < n; i++)
            {
                buf[i] = b[i] - buf[i];
            }
            double rnorm = Math.Sqrt(DotProduct(buf, buf));
            var r = LUDirect(buf);
            bnorm = Math.Sqrt(DotProduct(b, b));
            var z = LUReverse(r);
            buf = MatrixMult(z);
            var p = LUDirect(buf);
            double resid = 1;
            while (resid > eps && k < maxiter)
            {
                double pp = DotProduct(p, p);
                double pr = DotProduct(p, r);
                double alpha = pr / pp;
                for (int i = 0; i < n; i++)
                {
                    x[i] += alpha * z[i];
                    r[i] -= alpha * p[i];
                }
                rnorm = Math.Sqrt(DotProduct(r, r));
                var Ur = LUReverse(r);
                buf = MatrixMult(Ur);
                buf = LUDirect(buf);
                double betta = -(DotProduct(p, buf) / pp);
                for (int i = 0; i < n; i++)
                {
                    z[i] = Ur[i] + betta * z[i];
                    p[i] = buf[i] + betta * p[i];
                }
                double test1 = 0;
                double test2 = 0;
                var asd = MatrixMult(x);
                for (int i = 0; i < n; i++)
                {
                    test1 += (asd[i] - b[i]) * (asd[i] - b[i]);
                    test2 += b[i] * b[i];
                }
                resid = Math.Sqrt(test1 / test2);
                k++;
            }
            Console.WriteLine($"{k} {rnorm / bnorm} {resid}");
            return x;
        }

        #endregion
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
