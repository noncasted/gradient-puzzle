using System;
using Cysharp.Threading.Tasks;
using Global.Publisher.Itch;
using Global.Publisher.Yandex;
using Global.Saves;
using Internal;

namespace Global.Publisher
{
    public static class GlobalPublisherExtensions
    {
        public static async UniTask AddPublisher(this IScopeBuilder builder)
        {
            var platformOptions = builder.GetOptions<PlatformOptions>();

            builder.Register<DataStorageEventLoop>()
                .AsSelf()
                .AsSelfResolvable();

            switch (platformOptions.PlatformType)
            {
                case PlatformType.ItchIO:
                    AddItchIO(builder);
                    break;
                case PlatformType.Yandex:
                    await AddYandex(builder);
                    break;
                case PlatformType.IOS:
                    break;
                case PlatformType.Android:
                    break;
                case PlatformType.CrazyGames:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static void AddItchIO(IScopeBuilder builder)
        {
            var platformOptions = builder.GetOptions<PlatformOptions>();
            var options = builder.GetAsset<GlobalPublisherOptions>();

            var callbacks = builder.Instantiate(options.ItchCallbacksPrefab);

            builder.Register<ItchAds>()
                .As<IAds>();

            builder.RegisterInstance(callbacks)
                .As<IJsErrorCallback>();

            builder.Register<ItchDataStorage>()
                .WithParameter(SavesExtensions.GetSerializers())
                .As<IDataStorage>()
                .AsEventListener<IScopeBaseSetup>();

            builder.Register<ItchLanguageProvider>()
                .As<ISystemLanguageProvider>();

            if (platformOptions.IsEditor == true)
            {
                builder.Register<ItchLanguageDebugAPI>()
                    .As<IItchLanguageAPI>();
            }
            else
            {
                builder.Register<ItchLanguageExternAPI>()
                    .As<IItchLanguageAPI>();
            }
        }

        private static UniTask AddYandex(IScopeBuilder builder)
        {
            var options = builder.GetAsset<GlobalPublisherOptions>();
            var platformOptions = builder.GetOptions<PlatformOptions>();
            var yandexCallbacks = builder.Instantiate(options.YandexCallbacksPrefab);
            yandexCallbacks.name = "YandexCallbacks";

            builder.RegisterComponent(yandexCallbacks);

            RegisterModules();

            if (platformOptions.IsEditor == true)
                return RegisterEditorApis(yandexCallbacks);

            builder.Register<YandexInitialization>()
                .AsEventListener<IScopeBaseSetup>();

            RegisterBuildApis();

            return UniTask.CompletedTask;

            void RegisterModules()
            {
                builder.Register<YandexAds>()
                    .WithParameter<IProductLink>(options.YandexAdsDisableProduct)
                    .As<IAds>()
                    .AsEventListener<IDataStorageLoadListener>();

                builder.Register<YandexDataStorage>()
                    .As<IDataStorage>()
                    .WithParameter(SavesExtensions.GetSerializers())
                    .AsEventListener<IScopeBaseSetup>();

                builder.Register<SystemLanguageProvider>()
                    .As<ISystemLanguageProvider>();

                builder.Register<LeaderboardsProvider>()
                    .As<ILeaderboardsProvider>();

                builder.Register<Reviews>()
                    .As<IReviews>();

                builder.Register<Payments>()
                    .WithParameter(options.YandexProductsRegistry)
                    .As<IPayments>();
            }

            async UniTask RegisterEditorApis(YandexCallbacks callbacks)
            {
                var canvas = await builder.FindOrLoadScene<YandexDebugScene, YandexDebugCanvas>();
                canvas.Ads.Construct(callbacks);
                canvas.Reviews.Construct(callbacks);
                canvas.Purchase.Construct(callbacks);

                builder.Register<AdsDebugAPI>()
                    .As<IAdsAPI>()
                    .WithParameter<IAdsDebug>(canvas.Ads);

                builder.Register<StorageDebugAPI>()
                    .As<IStorageAPI>();

                builder.Register<LanguageDebugAPI>()
                    .As<ILanguageAPI>();

                builder.Register<LeaderboardsDebugAPI>()
                    .As<ILeaderboardsAPI>();

                builder.Register<PurchasesDebugAPI>()
                    .As<IPurchasesAPI>()
                    .WithParameter(options.YandexProductsRegistry)
                    .WithParameter<IPurchaseDebug>(canvas.Purchase);

                builder.Register<ReviewsDebugAPI>()
                    .As<IReviewsAPI>()
                    .WithParameter<IReviewsDebug>(canvas.Reviews);
            }

            void RegisterBuildApis()
            {
                builder.Register<AdsExternAPI>()
                    .As<IAdsAPI>();

                builder.Register<StorageExternAPI>()
                    .As<IStorageAPI>();

                builder.Register<LanguageExternAPI>()
                    .As<ILanguageAPI>();

                builder.Register<LeaderboardsExternAPI>()
                    .As<ILeaderboardsAPI>();

                builder.Register<PurchasesExternAPI>()
                    .As<IPurchasesAPI>();

                builder.Register<ReviewsExternAPI>()
                    .As<IReviewsAPI>();
            }
        }
    }
}