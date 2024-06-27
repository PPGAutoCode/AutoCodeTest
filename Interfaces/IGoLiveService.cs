
using System.Threading.Tasks;
using ProjectName.Types;

namespace ProjectName.Interfaces
{
    /// <summary>
    /// Interface for services related to going live.
    /// </summary>
    public interface IGoLiveService
    {
        /// <summary>
        /// Asynchronously creates a go-live event.
        /// </summary>
        /// <param name="createGoLive">The data required to create a go-live event.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the string identifier of the created go-live event.</returns>
        Task<string> CreateGoLiveAsync(CreateGoLive createGoLive);
    }
}
