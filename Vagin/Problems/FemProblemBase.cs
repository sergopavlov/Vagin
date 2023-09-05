using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vagin.interfaces;
using Vagin.Parameters;

namespace Vagin.Problems
{
    internal abstract class FemProblemBase<Tinput, Toutput> : IDirectProblem<Tinput, Toutput> where Tinput : ProblemInputParameters where Toutput : ProblemOutputParameters
    {
        protected IMesh mesh;
        SLAE slae;
        double[] q;

        protected void SolveFem(Tinput parameters)
        {
            GeneratePortrait();
            foreach (var elem in mesh.Elements)
            {
                AddLocal(elem, parameters);
            }
            Addboundary();
            q = slae.MSG(1e-15, 10000);
        }

        protected bool IsPointInsideElement(IElement element, double r, double z)
        {
            var dims = GetElemBoundaries(element);
            if (r >= dims.rmin && r <= dims.rmax && z >= dims.zmin && z <= dims.zmax)
                return true;
            return false;
        }
        protected (double ksi, double eta) GetLocalCoords(IElement element, double r, double z)
        {
            if (!IsPointInsideElement(element, r, z))
                throw new Exception("Point outside element");
            
        }

        protected (double rmin, double rmax, double zmin, double zmax) GetElemBoundaries(IElement element)
        {
            double maxR = mesh.R[element.LocalToGlobal[0]];
            double minR = mesh.R[element.LocalToGlobal[0]];
            double maxZ = mesh.Z[element.LocalToGlobal[0]];
            double minZ = mesh.Z[element.LocalToGlobal[0]];
            for (int i = 1; i < 4; i++)
            {
                if (mesh.R[element.LocalToGlobal[i]] > maxR)
                    maxR = mesh.R[element.LocalToGlobal[i]];
                if (mesh.R[element.LocalToGlobal[i]] < minR)
                    minR = mesh.R[element.LocalToGlobal[i]];
                if (mesh.Z[element.LocalToGlobal[i]] < minZ)
                    minZ = mesh.Z[element.LocalToGlobal[i]];
                if (mesh.Z[element.LocalToGlobal[i]] > maxZ)
                    maxZ = mesh.Z[element.LocalToGlobal[i]];
            }
            return (minR, maxR, minZ, maxZ);
        }
        protected abstract double CalcAverageSigma(IElement element, Tinput parameters);
        protected abstract double[] GetLocalRightPart(IElement element);
        protected void AddLocal(IElement element, Tinput parameters)
        {
            var dims = GetElemBoundaries(element);
            double hr = dims.rmax - dims.rmin;
            double hz = dims.zmax - dims.zmin;
            double cursigma = CalcAverageSigma(element, parameters);
            double[] rs = new double[4];
            for (int i = 0; i < 4; i++)
            {
                rs[i] = mesh.R[element.LocalToGlobal[i]];
            }
            double[] localvec = GetLocalRightPart(element);
            for (int i = 0; i < 4; i++)
            {
                for (int k = 0; k < 4; k++)
                {
                    slae.di[element.LocalToGlobal[i]] += cursigma * rs[k] * (hz / hr * Matrices.GR[i % 2][i % 2][k] * Matrices.MZ[i / 2][i / 2][k] + hr / hz * Matrices.MR[i % 2][i % 2][k] * Matrices.GZ[i / 2][i / 2][k]);
                }
                slae.b[element.LocalToGlobal[i]] += localvec[i];
                for (int j = 0; j < i; j++)
                {
                    double cur = 0;
                    for (int k = 0; k < 4; k++)
                    {
                        cur += cursigma * rs[k] * (hz / hr * Matrices.GR[i % 2][j % 2][k] * Matrices.MZ[i / 2][j / 2][k] + hr / hz * Matrices.MR[i % 2][j % 2][k] * Matrices.GZ[i / 2][j / 2][k]);
                    }

                    int max = element.LocalToGlobal[i] > element.LocalToGlobal[j] ? element.LocalToGlobal[i] : element.LocalToGlobal[j];
                    int min = element.LocalToGlobal[i] > element.LocalToGlobal[j] ? element.LocalToGlobal[j] : element.LocalToGlobal[i];
                    int index = Array.BinarySearch(slae.ja, slae.ia[max], slae.ia[max + 1] - slae.ia[max], min);
                    slae.al[index] += cur;
                }
            }

        }
        private void GeneratePortrait()
        {
            int n = mesh.R.Count;
            var nodeinfo = new HashSet<int>[n];
            for (int i = 0; i < n; i++)
            {
                nodeinfo[i] = new();
            }
            int count = 0;
            foreach (var element in mesh.Elements)
            {
                foreach (var i in element.LocalToGlobal)
                {
                    foreach (var j in element.LocalToGlobal)
                    {
                        if (i > j)
                        {
                            nodeinfo[i].Add(j);
                            count++;
                        }
                    }
                }
            }
            var di = new double[n];
            var b = new double[n];
            var ia = new int[n + 1];
            var ja = new int[count];
            var al = new double[count];
            ia[0] = 0;
            for (int i = 0; i < n; i++)
            {
                ia[i + 1] = ia[i] + nodeinfo[i].Count;
                int ptr = ia[i];
                foreach (var item in nodeinfo[i].OrderBy(t => t))
                {
                    ja[ptr++] = item;
                }
            }
            slae = new SLAE(n, ia, ja, di, al, b);
        }
        private void Addboundary()
        {
            foreach (var item in mesh.DirichleConditions)
            {
                ApplyDirichletToNode(item);
            }
        }
        private void ApplyDirichletToNode(int node)
        {
            slae.di[node] = 1;
            slae.b[node] = 0;
            for (int k = slae.ia[node]; k < slae.ia[node + 1]; k++)
            {
                slae.al[k] = 0;
            }
        }
        public abstract Toutput Calculate(Tinput parameters);

        private static class Matrices//посчитать
        {
            public static double[][][] GR = new double[][][]
            {
                new double[][]
                {
                    new double[]{0.5,-0.5},
                    new double[]{-0.5,0.5}
                },
                new double[][]
                {
                    new double[]{0.5,-0.5},
                    new double[]{-0.5,0.5}
                }
            };
            public static double[][][] MR = new double[][][]
            {
                new double[][]
                {
                    new double[]{0.25,1.0/12},
                    new double[]{ 1.0 / 12, 1.0 / 12 }
                },
                new double[][]
                {
                    new double[]{ 1.0 / 12, 1.0/12},
                    new double[]{ 1.0 / 12, 0.25}
                }
            };
            public static double[][][] MZ = new double[][][]
            {
            new double[][]
                {
                    new double[]{1.0/3,1.0/6},
                    new double[]{ 1.0/6, 1.0 / 3 }
                },
                new double[][]
                {
                    new double[]{1.0/3,1.0/6},
                    new double[]{ 1.0/6, 1.0 / 3 }
                }
            };
            public static double[][][] GZ = new double[][][]
            {
            new double[][]
                {
                    new double[]{1.0/3,1.0/6},
                    new double[]{ 1.0/6, 1.0 / 3 }
                },
                new double[][]
                {
                    new double[]{1.0/3,1.0/6},
                    new double[]{ 1.0/6, 1.0 / 3 }
                }
            };
        }
    }
}
