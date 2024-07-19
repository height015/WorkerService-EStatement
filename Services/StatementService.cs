using Microsoft.Extensions.Options;
using WorkerService.Entities;
using WorkerService.Utils;

namespace WorkerService.Services;

public class StatementService
{
    private readonly ILogger<StatementService> _logger;
    private readonly StatementSettings _settings;

    public StatementService(ILogger<StatementService> logger, IOptions<StatementSettings> settings)
    {
        _logger = logger;
        _settings = settings.Value;
    }

    public async Task SendStatementsAsync(List<Customer> customers, CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var now = DateTime.UtcNow;
            var lastDayOfMonth = DateTime.DaysInMonth(now.Year, now.Month);

            foreach (var customer in customers)
            {
                if (!HasReceivedStatementForPeriod(customer, now))
                {
                    switch (customer.Preference)
                    {
                        case StatementPreference.Daily:
                            await SendStatementAsync(customer);
                            customer.LastStatementSent = now;
                            break;

                        case StatementPreference.Monthly:
                            int statementDay = Math.Min(_settings.MonthlyStatementDay, lastDayOfMonth);
                            if (now.Day == statementDay)
                            {
                                await SendStatementAsync(customer);
                                customer.LastStatementSent = now;
                            }
                            break;

                        case StatementPreference.Instant:
                            await SendStatementAsync(customer);
                            customer.LastStatementSent = now;
                            break;
                    }
                }
            }

            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken); 
        }
    }

    private Task SendStatementAsync(Customer customer)
    {
        _logger.LogInformation("Sending statement to {customer} at {time}", customer.Name, DateTimeOffset.Now);

        return Task.CompletedTask;
    }

    private bool HasReceivedStatementForPeriod(Customer customer, DateTime now)
    {
        switch (customer.Preference)
        {
            case StatementPreference.Daily:
                return customer.LastStatementSent.Date == now.Date;

            case StatementPreference.Monthly:
                return customer.LastStatementSent.Month == now.Month && customer.LastStatementSent.Year == now.Year;

            case StatementPreference.Instant:
                return false;

            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}

