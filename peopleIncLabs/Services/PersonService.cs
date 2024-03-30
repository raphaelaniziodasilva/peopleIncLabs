using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using peopleIncLabs.Data;
using peopleIncLabs.Data.Dtos;
using peopleIncLabs.Exceptions;
using peopleIncLabs.Interfaces;
using peopleIncLabs.Models;
using System.ComponentModel.DataAnnotations;

namespace peopleIncLabs.Services
{
    public class PersonService : IPersonService
    {
        private readonly PersonContext _context;
        private readonly ILogger<PersonService> _logger;

        public PersonService(PersonContext context, ILogger<PersonService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Person> CreatePersonAsync(CreatePersonDto personDto)
        {
            try
            {
                if (await _context.Person.AnyAsync(p => p.Email == personDto.Email))
                {
                    throw new ArgumentException("E-mail já cadastrado");
                }

                var person = new Person
                {
                    Name = personDto.Name,
                    Age = personDto.Age,
                    Email = personDto.Email
                };

                _context.Person.Add(person);
                await _context.SaveChangesAsync();
                return person;
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Erro ao adicionar pessoa");
                throw;
            }
        }

        public async Task<IEnumerable<Person>> GetPersonsAsync(int pageNumber, int pageSize)
        {
            try
            {
                return await _context.Person
                    .OrderBy(p => p.Id)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter lista de pessoas.");
                throw;
            }
        }

        public async Task<Person> GetPersonByIdAsync(long id)
        {
            try
            {
                var person = await _context.Person.FirstOrDefaultAsync(p => p.Id == id);

                if (person == null)
                {
                    throw new NotFoundException("Pessoa não encontrada");
                }

                return person;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter pessoa por ID");
                throw;
            }
        }

        public async Task<Person> UpdatePersonAsync(long id, UpdatePersonDto personDto)
        {
            try
            {
                var person = await GetPersonByIdAsync(id);

                person.Name = personDto.Name;
                person.Age = personDto.Age;
                person.Email = personDto.Email;

                await _context.SaveChangesAsync();

                return person;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar pessoa");
                throw;
            }
        }



        public async Task<string> DeletePersonAsync(long id)
        {
            try
            {
                var person = await GetPersonByIdAsync(id);

                _context.Person.Remove(person);
                await _context.SaveChangesAsync();

                return "Pessoa removida com sucesso.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao deletar pessoa");
                throw;
            }
        }
    }
}
