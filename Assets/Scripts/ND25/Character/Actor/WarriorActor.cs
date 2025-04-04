using ND25.Core.ReactiveMachine;
using R3;
using UnityEngine;
namespace ND25.Character.Actor
{
    public enum WarriorAction
    {
        HandleMoveInput,
        Walk,
        Jump,
        Attack,
        Die,
    }


    /*
     * This version is using ReactiveMachine
     */
    public class WarriorActor : ReactiveMachineMono
    {

        [Header("Movement Settings")]
        [SerializeField]
        float moveSpeed = 5f;
        [SerializeField] float jumpForce = 2f;

        Animator animator;
        GroundChecker groundChecker;

        float horizontalInput;
        Rigidbody2D rb;
        protected override void Awake()
        {
            base.Awake();

            animator = GetComponentInChildren<Animator>();
            rb = GetComponent<Rigidbody2D>();
            groundChecker = GetComponent<GroundChecker>();
        }
        protected override string GetJsonFileName()
        {
            return "warrior_config";
        }
        protected override object[] GetActionHandlers()
        {
            return new object[]
            {
                this,
            };
        }

        [ReactiveMachineEffect]
        public ReactiveMachineActionHandler HandleMoveInput()
        {
            return upstream => upstream
                .OfAction(WarriorAction.HandleMoveInput)
                .Select(
                    _ =>
                    {
                        horizontalInput = Input.GetAxis("Horizontal");
                        Vector2 newVelocity = new Vector2(horizontalInput * moveSpeed, rb.linearVelocity.y);
                        rb.linearVelocity = newVelocity;

                        return ReactiveMachineCoreAction.Empty;
                    }
                );
        }
    }
}
