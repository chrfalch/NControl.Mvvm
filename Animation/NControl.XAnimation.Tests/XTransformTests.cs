using System;
using NUnit.Framework;

namespace NControl.XAnimation.Tests
{
	[TestFixture]
	public class XTransformTests
	{
		[Test]
public void Test_Slice_Simple_Bezier_Returns_Correct_Point_1()
		{
			// Arrange
			var bezier = new EasingFunctionBezier(0, 0, 1, 1);

			// Act
			var split = bezier.Split(0.5);

			// Assert
			Assert.AreEqual(0.5, split.Item1.P2.X, "X");
			Assert.AreEqual(0.5, split.Item1.P2.Y, "Y");
		}

		[Test]
		public void Test_Slice_Simple_Bezier_Returns_Correct_Point_2()
		{
			// Arrange
			var bezier = new EasingFunctionBezier(0, 0, 1, 1);

			// Act
			var split = bezier.Split(0.75);

			// Assert
			Assert.AreEqual(0.66666666666666663d, split.Item1.P2.X, "X");
			Assert.AreEqual(0.66666666666666663d, split.Item1.P2.Y, "Y");
		}

		[Test]
		public void Test_Slice_Simple_Bezier_Returns_Correct_Point_3()
		{
			// Arrange
			var bezier = new EasingFunctionBezier(0, 1, 0, 1);

			// Act
			var split = bezier.Split(0.5);

			// Assert
			Assert.AreEqual(0.0, split.Item1.P2.X, "X");
			Assert.AreEqual(0.8571428571428571d, split.Item1.P2.Y, "Y");
		}

		[Test]
		public void Test_Slice_Simple_Bezier_Returns_Correct_Point_4()
		{
			// Arrange
			var bezier = new EasingFunctionBezier(1, 0, 1, 0);

			// Act
			var split = bezier.Split(0.5);

			// Assert
			Assert.AreEqual(0.8571428571428571d, split.Item1.P2.X, "X");
			Assert.AreEqual(0.0, split.Item1.P2.Y, "Y");
		}
	}
}
