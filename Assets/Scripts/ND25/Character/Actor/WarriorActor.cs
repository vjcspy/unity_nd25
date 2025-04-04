using ND25.Core.ReactiveMachine;
using UnityEngine;
namespace ND25.Character.Actor
{
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
    }
}
