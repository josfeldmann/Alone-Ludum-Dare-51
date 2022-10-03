using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TopDownPlayerController : MonoBehaviour
{


    public AudioSource eatClip, deathClip;

    public static List<Enemy> currentlyChasingPlayer = new List<Enemy>();

    public static void RemoveIfPresent(Enemy e) {
        if (currentlyChasingPlayer.Contains(e)) {
            currentlyChasingPlayer.Remove(e);
            NumberChasingPlayerChange();
        }
    }

    public static void AddIfNotPresent(Enemy e) {
        if (!currentlyChasingPlayer.Contains(e)) {
            currentlyChasingPlayer.Add(e);
            NumberChasingPlayerChange();
        }
    }

    public static void NumberChasingPlayerChange() {
        if (currentlyChasingPlayer.Count == 0) {
            GameMasterManager.instance.dayController.MusicCheck();
        } else if (AudioManager.instance.currentAudioSource.source.clip != GameMasterManager.instance.chaseClip) {
            AudioManager.PlayTrack(GameMasterManager.instance.chaseClip);
        }
    }


    public const string WALKINGANIMSTRING = "WALKING", IDLEANIMSTRING = "IDLE", DEATH = "DEATH";

    public static TopDownPlayerController instance;
    public GameMasterManager manager;
    public InputManager input;
    public Transform lookTransform;
    public Rigidbody2D rb;
    public bool isDead;
    public Collider2D bodycollider;

    internal bool IsInKillableState() {
        if (Time.timeScale == 0) return false;
        if (statemachine.currentState is IdleState || statemachine.currentState is WalkState) return true;
        return false;
    }

    

    internal void Kill(bool isHole) {
        statemachine.ChangeState(new LoseState(isHole));
    }

    public Animator anim;

  

    public float maxHungerAmount = 100f;

    public void AddFood(float foodAmount) {
        currentHungerAmount += foodAmount;
        currentHungerAmount = Mathf.Min(maxHungerAmount, currentHungerAmount);
        hungerBar.fillAmount = currentHungerAmount / maxHungerAmount;
        hungerBar.color = GetHungerColor();
        eatClip.Play();
    }

    public bool hasKey = false;

    public float currentHungerAmount = 100f;
    public float hungerDecreasePerSecond = 2.5f;
    public Image hungerBar;
    public Image keyImage;
    public Transform hungerBarParent;


    public float turnspeed = 180f;
    public float moveSpeed = 5f;

    public StateMachine<TopDownPlayerController> statemachine;

    public void PlayIdle() {
        anim.Play(IDLEANIMSTRING);
    }

    public Color fullHunger = Color.green, midHunger = Color.yellow, lowHunger = Color.red;

    public void HungerDecrease() {
        currentHungerAmount -= hungerDecreasePerSecond * Time.deltaTime;
        hungerBar.fillAmount = currentHungerAmount / maxHungerAmount;
        hungerBar.color = Color.LerpUnclamped(hungerBar.color, GetHungerColor(), 0.75f * Time.deltaTime);
        if (currentHungerAmount < 0) {
            statemachine.ChangeState(new LoseState(false));
        }
    }

    public void Win() {
        statemachine.ChangeState(new WinState());
    }


    public void StopWalking() {
        anim.Play(IDLEANIMSTRING);
    }

    private void Awake() {
        instance = this;
    }

    public Color GetHungerColor() {
        if (hungerBar.fillAmount > 0.75f) return fullHunger;
        if (hungerBar.fillAmount > 0.5f) return midHunger;
        return lowHunger;
    }

    public void FaceDirection(Vector2 v) {
        lookTransform.right = v;
    }

    public void EnterDoNothingState() {
        if (statemachine == null) statemachine = new StateMachine<TopDownPlayerController>(new DoNothingState(), this);
        statemachine.ChangeState(new DoNothingState());
    }

    public void EnterIdleState() {
        statemachine.ChangeState(new IdleState());
    }

    private void Update() {
        statemachine.Update();
    }

    public void ResetHungerBar() {
        hungerBarParent.gameObject.SetActive(false);
        currentHungerAmount = maxHungerAmount;
        hungerBar.fillAmount = 1;
        hungerBar.color = GetHungerColor();
    }

    public void GetKey() {
        hasKey = true;
        keyImage.enabled = hasKey;
    }

    public class DoNothingState : State<TopDownPlayerController> {

        public override void Enter(StateMachine<TopDownPlayerController> obj) {
            obj.target.FreezeMovement();
            obj.target.FaceDirection(Vector2.down);
            obj.target.ResetHungerBar();
        }


    }

    public class IdleState : State<TopDownPlayerController> {
        public override void Enter(StateMachine<TopDownPlayerController> obj) {
            obj.target.rb.velocity = Vector2.zero;
            obj.target.UnFreezeMovement();
            obj.target.anim.Play(IDLEANIMSTRING);
            obj.target.hungerBarParent.gameObject.SetActive(true);
            obj.target.keyImage.enabled = obj.target.hasKey;
        }

        public override void Update(StateMachine<TopDownPlayerController> obj) {
            obj.target.HungerDecrease();
            if (obj.target.input.forward != 0 || obj.target.input.horizontal != 0) {
                Debug.Log("StartWalking");
                obj.ChangeState(new WalkState());
                return;
            }
        }
    }

    public class WalkState : State<TopDownPlayerController> {
        public override void Enter(StateMachine<TopDownPlayerController> obj) {
            obj.target.anim.Play(WALKINGANIMSTRING);
            obj.target.UnFreezeMovement();
        }

        public override void Update(StateMachine<TopDownPlayerController> obj) {
            obj.target.HungerDecrease();

            if (obj.target.input.forward == 0 && obj.target.input.horizontal == 0) {
                obj.ChangeState(new IdleState());
                return;
            }

            obj.target.rb.velocity = Vector2.ClampMagnitude(new Vector2(obj.target.input.horizontal, obj.target.input.forward),1) * obj.target.moveSpeed;
            obj.target.FaceDirection(obj.target.rb.velocity);
        }
    }

    public class LoseState : State<TopDownPlayerController> {
        private bool isHole;

        public LoseState(bool isHole) {
            this.isHole = isHole;
        }

        public override void Enter(StateMachine<TopDownPlayerController> obj) {
            obj.target.StopWalking();
            obj.target.deathClip.Play();
            obj.target.FreezeMovement();
            obj.target.manager.EnterLoseState();
            obj.target.anim.Play(DEATH);

        }


        public override void Update(StateMachine<TopDownPlayerController> obj) {
            if (isHole && (obj.target.transform.localScale != Vector3.zero || KillBox.killHole.position != obj.target.transform.position) ) {
                obj.target.transform.localScale = Vector3.MoveTowards(obj.target.transform.localScale, Vector3.zero, 3 * Time.deltaTime);
                obj.target.transform.position = Vector3.MoveTowards(obj.target.transform.position, KillBox.killHole.position, 2 *  Time.deltaTime);
            }
        }

        public override void Exit(StateMachine<TopDownPlayerController> obj) {
            obj.target.transform.localScale = Vector3.one;
        }
    }

    public void UnFreezeMovement() {
        rb.isKinematic = false;
        bodycollider.enabled = true;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    public void FreezeMovement() {
        rb.velocity = Vector2.zero;
        bodycollider.enabled = false;
        rb.isKinematic = true;
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
    }
    public class WinState : State<TopDownPlayerController> {



        public override void Enter(StateMachine<TopDownPlayerController> obj) {
            obj.target.StopWalking();
            obj.target.FreezeMovement();
            obj.target.manager.EnterWinState();
            AudioManager.PlayTrack(GameMasterManager.instance.victoryClip);
            
        }
    }

}



