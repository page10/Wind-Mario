
using UnityEngine;

public class WindEffectPlayer : MonoBehaviour
{ 
    public ParticleSystem windEffectOutward;
    public ParticleSystem windEffectInward;
    void Start()
    {
        windEffectOutward.Stop();
        windEffectInward.Stop();
    }

    public void PlayWindEffect(bool isOutward)
    {
        if (isOutward)
        {
            windEffectOutward.Play();
            windEffectInward.Stop();
        }
        else
        {
            windEffectOutward.Stop();
            windEffectInward.Play();
        }
    }
}
