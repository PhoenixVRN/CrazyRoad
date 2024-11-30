using System.Collections.Generic;
using UnityEngine;

namespace Watermelon
{

    public class Level : ScriptableObject
    {
        [SerializeField] RoverPart bodyPart;

        [SerializeField] RoverPartData[] parts;

        public PropSave[] itemSaves;

        public Vector3 finishPosition;

        public int[] activeSlots;
        public CardboardSave[] cardboards;

        public RoverPart BodyPart => bodyPart;
        public RoverPartData[] Parts => parts;
        public CardboardSave[] Cardboards => cardboards;

        public int PartsAmount => parts.Length;

    }

    [System.Serializable]
    public class RoverPartData
    {
        [SerializeField] RoverPart part;
        [SerializeField] List<int> alowedSlots;

        [Space]
        [SerializeField] bool isPlacedAlready;
        [SerializeField] int slot;

        public RoverPart Part => part;
        public List<int> AlowedSlots => alowedSlots;

        public bool IsPlacedAlready => isPlacedAlready;
        public int Slot => slot;

    }

    [System.Serializable]
    public class CardboardSave
    {
        public GroundPoint[] points;
        public Vector3 position;

        public bool startTape = false;
        public bool endTape = false;


        public void SetPoints(GroundPoint[] points, bool startTape, bool endTape)
        {
            this.points = points;
            this.startTape = startTape;
            this.endTape = endTape;
        }


    }
}
