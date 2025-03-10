using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpPad : MonoBehaviour
{
    [SerializeField] private float jumpForce = 200f;
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player")) 
        {
            Rigidbody playerRb = collision.gameObject.GetComponent<Rigidbody>();

            if (playerRb != null)
            {
                PlayerCondition condition = CharacterManager.Instance.Player.condition;
                playerRb.velocity = new Vector3(playerRb.velocity.x, 0f, playerRb.velocity.z);
                playerRb.AddForce(Vector3.up * (jumpForce + condition.JumpBoost / 2), ForceMode.Impulse);
            }
        }
    }
}