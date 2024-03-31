using peopleIncLabs.Data.Dtos;
using peopleIncLabs.Models;

namespace peopleIncLabs.Interfaces
{
    public interface IPersonService
    {
        Task<Person> CreatePersonAsync(CreatePersonDto personDto);
        Task<IEnumerable<Person>> GetPersonsAsync(int pageNumber, int pageSize);
        Task<Person> GetPersonByIdAsync(long id);
        Task<Person> UpdatePersonAsync(long id, UpdatePersonDto personDto);
        Task DeletePersonAsync(long id);
        Task UploadCsvFileAsync(IFormFile file);
    }
}
