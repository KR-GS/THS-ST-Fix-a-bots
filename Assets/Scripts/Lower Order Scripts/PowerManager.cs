using System;
using UnityEngine;
using UnityEngine.Rendering;

public class PowerManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private float pointer_speed;
    [SerializeField] private int movement_direction;
    [SerializeField] private Transform[] end_points;

    private int current_value;

    private bool stop_clicked = false;

    void Update()
    {
        if (!stop_clicked)
        {
            if (Vector2.Distance(transform.position, new Vector2(end_points[movement_direction].position.x, transform.position.y)) > 0.5f)
            {
                transform.position = Vector2.MoveTowards(transform.position, new Vector2(end_points[movement_direction].position.x, transform.position.y), pointer_speed * Time.deltaTime);
            }
            else
            {
                if(movement_direction == 1)
                {
                    movement_direction = 0;
                }
                else
                {
                    movement_direction = 1;
                }
            }
        }
    }

    /*
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.transform.name == "End 1")
        {
            Debug.Log("hello from end 1");
            movement_direction = 1;
        }
        else if (collision.transform.name == "End 2")
        {
            Debug.Log("hello from end 2");
            movement_direction = 0;
        }
        

        if (collision.transform.name == "green")
        {
            current_value = 20;

            Debug.Log(current_value);
        }
        else if (collision.transform.name == "yellow")
        {
            current_value = 10;

            Debug.Log(current_value);
        }
        else if (collision.transform.name == "red")
        {
            current_value = 5;

            Debug.Log(current_value);
        }
    }
    */

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.name == "green")
        {
            current_value = 20;

            Debug.Log(current_value);
        }
        else if (collision.transform.name == "yellow")
        {
            current_value = 10;

            Debug.Log(current_value);
        }
        else if(collision.transform.name == "red")
        {
            current_value = 0;

            Debug.Log(current_value);
        }
    }


    public void ToggleStop()
    {
        stop_clicked = !stop_clicked;
        Debug.Log("Number chosen: " + current_value);
    }

    public int GetPowerValue()
    {
        return current_value;
    }
}
