using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndicatorFadeInOut : MonoBehaviour
{
    public SpriteRenderer sprite;
    private float minimum = 0.0f;
    private float maximum = 0.8f;
    private float speed = 1.0f;

    private bool faded = false;

    void Update()
    {
        float step = speed * Time.deltaTime;

        if (faded)
        {
            sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, Mathf.Lerp(sprite.color.a, maximum, step));
            if (sprite.color.a >= maximum - 0.1f)
                faded = false;

        }
        else
        {
            sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, Mathf.Lerp(sprite.color.a, minimum, step));
            if (sprite.color.a <= minimum + 0.05f)
                faded = true;
        }
    }
}
