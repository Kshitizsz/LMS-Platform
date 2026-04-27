using Hangfire.Annotations;
using Hangfire.Dashboard;

namespace LMS.API.Middleware;

public class HangfireDevAuthFilter : IDashboardAuthorizationFilter
{
    public bool Authorize([NotNull] DashboardContext context) => true;
}