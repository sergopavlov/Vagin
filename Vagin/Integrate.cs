﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vagin
{
   internal class Integrate
   {
      static readonly double[] points =
      {
         0.995657163025808080735527280689003,
         0.973906528517171720077964012084452,
         0.930157491355708226001207180059508,
         0.865063366688984510732096688423493,
         0.780817726586416897063717578345042,
         0.679409568299024406234327365114874,
         0.562757134668604683339000099272694,
         0.433395394129247190799265943165784,
         0.294392862701460198131126603103866,
         0.148874338981631210884826001129720,
         0,
         -0.148874338981631210884826001129720,
         -0.294392862701460198131126603103866,
         -0.433395394129247190799265943165784,
         -0.562757134668604683339000099272694,
         -0.679409568299024406234327365114874,
         -0.780817726586416897063717578345042,
         -0.865063366688984510732096688423493,
         -0.930157491355708226001207180059508,
         -0.973906528517171720077964012084452,
         -0.995657163025808080735527280689003
      };
      static readonly double[] weights =
      {
         0.011694638867371874278064396062192 / 2.0,
         0.032558162307964727478818972459390 / 2.0,
         0.054755896574351996031381300244580 / 2.0,
         0.075039674810919952767043140916190 / 2.0,
         0.093125454583697605535065465083366 / 2.0,
         0.109387158802297641899210590325805 / 2.0,
         0.123491976262065851077958109831074 / 2.0,
         0.134709217311473325928054001771707 / 2.0,
         0.142775938577060080797094273138717 / 2.0,
         0.147739104901338491374841515972068 / 2.0,
         0.149445554002916905664936468389821 / 2.0,
         0.147739104901338491374841515972068 / 2.0,
         0.142775938577060080797094273138717 / 2.0,
         0.134709217311473325928054001771707 / 2.0,
         0.123491976262065851077958109831074 / 2.0,
         0.109387158802297641899210590325805 / 2.0,
         0.093125454583697605535065465083366 / 2.0,
         0.075039674810919952767043140916190 / 2.0,
         0.054755896574351996031381300244580 / 2.0,
         0.032558162307964727478818972459390 / 2.0,
         0.011694638867371874278064396062192 / 2.0
      };

     

      public static double Adaptive(Func<double, double> f, double a, double b, double eps = 1e-3)
      {
         double[] points =
         {
            0.995657163025808080735527280689003,
            0.973906528517171720077964012084452,
            0.930157491355708226001207180059508,
            0.865063366688984510732096688423493,
            0.780817726586416897063717578345042,
            0.679409568299024406234327365114874,
            0.562757134668604683339000099272694,
            0.433395394129247190799265943165784,
            0.294392862701460198131126603103866,
            0.148874338981631210884826001129720,
            0,
            -0.148874338981631210884826001129720,
            -0.294392862701460198131126603103866,
            -0.433395394129247190799265943165784,
            -0.562757134668604683339000099272694,
            -0.679409568299024406234327365114874,
            -0.780817726586416897063717578345042,
            -0.865063366688984510732096688423493,
            -0.930157491355708226001207180059508,
            -0.973906528517171720077964012084452,
            -0.995657163025808080735527280689003
         };
         double[] weights =
         {
            0.011694638867371874278064396062192 / 2.0,
            0.032558162307964727478818972459390 / 2.0,
            0.054755896574351996031381300244580 / 2.0,
            0.075039674810919952767043140916190 / 2.0,
            0.093125454583697605535065465083366 / 2.0,
            0.109387158802297641899210590325805 / 2.0,
            0.123491976262065851077958109831074 / 2.0,
            0.134709217311473325928054001771707 / 2.0,
            0.142775938577060080797094273138717 / 2.0,
            0.147739104901338491374841515972068 / 2.0,
            0.149445554002916905664936468389821 / 2.0,
            0.147739104901338491374841515972068 / 2.0,
            0.142775938577060080797094273138717 / 2.0,
            0.134709217311473325928054001771707 / 2.0,
            0.123491976262065851077958109831074 / 2.0,
            0.109387158802297641899210590325805 / 2.0,
            0.093125454583697605535065465083366 / 2.0,
            0.075039674810919952767043140916190 / 2.0,
            0.054755896574351996031381300244580 / 2.0,
            0.032558162307964727478818972459390 / 2.0,
            0.011694638867371874278064396062192 / 2.0
         };
         if (a == b)
            return 0;
         bool flag = true;
         int split = 0;
         List<Elem> result = new();
         Queue<Elem> elems = new Queue<Elem>();
         Queue<Elem> integrated = new();
         elems.Enqueue(new Elem(a, b, 0, 0));
         do
         {
            integrated.Clear();
            while (elems.Count > 0)
            {
               var cur = elems.Dequeue();
               double h = (cur.X1 - cur.X0);
               //foreach (var item in GaussNodes)
               for (int i = 0; i < points.Length; i++)
               {
                  double t = (1 - points[i]) * cur.X0 + points[i] * cur.X1;
                  cur.Res += h * f(t) * weights[i];
               }
               integrated.Enqueue(cur);
            }
            foreach (var item in integrated.Where(t => t.Parent == null))
            {
               var buf = item.Split();
               elems.Enqueue(buf.Item1);
               elems.Enqueue(buf.Item2);
            }
            foreach (var item in integrated.Where(t => t.Parent != null).GroupBy(t => t.Parent))
            {
               if (item.Count() != 2)
                  throw new Exception();
               var err = Math.Abs(item.Sum(t => t.Res) - item.Key.Res);
               if (!(err < eps / (1 << (item.Key.splits + 1))))
               {
                  foreach (var elem in item)
                  {
                     var buf = elem.Split();
                     elems.Enqueue(buf.Item1);
                     elems.Enqueue(buf.Item2);
                  }
               }
               else
               {
                  foreach (var elem in item)
                  {
                     result.Add(elem);
                  }
               }
            }
            split++;
         } while (elems.Count > 0 && split < 200);
         double res = result.Sum(t => t.Res);
         Console.WriteLine($"integral: {res} split: {split}");
         return res;
      }

      private class Elem
      {
         public Elem(double x0, double x1, double res, int splits)
         {
            X0 = x0;
            X1 = x1;
            Res = res;
            this.splits = splits;
         }

         public double X0 { get; init; }
         public double X1 { get; init; }
         public double Res { get; set; }
         public int splits { get; init; }
         public Elem? Parent { get; set; }
         public (Elem, Elem) Split()
         {
            return (new Elem(X0, (X0 + X1) / 2, 0, splits + 1) { Parent = this }, new Elem((X0 + X1) / 2, X1, 0, this.splits + 1) { Parent = this });
         }
      }

      static readonly double[] points21 =
         {
             0.995657163025808080735527280689003,
             0.973906528517171720077964012084452,
             0.930157491355708226001207180059508,
             0.865063366688984510732096688423493,
             0.780817726586416897063717578345042,
             0.679409568299024406234327365114874,
             0.562757134668604683339000099272694,
             0.433395394129247190799265943165784,
             0.294392862701460198131126603103866,
             0.148874338981631210884826001129720,
            0,
            - 0.148874338981631210884826001129720,
            - 0.294392862701460198131126603103866,
            -0.433395394129247190799265943165784,
            -0.562757134668604683339000099272694,
            -0.679409568299024406234327365114874,
            -0.780817726586416897063717578345042,
            -0.865063366688984510732096688423493,
            -0.930157491355708226001207180059508,
            -0.973906528517171720077964012084452,
            -0.995657163025808080735527280689003
         };
      static readonly double[] weights21 =
      {
            0.011694638867371874278064396062192,
            0.032558162307964727478818972459390,
            0.054755896574351996031381300244580,
            0.075039674810919952767043140916190,
            0.093125454583697605535065465083366,
            0.109387158802297641899210590325805,
            0.123491976262065851077958109831074,
            0.134709217311473325928054001771707,
            0.142775938577060080797094273138717,
            0.147739104901338491374841515972068,
            0.149445554002916905664936468389821,
            0.147739104901338491374841515972068,
            0.142775938577060080797094273138717,
            0.134709217311473325928054001771707,
            0.123491976262065851077958109831074,
            0.109387158802297641899210590325805,
            0.093125454583697605535065465083366,
            0.075039674810919952767043140916190,
            0.054755896574351996031381300244580,
            0.032558162307964727478818972459390,
            0.011694638867371874278064396062192
         };

      public static double Quad(Func<double, double> func, double A, double B, int n = 200)
      {



         double qi, pi;

         double sum = 0;
         double stepVector = (B - A) / n;
         for (int k = 0; k < n; k++)
         {
            double StepStart = A + stepVector * k;
            double StepEnd = A + stepVector * (k + 1);
            double step = StepEnd - StepStart;
            for (int i = 0; i < points21.Length; i++)
            {
               double t = (step * points21[i] + StepEnd + StepStart) / 2;
               sum += weights21[i] * func(t) * stepVector;
            }
         }
         return sum / 2;
      }

      static readonly double[] points5 = new double[5] { 0.0, 1.0 / 3.0 * Math.Sqrt(5.0 - 2.0 * Math.Sqrt(10.0 / 7.0)), -1.0 / 3.0 * Math.Sqrt(5.0 - 2.0 * Math.Sqrt(10.0 / 7.0)), 1.0 / 3.0 * Math.Sqrt(5 + 2 * Math.Sqrt(10.0 / 7.0)), -1.0 / 3.0 * Math.Sqrt(5 + 2 * Math.Sqrt(10.0 / 7.0)) }; // точки Гаусса
      static readonly double[] weights5 = new double[5] { 128.0 / 225.0, (322.0 + 13.0 * Math.Sqrt(70.0)) / 900.0, (322.0 + 13.0 * Math.Sqrt(70.0)) / 900.0, (322.0 - 13.0 * Math.Sqrt(70.0)) / 900.0, (322.0 - 13.0 * Math.Sqrt(70.0)) / 900.0 }; // веса Гаусса


      public static double Quad5(Func<double, double> func, double A, double B, int n = 200)
      {



         double qi, pi;

         double[] sum = new double[n];

         double stepVector = (B - A) / n;
         Parallel.For(0, n, k =>
         {
            double StepStart = A + stepVector * k;
            double StepEnd = A + stepVector * (k + 1);
            double step = StepEnd - StepStart;
            for (int i = 0; i < points5.Length; i++)
            {
               double t = (step * points5[i] + StepEnd + StepStart) / 2;
               sum[k] += weights5[i] * func(t) * stepVector;
            }
         });
         return sum.Sum() / 2;
      }
   }
}
