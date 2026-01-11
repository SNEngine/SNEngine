using Cysharp.Threading.Tasks;

namespace SNEngine.SnapshotSystem
{
    public interface ISnapshotProvider
    {
        UniTask AppendAsync(byte[] data);
        UniTask<byte[]> PopLastAsync();
        UniTask ClearAsync();
    }
}