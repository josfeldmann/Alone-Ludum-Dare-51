using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMasterManager : MonoBehaviour
{
    public TopDownPlayerController player;
    public static GameMasterManager instance;
    public GameObject mainMenu;
    public Transform startPlayerPosition;
    public Transform pauseMenu;
    public Transform winMenu, loseMenu;
    public Key key;
    public GameObject vault;
    public DayController dayController;
    public GameObject mainMenuEnv;
    public WorldGenerator worldGenerator;
    public Camera cam;
    public CinemachineVirtualCamera vCam;
    public AudioClip victoryClip, chaseClip;
    public GameObject howToPlayMenu;

    public void HideAll() {
        mainMenuEnv.SetActive(false);
        mainMenu.gameObject.SetActive(false);
        pauseMenu.gameObject.SetActive(false);
        winMenu.gameObject.SetActive(false);
        loseMenu.gameObject.SetActive(false);
        player.hungerBarParent.gameObject.SetActive(false);

    }

    public void ShowHowToPlayMenu(bool b) {
        howToPlayMenu.SetActive(b);
    }


    public InputManager input;

    public StateMachine<GameMasterManager> machine;

    private void Awake() {
        instance = this;
        machine = new StateMachine<GameMasterManager>(new MainMenuState(), this);

    }

    public void PlayButton() {
        machine.ChangeState(new StartGameState());
    }

    public void ReplayButton() {
        MainMenuButton();
        PlayButton();
    }


    public void ResumeButton() {
        machine.ChangeState(new PlayGameState());
    }

    internal bool InMainMenu() {
        return machine.currentState is MainMenuState || TopDownPlayerController.instance.statemachine.currentState is TopDownPlayerController.WinState;
    }

    public void MainMenuButton() {
        machine.ChangeState(new MainMenuState());
    }


    public void QuitButton() {
        Application.Quit();
    }

    private void Update() {
        machine.Update();
    }

    public class MainMenuState : State<GameMasterManager> {

        public override void Enter(StateMachine<GameMasterManager> obj) {
            AudioManager.musicVolume = 0.25f;
          //  AudioManager.instance.SetVolumes();
            obj.target.HideAll();
            obj.target.worldGenerator.gameObject.SetActive(false);
            obj.target.worldGenerator.ClearRooms();
            obj.target.mainMenuEnv.SetActive(true);
            obj.target.mainMenu.SetActive(true);
            obj.target.player.transform.position = obj.target.startPlayerPosition.position;
            obj.target.MoveCameraTo(obj.target.startPlayerPosition.position);
            AudioManager.PlayTrack(obj.target.victoryClip);
            obj.target.player.EnterDoNothingState();
            obj.target.howToPlayMenu.gameObject.SetActive(false);
            obj.target.player.PlayIdle();
        }

        public override void Exit(StateMachine<GameMasterManager> obj) {
            AudioManager.musicVolume = .75f;
            //AudioManager.instance.SetVolumes();
        }

    }



    public class StartGameState : State<GameMasterManager> {
        public override void Enter(StateMachine<GameMasterManager> obj) {
            obj.target.key.Reset();
            obj.target.player.hasKey = false;
            obj.target.player.keyImage.enabled = false;
            obj.target.dayController.ResetTime();
            obj.target.worldGenerator.gameObject.SetActive(true);
            obj.target.worldGenerator.GenerateRooms();

            obj.ChangeState(new PlayGameState());
        }

    }


    public class PlayGameState : State<GameMasterManager> {
        public override void Enter(StateMachine<GameMasterManager> obj) {
            obj.target.HideAll();
            obj.target.player.EnterIdleState();
        }

        public override void Update(StateMachine<GameMasterManager> obj) {
            if (obj.target.input.pauseButtonDown) {
                obj.ChangeState(new PauseGameState());
            }
        }
    }

    public class LoseGameState : State<GameMasterManager> {
        public override void Enter(StateMachine<GameMasterManager> obj) {
            obj.target.HideAll();
            obj.target.loseMenu.gameObject.SetActive(true);
        }
    }

    public class WinGameState : State<GameMasterManager> {
        public override void Enter(StateMachine<GameMasterManager> obj) {
            obj.target.HideAll();
            obj.target.winMenu.gameObject.SetActive(true);
        }
    }

    public void MoveCameraTo(Vector3 position) {
        cam.transform.position = new Vector3(position.x, position.y, -10);
        vCam.enabled = false;
        vCam.transform.position = new Vector3(position.x, position.y, -10);
        StartCoroutine(WorkAroundCam());
    }

    public IEnumerator WorkAroundCam() {
        yield return null;
        vCam.enabled = true;
    }

    public class PauseGameState : State<GameMasterManager> {
        public override void Enter(StateMachine<GameMasterManager> obj) {
            Time.timeScale = 0;
            obj.target.HideAll();
            obj.target.pauseMenu.gameObject.SetActive(true);
        }

        public override void Exit(StateMachine<GameMasterManager> obj) {
            Time.timeScale = 1;
        }

        public override void Update(StateMachine<GameMasterManager> obj) {
            if (obj.target.input.pauseButtonDown) {
                obj.ChangeState(new PlayGameState());
            }
        }

    }

    internal void EnterWinState() {
        machine.ChangeState(new WinGameState());
    }

    internal void EnterLoseState() {
        machine.ChangeState(new LoseGameState());
    }
}
