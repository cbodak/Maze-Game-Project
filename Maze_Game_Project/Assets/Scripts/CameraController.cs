using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    public GameObject player;
    private Vector3 offsetDistance;
    private float lookspeed;
    private float followspeed;
    private void Awake()
    {
        Vector3 initialOffset = new Vector3(0.0f, 10.0f, -2.5f);
        transform.position = player.transform.position + initialOffset;
        offsetDistance = transform.position - player.transform.position;
        lookspeed = 7.5f;
        followspeed = 5;
    }

    private void LateUpdate()
    {
        Vector3 look = player.transform.position - transform.position;
        Quaternion rotation = Quaternion.LookRotation(look, Vector3.up);
        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, lookspeed * Time.deltaTime);

        Vector3 position = player.transform.position + player.transform.forward * offsetDistance.z + player.transform.right * offsetDistance.x + player.transform.up * offsetDistance.y;
        transform.position = Vector3.Lerp(transform.position, position, followspeed * Time.deltaTime);
    }
}
