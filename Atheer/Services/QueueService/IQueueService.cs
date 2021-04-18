using System.Threading.Tasks;

namespace Atheer.Services.QueueService
{
    public interface IQueueService
    {
        Task Queue(QueueType type, QueueMessage message);
    }
}