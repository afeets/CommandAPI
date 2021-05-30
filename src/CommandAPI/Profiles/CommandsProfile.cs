using AutoMapper;
using CommandAPI.Dtos;
using CommandAPI.Models;

namespace CommandAPI.Profiles
{
    // class inherits from Automapper.profile
    public class CommandsProfile : Profile
    {
        // create constructor
        public CommandsProfile()
        {
            // use createmap method to map Command to CommandReadDto
            CreateMap<Command, CommandReadDto>();
        }
    }
}