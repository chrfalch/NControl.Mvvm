using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;

namespace NControl.XAnimation
{
	public sealed class EasingFunctionBezier
	{
		public Point P0 { get; private set; }
		public Point P1 { get; private set; }
		public Point P2 { get; private set; }
		public Point P3 { get; private set; }

        public EasingFunctionBezier(Point start, Point end)
		{
			P0 = new Point(0, 0);
			P1 = start;
			P2 = end;
			P3 = new Point(1, 1);
		}

		public EasingFunctionBezier(double startX, double startY, double endX, double endY) :
			this(new Point(startX, startY), new Point(endX, endY))
		{
		}

		public Tuple<EasingFunctionBezier, EasingFunctionBezier> Split(double time)
		{
			// First find the splitting point, this will be the last/first of the
			// two resulting curves.
			var p4 = Bezier4(P0, P1, P2, P3, time);

			var p5 = new Point(((P0.X + P1.X) * time), ((P0.Y + P1.Y) * time));
			var p6 = new Point(((P2.X + P3.X) * time), ((P2.Y + P3.Y) * time));
			var p7 = new Point(((P1.X + P2.X) * time), ((P1.Y + P2.Y) * time));
			var p8 = new Point(((p5.X + p7.X) * time), ((p5.Y + p7.Y) * time));
			var p9 = new Point(((p6.X + p7.X) * time), ((p6.Y + p7.Y) * time));

			var item1 = new EasingFunctionBezier(p5, p8) { P3 = p4 };
			item1 = item1.Mul(p4);
			var item2 = new EasingFunctionBezier(p9, p6) { P0 = p4 };

			return new Tuple<EasingFunctionBezier, EasingFunctionBezier>(
				item1, item2);
				
		}

		public EasingFunctionBezier Mul(Point p)
		{
			return new EasingFunctionBezier(new Point(P1.X / p.X, P1.Y / p.Y),
											new Point(P2.X / p.X, P2.Y / p.Y))
			{
				P0 = new Point(P0.X / p.X, P0.Y / p.Y),
				P3 = new Point(P3.X / p.X, P3.Y / p.Y),
			};
		}

		public override string ToString()
		{
			return string.Format("[EasingFunctionBezier: P0={0}, P1={1}, P2={2}, P3={3}]", 
				P0, P1, P2, P3);
		}

		Point Bezier4(Point p1, Point p2, Point p3, Point p4, double mu)
		{
			double mum1, mum13, mu3;
			Point p = new Point();

			mum1 = 1 - mu;
			mum13 = mum1 * mum1 * mum1;
			mu3 = mu * mu * mu;

			p.X = mum13 * p1.X + 3 * mu * mum1 * mum1 * p2.X + 3 * 
				mu * mu * mum1 * p3.X + mu3 * p4.X;
			
			p.Y = mum13 * p1.Y + 3 * mu * mum1 * mum1 * p2.Y + 3 * 
				mu * mu * mum1 * p3.Y + mu3 * p4.Y;
			
		   return(p);
		}
	}
}
