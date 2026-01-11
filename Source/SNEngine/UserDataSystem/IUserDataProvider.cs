using Cysharp.Threading.Tasks;
using SNEngine.UserDataSystem.Models;

namespace SNEngine.UserDataSystem
{
    public interface IUserDataProvider
    {
        UniTask<UserData> LoadAsync();
        UniTask SaveAsync(UserData data);
    }
}
