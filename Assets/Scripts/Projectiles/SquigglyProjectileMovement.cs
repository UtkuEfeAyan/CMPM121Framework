using UnityEngine;

public class SquigglyProjectileMovement : ProjectileMovement
{
    public float rnd;
    public SquigglyProjectileMovement(float speed) : base(speed)
    {
        rnd = Random.Range(0,314);
    }

    public override void Movement(Transform transform)
    {
        transform.Translate(new Vector3(speed * Time.deltaTime, Mathf.Sin(Time.time*20+rnd)/15, 0), Space.Self);
        //transform.Rotate(0, 0, speed * Mathf.Sin(Time.time)/20);
    }
}
