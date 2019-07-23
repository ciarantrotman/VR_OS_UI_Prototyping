/******************************************************************************
 * Copyright (C) Leap Motion, Inc. 2011-2018.                                 *
 * Leap Motion proprietary and confidential.                                  *
 *                                                                            *
 * Use subject to the terms of the Leap Motion SDK Agreement available at     *
 * https://developer.leapmotion.com/sdk_agreement, or another agreement       *
 * between Leap Motion and you, your company or other organization.           *
 ******************************************************************************/

using UnityEngine;
using System.Collections;
using System;
using Leap;
using UnityEngine.Events;

namespace Leap.Unity{
  public class HandEnableDisable : HandTransitionBehavior
  {
    [HideInInspector] public UnityEvent handEnabledEvent;
    public bool handEnabled;
    protected override void Awake() {
      base.Awake();
      gameObject.SetActive(false);
      handEnabled = false;
    }

  	protected override void HandReset() {
      gameObject.SetActive(true);
      handEnabled = true;
      handEnabledEvent.Invoke();
    }

    protected override void HandFinish() {
      gameObject.SetActive(false);
      handEnabled = false;
    }

  }
}
