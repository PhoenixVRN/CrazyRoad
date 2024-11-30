using System.Collections.Generic;
using UnityEngine;

namespace Watermelon
{
    public class PoolsHandler
    {
        private Dictionary<RoverPart, PoolGeneric<RoverPartBehavior>> roverPartPools = new Dictionary<RoverPart, PoolGeneric<RoverPartBehavior>>();
        private Dictionary<RoverPart, Pool> partVisualsPools = new Dictionary<RoverPart, Pool>();
        private Dictionary<Item, Pool> propPools = new Dictionary<Item, Pool>();

        public IPool torchPool;
        public IPool cardboardPool;
        public IPool coinPool;
        public IPool tapePool;

        public void Init()
        {
            roverPartPools.Clear();

            for (int i = 0; i < GameController.LevelDatabase.PartsAmount; i++)
            {
                RoverPart part = GameController.LevelDatabase.GetPart(i);

                PoolGeneric<RoverPartBehavior> partPool = new PoolGeneric<RoverPartBehavior>(part.PartObject, $"Rover Part {i}");
                Pool visualsPool = new Pool(part.BuildVisuals, $"Rover Part {i} visuals");

                roverPartPools.Add(part, partPool);
                partVisualsPools.Add(part, visualsPool);
            }

            propPools.Clear();

            for (int i = 0; i < GameController.LevelDatabase.PropAmount; i++)
            {
                Prop prop = GameController.LevelDatabase.GetProp(i);

                propPools.Add(prop.Type, new Pool(prop.Prefab, "Prop_" + prop.Type.ToString()));
            }

            torchPool = PoolManager.GetPoolByName("Torch");
            cardboardPool = PoolManager.GetPoolByName("Cardboard");
            coinPool = PoolManager.GetPoolByName("Coin");
            tapePool = PoolManager.GetPoolByName("Tape");
        }

        public void Unload()
        {
            foreach (var pool in roverPartPools.Values)
            {
                PoolManager.DestroyPool(pool);
            }
            roverPartPools.Clear();

            foreach (var pool in partVisualsPools.Values)
            {
                PoolManager.DestroyPool(pool);
            }
            partVisualsPools.Clear();

            foreach (var pool in propPools.Values)
            {
                PoolManager.DestroyPool(pool);
            }
            propPools.Clear();
        }

        public RoverPartBehavior GetRoverPartBehavior(RoverPart part)
        {
            var pool = roverPartPools[part];

            var roverPartBehavior = pool.GetPooledComponent();

            return roverPartBehavior;
        }

        public GameObject GetRoverpartVisuals(RoverPart part)
        {
            var pool = partVisualsPools[part];

            var visuals = pool.GetPooledObject();

            return visuals;
        }


        public void ReturnEverythingToPool()
        {
            foreach (var pool in roverPartPools.Values)
            {
                pool.ReturnToPoolEverything();
            }

            foreach (var pool in partVisualsPools.Values)
            {
                pool.ReturnToPoolEverything();
            }

            foreach (var pool in propPools.Values)
            {
                pool.ReturnToPoolEverything();
            }

            torchPool.ReturnToPoolEverything();
            cardboardPool.ReturnToPoolEverything();
            coinPool.ReturnToPoolEverything();
            tapePool.ReturnToPoolEverything();
        }

        public GroundRenderer GetCardboard()
        {
            return cardboardPool.GetPooledObject().GetComponent<GroundRenderer>();
        }

        public CoinBehavior GetCoin()
        {
            return coinPool.GetPooledObject().GetComponent<CoinBehavior>();
        }

        public Transform GetProp(Item item)
        {
            return propPools[item].GetPooledObject().transform;
        }

        public Transform GetTape()
        {
            return tapePool.GetPooledObject().transform;
        }
    }
}