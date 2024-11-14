using System;
using Mirror;
using UnityEngine;

namespace CodeBase.Hero
{
    public class PlayerAnimator : NetworkBehaviour, IAnimationStateReader
    {
        [SerializeField] private CharacterController _characterController;
        [SerializeField] public Animator _animator;

        private static readonly int RunHash = Animator.StringToHash("Run");
        private static readonly int JumpHash = Animator.StringToHash("Jump");
        private static readonly int IdleHash = Animator.StringToHash("Idle");

        private readonly int _idleStateHash = Animator.StringToHash("Idle");
        private readonly int _runStateHash = Animator.StringToHash("Run");
        private readonly int _jumpStateHash = Animator.StringToHash("Jump");

        public event Action<AnimatorState> StateEntered;
        public event Action<AnimatorState> StateExited;

        public AnimatorState State { get; private set; }

        private void Update()
        {
            if (isLocalPlayer)
            {
                // Передаем значение RunHash на сервер через Command
                CmdUpdateRunAnimation(_characterController.velocity.magnitude);
            }
        }

        [Command]
        private void CmdUpdateRunAnimation(float velocity)
        {
            // Обновляем анимацию на сервере
            RpcUpdateRunAnimation(velocity);
        }

        [ClientRpc]
        private void RpcUpdateRunAnimation(float velocity)
        {
            _animator.SetFloat(RunHash, velocity, 0.1f, Time.deltaTime);
        }

        public void PlayJump()
        {
            if (isLocalPlayer)
            {
                // Инициируем прыжок только на локальном клиенте
                CmdPlayJump();
            }
        }

        [Command]
        private void CmdPlayJump()
        {
            RpcPlayJump();
        }

        [ClientRpc]
        private void RpcPlayJump()
        {
            _animator.SetTrigger(JumpHash);
        }

        public void ResetToIdle()
        {
            if (isLocalPlayer)
            {
                CmdResetToIdle();
            }
        }

        [Command]
        private void CmdResetToIdle()
        {
            RpcResetToIdle();
        }

        [ClientRpc]
        private void RpcResetToIdle()
        {
            _animator.Play(_idleStateHash);
        }

        public void EnteredState(int stateHash)
        {
            State = StateFor(stateHash);
            StateEntered?.Invoke(State);
        }

        public void ExitedState(int stateHash)
        {
            StateExited?.Invoke(StateFor(stateHash));
        }

        private AnimatorState StateFor(int stateHash)
        {
            AnimatorState state;
            if (stateHash == _idleStateHash)
            {
                state = AnimatorState.Idle;
            }
            else if (stateHash == _runStateHash)
            {
                state = AnimatorState.Run;
            }
            else if (stateHash == _jumpStateHash)
            {
                state = AnimatorState.Jump;
            }
            else
            {
                state = AnimatorState.Unknown;
            }

            return state;
        }
    }
}

public enum AnimatorState
{
    Idle,
    Run,
    Jump,
    Unknown
}
