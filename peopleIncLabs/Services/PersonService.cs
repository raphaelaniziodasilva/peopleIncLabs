using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using PeopleIncApi.Exceptions;
using peopleIncLabs.Data;
using peopleIncLabs.Data.Dtos;
using peopleIncLabs.Exceptions;
using peopleIncLabs.Interfaces;
using peopleIncLabs.Models;
using System.Reflection.PortableExecutable;

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

        public async Task DeletePersonAsync(long id)
        {
            try
            {
                var person = await GetPersonByIdAsync(id);

                _context.Person.Remove(person);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao deletar pessoa");
                throw;
            }
        }

        public async Task UploadCsvFileAsync(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    throw new BadRequestException("Arquivo não adicionado ou arquivo está vazio.");
                }

                if (file.Length > 1024 * 1024)
                {
                    throw new BadRequestException("Tamanho do arquivo 1MB.");
                }

                List<string> invalidLines = new List<string>();

                using (var reader = new StreamReader(file.OpenReadStream()))
                {
                    const string expectedHeader = "nome;idade;email;";

                    var headerLine = await reader.ReadLineAsync();
                    var header = headerLine.ToLower();

                    if (header != expectedHeader.ToLower())
                    {
                        throw new HeaderException("Arquivo CSV inválido.");
                    }

                    int lineNumber = 2;

                    while (!reader.EndOfStream)
                    {
                        var line = (await reader.ReadLineAsync()).TrimEnd(';');
                        var values = line.Split(';');

                        if (values.Length != 3)
                        {
                            invalidLines.Add($"Linha {lineNumber}: {line} -  inválida");
                            continue;
                        }

                        if (!int.TryParse(values[1], out _))
                        {
                            invalidLines.Add($"Linha {lineNumber}: {line} - Idade inválida");
                            continue;
                        }

                        if (await _context.Person.AnyAsync(p => p.Email == values[2]))
                        {
                            invalidLines.Add($"Linha {lineNumber}: {line} - E-mail já cadastrado");
                            continue;
                        }

                        lineNumber++;

                        try
                        {
                            var person = new CreatePersonDto
                            {
                                Name = values[0],
                                Age = int.Parse(values[1]),
                                Email = values[2]
                            };

                            await CreatePersonAsync(person);
                        }
                        catch (FormatException)
                        {
                            invalidLines.Add($"Linha {lineNumber}: {line} - Erro ao converter idade.");
                        }
                    }
                }

                if (invalidLines.Any())
                {
                    throw new InvalidDataException($"Linhas inválidas:\n{string.Join("\n", invalidLines)}.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao adicionar arquivo csv");
                throw;
            }
        }

    }
}
