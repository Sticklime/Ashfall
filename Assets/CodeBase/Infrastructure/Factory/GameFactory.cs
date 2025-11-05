using System;
using CodeBase.Config.Player;
using CodeBase.GameLogic.Common;
using CodeBase.GameLogic.CustomPhysics;
using CodeBase.GameLogic.Input;
using CodeBase.GameLogic.Movement;
using CodeBase.GameLogic.PickUp;
using CodeBase.Infrastructure.ECS;
using CodeBase.Infrastructure.Services.Asset;
using CodeBase.Infrastructure.Services.Config;
using Cysharp.Threading.Tasks;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace CodeBase.Infrastructure.Factory
{
    public class GameFactory : IGameFactory
    {
        private readonly IConfigProvider _configProvider;
        private readonly IAssetProvider _assetProvider;
        private readonly World _fallbackWorld;
        private readonly SystemEngine _systemEngine;
        private readonly IObjectResolver _resolver;

        public GameFactory(IConfigProvider configProvider, IAssetProvider assetProvider, World world, SystemEngine systemEngine,
            IObjectResolver resolver)
        {
            _configProvider = configProvider;
            _assetProvider = assetProvider;
            _fallbackWorld = world;
            _systemEngine = systemEngine;
            _resolver = resolver;
        }

        private EntityManager EntityManager
        {
            get
            {
                var world = _systemEngine.ActiveWorld ?? _fallbackWorld;

                if (world == null || !world.IsCreated)
                    throw new InvalidOperationException("Active DOTS world is not available.");

                return world.EntityManager;
            }
        }

        public async UniTask<GameObject> CreatePlayer()
        {
            PlayerConfig playerConfig = await _configProvider.GetPlayerConfig();
            GameObject prefab = await _assetProvider.LoadAsync<GameObject>(playerConfig.PlayerReference);
            GameObject instance = Object.Instantiate(prefab, Vector3.zero, Quaternion.identity);
            _resolver.InjectGameObject(instance);
            return instance;
        }

        public async UniTask<Entity> CreateEntityPlayer(GameObject instance)
        {
            PlayerConfig playerConfig = await _configProvider.GetPlayerConfig();
            EntityManager entityManager = EntityManager;
            Entity entity = entityManager.CreateEntity();

            entityManager.AddComponent<PlayerTag>(entity);

            var transform = instance.transform;
            var controller = instance.GetComponent<CharacterController>();
            var camera = instance.GetComponentInChildren<Camera>();
            var networkInputReceiver = instance.GetComponent<NetworkInputReceiver>();

            float3 position = transform.position;
            quaternion rotation = transform.rotation;
            entityManager.AddComponentData(entity, LocalTransform.FromPositionRotationScale(position, rotation, 1f));

            entityManager.AddComponentData(entity, new PhysicsComponent
            {
                Weight = playerConfig.Weight,
                Velocity = float3.zero
            });

            entityManager.AddComponentData(entity, new MoveComponent
            {
                Speed = playerConfig.MoveSpeed,
                SpeedBase = playerConfig.MoveSpeed,
                SprintSpeed = playerConfig.SprintSpeed
            });

            entityManager.AddComponentData(entity, new JumpComponent
            {
                JumpForce = playerConfig.JumpForce
            });

            entityManager.AddComponentData(entity, new GroundCheckComponent
            {
                CheckGroundDistance = playerConfig.CheckGroundDistance,
                GroundMask = playerConfig.GroundCheckLayer
            });

            entityManager.AddComponentData(entity, new CameraRotationComponent
            {
                Sensitivity = playerConfig.CameraSensitivityDefault,
                VerticalAngle = 0f,
                HorizontalAngle = transform.eulerAngles.y
            });

            entityManager.AddComponentData(entity, new InteractComponent
            {
                InteractDistance = playerConfig.InteractDistance,
                InteractMask = playerConfig.InteractMask
            });

            entityManager.AddComponentData(entity, new InputComponent
            {
                PlayerInput = new Input
                {
                    Move = Vector2.zero,
                    Look = Vector3.zero,
                    JumpTriggered = false,
                    SprintProgress = false
                }
            });

            entityManager.AddComponentData(entity, new CharacterControllerComponent
            {
                Controller = controller
            });

            entityManager.AddComponentData(entity, new CameraComponent
            {
                Camera = camera
            });

            entityManager.AddComponentData(entity, new NetworkInputReceiverComponent
            {
                PlayerInput = new Input
                {
                    Move = Vector2.zero,
                    Look = Vector3.zero,
                    JumpTriggered = false,
                    SprintProgress = false
                }
            });

            if (networkInputReceiver != null)
            {
                networkInputReceiver.BindEntity(entity, entityManager.World);
            }

            Debug.Log($"[DOTS] Created player entity: {entity.Index}");
            return entity;
        }
    }

    public interface IGameFactory
    {
        UniTask<GameObject> CreatePlayer();
        UniTask<Entity> CreateEntityPlayer(GameObject instance);
    }
}
