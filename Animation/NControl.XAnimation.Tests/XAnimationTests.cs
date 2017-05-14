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
		public void Test_Animate_Contains_Zero_AnimationInfo()
		{
			// Arrange
			var animation = new XAnimationPackage();

			// Act
			animation.Then();

			// Assert
			Assert.AreEqual(0, animation.AnimationInfos.Count());
		}

		[Test()]
		public void Test_Duration_With_Animate_Contains_One_AnimationInfo()
		{
			// Arrange
			var animation = new XAnimationPackage();

			// Act
			animation.Duration(100).Then();

			// Assert
			Assert.AreEqual(1, animation.AnimationInfos.Count());
		}

		[Test()]
		public void Test_Duration_And_Scale_With_Animate_Contains_One_AnimationInfo()
		{
			// Arrange
			var animation = new XAnimationPackage();

			// Act
			animation.Duration(100).Scale(0.5).Then();

			// Assert
			Assert.AreEqual(1, animation.AnimationInfos.Count());
		}

		[Test()]
		public void Test_Duration_And_Scale_And_Translation_With_Animate_Contains_One_AnimationInfo()
		{
			// Arrange
			var animation = new XAnimationPackage();

			// Act
			animation.Duration(100).Scale(0.5).Translate(10, 10).Then();

			// Assert
			Assert.AreEqual(1, animation.AnimationInfos.Count());
		}

		[Test()]
		public void Test_Rotation_Set_And_Scale_With_Animate_Contains_Two_AnimationInfos()
		{
			// Arrange
			var animation = new XAnimationPackage();

			// Act
			animation.Rotate(90).Set().Scale(0.5).Translate(10, 10).Then();

			// Assert
			Assert.AreEqual(2, animation.AnimationInfos.Count());
		}

		[Test()]
		public void Test_Duration_Multiple_Animate_Returns_One_AnimationInfo()
		{
			// Arrange
			var animation = new XAnimationPackage();

			// Act
			animation
				.Duration(1250)
				.Then()
				.Then()
				.Then()
				.Then();

			// Assert
			Assert.AreEqual(1, animation.AnimationInfos.Count());
		}

		[Test()]
		public void Test_Duration_Multiple_Set_Returns_One_AnimationInfo()
		{
			// Arrange
			var animation = new XAnimationPackage();

			// Act
			animation
				.Duration(1250)
				.Set()
				.Set()
				.Set()
				.Set();

			// Assert
			Assert.AreEqual(1, animation.AnimationInfos.Count());
		}

		[Test()]
		public void Test_Duration_Opacity_Animat_Reset_Animat_Returns_Two_AnimationInfos()
		{
			// Arrange
			var animation = new XAnimationPackage();

			// Act
			animation
				.Duration(1250)
				.Opacity(0)
				.Then()
				.Reset()
				.Then();

			// Assert
			Assert.AreEqual(2, animation.AnimationInfos.Count());
		}
	}
}
