using Cronos;

namespace Example.BackgroundTasks.Services
{
    public abstract class CronJobServiceBase : IHostedService, IDisposable
    {

        private System.Timers.Timer? _timer;
        private readonly CronExpression _expression;
        private readonly TimeZoneInfo _timeZoneInfo;
        private readonly ILogger<CronJobServiceBase> _logger;

        protected CronJobServiceBase(string cronExpression, TimeZoneInfo timeZoneInfo, ILogger<CronJobServiceBase> logger) 
        {
            _timer = null;
            _expression = CronExpression.Parse(cronExpression);
            _timeZoneInfo = timeZoneInfo;
            _logger = logger;
        }

        public virtual async Task StartAsync(CancellationToken cancellationToken)
        {
            await ScheduleJob(cancellationToken);
        }

        protected virtual async Task ScheduleJob(CancellationToken cancellationToken)
        {
            var next = _expression.GetNextOccurrence(DateTimeOffset.Now, _timeZoneInfo);
            if (next.HasValue)
            {
                var delay = next.Value - DateTimeOffset.Now;
                if (delay.TotalMilliseconds <= 0)   // prevent non-positive values from being passed into Timer
                {
                    await ScheduleJob(cancellationToken);
                }
                _timer = new System.Timers.Timer(delay.TotalMilliseconds);
                _timer.Elapsed += async (sender, args) =>
                {
                    _timer.Dispose();  // reset and dispose timer
                    _timer = null;
                    if (!cancellationToken.IsCancellationRequested)
                    {
                        try { await ExecuteAsync(cancellationToken); } catch(Exception ex) {
                            _logger.LogError($"Falied execute job Error {ex}");
                        }
                    }
                    if (!cancellationToken.IsCancellationRequested)
                    {
                        await ScheduleJob(cancellationToken);    // reschedule next
                    }
                };
                _timer.Start();
            }
            await Task.CompletedTask;
        }

        public virtual async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            await Task.Delay(500, cancellationToken);  // do the work
        }

        public virtual async Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Stop();
            await Task.CompletedTask;
        }

        public virtual void Dispose()
        {
            _timer?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
