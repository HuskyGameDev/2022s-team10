using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class animationRandomStart : MonoBehaviour
{
    private Animator anim;
    private SpriteRenderer spriteRend;

    [System.Obsolete]
    void Start()
  {
    anim = GetComponent<Animator>();
    var state = anim.GetCurrentAnimatorStateInfo(0);
    anim.Play(state.fullPathHash, 0, Random.RandomRange(0f, 1f));
  }

}
