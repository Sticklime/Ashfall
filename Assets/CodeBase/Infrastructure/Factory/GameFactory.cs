using CodeBase.Config.Player;
using CodeBase.GameLogic.Common;
using CodeBase.GameLogic.CustomPhysics;
using CodeBase.GameLogic.Input;
using CodeBase.GameLogic.Movement;
using CodeBase.GameLogic.PickUp;
using CodeBase.Infrastructure.Services.Asset;
using CodeBase.Infrastructure.Services.Config;
using Cysharp.Threading.Tasks;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
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
        private readonly IObjectResolver _resolver;

        public GameFactory(IConfigProvider configProvider, IAssetProvider assetProvider, IObjectResolver resolver)
        {
            _configProvider = configProvider;
            _assetProvider = assetProvider;
            _resolver = resolver;
        }

        public async UniTask<GameObject> CreatePlayer(int ownerNetworkId = -1)
        {
            var world = World.DefaultGameObjectInjectionWorld;
            if (world == null || !world.IsCreated)
            {
                Debug.LogError("[DOTS] Unable to create player — default world is not set or not created.");
                return null;
            }

            var entityManager = world.EntityManager;
            PlayerConfig playerConfig = await _configProvider.GetPlayerConfig();
            GameObject prefab = await _assetProvider.LoadAsync<GameObject>(playerConfig.PlayerReference);
            GameObject instance = Object.Instantiate(prefab, Vector3.zero, Quaternion.identity);
            _resolver.InjectGameObject(instance);

            Entity entity = entityManager.CreateEntity();

            entityManager.AddComponent<PlayerTag>(entity);

            if (ownerNetworkId >= 0)
            {
                entityManager.AddComponentData(entity, new OwnerComponent
                {
                    NetworkId = ownerNetworkId
                });

                if (!entityManager.HasComponent<GhostOwner>(entity))
                {
                    entityManager.AddComponentData(entity, new GhostOwner
                    {
                        NetworkId = ownerNetworkId
                    });
                }
            }

            var transform = instance.transform;
            var controller = instance.GetComponent<CharacterController>();
            var camera = instance.GetComponentInChildren<Camera>();

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
                PlayerInput = new PlayerCommand
                {
                    Move = Vector2.zero,
                    Look = Vector3.zero,
                    JumpTriggered = false,
                    SprintProgress = false,
                    Tick = default
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

            var commandEntity = entityManager.CreateEntity();
            var commandBuffer = entityManager.AddBuffer<PlayerCommand>(commandEntity);
            
            commandBuffer.Add(new PlayerCommand
            {
                Move = Vector2.zero,
                Look = Vector3.zero,
                JumpTriggered = false,
                SprintProgress = false,
                Tick = default
            });

            if (!entityManager.HasComponent<CommandTarget>(entity)) 
                entityManager.AddComponent<CommandTarget>(entity);
            
            entityManager.SetComponentData(entity, new CommandTarget { targetEntity = commandEntity });

            Debug.Log($"[DOTS] Created player entity: {entity.Index}");
            return instance;
        }
    }

    public interface IGameFactory
    {
        UniTask<GameObject> CreatePlayer(int ownerNetworkId = -1);
    }
}
