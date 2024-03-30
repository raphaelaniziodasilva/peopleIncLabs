using Microsoft.EntityFrameworkCore;
using peopleIncLabs.Data;
using peopleIncLabs.Data.Dtos;
using peopleIncLabs.Interfaces;
using peopleIncLabs.Models;

namespace peopleIncLabs.Services
{
    public class PersonService : IPersonService
    {
        private readonly PersonContext _context;
        public PersonService(PersonContext context)
        {
            _context = context;
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
            catch (Exception ex)
            {
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
                throw new Exception("Ocorreu um erro ao obter as pessoas.", ex);
            }
        }

        public async Task<Person> GetPersonByIdAsync(long id)
        {
            throw new NotImplementedException();
        }

        public async Task<Person> UpdatePersonAsync(long id, CreatePersonDto personDto)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> DeletePersonAsync(long id)
        {
            throw new NotImplementedException();
        }
    }
}
