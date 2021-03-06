﻿using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class GearPin : MonoBehaviour {
    [Header("Animation Settings")]
    [SerializeField] private bool rotateClockwise = true;
    [SerializeField, Range(0.1f, 3f)] private float animationDuration = 1f;
    
    /// <summary>
    /// Does this slot already have a gear on it.
    /// </summary>
    public bool HasGear { get; private set; }
    
    /// <summary>
    /// Updates the HasGear property. 
    /// </summary>
    public void SetGear(bool value) {
        HasGear = value;
    }

    // The gear in this slot.
    private Gear currentGear;

    // Subscriptions.
    private void Awake() {
        GearPinController.OnGearsCompleted += RotationAnimation;
        GearPinController.OnGearRemoved += (Gear g) => DOTween.Kill(gameObject);
        GearPinController.OnGearsReset += () => DOTween.Kill(gameObject);
    }

    // Unsubscriptions.
    private void OnDestroy()=> GearPinController.OnGearsCompleted -= RotationAnimation;

    // Rotation animation.
    private void RotationAnimation() {
        if(!HasGear) return;
        transform.DORotate(rotateClockwise ? Vector3.back * 360f : Vector3.forward * 360f, 
                animationDuration, RotateMode.FastBeyond360).
            SetEase(Ease.Linear).SetId(gameObject).onComplete = RotationAnimation;
    }

    // Collision events.
    private void OnTriggerEnter2D(Collider2D other) {
        if(HasGear) return;
        if(!other.GetComponent<GearUI>()) return;
        currentGear = other.GetComponent<GearUI>().SpawnGear();
        currentGear.transform.position = transform.position;
        currentGear.transform.parent = transform;
        currentGear.Pin = this;
        HasGear = true;
        GearPinController.OnGearPlaced.Invoke(currentGear);
    }

    private void OnTriggerExit2D(Collider2D other) {
        if(currentGear != null) return;
        HasGear = false;
    }
}
