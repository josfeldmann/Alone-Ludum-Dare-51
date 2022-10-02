using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{

    public const string RUNNING = "RUN", WALKING = "WALK", IDLE = "IDLE", SLEEPING = "SLEEPING";

    public float sideRange = 67.5f;
    public float fastView = 45f;
    public float viewRange = 10f;

    public Vector2 moveTimeRange = new Vector2(4, 6);
    public Vector2 waitTimeRange = new Vector2(1, 2);

    public Rigidbody2D rb;
    public float indirectMoveSpeed = 3;
    public float directMoveSpeed = 5;
    public Transform looker;
    public ParticleSystem sleepSystem;

    public float waitTime = 1;


    public Animator anim;

    StateMachine<Enemy> machine;

    private void Awake() {
        machine = new StateMachine<Enemy>(new PatrolState(), this);
    }

    private void Update() {
        machine.Update();
    }

    public class RotateState : State<Enemy> {
        public override void Enter(StateMachine<Enemy> obj) {
            
        }

        public override void Update(StateMachine<Enemy> obj) {
            if (obj.target.isPlayerInRange() && obj.target.isPlayerInDirectSight()) {

            } else {

                obj.target.transform.Rotate(0, 0, 90 * Time.deltaTime);
            }
        }
    }

    public class PatrolState : State<Enemy> {

        public float nextTimeChange;
        public Vector3 targetdirection;

        
        public override void Enter(StateMachine<Enemy> obj) {
            targetdirection = GetRandomDirection();
            obj.target.anim.Play(IDLE);
            nextTimeChange = Time.time + obj.target.waitTime;
            
        }

        public Vector3 GetRandomDirection() {
           return new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0).normalized;
        }

        public override void Update(StateMachine<Enemy> obj) {

            if (!GameMasterManager.instance.dayController.isDay) {
                obj.ChangeState(new SleepState());
                return;
            }


            
                if (obj.target.isPlayerInRange() && obj.target.isPlayerInDirectSight()) {
                    //  Debug.Log("Direct");
                    obj.target.looker.transform.right = Vector3.MoveTowards(obj.target.looker.transform.right, (TopDownPlayerController.instance.transform.position - obj.target.transform.position).normalized, 4 * Time.deltaTime);
                    obj.target.anim.Play(RUNNING);
                    obj.target.rb.velocity = (TopDownPlayerController.instance.transform.position - obj.target.transform.position).normalized * obj.target.directMoveSpeed;
                } else if (obj.target.isPlayerInRange() && (obj.target.isPlayerOnEdgeOfVision())) {
                    // Debug.Log("Indirect " + Time.time);
                    obj.target.looker.transform.right = Vector3.MoveTowards(obj.target.looker.transform.right, (TopDownPlayerController.instance.transform.position - obj.target.transform.position).normalized, 2 * Time.deltaTime);
                    obj.target.rb.velocity = (TopDownPlayerController.instance.transform.position - obj.target.transform.position).normalized * obj.target.indirectMoveSpeed;
                    obj.target.anim.Play(WALKING);
                } else {

                    if (Time.time > nextTimeChange) {
                        if (targetdirection == Vector3.zero) {
                            targetdirection = GetRandomDirection();
                            nextTimeChange = Time.time + Random.Range(obj.target.moveTimeRange.x, obj.target.moveTimeRange.y);
                            obj.target.anim.Play(WALKING);
                        } else {
                            targetdirection = Vector2.zero;
                            nextTimeChange = Time.time + Random.Range(obj.target.waitTimeRange.x, obj.target.waitTimeRange.y);
                            obj.target.anim.Play(IDLE);
                        }
                    }
                    if (targetdirection != Vector3.zero) {
                        obj.target.looker.transform.right = Vector3.MoveTowards(obj.target.looker.transform.right, obj.target.rb.velocity.normalized, 10 * Time.deltaTime);
                    }
                    obj.target.rb.velocity = targetdirection * obj.target.indirectMoveSpeed;
                }
            
        }
    }

    public bool isPlayerInRange() {
        if (Vector2.Distance(TopDownPlayerController.instance.transform.position, transform.position) <= viewRange) return true;
        return false;
    }

    public LayerMask maskToLookForPlayerOn;
    RaycastHit2D hit;
    public bool RayCastSeesPlayer() {
        hit = Physics2D.Raycast(transform.position, (TopDownPlayerController.instance.transform.position - transform.position).normalized, viewRange, maskToLookForPlayerOn);
        if (hit.collider != null) {
            if (hit.collider.gameObject.layer == Layers.player) {
                return true;
            }
        }
        return false;
    }
    public bool isPlayerInDirectSight() {
        return angleCalc(fastView) && RayCastSeesPlayer();
    }

    public bool isPlayerOnEdgeOfVision() {
        return angleCalc(sideRange) && RayCastSeesPlayer();
    }
    public bool angleCalc( float angle) { 
        float angleBetween = Angle((TopDownPlayerController.instance.transform.position - transform.position).normalized);
        float lookAngle = Angle(looker.transform.right);

        if (angleBetween >= 270 && lookAngle <= 90) {
            angleBetween -= 360;
        }

        if (lookAngle >= 270 && angleBetween <= 90) {
            lookAngle -= 360;
        }

        float angleDifference = Mathf.Abs(lookAngle - angleBetween);
      //  print("Angle Between " + angleBetween + " Look Angle: " + lookAngle + " Angle Diff " + angleDifference);

        if (Mathf.Abs(angleDifference) <= angle) return true;

        return false;
    }


    public static float Angle(Vector2 vector2) {
        if (vector2.x < 0) {
            return 360 - (Mathf.Atan2(vector2.x, vector2.y) * Mathf.Rad2Deg * -1);
        } else {
            return Mathf.Atan2(vector2.x, vector2.y) * Mathf.Rad2Deg;
        }
    }


    public Vector3 GetZLookVector(float z) {
        
        float radians = z * (Mathf.PI / 180);
        return new Vector3(Mathf.Cos(radians), Mathf.Sin(radians), 0);
    }

    public class SleepState : State<Enemy> {

        public override void Enter(StateMachine<Enemy> obj) {
            obj.target.anim.Play(SLEEPING);
            obj.target.rb.velocity = Vector2.zero;
            obj.target.rb.constraints = RigidbodyConstraints2D.FreezeAll;
        }

        public override void Exit(StateMachine<Enemy> obj) {

            obj.target.rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        }

        public override void Update(StateMachine<Enemy> obj) {
            if (GameMasterManager.instance.dayController.isDay) {
                obj.ChangeState(new PatrolState());
            }
        }


    }

    private void OnDrawGizmos() {
        Debug.DrawLine(transform.position, transform.position + GetZLookVector(sideRange + looker.transform.localEulerAngles.z) * viewRange, Color.green);
        Debug.DrawLine(transform.position, transform.position + GetZLookVector(-sideRange + looker.transform.localEulerAngles.z) * viewRange, Color.green);
        Debug.DrawLine(transform.position, transform.position + GetZLookVector(fastView + looker.transform.localEulerAngles.z) * viewRange, Color.yellow);
        Debug.DrawLine(transform.position, transform.position + GetZLookVector(-fastView + looker.transform.localEulerAngles.z) * viewRange, Color.yellow);
        Debug.DrawLine(transform.position, transform.position + GetZLookVector(looker.transform.localEulerAngles.z) * viewRange, Color.red);
    }

}
