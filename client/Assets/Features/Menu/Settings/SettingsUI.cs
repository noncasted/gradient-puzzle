using Cysharp.Threading.Tasks;
using Global.Audio;
using Global.Publisher;
using Global.Saves;
using Global.UI;
using Internal;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace Menu.Settings
{
    [DisallowMultipleComponent]
    public class SettingsUI : MonoBehaviour, ISettingsUI, IUIStateAsyncEnterHandler, ISceneService
    {
        [SerializeField] private DesignButton _closeButton;
        [SerializeField] private Slider _volumeSlider;

        private IBackground _background;
        private ILevelVisibility _levelVisibility;
        private IMenuNavigation _navigation;
        private IDataStorage _data;
        private IAudioVolume _audioVolume;

        public IUIConstraints Constraints { get; } = UIConstraints.Game;

        [Inject]
        private void Construct(
            IDataStorage data,
            IBackground background,
            ILevelVisibility levelVisibility,
            IAudioVolume audioVolume,
            IMenuNavigation navigation)
        {
            _audioVolume = audioVolume;
            _data = data;
            _navigation = navigation;
            _levelVisibility = levelVisibility;
            _background = background;
        }

        public void Create(IScopeBuilder builder)
        {
            builder.RegisterComponent(this)
                .As<ISettingsUI>();

            gameObject.SetActive(false);
        }

        public async UniTask OnEntered(IUIStateHandle handle)
        {
            handle.AttachGameObject(gameObject);

            var volumeSave = _data.Get<VolumeSave>();
            _volumeSlider.value = volumeSave.Values[AudioLine.Music];
            
            _volumeSlider.Advise(handle.InnerLifetime, volume =>
            {
                _audioVolume.SetVolume(AudioLine.Music, volume);
                _audioVolume.SetVolume(AudioLine.SFX, volume);
            });

            _background.ToSettings();
            _levelVisibility.Hide();
            _navigation.Hide();

            await _closeButton.WaitClick(handle);

            volumeSave.Values[AudioLine.Music] = _volumeSlider.value;
            _data.Save(volumeSave).Forget();

            _background.ToGame();
            _levelVisibility.Show();
            _navigation.Show();
        }
    }
}