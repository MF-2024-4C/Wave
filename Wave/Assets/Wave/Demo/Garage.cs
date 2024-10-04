using UnityEngine;
using Wave.GameEvent;

namespace Wave.Demo
{
    [RequireComponent(typeof(MissionEventEffect))]
    public class Garage : MonoBehaviour
    {
        private MissionEventEffect _missionEventEffect;
        private AudioSource _audioSource;

        private void Start()
        {
            _missionEventEffect = GetComponent<MissionEventEffect>();
            _audioSource = GetComponent<AudioSource>();

            _missionEventEffect.OnMissionStartEvent.AddListener(OnMissionStart);
            _missionEventEffect.OnMissionCompleteEvent.AddListener(OnMissionComplete);
        }

        private void OnMissionStart()
        {
            _audioSource.Play();
        }

        private void OnMissionComplete()
        {
            _audioSource.Stop();
        }
    }
}