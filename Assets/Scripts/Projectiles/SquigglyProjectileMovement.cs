using UnityEngine;

public class SquigglyProjectileMovement : ProjectileMovement
{
    //public float start;
    public SquigglyProjectileMovement(float speed) : base(speed)
    {
        //start = Time.time;
    }

    public override void Movement(Transform transform)
    {
        transform.Translate(new Vector3(speed * Time.deltaTime, Mathf.Sin(Time.time*20)/15, 0), Space.Self);
        //transform.Rotate(0, 0, speed * Mathf.Sin(Time.time)/20);
    }
}
