using peopleIncLabs.Data.Dtos;
using peopleIncLabs.Models;

namespace peopleIncLabs.Interfaces
{
    public interface IPersonService
    {
        Task<Person> CreatePersonAsync(CreatePersonDto personDto);
        Task<IEnumerable<Person>> GetPersonsAsync(int pageNumber, int pageSize);
        Task<Person> GetPersonByIdAsync(long id);
        Task<Person> UpdatePersonAsync(long id, CreatePersonDto personDto);
        Task<bool> DeletePersonAsync(long id);
       
    }
}
