
using ProjectName.Types;

namespace ProjectName.Interfaces
{
    /// <summary>
    /// Service interface for managing contact-related operations.
    /// </summary>
    public interface IContactService
    {
        /// <summary>
        /// Asynchronously creates a new contact.
        /// </summary>
        /// <param name="createContactDto">The data transfer object containing the details of the contact to be created.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the ID of the newly created contact.</returns>
        Task<string> CreateContact(CreateContactDto createContactDto);
    }
}
