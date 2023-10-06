using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class PlayerController : MonoBehaviour
{
    private Vector2 moveValue;
    private Vector3 oldPosition;
    public float speed;
    private int count;
    private int numPickups = 16;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI winText;
    public TextMeshProUGUI positionText;
    public TextMeshProUGUI velocityText;

    void Start()
    {
        count = 0;
        winText.text = "";
        positionText.text = "";
        velocityText.text = "";
        oldPosition = new Vector3(0, 0, 0);
        SetCountText();
    }

    void OnMove(InputValue value)
    {
        moveValue = value.Get<Vector2>();
    }

    void FixedUpdate()
    {
        Vector3 movement = new Vector3(moveValue.x, 0.0f, moveValue.y);
        GetComponent<Rigidbody>().AddForce(movement * speed * Time.fixedDeltaTime);

        SetCountText();
        SetPositionVelocityText();
    }

    private void SetCountText()
    {
        scoreText.text = "Score: " + count.ToString();
        if (count >= numPickups)
        {
            winText.text = "You win! :)";
        }
    }

    private void SetPositionVelocityText()
    {
        Vector3 posDiff = transform.position - oldPosition;
        velocityText.text = (posDiff / Time.fixedDeltaTime).magnitude.ToString();
        positionText.text = transform.position.ToString() + " m/s";

        oldPosition = transform.position;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "PickUP")
        {
            other.gameObject.SetActive(false);
            count++;
        }
    }

    
}
