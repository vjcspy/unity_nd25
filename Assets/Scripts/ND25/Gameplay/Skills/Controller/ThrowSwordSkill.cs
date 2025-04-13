using ND25.Gameplay.Character.Player;
using UnityEngine;
namespace ND25.Gameplay.Skills.Controller
{
    public class ThrowSwordSkill : MonoBehaviour
    {
        private Animator animator;
        private CircleCollider2D circleCollider;
        private PlayerActor playerActor;
        private Rigidbody2D rg;

        private void Awake()
        {
            animator = GetComponent<Animator>();
            circleCollider = GetComponent<CircleCollider2D>();
            playerActor = GetComponentInParent<PlayerActor>();
            rg = GetComponent<Rigidbody2D>();
        }
    }
}
