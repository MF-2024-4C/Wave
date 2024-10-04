using Quantum;
using UnityEngine;
using UnityEngine.Events;

namespace Wave.GameEvent
{
    public class MissionEventEffect : MonoBehaviour
    {
        [SerializeField] private int _missionStartEventId;
        [SerializeField] private int _missionEndEventId;

        public UnityEvent OnMissionStartEvent;
        public UnityEvent OnMissionCompleteEvent;

        private void Start()
        {
            QuantumEvent.Subscribe<EventMissionStart>(this, OnMissionStart);
            QuantumEvent.Subscribe<EventMissionComplete>(this, OnMissionEnd);
        }

        private void OnMissionStart(EventMissionStart e)
        {
            var settings = UnityDB.FindAsset<DefenseMissionTriggerConfigAsset>(e.MissionConfig.Id).Settings;
            if (settings.StartEventID != _missionStartEventId) return;
            OnMissionStartEvent?.Invoke();
        }

        private void OnMissionEnd(EventMissionComplete e)
        {
            var settings = UnityDB.FindAsset<DefenseMissionTriggerConfigAsset>(e.MissionConfig.Id).Settings;
            if (settings.EndEventID != _missionEndEventId) return;
            OnMissionCompleteEvent?.Invoke();
        }
    }
}