using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class DayController : MonoBehaviour
{

    public float lightChangeSpeed = 1;

    public bool isDay;

    public AudioClip dayClip;
    public AudioClip nightClip;

    public float dayNightTime;
    public Color dayColor = Color.white;


    public float GetSeconds() {
        return (Time.time - startTime) % 10;
    }

    internal void MusicCheck() {
        if (isDay) {
            if (!GameMasterManager.instance.InMainMenu()) AudioManager.PlayTrack(dayClip);
        } else {
            if (!GameMasterManager.instance.InMainMenu()) AudioManager.PlayTrack(nightClip);
        }
        AudioManager.instance.currentAudioSource.source.time = GetSeconds() % AudioManager.instance.currentAudioSource.source.clip.length;
    }

    public Color nightColor = Color.black;
    public Light2D globalLight;

    [Header("Outer Light")]
    public Light2D innerPlayerLight;
    public float innerPlayerLightTargetIntensity = 1.5f;
    public float innerLightRadius = 2f;
    public float innerFlickerAmount = 0.25f;
    public float innerFlickerSpeed = 1f;
    float currentOuterTargetFlicker = 0;


    [Header("Outer Light")]
    public Light2D outerPlayerLight;
    public float outerPlayerLightTargetIntensity = 1.5f;
    public float outerLightRadius = 4f;
    public float outerFlickerAmount = 0.5f;
    public float outerFlickerSpeed = 2f;
    float currentInnerTargetFlicker = 0;


    private void Awake() {
        ResetTime();
    }

    float startTime;
    float currentTime;

    public void ResetTime() {
        startTime = Time.time;
        currentTime = 0;
        innerPlayerLight.intensity = 0;
        outerPlayerLight.intensity = 0;
        currentInnerTargetFlicker = innerLightRadius;
        currentOuterTargetFlicker = outerLightRadius;
       if (!GameMasterManager.instance.InMainMenu()) AudioManager.PlayTrack(dayClip);
    }

    bool prevDay;

    public void DayCheck() {
        currentTime = Time.time - startTime;
        isDay = currentTime % (2 * dayNightTime) <= dayNightTime;
        if (isDay != prevDay) {
            MusicCheck();
        }
        prevDay = isDay;
    }

    float lerpamount;

    private void Update() {
        DayCheck();


        if (isDay && globalLight.color != dayColor) {
            lerpamount = (float)(currentTime % (2 * dayNightTime)) / 1f;
            globalLight.color = Color.Lerp(nightColor, dayColor, lerpamount);
            innerPlayerLight.intensity = innerPlayerLightTargetIntensity - (lerpamount * innerPlayerLightTargetIntensity);
            outerPlayerLight.intensity = outerPlayerLightTargetIntensity - (lerpamount * outerPlayerLightTargetIntensity);
        }

        if (!isDay && globalLight.color != nightColor) {
            lerpamount = (float)((currentTime % (2 * dayNightTime)) - dayNightTime) / 1f;
            globalLight.color = Color.Lerp(dayColor, nightColor, lerpamount);
            innerPlayerLight.intensity = lerpamount * innerPlayerLightTargetIntensity;
            outerPlayerLight.intensity = lerpamount * outerPlayerLightTargetIntensity;

        }

        if (!isDay) {
            if (innerPlayerLight.intensity != currentInnerTargetFlicker) {
                // innerPlayerLight.intensity = Mathf.MoveTowards(innerPlayerLight.intensity, currentInnerTargetFlicker, innerFlickerSpeed * Time.deltaTime);

                SetFieldValue<float>(innerPlayerLight, "m_ShapeLightFalloffSize", Mathf.MoveTowards(innerPlayerLight.shapeLightFalloffSize, currentInnerTargetFlicker, innerFlickerSpeed * Time.deltaTime));

            } else {
                if (currentInnerTargetFlicker == innerLightRadius) {
                    currentInnerTargetFlicker = innerLightRadius - innerFlickerAmount;
                } else {
                    currentInnerTargetFlicker = innerLightRadius;
                }
            }

            if (outerPlayerLight.shapeLightFalloffSize != currentOuterTargetFlicker) {
                SetFieldValue<float>(outerPlayerLight, "m_ShapeLightFalloffSize", Mathf.MoveTowards(outerPlayerLight.shapeLightFalloffSize, currentOuterTargetFlicker, outerFlickerSpeed * Time.deltaTime));
            } else {
                if (currentOuterTargetFlicker == outerLightRadius) {
                    currentOuterTargetFlicker = outerLightRadius - outerFlickerAmount;
                } else {
                    currentOuterTargetFlicker = outerLightRadius;
                }
            }


        }


    }

    void SetFieldValue<T>(object obj, string name, T val) {
        var field = obj.GetType().GetField(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        field?.SetValue(obj, val);
    }










}
