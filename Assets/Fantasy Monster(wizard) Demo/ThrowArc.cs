using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowArc : MonoBehaviour
{
    public Transform Targ;
    public float firingAngle = 45.0f;
    public float gravity = 9.8f;

    //public Transform Projectile;
    private Transform myTransform;

    void Awake()
    {
        myTransform = transform;
    }

    void Start()
    {
        if (Targ != null) StartCoroutine(SimulateProjectile(Targ));
        //SimulateProjectile();
    }


    IEnumerator SimulateProjectile(Transform Target)
    {
        // Short delay added before Projectile is thrown
        //yield return new WaitForSeconds(1.5f);

        // Move projectile to the position of throwing object + add some offset if needed.
        //Projectile.position = myTransform.position + new Vector3(0, 0.0f, 0);

        // Calculate distance to target
        float target_Distance = Vector3.Distance(gameObject.transform.position, Target.position);

        // Calculate the velocity needed to throw the object to the target at specified angle.
        float projectile_Velocity = target_Distance / (Mathf.Sin(2 * firingAngle * Mathf.Deg2Rad) / gravity);

        // Extract the X  Y componenent of the velocity
        float Vx = Mathf.Sqrt(projectile_Velocity) * Mathf.Cos(firingAngle * Mathf.Deg2Rad);
        float Vy = Mathf.Sqrt(projectile_Velocity) * Mathf.Sin(firingAngle * Mathf.Deg2Rad);

        // Calculate flight time.
        float flightDuration = target_Distance / Vx;

        // Rotate projectile to face the target.
        gameObject.transform.rotation = Quaternion.LookRotation(Target.position - gameObject.transform.position);

        float elapse_time = 0;

        while (elapse_time < flightDuration)
        {
            gameObject.transform.Translate(0, (Vy - (gravity * elapse_time)) * Time.deltaTime, Vx * Time.deltaTime);

            elapse_time += Time.deltaTime;

            yield return null;
        }
    }
}
