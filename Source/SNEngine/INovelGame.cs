using SNEngine.Repositories;
using SNEngine.Services;

namespace SNEngine
{
    public interface INovelGame
    {
        T GetRepository<T>() where T : RepositoryBase;
        T GetService<T>() where T : ServiceBase;
        void ResetStateServices();
    }
}