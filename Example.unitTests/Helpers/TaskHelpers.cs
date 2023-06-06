using Microsoft.Extensions.Hosting;

namespace Example.unitTests.Helpers
{
    static class TaskHelpers
    {
        public static async Task CancelAfter(IHostedService? task, TimeSpan time)
        {
            CancellationTokenSource source = new();
            source.CancelAfter(time);
            if (task != null) { 
                await Task.Run(() => task.StartAsync(source.Token), source.Token);
            }
        }
    }
}
