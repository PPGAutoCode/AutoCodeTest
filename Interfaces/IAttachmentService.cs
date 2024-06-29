
using System.Collections.Generic;
using System.Threading.Tasks;
using ProjectName.Types;

namespace ProjectName.Interfaces
{
    /// <summary>
    /// Interface for managing attachments.
    /// </summary>
    public interface IAttachmentService
    {
        /// <summary>
        /// Creates a new attachment.
        /// </summary>
        /// <param name="createAttachmentDto">The data transfer object containing the information needed to create an attachment.</param>
        /// <returns>A string representing the identifier of the newly created attachment.</returns>
        Task<string> CreateAttachment(CreateAttachmentDto createAttachmentDto);

        /// <summary>
        /// Retrieves an attachment based on the provided request data.
        /// </summary>
        /// <param name="attachmentRequestDto">The data transfer object containing the request information to retrieve an attachment.</param>
        /// <returns>An Attachment object representing the retrieved attachment.</returns>
        Task<Attachment> GetAttachment(AttachmentRequestDto attachmentRequestDto);

        /// <summary>
        /// Updates an existing attachment.
        /// </summary>
        /// <param name="updateAttachmentDto">The data transfer object containing the information needed to update an attachment.</param>
        /// <returns>A string representing the identifier of the updated attachment.</returns>
        Task<string> UpdateAttachment(UpdateAttachmentDto updateAttachmentDto);

        /// <summary>
        /// Deletes an attachment based on the provided request data.
        /// </summary>
        /// <param name="deleteAttachmentDto">The data transfer object containing the information needed to delete an attachment.</param>
        /// <returns>A boolean indicating whether the attachment was successfully deleted.</returns>
        Task<bool> DeleteAttachment(DeleteAttachmentDto deleteAttachmentDto);

        /// <summary>
        /// Retrieves a list of attachments based on the provided request data.
        /// </summary>
        /// <param name="listAttachmentRequestDto">The data transfer object containing the request information to retrieve a list of attachments.</param>
        /// <returns>A list of Attachment objects representing the retrieved attachments.</returns>
        Task<List<Attachment>> GetListAttachment(ListAttachmentRequestDto listAttachmentRequestDto);
    }
}
