using System.Threading;
using Cysharp.Threading.Tasks;

namespace DDoveFramework.Extension.DDoveBoot
{
    public interface IExtensionInitializer
    {
        UniTask<bool> InitializeAsync(CancellationToken cancellationToken);
    }
}
