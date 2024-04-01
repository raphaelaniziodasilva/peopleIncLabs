using System;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using peopleIncLabs.Data;
using peopleIncLabs.Services;
using peopleIncLabs.Data.Dtos;
using peopleIncLabs.Models;
using peopleIncLabs.Exceptions;
using Microsoft.AspNetCore.Http;
using PeopleIncApi.Exceptions;

namespace PeopleIncLabsTests.service
{
    public class PersonServiceTests
    {
        private readonly DbContextOptions<PersonContext> _contextOptions;

        public PersonServiceTests()
        {
            _contextOptions = new DbContextOptionsBuilder<PersonContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
        }

        public long GetNewId()
        {
            using (var context = new PersonContext(_contextOptions))
            {
                long maxId = context.Person.Any() ? context.Person.Max(p => p.Id) : 0;
                return maxId + 1;
            }
        }


        [Fact]
        public async Task CreatePersonAsyncEmailNotRegistered()
        {
            using (var context = new PersonContext(_contextOptions))
            {
                var service = new PersonService(context, Mock.Of<ILogger<PersonService>>());
                var person = new CreatePersonDto
                {
                    Name = "Uzumaki Naruto",
                    Age = 25,
                    Email = "narutouzumaki@gmail.com"
                };

                var result = await service.CreatePersonAsync(person);

                Assert.NotNull(result);
                Assert.Equal("Uzumaki Naruto", result.Name);
                Assert.Equal(25, result.Age);
                Assert.Equal("narutouzumaki@gmail.com", result.Email);
            }
        }

        [Fact]
        public async Task CreatePersonAsyncEmailAlreadyRegistered()
        {
            using (var context = new PersonContext(_contextOptions))
            {
                var existingPerson = new Person
                {
                    Name = "Uchiha Madara",
                    Age = 36,
                    Email = "madara@gmail.com"
                };
                context.Person.Add(existingPerson);
                context.SaveChanges();

                var service = new PersonService(context, Mock.Of<ILogger<PersonService>>());
                var person = new CreatePersonDto
                {
                    Name = "Uchiha Madara",
                    Age = 36,
                    Email = "madara@gmail.com"
                };

                await Assert.ThrowsAsync<ArgumentException>(() => service.CreatePersonAsync(person));
            }
        }

        [Fact]
        public async Task GetPersonByIdAsyncValidId()
        {
            long id = GetNewId();
            var person = new Person { Id = id, Name = "Uzumaki Naruto", Age = 25, Email = "narutouzumaki@gmail.com" };

            using (var context = new PersonContext(_contextOptions))
            {
                context.Person.Add(person);
                context.SaveChanges();
            }

            using (var context = new PersonContext(_contextOptions))
            {
                var service = new PersonService(context, Mock.Of<ILogger<PersonService>>());

                var result = await service.GetPersonByIdAsync(id);

                Assert.NotNull(result);
                Assert.Equal(id, result.Id);
                Assert.Equal("Uzumaki Naruto", result.Name);
                Assert.Equal(25, result.Age);
                Assert.Equal("narutouzumaki@gmail.com", result.Email);
            }
        }

        [Fact]
        public async Task GetPersonByIdAsyncIdNotExist()
        {
            long IdNotExist = 194;

            using (var context = new PersonContext(_contextOptions))
            {
                var service = new PersonService(context, Mock.Of<ILogger<PersonService>>());

                await Assert.ThrowsAsync<NotFoundException>(() => service.GetPersonByIdAsync(IdNotExist));
            }
        }

        [Fact]
        public async Task UpdatePersonAsyncValidId()
        {
            long id = GetNewId();
            var existingPerson = new Person {
                Id = id,
                Name = "Uzumaki Naruto",
                Age = 25,
                Email = "narutouzumaki@gmail.com"
            };
            var newPerson = new UpdatePersonDto {
                Name = "Eren Yeager",
                Age = 28,
                Email = "eren@gmail.com"
            };

            using (var context = new PersonContext(_contextOptions))
            {
                context.Person.Add(existingPerson);
                context.SaveChanges();
            }

            using (var context = new PersonContext(_contextOptions))
            {
                var service = new PersonService(context, Mock.Of<ILogger<PersonService>>());

                var updatedPerson = await service.UpdatePersonAsync(id, newPerson);

                Assert.Equal(id, updatedPerson.Id);
                Assert.Equal("Eren Yeager", updatedPerson.Name);
                Assert.Equal(28, updatedPerson.Age);
                Assert.Equal("eren@gmail.com", updatedPerson.Email);
            }

            using (var context = new PersonContext(_contextOptions))
            {
                context.Database.EnsureDeleted();
            }
        }

        [Fact]
        public async Task UpdatePersonAsyncIdNotExist()
        {
            long IdNotExist = 232;
            var person = new UpdatePersonDto { Name = "Rainer", Age = 30, Email = "rainer@gmail.com" };

            using (var context = new PersonContext(_contextOptions))
            {
                var service = new PersonService(context, Mock.Of<ILogger<PersonService>>());

                await Assert.ThrowsAsync<NotFoundException>(() => service.UpdatePersonAsync(IdNotExist, person));
            }
        }

        [Fact]
        public async Task DeletePersonAsyncValidId()
        {
            long id = GetNewId();
            var existingPerson = new Person { Id = id, Name = "Uzumaki Naruto", Age = 25, Email = "narutouzumaki@gmail.com" };

            using (var context = new PersonContext(_contextOptions))
            {
                context.Person.Add(existingPerson);
                context.SaveChanges();
            }

            using (var context = new PersonContext(_contextOptions))
            {
                var service = new PersonService(context, Mock.Of<ILogger<PersonService>>());

                await service.DeletePersonAsync(id);

                var deletedPerson = await context.Person.FirstOrDefaultAsync(p => p.Id == id);
                Assert.Null(deletedPerson);
            }

            using (var context = new PersonContext(_contextOptions))
            {
                context.Database.EnsureDeleted();
            }
        }

        [Fact]
        public async Task DeletePersonAsyncIdNotExist()
        {
            long IdNotExist = 142;

            using (var context = new PersonContext(_contextOptions))
            {
                var service = new PersonService(context, Mock.Of<ILogger<PersonService>>());

                await Assert.ThrowsAsync<NotFoundException>(() => service.DeletePersonAsync(IdNotExist));
            }
        }

        [Fact]
        public async Task UploadCsvFileAsyncCsvValid()
        {
            var file = new Mock<IFormFile>();
            var content = "nome;idade;email;\n" +
                          "Kokushibo;125;kokushi@gmail.com\n" +
                          "Akaza;112;akaza@gmail.com\n" +
                          "Doma;128;doma@gmail.com\n";

            var fileName = "test.csv";
            var ms = new MemoryStream();
            var writer = new StreamWriter(ms);
            writer.Write(content);
            writer.Flush();
            ms.Position = 0;
            file.Setup(_ => _.FileName).Returns(fileName);
            file.Setup(_ => _.Length).Returns(ms.Length);
            file.Setup(_ => _.OpenReadStream()).Returns(ms);

            using (var context = new PersonContext(_contextOptions))
            {
                var service = new PersonService(context, Mock.Of<ILogger<PersonService>>());

                await service.UploadCsvFileAsync(file.Object);
                var person = await context.Person.FirstOrDefaultAsync(p => p.Email == "akaza@gmail.com");
                Assert.NotNull(person);
            }

            using (var context = new PersonContext(_contextOptions))
            {
                context.Database.EnsureDeleted();
            }
        }

        [Fact]
        public async Task UploadCsvFileAsyncEmptyFile()
        {
            var file = new Mock<IFormFile>();
            var content = "";
            var fileName = "test.csv";
            var ms = new MemoryStream();
            var writer = new StreamWriter(ms);
            writer.Write(content);
            writer.Flush();
            ms.Position = 0;
            file.Setup(_ => _.FileName).Returns(fileName);
            file.Setup(_ => _.Length).Returns(ms.Length);
            file.Setup(_ => _.OpenReadStream()).Returns(ms);

            using (var context = new PersonContext(_contextOptions))
            {
                var service = new PersonService(context, Mock.Of<ILogger<PersonService>>());

                await Assert.ThrowsAsync<BadRequestException>(() => service.UploadCsvFileAsync(file.Object));
            }
        }

        [Fact]
        public async Task UploadCsvFileAsyncFileSizeExceeded()
        {
            var file = new Mock<IFormFile>();
            var content = new byte[1024 * 1024 + 1];
            var fileName = "test.csv";
            var ms = new MemoryStream(content);
            file.Setup(_ => _.FileName).Returns(fileName);
            file.Setup(_ => _.Length).Returns(ms.Length);
            file.Setup(_ => _.OpenReadStream()).Returns(ms);

            using (var context = new PersonContext(_contextOptions))
            {
                var service = new PersonService(context, Mock.Of<ILogger<PersonService>>());

                await Assert.ThrowsAsync<BadRequestException>(() => service.UploadCsvFileAsync(file.Object));
            }
        }
    }
}
