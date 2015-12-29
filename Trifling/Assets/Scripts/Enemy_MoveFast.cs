using UnityEngine;
using System.Collections;

public class Enemy_MoveFast : Enemy_MoveStraight {

	protected override void Awake () {
        base.Awake();
        instance = this;
        moveSpeed = 2.25f;
        points = 2;
    }
}
