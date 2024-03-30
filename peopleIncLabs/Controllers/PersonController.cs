using Microsoft.AspNetCore.Mvc;
using peopleIncLabs.Data.Dtos;
using peopleIncLabs.Exceptions;
using peopleIncLabs.Interfaces;
using peopleIncLabs.Models;
using System;

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
        [HttpPost("cadastro")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreatePerson(CreatePersonDto personDto)
        {
            try
            {
                var person = await _personService.CreatePersonAsync(personDto);
                return Ok(person);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno no servidor" });
            }
        }

        /// <summary>
        /// Listando todas as pessoas
        /// </summary>
        /// <returns>IActionResult</returns>
        /// <param name="pageNumber">Número de páginas.</param>
        /// <param name="pageSize">Contém pessoas em cada página.</param>
        [HttpGet("lista-de-pessoas")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetPerson(int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var persons = await _personService.GetPersonsAsync(pageNumber, pageSize);
                return Ok(persons);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno no servidor" });
            }
        }

        /// <summary>
        /// Obtendo pessoa pelo seu ID.
        /// </summary>
        /// <returns>IActionResult</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetPersonById(long id)
        {
            try
            {
                return Ok(await _personService.GetPersonByIdAsync(id));
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = "Pessoa não encontrada" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno no servidor" });
            }
        }

        /// <summary>
        /// Atualizando Pessoa
        /// </summary>
        /// <param name="personDto">Dados necessário para atualizar pessoa</param>
        /// <returns>IActionResult</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdatePerson(long id, UpdatePersonDto personDto)
        {
            try
            {
                var updatedPerson = await _personService.UpdatePersonAsync(id, personDto);
                return Ok(updatedPerson);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = "Pessoa não encontrada" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno no servidor" });
            }

        }

        /// <summary>
        /// Removendo pessoa
        /// </summary>
        /// <returns>IActionResult</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeletePerson(long id)
        {
            try
            {
                return Ok(await _personService.DeletePersonAsync(id));
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = "Pessoa não encontrada" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno no servidor" });
            }
        }

    }
}
