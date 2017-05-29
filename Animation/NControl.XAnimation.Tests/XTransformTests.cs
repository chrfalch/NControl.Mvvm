using System;
using NUnit.Framework;

namespace NControl.XAnimation.Tests
{
	[TestFixture]
	public class XTransformTests
	{
		[Test]
		public void Test_Split_Simple_Bezier_In_Two_Returns_Correct_Point()
		{
			// Arrange
			var bezier = new EasingFunctionBezier(0, 0, 1, 1);

			// Act
			var split = bezier.Interpolate(0.5);

			// Assert
			Assert.AreEqual(1.0, split.X, "X");
			Assert.AreEqual(1.0, split.Y, "Y");
		}

		[Test]
		public void Test_Split_Simple_Bezier_In_Three_Returns_Correct_Point()
		{
			// Arrange
			var bezier = new EasingFunctionBezier(0, 0, 1, 1);

			// Act
			var split = bezier.Interpolate(0.75);

			// Assert
			Assert.AreEqual(1.0, split.X, "X");
			Assert.AreEqual(1.0, split.Y, "Y");
		}
	}
}
