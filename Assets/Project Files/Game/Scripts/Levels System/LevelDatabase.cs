using UnityEngine;

namespace Watermelon
{
    [CreateAssetMenu(fileName = "Level Database", menuName = "Data/Level Database")]
    public class LevelDatabase : ScriptableObject
    {

        [SerializeField] Level[] levels;
        [SerializeField] RoverPart[] roverParts;
        [SerializeField] Prop[] prop;
        [SerializeField] string[] slotNames; // level editor only
        [SerializeField] IntArray[] allowedSlotsArray;// level editor only

        public int PropAmount => prop.Length;
        public int LevelsAmount => levels.Length;
        public int PartsAmount => roverParts.Length;

        public Level GetLevel(int levelNumber)
        {
            int levelIndex = Mathf.Clamp(levelNumber - 1, 0, int.MaxValue);
            return levels[levelIndex % levels.Length];
        }

        public RoverPart GetPart(int id)
        {
            if (id < 0 || id >= PartsAmount) return null;

            return roverParts[id];
        }

        public Prop GetProp(int id)
        {
            if (id < 0 || id >= PropAmount) return null;

            return prop[id];
        }


        [System.Serializable]
        private class IntArray
        {
            [SerializeField] private int[] slots;
        }
    }
}