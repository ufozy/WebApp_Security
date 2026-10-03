using Microsoft.AspNetCore.Authorization;

namespace WebApp_UnderTheHood.Authorization
{
    public class HRManagerProbationRequirement : IAuthorizationRequirement
    {
        public HRManagerProbationRequirement(int probationPeriod)
        {
            ProbationPeriod = probationPeriod;
        }

        public int ProbationPeriod { get; }
    }

    public class HRManagerProbationRequirementHandler : AuthorizationHandler<HRManagerProbationRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, HRManagerProbationRequirement requirement)
        {
            if (!context.User.HasClaim(x => x.Type == "EmploymentDate"))
                return Task.CompletedTask;

            if(DateTime.TryParse(context.User.FindFirst(x=>x.Type== "EmploymentDate")?.Value, out DateTime employmentDate))
            {
                var period = DateTime.Now - employmentDate;
                if(period.Days > 30 * requirement.ProbationPeriod)
                {
                    context.Succeed(requirement);
                }
            }

            if (context.User.HasClaim(c => c.Type == "Department" && c.Value == "HR") &&
                context.User.HasClaim(c => c.Type == "ProbationPeriod" && int.TryParse(c.Value, out int probationPeriod) && probationPeriod >= requirement.ProbationPeriod))
            {
                context.Succeed(requirement);
            }
            return Task.CompletedTask;
        }
    }
}
