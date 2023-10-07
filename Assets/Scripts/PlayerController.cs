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
    private int numPickups;
    private Vector3 velocity;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI winText;
    public TextMeshProUGUI positionText;
    public TextMeshProUGUI velocityText;
    public TextMeshProUGUI pickUpDistanceText;
    private GameObject[] PickUps;
    private LineRenderer lineRenderer;
    private bool distanceMode = false;
    private bool visionMode = false;

    void Start()
    {
        count = 0;
        winText.text = "";
        positionText.text = "";
        velocityText.text = "";
        pickUpDistanceText.text = "";
        oldPosition = new Vector3(0, 0, 0);
        PickUps = GameObject.FindGameObjectsWithTag("PickUP");
        numPickups = PickUps.Length;
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        RenderLine(new Vector3(0,0,0), new Vector3(0,0,0));
        SetCountText();
    }

    void OnMove(InputValue value)
    {
        moveValue = value.Get<Vector2>();
    }

    private void OnDebugMode(InputValue value)
    {
        foreach(GameObject pickUp in PickUps)
        {
            pickUp.GetComponent<Renderer>().material.color = Color.white;
        }
        if(!distanceMode && !visionMode)
        {
            distanceMode = true;
        } else if(distanceMode)
        {
            distanceMode = false;
            visionMode = true;
        } else
        {
            distanceMode = false;
            visionMode = false;
            RenderLine(new Vector3(0,0,0), new Vector3(0, 0, 0));
        }
    }

    void FixedUpdate()
    {
        Vector3 movement = new Vector3(moveValue.x, 0.0f, moveValue.y);
        GetComponent<Rigidbody>().AddForce(movement * speed * Time.fixedDeltaTime);

        SetCountText();
        if (distanceMode)
        {
            SetPositionVelocityText();
            SetPickUpDistanceText();
        }
        if (visionMode)
        {
            VisionDebug();
        }
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
        CalculateVelocity();
        velocityText.text = velocity.magnitude.ToString() + " m/s";
        positionText.text = transform.position.ToString();
    }

    private void CalculateVelocity()
    {
        velocity = ((transform.position - oldPosition) / Time.fixedDeltaTime);
        oldPosition = transform.position;
    }

    private void SetPickUpDistanceText()
    {
        float closestDistance = 1000f;
        GameObject closestPickUp = null;
        foreach(GameObject pickUp in PickUps)
        {
            pickUp.GetComponent<Renderer>().material.color = Color.white;
            if (pickUp.activeSelf)
            {
                float currentDistance = Vector3.Distance(transform.position, pickUp.transform.position);
                if (currentDistance < closestDistance)
                {
                    closestDistance = currentDistance;
                    closestPickUp = pickUp;
                }
            }
        }
        if (closestPickUp != null)
        {
            RenderLine(transform.position, closestPickUp.transform.position);
            closestPickUp.GetComponent<Renderer>().material.color = Color.blue;
            pickUpDistanceText.text = closestDistance.ToString() + " m";
        }
        else
        {
            lineRenderer.enabled = false;
        }
    }

    private void RenderLine(Vector3 start, Vector3 end)
    {
        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, end);
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
    }

    private void VisionDebug()
    {

        positionText.text = "";
        velocityText.text = "";
        pickUpDistanceText.text = "";
        CalculateVelocity();
        RenderLine(transform.position, transform.position + velocity);

        foreach(GameObject pickUp in PickUps)
        {
            pickUp.GetComponent<Renderer>().material.color = Color.white;
        }

        RaycastHit hit;
        if(Physics.Raycast(transform.position, velocity*1000, out hit))
        {
            if(hit.transform.tag == "PickUP")
            {
                hit.transform.GetComponent<Renderer>().material.color = Color.green;
                hit.transform.LookAt(transform.position);
            }
        }
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
