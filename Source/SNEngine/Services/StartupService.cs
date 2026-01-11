using UnityEngine;

namespace SNEngine.Services
{
    [CreateAssetMenu(menuName = "SNEngine/Services/Startup Service")]
    public class StartupService : ServiceBase
    {
        public override void Initialize()
        {
            Application.targetFrameRate = 60;
            Application.runInBackground = true;
        }
    }
}
