using Assets.Scripts.BSSocket.Enums;
using System;
using System.Collections;
using UnityEngine;

public class CoinController : MonoBehaviour
{
    [Header("When heads side shown to player its going to be activated.")]
    public GameObject GOHeads;

    [Header("When tails side shown to player its going to be activated.")]
    public GameObject GOTails;

    [Header("Short order canvas.")]
    public Canvas ShortOrderCanvas;

    private RectTransform rectTransform;
    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        rectTransform = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        // We get the angle of coin.
        Vector3 angle = rectTransform.localRotation.eulerAngles;

        // We check is the angle between tails face.
        bool isTails = angle.z >= 90 && angle.z < 270;

        // if it is tails going to be active.
        GOTails.SetActive(isTails);

        // Other wise heads going to be active.
        GOHeads.SetActive(!isTails);
    }

    public void FlipCoin(Action onCompleted = null, params Flips[] flips) => StartCoroutine(flipCoin(onCompleted, flips));
    private IEnumerator flipCoin(Action onCompleted = null, params Flips[] flips)
    {
        // if no flips exists we return.
        if (flips.Length == 0)
        {
            // We call if callback exists.
            if (onCompleted != null)
                onCompleted.Invoke();

            yield break;
        }

        // We change its layer as coinflip to draw at top.
        ShortOrderCanvas.sortingLayerName = "CoinFlip";

        for (int i = 0; i < flips.Length; i++)
        {
            // When was the first time we will play main animation.
            if (i == 0)
            {
                if (flips[i] == Flips.Heads)
                    animator.Play("HeadsRise", 0, 0);
                else
                    animator.Play("TailsRise", 0, 0);
            }
            else // After first time we will just rotate arount coins.
            {
                if (flips[i] == Flips.Heads)
                    animator.Play("HeadsRepeat", 0, 0);
                else
                    animator.Play("TailsRepeat", 0, 0);
            }

            // We wait for a frame.
            yield return new WaitForEndOfFrame();

            // We wait until the animation completed.
            yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1);
        }

        // Finally we play the return anim.
        animator.Play("Return", 0, 0);
        
        // We change its layer as coinflip to draw at top.
        ShortOrderCanvas.sortingLayerName = "Coin";

        // We call if callback exists.
        if (onCompleted != null)
            onCompleted.Invoke();
    }

}
