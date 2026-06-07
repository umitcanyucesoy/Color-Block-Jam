using _Game.Core.Shapes;
using UnityEngine;

namespace _Game.Events
{
    public struct ShapeGrabbedEvent : IEvent
    {
        public Shape Shape;
        public Vector3 WorldPoint;
    }

    public struct ShapeDraggedEvent : IEvent
    {
        public Vector3 WorldPoint;
    }

    public struct ShapeReleasedEvent : IEvent { }

    public struct ShapeReturnedEvent : IEvent
    {
        public Shape Shape;
    }

    public struct GameWonEvent : IEvent { }

    public struct GameLostEvent : IEvent { }
}
