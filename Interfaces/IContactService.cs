
using System.Collections.Generic;
using System.Threading.Tasks;
using ProjectName.Types;

namespace ProjectName.Interfaces
{
    /// <summary>
    /// Interface for managing contact-related operations.
    /// </summary>
    public interface IContactService
    {
        /// <summary>
        /// Creates a new contact.
        /// </summary>
        /// <param name="createContactDto">The data transfer object containing the details of the contact to be created.</param>
        /// <returns>A string representing the result of the operation.</returns>
        Task<string> CreateContact(CreateContactDto createContactDto);

        /// <summary>
        /// Retrieves a contact based on the provided request details.
        /// </summary>
        /// <param name="contactRequestDto">The data transfer object containing the request details to retrieve a contact.</param>
        /// <returns>A Contact object representing the retrieved contact.</returns>
        Task<Contact> GetContact(ContactRequestDto contactRequestDto);

        /// <summary>
        /// Updates an existing contact.
        /// </summary>
        /// <param name="updateContactDto">The data transfer object containing the details of the contact to be updated.</param>
        /// <returns>A string representing the result of the operation.</returns>
        Task<string> UpdateContact(UpdateContactDto updateContactDto);

        /// <summary>
        /// Deletes a contact based on the provided details.
        /// </summary>
        /// <param name="deleteContactDto">The data transfer object containing the details of the contact to be deleted.</param>
        /// <returns>A boolean indicating whether the operation was successful.</returns>
        Task<bool> DeleteContact(DeleteContactDto deleteContactDto);

        /// <summary>
        /// Retrieves a list of contacts based on the provided request details.
        /// </summary>
        /// <param name="listContactRequestDto">The data transfer object containing the request details to retrieve a list of contacts.</param>
        /// <returns>A list of Contact objects representing the retrieved contacts.</returns>
        Task<List<Contact>> GetListContact(ListContactRequestDto listContactRequestDto);
    }
}
