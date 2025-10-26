using CodeBase.Config.Player;
using CodeBase.GameLogic.Common;
using CodeBase.GameLogic.CustomPhysics;
using CodeBase.GameLogic.Input;
using CodeBase.GameLogic.Movement;
using CodeBase.GameLogic.PickUp;
using CodeBase.Infrastructure.Services.Asset;
using CodeBase.Infrastructure.Services.Config;
using Cysharp.Threading.Tasks;
using Fusion;
using Scellecs.Morpeh;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using Input = CodeBase.GameLogic.Input.Input;

namespace CodeBase.Infrastructure.Factory
{
    public class GameFactory : IGameFactory
    {
        private readonly IConfigProvider _configProvider;
        private readonly IAssetProvider _assetProvider;
        private readonly World _world;
        private readonly NetworkRunner _networkRunner;
        private readonly IObjectResolver _objectResolver;

        public GameFactory(IConfigProvider configProvider, IAssetProvider assetProvider, World world,
            NetworkRunner networkRunner, IObjectResolver objectResolver)
        {
            _configProvider = configProvider;
            _assetProvider = assetProvider;
            _world = world;
            _networkRunner = networkRunner;
            _objectResolver = objectResolver;
        }

        public async UniTask<GameObject> CreatePlayer(PlayerRef playerRef)
        {
            PlayerConfig playerConfig = await _configProvider.GetPlayerConfig();
            GameObject playerPrefab = await _assetProvider.LoadAsync<GameObject>(playerConfig.PlayerReference);

            var networkObject = _networkRunner.Spawn(playerPrefab, Vector3.zero, Quaternion.identity, playerRef);
            GameObject playerInstance = networkObject.gameObject;
            _objectResolver.InjectGameObject(playerInstance);

            return playerInstance;
        }

        public async UniTask<Entity> CreateEntityPlayer(PlayerRef playerRef, GameObject playerInstance)
        {
            PlayerConfig playerConfig = await _configProvider.GetPlayerConfig();
            Entity playerEntity = _world.CreateEntity();

            NetworkInputReceiver networkInputReceiver = playerInstance.GetComponentInChildren<NetworkInputReceiver>();

            networkInputReceiver.Construct(playerRef);

            playerEntity.AddComponent<PlayerTag>();
            AddCameraRotationComponent(ref playerEntity, playerConfig.CameraSensitivityDefault);
            AddMoveComponent(ref playerEntity, playerConfig.MoveSpeed, playerConfig.SprintSpeed);
            AddCharacterControllerComponent(ref playerEntity, playerInstance.GetComponent<CharacterController>());
            AddTransformComponent(ref playerEntity, playerInstance.transform);
            AddJumpComponent(ref playerEntity, playerConfig.JumpForce);
            AddPhysicComponent(ref playerEntity, playerConfig.Weight);
            AddOwnerComponent(ref playerEntity, playerRef);
            AddInputComponent(ref playerEntity, new Input(), networkInputReceiver);
            AddCameraComponent(ref playerRef, playerEntity, playerInstance);
            AddInteractComponent(ref playerEntity, playerConfig.InteractDistance, playerConfig.InteractMask);
            AddGroundCheckComponent(ref playerEntity, playerConfig.CheckGroundDistance, playerConfig.GroundCheckLayer);

            _world.Commit();
            
            Debug.Log($"Created player {playerInstance}");

            return playerEntity;
        }

        private void AddInteractComponent(ref Entity playerEntity, float interactDistance, LayerMask interactMask)
        {
            ref var interactComponent = ref playerEntity.AddComponent<InteractComponent>();

            interactComponent.InteractDistance = interactDistance;
            interactComponent.InteractMask = interactMask;
        }

        private void AddCameraComponent(ref PlayerRef playerRef, Entity playerEntity, GameObject playerInstance)
        {
            ref var cameraComponent = ref playerEntity.AddComponent<CameraComponent>();

            cameraComponent.Camera = playerInstance.GetComponentInChildren<Camera>();
            cameraComponent.Owned = playerRef;
        }

        private void AddInputComponent(ref Entity playerEntity, Input input, NetworkInputReceiver receiver)
        {
            ref var inputComponent = ref playerEntity.AddComponent<InputComponent>();
            inputComponent.NetworkInputReceiver = receiver;
            inputComponent.PlayerInput = input;
        }

        private void AddOwnerComponent(ref Entity playerEntity, PlayerRef owner)
        {
            ref var ownerComponent = ref playerEntity.AddComponent<OwnerComponent>();
            ownerComponent.Owner = owner;
        }

        private void AddPhysicComponent(ref Entity playerEntity, float weight)
        {
            ref var physicsComponent = ref playerEntity.AddComponent<PhysicsComponent>();

            physicsComponent.Weight = weight;
        }

        private void AddGroundCheckComponent(ref Entity playerEntity, float checkGroundDistance, LayerMask layerMask)
        {
            ref var physicsComponent = ref playerEntity.AddComponent<GroundCheckComponent>();

            physicsComponent.CheckGroundDistance = checkGroundDistance;
            physicsComponent.LayerGround = layerMask;
        }

        private void AddJumpComponent(ref Entity playerEntity, float jumpForce)
        {
            ref var jumpComponent = ref playerEntity.AddComponent<JumpComponent>();
            jumpComponent.JumpForce = jumpForce;
        }

        private void AddTransformComponent(ref Entity playerEntity, Transform transform)
        {
            ref var transformComponent = ref playerEntity.AddComponent<TransformComponent>();
            transformComponent.Transform = transform;
        }

        private void AddCharacterControllerComponent(ref Entity playerEntity, CharacterController characterController)
        {
            ref var controllerComponent = ref playerEntity.AddComponent<CharacterControllerComponent>();
            controllerComponent.Controller = characterController;
        }

        private void AddMoveComponent(ref Entity playerEntity, float moveSpeed, float sprintSpeed)
        {
            ref var moveComponent = ref playerEntity.AddComponent<MoveComponent>();
            moveComponent.Speed = moveSpeed;
            moveComponent.SpeedBase = moveSpeed;
            moveComponent.SprintSpeed = sprintSpeed;
        }

        private void AddCameraRotationComponent(ref Entity playerEntity, float cameraSensitivity)
        {
            ref var cameraRotationComponent = ref playerEntity.AddComponent<CameraRotationComponent>();
            cameraRotationComponent.Sensitivity = cameraSensitivity;
        }
    }

    public interface IGameFactory
    {
        UniTask<GameObject> CreatePlayer(PlayerRef playerRef);
        UniTask<Entity> CreateEntityPlayer(PlayerRef playerRef, GameObject playerInstance);
    }
}