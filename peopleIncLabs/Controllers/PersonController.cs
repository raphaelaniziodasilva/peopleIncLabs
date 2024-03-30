using Microsoft.AspNetCore.Mvc;
using peopleIncLabs.Data.Dtos;
using peopleIncLabs.Interfaces;
using peopleIncLabs.Models;

namespace peopleIncLabs.Controllers
{
    
    [ApiController]
    [Route("api/[controller]")]
    public class PersonController : ControllerBase
    {
        private readonly IPersonService _personService;

        public PersonController(IPersonService personService)
        {
            _personService = personService;
        }

        /// <summary>
        /// Adicionar Pessoa
        /// </summary>
        /// <param name="personDto">Dados necessário para adicionar uma pessoa</param>
        /// <returns>IActionResult</returns>
        /// <response code="201">Pessoa adicionada com sucesso</response>
        /// <response code="400">Dados inválidos</response>
        /// <response code="500">Erro interno no servidor</response>
        [HttpPost("cadastro")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)] 
        public async Task<ActionResult<Person>> CreatePerson([FromBody] CreatePersonDto personDto)
        {
            try
            {
                var person = await _personService.CreatePersonAsync(personDto);
                return Ok(person);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {   
                return StatusCode(500, "Ocorreu um erro ao cadastrar a pessoa.");
            }
        }

        /// <summary>
        /// Listando todas as pessoas
        /// </summary>
        /// <returns>IActionResult</returns>
        /// <param name="pageNumber">Número de páginas.</param>
        /// <param name="pageSize">Contém pessoas em cada página.</param>
        /// <response code="200">Lista de pessoas obtida com sucesso</response>
        /// <response code="400">Erro ao obter lista de pessoas</response>
        /// <response code="500">Erro interno no servidor</response>
        [HttpGet("Lista de Pessoas")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<Person>>> GetPerson([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var persons = await _personService.GetPersonsAsync(pageNumber, pageSize);
                return Ok(persons);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Ocorreu um erro ao obter as pessoas.");
            }
        }


    }
}
