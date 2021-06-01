using System;
using System.Collections.Generic;
using Moq;
using AutoMapper;
using CommandAPI.Models;
using CommandAPI.Data;
using Xunit;
using CommandAPI.Controllers;
using Microsoft.AspNetCore.Mvc;
using CommandAPI.Profiles;
using CommandAPI.Dtos;

namespace CommandAPI.Tests
{
    public class CommandsControllerTests : IDisposable
    {
        Mock<ICommandAPIRepo> mockRepo;
        CommandsProfile realprofile;
        MapperConfiguration configuration;
        IMapper mapper;

        public CommandsControllerTests()
        {
            // setup mock instance of repository
            mockRepo = new Mock<ICommandAPIRepo>();
            
            // setup instance, and assign
            realprofile = new CommandsProfile();
            configuration = new MapperConfiguration(
                cfg => cfg.AddProfile(realprofile)
            );
            
            // create concrete instance pass MapperConfiguration
            mapper = new Mapper(configuration);
        }

        public void Dispose()
        {
            mockRepo = null;
            mapper = null;
            configuration = null;
            realprofile = null;
        }

        [Fact]
        public void GetCommandItems_ReturnZeroItems_WhenDBIsEmpty()
        {
            // Arrange

            // use setup method to establish how it will behave, 
            // specifiying interface method to mock, and what we want to return
            mockRepo.Setup(repo => repo.GetAllCommands()).Returns(GetCommands(0));

            // We need to create an instance of our CommandsController class
            var controller = new CommandsController( 
                // repository
                mockRepo.Object, 
                // Automapper - pass IMapper instance to CommandController
                mapper 
            );

            // Act
            var result = controller.GetAllCommands();

            // Assert
            Assert.IsType<OkObjectResult>(result.Result);
        }


        [Fact]
        public void GetAllCommands_ReturnsOneItem_WhenDBHasOneResource()
        {
            // Arrange - return single command resource
            mockRepo.Setup( repo => repo.GetAllCommands()).Returns(GetCommands(1));

            var controller = new CommandsController(mockRepo.Object, mapper);

            // Act
            var result = controller.GetAllCommands();

            // Assert - convert result to OkObjectResult object, to navigate object hierarchy 
            var okResult = result.Result as OkObjectResult;

            // obtain list of Dtos
            var commands = okResult.Value as List<CommandReadDto>;

            // check we have single resultset on commands list
            Assert.Single(commands);
        }


        [Fact]
        public void GetAllCommands_Returns200Ok_WhenDBhasOneResource()
        {
            // Arrange
            mockRepo.Setup( repo => repo.GetAllCommands()).Returns(GetCommands(1));

            var controller = new CommandsController(mockRepo.Object, mapper);

            // Act
            var result = controller.GetAllCommands();

            // Assert
            Assert.IsType<OkObjectResult>(result.Result);
        }

        [Fact]
        public void GetAllCommands_ReturnsCorrectType_WhenDBHasOneResource()
        {
            // Arrange
            mockRepo.Setup( repo => repo.GetAllCommands()).Returns(GetCommands(1));

            var controller = new CommandsController(mockRepo.Object, mapper);

            // Act
            var result = controller.GetAllCommands();

            // Assert
            Assert.IsType<ActionResult<IEnumerable<CommandReadDto>>>(result);
        }

        private List<Command> GetCommands(int num)
        {
            var commands = new List<Command>();
            if ( num > 0 ){
                commands.Add(new Command 
                {
                 Id = 0,
                 HowTo = "How to generate a migration",
                 CommandLine = "dotnet-ef migrations add <name of migration>",
                 Platform = ".NET Core EF"   
                });
            }
            return commands;
        }
    }
}