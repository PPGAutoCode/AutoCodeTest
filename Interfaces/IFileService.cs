
using System.Collections.Generic;
using System.Threading.Tasks;
using ProjectName.Types;

namespace ProjectName.Interfaces
{
    /// <summary>
    /// Interface for managing file operations.
    /// </summary>
    public interface IFileService
    {
        /// <summary>
        /// Creates a new file based on the provided data.
        /// </summary>
        /// <param name="createFileDto">Data transfer object containing information needed to create a file.</param>
        /// <returns>A string representing the path or identifier of the created file.</returns>
        Task<string> CreateFile(CreateFileDto createFileDto);

        /// <summary>
        /// Retrieves a file based on the provided request data.
        /// </summary>
        /// <param name="fileRequestDto">Data transfer object containing information needed to retrieve a file.</param>
        /// <returns>A File object representing the retrieved file.</returns>
        Task<File> GetFile(FileRequestDto fileRequestDto);

        /// <summary>
        /// Updates an existing file based on the provided data.
        /// </summary>
        /// <param name="updateFileDto">Data transfer object containing information needed to update a file.</param>
        /// <returns>A string representing the path or identifier of the updated file.</returns>
        Task<string> UpdateFile(UpdateFileDto updateFileDto);

        /// <summary>
        /// Deletes a file based on the provided data.
        /// </summary>
        /// <param name="deleteFileDto">Data transfer object containing information needed to delete a file.</param>
        /// <returns>A boolean indicating whether the file was successfully deleted.</returns>
        Task<bool> DeleteFile(DeleteFileDto deleteFileDto);

        /// <summary>
        /// Retrieves a list of files based on the provided request data.
        /// </summary>
        /// <param name="listFileRequestDto">Data transfer object containing information needed to retrieve a list of files.</param>
        /// <returns>A list of File objects representing the retrieved files.</returns>
        Task<List<File>> GetListFile(ListFileRequestDto listFileRequestDto);
    }
}
