using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using CommandAPI.Data;
using CommandAPI.Models;
using AutoMapper;
using CommandAPI.Dtos;
using Microsoft.AspNetCore.JsonPatch;

namespace CommandAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommandsController : ControllerBase
    {
        private readonly ICommandAPIRepo _repository;
        private readonly IMapper _mapper;

        // instance of IMapper injected by Dependency Injection into constructor
        public CommandsController(ICommandAPIRepo repository, IMapper mapper)
        {
            _repository = repository;

            // assign injected instance to private member for future use
            _mapper = mapper;
        }

        [HttpGet]
        public ActionResult<IEnumerable<CommandReadDto>> GetAllCommands()
        {
            var commandItems = _repository.GetAllCommands();
            return Ok(_mapper.Map<IEnumerable<CommandReadDto>>(commandItems));
        }

        // name action, in order for any call to CreatedAtRoute to work
        [HttpGet("{id}", Name = "GetCommandById")]
        public ActionResult<CommandReadDto> GetCommandById(int id)
        {
            var commandItem = _repository.GetCommandById(id);
            if (commandItem == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<CommandReadDto>(commandItem));
        }


        [HttpPost]
        public ActionResult <CommandReadDto> CreateCommand (CommandCreateDto commandCreateDto)
        {
            var commandModel = _mapper.Map<Command>(commandCreateDto);
            _repository.CreateCommand(commandModel);
            _repository.SaveChanges();

            var CommandReadDto = _mapper.Map<CommandReadDto>(commandModel);

            return CreatedAtRoute(nameof(GetCommandById), 
                new { Id = CommandReadDto.Id }, CommandReadDto);
        }


        // PUT : api/commands/{id}
        [HttpPut("{id}")]
        // id passed in from route, commandUpdateDto passed in request body
        public ActionResult UpdateCommand(int id, CommandUpdateDto commandUpdateDto)
        {
            // retrieve existing value from repository
            var commandModelFromRepo = _repository.GetCommandById(id);
            if ( commandModelFromRepo == null )
            {
                return NotFound();
            }

            // where update takes place
            _mapper.Map(commandUpdateDto, commandModelFromRepo);
            
            // line is not necessary, but a different provider 
            // may require call to UpdateCommand Method
            _repository.UpdateCommand(commandModelFromRepo);

            _repository.SaveChanges();

            return NoContent();
        }


        // PATCH : api/commands/{id}
        [HttpPatch("{id}")]
        // id passed in from route, commandUpdateDto passed in request body
        public ActionResult PartialCommandUpdate(int id, 
            JsonPatchDocument<CommandUpdateDto> patchDoc)
        {
            // retrieve existing value from repository
            var commandModelFromRepo = _repository.GetCommandById(id);
            if ( commandModelFromRepo == null )
            {
                return NotFound();
            }

            // where update takes place
            var commandToPatch = _mapper.Map<CommandUpdateDto>(commandModelFromRepo);
            patchDoc.ApplyTo(commandToPatch, ModelState);
            

            if(!TryValidateModel(commandToPatch))
            {
                return ValidationProblem(ModelState);
            }

            _mapper.Map(commandToPatch, commandModelFromRepo);

            // line is not necessary, but a different provider 
            // may require call to UpdateCommand Method
            _repository.UpdateCommand(commandModelFromRepo);

            _repository.SaveChanges();

            return NoContent();
        }


        // DELETE: /api/commands/{id}
        [HttpDelete("{id}")]
        public ActionResult DeleteCommand(int id)
        {
            var commandModelFromRepo = _repository.GetCommandById(id);
            
            if( commandModelFromRepo == null )
            {
                return NotFound();
            }

            _repository.DeleteCommand(commandModelFromRepo);
            _repository.SaveChanges();
            return NoContent();
        }        
       /*  
       [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] {"this","is","hard","coded"};
        } 
        */
    }
}