using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Atheer.Extensions
{
    public static class AsyncExtensions
    {
        public static ConfiguredTaskAwaitable CAF(this Task task) => task.ConfigureAwait(false);
        public static ConfiguredTaskAwaitable<T> CAF<T>(this Task<T> task) => task.ConfigureAwait(false);
        public static ConfiguredValueTaskAwaitable CAF(this ValueTask task) => task.ConfigureAwait(false);
        public static ConfiguredValueTaskAwaitable<T> CAF<T>(this ValueTask<T> task) => task.ConfigureAwait(false);
    }
}