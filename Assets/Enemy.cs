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

    public Vector2 waitTime = new Vector2(0.5f,1.5f);

    bool prevChase = false;
    public LayerMask obstacleLayer;
    public float checkDistance =  1.2f;
    public List<float> anglesToCheck = new List<float>();
    public Animator anim;
    public SpriteRenderer srenderer;
    public ParticleSystem sleepSystem;

    StateMachine<Enemy> machine;
    public bool randomizeOnStart = false;
    public bool canBeGolem = true;
    private void Awake() {
        
        machine = new StateMachine<Enemy>(new PatrolState(), this);
    }

    private void Start() {
        if (randomizeOnStart) RandomizeColors();
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

    private void OnCollisionEnter2D(Collision2D collision) {
        if (prevChase) return;
        if ( Layers.InLayerMask(obstacleLayer,collision.gameObject.layer)) {
            targetdirection = GetBestDirection();
        }
    }
    public Vector3 targetdirection;

    

    public Vector3 GetBestDirection() {

        Vector3 averageDir = new Vector3();
        int amt = 0;

        foreach (float f in anglesToCheck) {
            Vector3 check = Quaternion.Euler(0, 0, f) * targetdirection;
            if (Physics2D.Raycast(transform.position, check, checkDistance, obstacleLayer)) {
                averageDir -= check;
                amt++;
            }
        }

        

        return (averageDir/amt).normalized;

    }


    public void RemoveCheck() {
        if (prevChase && TopDownPlayerController.currentlyChasingPlayer.Contains(this)) {
            TopDownPlayerController.RemoveIfPresent(this);
        }
    }

    public class PatrolState : State<Enemy> {

        public float nextTimeChange;
        

        
        public override void Enter(StateMachine<Enemy> obj) {
            obj.target.targetdirection = GetRandomDirection();
            obj.target.anim.Play(IDLE);
            nextTimeChange = Time.time + Random.Range(obj.target.waitTime.x, obj.target.waitTime.y);
            obj.target.prevChase = false;
            obj.target.sleepSystem.Stop();
            
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
                    if (obj.target.prevChase == false && !TopDownPlayerController.currentlyChasingPlayer.Contains(obj.target)) {
                        TopDownPlayerController.AddIfNotPresent(obj.target);
                    }
                obj.target.targetdirection = obj.target.looker.transform.right;
                    obj.target.prevChase = true;
                   
                } else if (obj.target.isPlayerInRange() && (obj.target.isPlayerOnEdgeOfVision())) {
                    // Debug.Log("Indirect " + Time.time);
                    obj.target.looker.transform.right = Vector3.MoveTowards(obj.target.looker.transform.right, (TopDownPlayerController.instance.transform.position - obj.target.transform.position).normalized, 2 * Time.deltaTime);
                    obj.target.rb.velocity = (TopDownPlayerController.instance.transform.position - obj.target.transform.position).normalized * obj.target.indirectMoveSpeed;
                    obj.target.anim.Play(WALKING);
                    obj.target.RemoveCheck();
                obj.target.targetdirection = obj.target.looker.transform.right;
                    obj.target.prevChase = false;
                } else {
                    obj.target.RemoveCheck();
                    if (Time.time > nextTimeChange) {
                        if (obj.target.targetdirection == Vector3.zero) {
                        obj.target.targetdirection = GetRandomDirection();
                            nextTimeChange = Time.time + Random.Range(obj.target.moveTimeRange.x, obj.target.moveTimeRange.y);
                            obj.target.anim.Play(WALKING);
                        } else {
                        obj.target.targetdirection = Vector2.zero;
                            nextTimeChange = Time.time + Random.Range(obj.target.waitTimeRange.x, obj.target.waitTimeRange.y);
                            obj.target.anim.Play(IDLE);
                        }
                    }
                    if (obj.target.targetdirection != Vector3.zero) {
                        obj.target.looker.transform.right = Vector3.MoveTowards(obj.target.looker.transform.right, obj.target.rb.velocity.normalized, 10 * Time.deltaTime);
                    }
                    obj.target.rb.velocity = obj.target.targetdirection * obj.target.indirectMoveSpeed;
                    obj.target.prevChase = false;
            }
            
        }
    }

    public void RandomizeColors() {

        if (Random.Range(0, 1f) > 0.66f && canBeGolem) {
            anim.runtimeAnimatorController = GameMasterManager.instance.worldGenerator.golemAnimator;
            srenderer.material = GameMasterManager.instance.worldGenerator.golemMaterial;
        } else {

            srenderer.material.SetColor("_NewSpotColor", GameMasterManager.instance.worldGenerator.GetNewColor());
            srenderer.material.SetColor("_NewSkinColor", GameMasterManager.instance.worldGenerator.GetNewColor());
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
            obj.target.prevChase = true;
            obj.target.RemoveCheck();
            obj.target.prevChase = false;
            obj.target.sleepSystem.Play();

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
