using System.Collections.Generic;
using UnityEngine;

namespace Watermelon
{
    public class LevelController : MonoBehaviour
    {
        private static LevelController instance;

        [Header("Settings")]
        [SerializeField] int levelCompleteReward;

        [Header("References")]
        [SerializeField] SlotsController slotsController;
        [SerializeField] RoverBehavior rover;
        [SerializeField] Transform finishTransform;

        List<GroundRenderer> grounds = new List<GroundRenderer>();

        public static RoverBehavior Rover => instance.rover;

        public static int LevelReward => instance.levelCompleteReward;
        public static Level Level { get; private set; }
        public static SlotsController SlotsController { get => instance.slotsController; }
        public static PoolsHandler PoolsHandler { get; private set; }
        public static Transform Finish => instance.finishTransform;

        private UIGame gameUI;

        public void Init()
        {
            instance = this;

            PoolsHandler = new PoolsHandler();
            PoolsHandler.Init();
        }

        private void OnDestroy()
        {
            PoolsHandler.Unload();
        }

        public static void LoadLevel(Level level)
        {
            Level = level;

            var bodyPart = PoolsHandler.GetRoverPartBehavior(Level.BodyPart);

            Rover.SetBody(bodyPart as BodyPartBehavior, Level.BodyPart, Level.activeSlots);

            instance.slotsController.InitSlots(level.Parts);
            instance.InitProp();
            instance.gameUI = UIController.GetPage<UIGame>();
        }

        public static void LaunchVehicle()
        {
            instance.slotsController.HideSlots();
            Rover.Drive();

            CameraController.Follow(Rover.transform);

            instance.gameUI.InitPartButtons(instance.rover.parts);
        }

        public void InitProp()
        {
            grounds.Clear();

            foreach (var save in Level.Cardboards)
            {
                var ground = PoolsHandler.GetCardboard();

                ground.transform.position = save.position;

                ground.RecalculateGround(save.points);

                grounds.Add(ground);

                bool isBattery = false;

                for (int i = 0; i < save.points.Length - 1; i++)
                {
                    var currentPoint = save.points[i];
                    var nextPoint = save.points[i + 1];

                    if (save.points[i].battery)
                    {
                        isBattery = !isBattery;
                    }

                    if (isBattery)
                    {
                        var step = 2f;
                        var length = Bezier.AproximateLength(
                            currentPoint.position,
                            currentPoint.position + currentPoint.rightKey,
                            nextPoint.position + nextPoint.leftKey,
                            nextPoint.position);

                        var batteriesCount = Mathf.FloorToInt(length / step);

                        for (int j = 0; j < batteriesCount; j++)
                        {
                            var t = (float)j / batteriesCount;

                            var pos = Bezier.EvaluateCubic(
                            currentPoint.position,
                            currentPoint.position + currentPoint.rightKey,
                            nextPoint.position + nextPoint.leftKey,
                            nextPoint.position,
                            t);

                            var normal = ground.CalculateNormal(i, t);

                            var coin = PoolsHandler.GetCoin();

                            coin.Init(save.position + pos, t);
                            coin.transform.up = normal;
                        }
                    }
                }

                if (save.startTape)
                {
                    var tape = PoolsHandler.GetTape();

                    tape.position = save.position + save.points[0].position;
                }

                if (save.endTape)
                {
                    var tape = PoolsHandler.GetTape();

                    tape.position = save.position + save.points[save.points.Length - 1].position;
                }
            }

            foreach (var prop in Level.itemSaves)
            {
                var item = PoolsHandler.GetProp(prop.Prop);

                item.transform.position = prop.Position;
                item.transform.eulerAngles = prop.Rotation;

                var initable = item.GetComponent<IInitialized>();
                if (initable != null)
                {
                    initable.Init();
                }
            }

            finishTransform.position = Level.finishPosition;
        }
    }
}