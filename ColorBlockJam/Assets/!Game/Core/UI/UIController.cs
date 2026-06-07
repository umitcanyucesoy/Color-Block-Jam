using System.Text;
using _Game.Core.Flow;
using _Game.Events;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.Core.UI
{
    public class UIController : MonoBehaviour, IUIController
    {
        [SerializeField] private GameObject winPanel;
        [SerializeField] private GameObject losePanel;
        [SerializeField] private TMP_Text timerText;
        [SerializeField] private Button nextLevelButton;
        [SerializeField] private Button retryButton;

        private readonly StringBuilder _sb = new();
        private IGameFlowController _gameFlow;
        private int _shownSeconds = -1;

        public void Init(IGameFlowController gameFlow)
        {
            _gameFlow = gameFlow;
            HidePanels();

            nextLevelButton.onClick.AddListener(OnNextLevel);
            retryButton.onClick.AddListener(OnRetry);

            EventBus.Subscribe<GameWonEvent>(OnGameWon);
            EventBus.Subscribe<GameLostEvent>(OnGameLost);
        }

        private void OnDestroy()
        {
            nextLevelButton.onClick.RemoveListener(OnNextLevel);
            retryButton.onClick.RemoveListener(OnRetry);

            EventBus.Unsubscribe<GameWonEvent>(OnGameWon);
            EventBus.Unsubscribe<GameLostEvent>(OnGameLost);
        }

        private void Update()
        {
            if (_gameFlow == null || !timerText)
                return;
            
            var seconds = Mathf.CeilToInt(_gameFlow.TimeLeft);
            if (seconds == _shownSeconds)
                return;

            _shownSeconds = seconds;
            WriteTime(seconds);
        }

        private void WriteTime(int totalSeconds)
        {
            var minutes = totalSeconds / 60;
            var secs = totalSeconds % 60;

            _sb.Clear();
            _sb.Append("Timer : ");
            if (minutes < 10) _sb.Append('0');
            _sb.Append(minutes);
            _sb.Append(':');
            if (secs < 10) _sb.Append('0');
            _sb.Append(secs);

            timerText.SetText(_sb);
        }

        private void OnGameWon(GameWonEvent e) => winPanel.SetActive(true);
        private void OnGameLost(GameLostEvent e) => losePanel.SetActive(true);

        public void OnNextLevel()
        {
            _gameFlow.NextLevel();
            HidePanels();
        }

        public void OnRetry()
        {
            _gameFlow.RetryLevel();
            HidePanels();
        }

        private void HidePanels()
        {
            winPanel.SetActive(false);
            losePanel.SetActive(false);
            _shownSeconds = -1;
        }
    }
}
