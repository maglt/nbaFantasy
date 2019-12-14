using System;
using System.Collections.Generic;
using System.Text;
using Fantasy;
using DataTransferLibrary;
using Xunit;

namespace Fantasy.Tests
{
    public class TeamFlowTests
    {
        [Fact]
        public void DoesUserExist()
        {
            //Arrange   
           // bool expexted = true;

            //Act
            var p = new DataAccess();
            bool actual = p.DoesUserExist("usr1");

            //Assert

            Assert.True(actual);
           
        }

        [Theory]
        [InlineData("usr1")]
        [InlineData("poo")]
        public void DoesUserExistTheory(string userName)
        {
            //Arrange   
            // bool expexted = true;

            //Act
            var p = new DataAccess();
            bool actual = p.DoesUserExist(userName);

            //Assert

            Assert.True(actual);

        }


        /*[Fact]
        public void ProfileRepository_GetSettingsForUserIDWithInvalidArguments_ThrowsArgumentException()
        {
            //arrange
            ProfileRepository profiles = new ProfileRepository();
            // act & assert
            Assert.Throws<ArgumentException>(() => profiles.GetSettingsForUserID(""));
        }
        

   
        */
    }
}
