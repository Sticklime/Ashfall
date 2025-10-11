using CodeBase.Config.Player;
using CodeBase.Infrastructure.Services.Asset;
using Cysharp.Threading.Tasks;

namespace CodeBase.Infrastructure.Services.Config
{
    public class ConfigProvider : IConfigProvider
    {
        private readonly IAssetProvider _assetProvider;

        public ConfigProvider(IAssetProvider assetProvider)
        {
            _assetProvider = assetProvider;
        }

        public async UniTask<PlayerConfig> GetPlayerConfig()
        {
            return await _assetProvider.LoadAsync<PlayerConfig>("PlayerConfig");
        }
    }

    public interface IConfigProvider
    {
        UniTask<PlayerConfig> GetPlayerConfig();
    }
}