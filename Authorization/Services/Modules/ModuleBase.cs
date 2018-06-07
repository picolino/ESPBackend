using Authorization.Providers;
using Shared.Logging;

namespace Authorization.Services.Modules
{
    public abstract class ModuleBase
    {
        protected readonly ILogger Logger;
        protected readonly AuthRepository Repository;

        protected ModuleBase(ILogger logger, AuthRepository repository)
        {
            Logger = logger;
            Repository = repository;
        }
    }
}