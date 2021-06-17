using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Projectiles : NetworkBehaviour
{
    public int speed;
    public GameObject target;

    // Update is called once per frame
    void Update()
    {
        Vector3 direction = (target.transform.position - transform.position).normalized;
        transform.Translate(direction * speed * Time.deltaTime);
        transform.rotation = Quaternion.FromToRotation(Vector3.left, direction);

        if (Vector3.Distance(target.transform.position, transform.position) < 1)
            Destroy(gameObject);
    }
}
