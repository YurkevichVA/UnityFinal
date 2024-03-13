﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private string horizontalInputName;        // аліаси на кнопки A-D
    [SerializeField] private string verticalInputName;          // аліаси на кнопки W-S
    [SerializeField] private float movementSpeed;

    private CharacterController charController; // компонент, котрий спрощує програмування та керування персонажем
    private Animator animator;

    [SerializeField] private AnimationCurve jumpFallOff; // графік прямої, за якою в мене стрибає персонаж
    [SerializeField] private float jumpMultiplier;
    [SerializeField] private KeyCode jumpKey;

    private Vector3 playerPosition;

    private bool isJumping;

    private Enums.State state;

    private void Awake() // Це майже те ж саме, що і Старт, але запускається при створенні об'єкту
    {
        charController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        state = Enums.State.Idle;
    }

    private void Update()
    {
        if (state == Enums.State.Idle)
        {
            PlayerMovements();
        }

        if (this.transform.position.y < 0f)
        {
            this.transform.position = new Vector3(0, 1, 0);
        }
    }

    private void PlayerMovements()
    {
        float horizInput = Input.GetAxis(horizontalInputName) * movementSpeed;
        float vertInput = Input.GetAxis(verticalInputName) * movementSpeed;

        if(vertInput != 0f || horizInput != 0f)
        {
            animator.SetInteger("State", 1);
        }
        else
        {
            animator.SetInteger("State", 0);
        }

        Vector3 forwardMovement = transform.forward * vertInput;
        Vector3 rightMovement = transform.right * horizInput;

        charController.SimpleMove(forwardMovement + rightMovement);

        JumpInput();

    }

    private void JumpInput()
    {
        if (Input.GetKeyDown(jumpKey) && !isJumping)
        {
            isJumping = true;
            StartCoroutine(JumpEvent());
        }
    }

    private IEnumerator JumpEvent() // Delay()
    {
        charController.slopeLimit = 90.0f;
        float timeInAir = 0.4f;

        do
        {
            float jumpForce = jumpFallOff.Evaluate(timeInAir);
            charController.Move(Vector3.up * jumpForce * jumpMultiplier * Time.deltaTime);
            timeInAir += Time.deltaTime;
            yield return null;
        } while (this.transform.position.y > 0 && !charController.isGrounded && charController.collisionFlags != CollisionFlags.Above);

        charController.slopeLimit = 45.0f;
        isJumping = false;
    }

    public void ChangeState(Enums.State newState, Vector3 position)
    {
        if (newState == Enums.State.Fight && state != Enums.State.Fight)
        {
            state = newState;
            animator.SetInteger("State", 0);
            MoveToPoint(position);
        }
    }

    private void MoveToPoint(Vector3 point)
    {
        charController.SimpleMove(point);
    }
}
