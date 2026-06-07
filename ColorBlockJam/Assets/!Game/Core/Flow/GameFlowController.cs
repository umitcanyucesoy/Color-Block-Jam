using _Game.Core.Factory;
using _Game.Core.Level;
using _Game.Enums;
using _Game.Events;
using UnityEngine;

namespace _Game.Core.Flow
{
    public class GameFlowController : MonoBehaviour, IGameFlowController
    {
        private ILevelController _levelController;
        private IShapeFactory _shapeFactory;
        private int _remaining;
        private float _timeLeft;

        public GameState State { get; private set; }
        public float TimeLeft => Mathf.Max(0f, _timeLeft);

        public void Init(ILevelController levelController, IShapeFactory shapeFactory)
        {
            _levelController = levelController;
            _shapeFactory = shapeFactory;

            EventBus.Subscribe<ShapeReturnedEvent>(OnShapeReturned);
        }
       
        private void Update()
        {
            if (State != GameState.Playing)
                return;

            _timeLeft -= Time.deltaTime;
        }

        private void LateUpdate()
        {
            if (State == GameState.Playing && _timeLeft <= 0f)
                Lose();
        }
        
        private void OnDestroy()
        {
            EventBus.Unsubscribe<ShapeReturnedEvent>(OnShapeReturned);
        }

        private void OnShapeReturned(ShapeReturnedEvent e)
        {
            if (State != GameState.Playing)
                return;

            _remaining--;
            if (_remaining <= 0)
                Win();
        }
        
        private void Begin()
        {
            var level = _levelController.Current;
            _remaining = _shapeFactory.ActiveCount;
            _timeLeft = level ? level.Duration : 0f;
            State = GameState.Playing;
        }
        
        public void StartLevel()
        {
            _levelController.LoadCurrent();
            Begin();
        }

        public void NextLevel()
        {
            _levelController.NextLevel();
            Begin();
        }

        public void RetryLevel()
        {
            _levelController.RetryLevel();
            Begin();
        }

        private void Win()
        {
            State = GameState.Won;
            EventBus.Publish(new GameWonEvent());
        }

        private void Lose()
        {
            State = GameState.Lost;
            EventBus.Publish(new GameLostEvent());
        }
    }
}
