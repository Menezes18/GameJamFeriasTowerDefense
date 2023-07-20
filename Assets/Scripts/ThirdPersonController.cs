using UnityEngine;
using UnityEngine.InputSystem;


namespace ControlerInput
{
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(PlayerInput))]
    public class ThirdPersonController : MonoBehaviour
    {
        [Header("Player")]
        [Tooltip("Velocidade de movimento do personagem em m/s")]
        public float MoveSpeed = 2.0f;

        [Tooltip("Velocidade de corrida do personagem em m/s")]
        public float SprintSpeed = 5.335f;

        [Tooltip("Velocidade de rotação do personagem para virar na direção do movimento")]
        [Range(0.0f, 0.3f)]
        public float RotationSmoothTime = 0.12f;

        [Tooltip("Aceleração e desaceleração")]
        public float SpeedChangeRate = 10.0f;

        public AudioClip LandingAudioClip;
        public AudioClip[] FootstepAudioClips;
        [Range(0, 1)] public float FootstepAudioVolume = 0.5f;

        [Space(10)]
        [Tooltip("A altura que o jogador pode pular")]
        public float JumpHeight = 1.2f;

        [Tooltip("A gravidade do personagem. O padrão do motor é -9.81f")]
        public float Gravity = -15.0f;

        [Space(10)]
        [Tooltip("Tempo necessário para poder pular novamente. Defina como 0f para pular instantaneamente novamente")]
        public float JumpTimeout = 0.50f;

        [Tooltip("Tempo necessário para entrar no estado de queda. Útil para descer escadas")]
        public float FallTimeout = 0.15f;

        [Header("Player Grounded")]
        [Tooltip("Se o personagem está no chão ou não. Não faz parte da verificação de chão embutida no CharacterController")]
        public bool Grounded = true;

        [Tooltip("Útil para superfícies irregulares")]
        public float GroundedOffset = -0.14f;

        [Tooltip("O raio da verificação de chão. Deve corresponder ao raio do CharacterController")]
        public float GroundedRadius = 0.28f;

        [Tooltip("As camadas que o personagem usa como chão")]
        public LayerMask GroundLayers;

        [Header("Cinemachine")]
        [Tooltip("O alvo de acompanhamento definido na Cinemachine Virtual Camera que a câmera seguirá")]
        public GameObject CinemachineCameraTarget;

        [Tooltip("Até quantos graus você pode mover a câmera para cima")]
        public float TopClamp = 70.0f;

        [Tooltip("Até quantos graus você pode mover a câmera para baixo")]
        public float BottomClamp = -30.0f;

        [Tooltip("Graus adicionais para substituir a posição da câmera. Útil para ajustar finamente a posição da câmera quando travada")]
        public float CameraAngleOverride = 0.0f;

        [Tooltip("Para travar a posição da câmera em todos os eixos")]
        public bool LockCameraPosition = false;

        // cinemachine
        private float _cinemachineTargetYaw;
        private float _cinemachineTargetPitch;

        // player
        private float _speed;
        private float _animationBlend;
        private float _targetRotation = 0.0f;
        private float _rotationVelocity;
        private float _verticalVelocity;
        private float _terminalVelocity = 53.0f;

        // timeout deltatime
        private float _jumpTimeoutDelta;
        private float _fallTimeoutDelta;

        // animation IDs
        private int _animIDSpeed;
        private int _animIDGrounded;
        private int _animIDJump;
        private int _animIDFreeFall;
        private int _animIDMotionSpeed;


        private PlayerInput _playerInput;
        private Animator _animator;
        private CharacterController _controller;
        private ControlerInput _input;
        private GameObject _mainCamera;

        private const float _threshold = 0.01f;

        private bool _hasAnimator;

        private bool IsCurrentDeviceMouse
        {
            get
            {
                return _playerInput.currentControlScheme == "KeyboardMouse";
				return false;
            }
        }


        private void Awake()
        {
            // obter uma referência para nossa câmera principal
            if (_mainCamera == null)
            {
                _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
            }
        }

        private void Start()
        {
            _cinemachineTargetYaw = CinemachineCameraTarget.transform.rotation.eulerAngles.y;
            
            _hasAnimator = TryGetComponent(out _animator);
            _controller = GetComponent<CharacterController>();
            _input = GetComponent<ControlerInput>();
            _playerInput = GetComponent<PlayerInput>();

            AssignAnimationIDs();

            // redefinir os timeouts no início
            _jumpTimeoutDelta = JumpTimeout;
            _fallTimeoutDelta = FallTimeout;
        }

        private void Update()
        {
            _hasAnimator = TryGetComponent(out _animator);

            JumpAndGravity();
            GroundedCheck();
            Move();
        }

        private void LateUpdate()
        {
            CameraRotation();
        }

        private void AssignAnimationIDs()
        {
            _animIDSpeed = Animator.StringToHash("Speed");
            _animIDGrounded = Animator.StringToHash("Grounded");
            _animIDJump = Animator.StringToHash("Jump");
            _animIDFreeFall = Animator.StringToHash("FreeFall");
            _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
        }

        private void GroundedCheck()
        {
            // definir a posição da esfera, com o offset
            Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset,
                transform.position.z);
            Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers,
                QueryTriggerInteraction.Ignore);

            // atualizar o animator se estiver usando um character
            if (_hasAnimator)
            {
                _animator.SetBool(_animIDGrounded, Grounded);
            }
        }

        private void CameraRotation()
        {
            // se houver uma entrada e a posição da câmera não estiver fixa
            if (_input.look.sqrMagnitude >= _threshold && !LockCameraPosition)
            {
                // Não multiplicar a entrada do mouse por Time.deltaTime;
                float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;

                _cinemachineTargetYaw += _input.look.x * deltaTimeMultiplier;
                _cinemachineTargetPitch += _input.look.y * deltaTimeMultiplier;
            }

            // limitar nossas rotações para que os valores sejam limitados a 360 graus
            _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
            _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

            // Cinemachine seguirá esse alvo
            CinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch + CameraAngleOverride,
                _cinemachineTargetYaw, 0.0f);
        }

        private void Move()
        {
            // definir a velocidade de destino com base na velocidade de movimento, velocidade de corrida e se o sprint está pressionado
            float targetSpeed = _input.sprint ? SprintSpeed : MoveSpeed;

            // uma aceleração e desaceleração simplista projetada para ser fácil de remover, substituir ou iterar

            // observe: o operador == do Vector2 usa aproximação, portanto, não é propenso a erros de ponto flutuante e é mais barato que a magnitude
            // se não houver entrada, defina a velocidade de destino como 0
            if (_input.move == Vector2.zero) targetSpeed = 0.0f;

            // uma referência para a velocidade horizontal atual do jogador
            float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;

            float speedOffset = 0.1f;
            float inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;

            // acelerar ou desacelerar para a velocidade de destino
            if (currentHorizontalSpeed < targetSpeed - speedOffset ||
                currentHorizontalSpeed > targetSpeed + speedOffset)
            {
                // cria um resultado curvo em vez de um linear, resultando em uma mudança de velocidade mais orgânica
                // note que o valor T no Lerp é limitado, portanto, não precisamos limitar nossa velocidade
                _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude,
                    Time.deltaTime * SpeedChangeRate);

                // arredonda a velocidade para 3 casas decimais
                _speed = Mathf.Round(_speed * 1000f) / 1000f;
            }
            else
            {
                _speed = targetSpeed;
            }

            _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * SpeedChangeRate);
            if (_animationBlend < 0.01f) _animationBlend = 0f;

            // normalizar a direção da entrada
            Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;

            // observe: o operador != do Vector2 usa aproximação, portanto, não é propenso a erros de ponto flutuante e é mais barato que a magnitude
            // se houver uma entrada de movimento, girar o jogador quando ele estiver se movendo
            if (_input.move != Vector2.zero)
            {
                _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg +
                                  _mainCamera.transform.eulerAngles.y;
                float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity,
                    RotationSmoothTime);

                // girar para enfrentar a direção da entrada em relação à posição da câmera
                transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
            }


            Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;

            // mover o jogador
            _controller.Move(targetDirection.normalized * (_speed * Time.deltaTime) +
                             new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);

            // atualizar o animator se estiver usando um character
            if (_hasAnimator)
            {
                _animator.SetFloat(_animIDSpeed, _animationBlend);
                _animator.SetFloat(_animIDMotionSpeed, inputMagnitude);
            }
        }

        private void JumpAndGravity()
        {
            if (Grounded)
            {
                // redefinir o temporizador de tempo limite de queda
                _fallTimeoutDelta = FallTimeout;

                // atualizar o animator se estiver usando um character
                if (_hasAnimator)
                {
                    _animator.SetBool(_animIDJump, false);
                    _animator.SetBool(_animIDFreeFall, false);
                }

                // parar nossa velocidade de cair infinitamente quando estiver no chão
                if (_verticalVelocity < 0.0f)
                {
                    _verticalVelocity = -2f;
                }

                // Pular
                if (_input.jump && _jumpTimeoutDelta <= 0.0f)
                {
                    // a raiz quadrada de H * -2 * G = quanta velocidade é necessária para alcançar a altura desejada
                    _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);

                    // atualizar o animator se estiver usando um character
                    if (_hasAnimator)
                    {
                        _animator.SetBool(_animIDJump, true);
                    }
                }

                // tempo limite para pular novamente
                if (_jumpTimeoutDelta >= 0.0f)
                {
                    _jumpTimeoutDelta -= Time.deltaTime;
                }
            }
            else
            {
                // redefinir o temporizador de tempo limite de pulo
                _jumpTimeoutDelta = JumpTimeout;

                // tempo limite de queda
                if (_fallTimeoutDelta >= 0.0f)
                {
                    _fallTimeoutDelta -= Time.deltaTime;
                }
                else
                {
                    // atualizar o animator se estiver usando um character
                    if (_hasAnimator)
                    {
                        _animator.SetBool(_animIDFreeFall, true);
                    }
                }

                // se não estivermos no chão, não pular
                _input.jump = false;
            }

            // aplicar a gravidade ao longo do tempo se estiver abaixo da velocidade terminal (multiplicar por delta time duas vezes para acelerar linearmente ao longo do tempo)
            if (_verticalVelocity < _terminalVelocity)
            {
                _verticalVelocity += Gravity * Time.deltaTime;
            }
        }

        private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
        {
            if (lfAngle < -360f) lfAngle += 360f;
            if (lfAngle > 360f) lfAngle -= 360f;
            return Mathf.Clamp(lfAngle, lfMin, lfMax);
        }

        private void OnDrawGizmosSelected()
        {
            Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
            Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

            if (Grounded) Gizmos.color = transparentGreen;
            else Gizmos.color = transparentRed;

            // quando selecionado, desenhar um gizmo na posição do colisor do chão e com o mesmo raio
            Gizmos.DrawSphere(
                new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z),
                GroundedRadius);
        }

        private void OnFootstep(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                if (FootstepAudioClips.Length > 0)
                {
                    var index = Random.Range(0, FootstepAudioClips.Length);
                    AudioSource.PlayClipAtPoint(FootstepAudioClips[index], transform.TransformPoint(_controller.center), FootstepAudioVolume);
                }
            }
        }

        private void OnLand(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                AudioSource.PlayClipAtPoint(LandingAudioClip, transform.TransformPoint(_controller.center), FootstepAudioVolume);
            }
        }
    }
}
