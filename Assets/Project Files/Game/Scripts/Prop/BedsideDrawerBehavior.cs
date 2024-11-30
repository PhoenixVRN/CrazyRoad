using UnityEngine;

namespace Watermelon
{
    public class BedsideDrawerBehavior : MonoBehaviour,IInitialized
    {
        [SerializeField] Transform upperShelf;
        [SerializeField] Transform lowerShelf;

        float min = 0.7f;
        float max = 2.88f;

        public void Init()
        {
            var upperPos = upperShelf.transform.localPosition;
            upperPos.z = Random.Range(min, max);
            upperShelf.transform.localPosition = upperPos;

            var lowerPos = lowerShelf.transform.localPosition;
            lowerPos.z = Random.Range(min, max);
            lowerShelf.transform.localPosition = lowerPos;
        }
    }
}