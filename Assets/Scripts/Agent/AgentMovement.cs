using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using System;

public partial class AgentMovement : MonoBehaviour
{
    public event Action<Type> OnStateChange;
    public Type CurrentState => currentState.GetType();

    [SerializeField] LayerMask groundLayer;
    [SerializeField] float walkSpeed = 2f;
    [SerializeField] float runSpeed = 5f;
    [SerializeField] float crouchSpeed = 1f;
    [SerializeField] float gravityScale = 1f;
    [SerializeField] float jumpVelocity = 5f;
    [SerializeField] float airMoveSpeed = 1f;
    [SerializeField] float collisionCheckDistance = .5f;

    AgentController controller;
    AgentEquipment equipment;
    HumanoidAnimator humanoidAnimator;
    Rig rig;

    State currentState;
    Dictionary<Type, State> availableStates;

    float verticalVelocity = 0f;
    Vector3 velocity;
    float groundCheckRadius = .4f;
    Vector3 groundCheckHeight = new Vector3(0, .3f, 0);
    Vector3 ceilingCheckHeight = new Vector3(0, 2, 0);
    float obstacleCheckHeight = .5f;
    float obstacleCheckWidth = .25f;

    private void Awake()
    {
        controller = GetComponent<AgentController>();
        equipment = GetComponent<AgentEquipment>();
        humanoidAnimator = GetComponentInChildren<HumanoidAnimator>();
        rig = GetComponentInChildren<Rig>();
        availableStates = new Dictionary<Type, State>()
        {
            {typeof(StandState), new StandState(this) },
            {typeof(WalkState), new WalkState(this) },
            {typeof(JumpState), new JumpState(this) },
            {typeof(FallState), new FallState(this) },
            {typeof(RunState), new RunState(this) },
            {typeof(CrouchState), new CrouchState(this) },
        };
        currentState = availableStates[typeof(StandState)];
    }

    private void Start()
    {
        currentState.Before();
    }

    private void Update()
    {
        currentState.During();
        Type nextState = currentState.CheckTransitions();
        if (nextState != null)
        {
            currentState.After();
            currentState = availableStates[nextState];
            OnStateChange?.Invoke(nextState);
            currentState.Before();
        }
        GroundPlayer();
    }

    public bool IsGrounded()
    {
        if (Physics.CheckSphere(transform.position + groundCheckHeight, groundCheckRadius, groundLayer, QueryTriggerInteraction.Ignore))
        {
            return true;
        }
        return false;
    }

    public bool HittingCeiling()
    {
        if (Physics.CheckSphere(transform.position + ceilingCheckHeight, groundCheckRadius, groundLayer, QueryTriggerInteraction.Ignore))
        {
            return true;
        }
        return false;
    }

    public void Move(Vector3 direction, float speed)
    {
        direction.Normalize();
        velocity = speed * direction;

        Vector3 obstacleCheckPoint = transform.position;
        obstacleCheckPoint.y += obstacleCheckHeight;
        int hits = 0;
        for (float offset = -obstacleCheckWidth; offset <= obstacleCheckWidth; offset += obstacleCheckWidth / 2)
        {
            if (Physics.Raycast(obstacleCheckPoint + transform.right * offset, direction, collisionCheckDistance, groundLayer, QueryTriggerInteraction.Ignore))
            {
                hits++;
            }
        }
        if (hits < 2)
        {   
            transform.Translate(velocity * Time.deltaTime, Space.World);
        }
    }

    void GroundPlayer()
    {
        RaycastHit rayHit;
        if (Physics.Raycast(transform.position + groundCheckHeight, -transform.up, out rayHit, groundCheckHeight.y - .05f, groundLayer))
        {
            transform.position = rayHit.point;
        }
    }

    public void SetRigWeight(float weight)
    {
        weight = Mathf.Clamp(weight, 0f, 1f);
        rig.weight = weight;
    }

    public void SwitchState(Type state)
    {
        currentState.After();
        currentState = availableStates[state];
        OnStateChange?.Invoke(state);
        currentState.Before();
    }

    private void FixedUpdate()
    {
        currentState.DuringPhysics();
        if (HittingCeiling() && verticalVelocity > 0)
        {
            verticalVelocity = 0;
        }
        transform.Translate(verticalVelocity * Time.deltaTime * Vector3.up);
        if (!IsGrounded())
        {
            verticalVelocity += Physics.gravity.y * gravityScale * Time.deltaTime;
        }
        else if (verticalVelocity < 0)
        {
            verticalVelocity = 0;
        }
    }

    public abstract class State
    {
        protected AgentMovement movement;
        protected GameObject gameObject;
        protected Transform transform;

        public State(AgentMovement movement)
        {
            this.movement = movement;
            gameObject = movement.gameObject;
            transform = movement.transform;
        }

        public virtual void Before() { }
        public virtual void During() { }
        public virtual void DuringPhysics() { }
        public virtual void After() { }
        public virtual Type CheckTransitions() { return null; }
    }
    public partial class StandState : State { }
    public partial class WalkState : State { }
    public partial class JumpState : State { }
    public partial class FallState : State { }
    public partial class RunState : State { }
    public partial class CrouchState : State { }
}