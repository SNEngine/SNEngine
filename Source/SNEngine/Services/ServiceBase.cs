using UnityEngine;

namespace SNEngine.Services
{
    public abstract class ServiceBase : ScriptableObject, IService, IResetable
    {
        public virtual void Initialize () { }
        public virtual void ResetState () { }
    }
}
