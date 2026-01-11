using UnityEngine;

namespace SNEngine.Repositories
{
    public class RepositoryBase : ScriptableObject, IRepository
    {
        public virtual void Initialize() { }
    }
}
