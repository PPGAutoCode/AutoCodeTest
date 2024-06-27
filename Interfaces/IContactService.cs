
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
        /// <param name="contactDTO">The data transfer object containing the contact information to be created.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the unique identifier of the created contact.</returns>
        Task<string> CreateContact(CreateContactDTO contactDTO);
    }
}
