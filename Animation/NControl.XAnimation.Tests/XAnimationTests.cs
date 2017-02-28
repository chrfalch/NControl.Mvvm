using NUnit.Framework;
using System;
using NControl.XAnimation;
using System.Linq;

namespace NControl.XAnimation.Tests
{	
	[TestFixture]
	public class XAnimationTests
	{		
		[Test()]
		public void Test_Animate_Contains_One_AnimationInfo()
		{
			// Arrange
			var animation = new XAnimationPackage();

			// Act
			animation.Animate();

			// Assert
			Assert.AreEqual(1, animation.AnimationInfos.Count());
		}

		[Test()]
		public void Test_Duration_With_Animate_Contains_One_AnimationInfo()
		{
			// Arrange
			var animation = new XAnimationPackage();

			// Act
			animation.Duration(100).Animate();

			// Assert
			Assert.AreEqual(1, animation.AnimationInfos.Count());
		}

		[Test()]
		public void Test_Duration_And_Scale_With_Animate_Contains_One_AnimationInfo()
		{
			// Arrange
			var animation = new XAnimationPackage();

			// Act
			animation.Duration(100).Scale(0.5).Animate();

			// Assert
			Assert.AreEqual(1, animation.AnimationInfos.Count());
		}

		[Test()]
		public void Test_Duration_And_Scale_And_Translation_With_Animate_Contains_One_AnimationInfo()
		{
			// Arrange
			var animation = new XAnimationPackage();

			// Act
			animation.Duration(100).Scale(0.5).Translate(10, 10).Animate();

			// Assert
			Assert.AreEqual(1, animation.AnimationInfos.Count());
		}

		[Test()]
		public void Test_Rotation_Set_And_Scale_With_Animate_Contains_Two_AnimationInfos()
		{
			// Arrange
			var animation = new XAnimationPackage();

			// Act
			animation.Rotate(90).Set().Scale(0.5).Translate(10, 10).Animate();

			// Assert
			Assert.AreEqual(2, animation.AnimationInfos.Count());
		}
	}
}
