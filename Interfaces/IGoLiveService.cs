
using ProjectName.Types;

namespace ProjectName.Interfaces
{
    /// <summary>
    /// Interface for services related to going live.
    /// </summary>
    public interface IGoLiveService
    {
        /// <summary>
        /// Asynchronously creates a go-live entry.
        /// </summary>
        /// <param name="createGoLiveDTO">The data transfer object containing the information needed to create a go-live entry.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the string identifier of the created go-live entry.</returns>
        Task<string> CreateGoLive(CreateGoLiveDTO createGoLiveDTO);
    }
}
