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
        private EntityManager _entityManager;
        private readonly IObjectResolver _resolver;

        public GameFactory(IConfigProvider configProvider, IAssetProvider assetProvider, World world, IObjectResolver resolver)
        {
            _configProvider = configProvider;
            _assetProvider = assetProvider;
            _entityManager = world.EntityManager;
            _resolver = resolver;
        }

        public async UniTask<GameObject> CreatePlayer()
        {
            PlayerConfig playerConfig = await _configProvider.GetPlayerConfig();
            GameObject prefab = await _assetProvider.LoadAsync<GameObject>(playerConfig.PlayerReference);
            GameObject instance = Object.Instantiate(prefab, Vector3.zero, Quaternion.identity);
            _resolver.InjectGameObject(instance);

            Entity entity = _entityManager.CreateEntity();

            _entityManager.AddComponent<PlayerTag>(entity);

            var transform = instance.transform;
            var controller = instance.GetComponent<CharacterController>();
            var camera = instance.GetComponentInChildren<Camera>();

            float3 position = transform.position;
            quaternion rotation = transform.rotation;
            _entityManager.AddComponentData(entity, LocalTransform.FromPositionRotationScale(position, rotation, 1f));

            _entityManager.AddComponentData(entity, new PhysicsComponent
            {
                Weight = playerConfig.Weight,
                Velocity = float3.zero
            });

            _entityManager.AddComponentData(entity, new MoveComponent
            {
                Speed = playerConfig.MoveSpeed,
                SpeedBase = playerConfig.MoveSpeed,
                SprintSpeed = playerConfig.SprintSpeed
            });

            _entityManager.AddComponentData(entity, new JumpComponent
            {
                JumpForce = playerConfig.JumpForce
            });

            _entityManager.AddComponentData(entity, new GroundCheckComponent
            {
                CheckGroundDistance = playerConfig.CheckGroundDistance,
                GroundMask = playerConfig.GroundCheckLayer
            });

            _entityManager.AddComponentData(entity, new CameraRotationComponent
            {
                Sensitivity = playerConfig.CameraSensitivityDefault,
                VerticalAngle = 0f,
                HorizontalAngle = transform.eulerAngles.y
            });

            _entityManager.AddComponentData(entity, new InteractComponent
            {
                InteractDistance = playerConfig.InteractDistance,
                InteractMask = playerConfig.InteractMask
            });

            _entityManager.AddComponentData(entity, new InputComponent
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

            _entityManager.AddComponentData(entity, new CharacterControllerComponent
            {
                Controller = controller
            });

            _entityManager.AddComponentData(entity, new CameraComponent
            {
                Camera = camera
            });

            var commandEntity = _entityManager.CreateEntity();
            var commandBuffer = _entityManager.AddBuffer<PlayerCommand>(commandEntity);
            
            commandBuffer.Add(new PlayerCommand
            {
                Move = Vector2.zero,
                Look = Vector3.zero,
                JumpTriggered = false,
                SprintProgress = false,
                Tick = default
            });

            if (!_entityManager.HasComponent<CommandTarget>(entity)) 
                _entityManager.AddComponent<CommandTarget>(entity);
            
            _entityManager.SetComponentData(entity, new CommandTarget { targetEntity = commandEntity });

            Debug.Log($"[DOTS] Created player entity: {entity.Index}");
            return instance;
        }
    }

    public interface IGameFactory
    {
        UniTask<GameObject> CreatePlayer();
    }
}
