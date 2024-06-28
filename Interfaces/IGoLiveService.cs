
using ProjectName.Types;

namespace ProjectName.Interfaces
{
    /// <summary>
    /// Interface for services related to going live.
    /// </summary>
    public interface IGoLiveService
    {
        /// <summary>
        /// Asynchronously creates a go-live event based on the provided data.
        /// </summary>
        /// <param name="createGoLiveDto">The data transfer object containing information needed to create a go-live event.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a string indicating the result of the creation process.</returns>
        Task<string> CreateGoLive(CreateGoLiveDto createGoLiveDto);
    }
}
