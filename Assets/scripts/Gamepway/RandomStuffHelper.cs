using System;
using UnityEngine;

[Serializable]
public struct TimerUtility
{
    public float Timeout;
    private float _prevTriggerTime;

    public TimerUtility(int timeout)
    {
        Timeout = timeout;
        _prevTriggerTime = 0;
    }

    public bool Update()
    {
        if (_prevTriggerTime == 0)
        {
            _prevTriggerTime = Time.time;
        }

        var currentTime = Time.time;
        if (currentTime - _prevTriggerTime >= Timeout)
        {
            _prevTriggerTime = currentTime;
            return true;
        }
        return false;
    }

    public void Reset()
    {
        _prevTriggerTime = Time.time;
    }
}

public readonly struct ExcludedValues
{
    public readonly int Number;

    private const int NoValueSentinel = int.MaxValue;
    public static ExcludedValues None => new(NoValueSentinel);
    public bool HasExcludedValues => Number != NoValueSentinel;

    public ExcludedValues(int prevNumber)
    {
        Number = prevNumber;
    }

    public bool Excludes(int value) => Number != value;
}

public static class RandomStuffHelper
{
    public static Vector2 ToVector2(this Vector3 v)
    {
        return v;
    }
    public static int PickUniqueNumber(int minInclude, int maxExclude, ExcludedValues excluded)
    {
        Debug.Assert(maxExclude - minInclude >= 2);
        if (excluded.HasExcludedValues)
        {
            Debug.Assert(excluded.Number >= minInclude && excluded.Number < maxExclude);
        }
        while (true)
        {
            int ret = UnityEngine.Random.Range(minInclude, maxExclude);
            if (excluded.Excludes(ret))
            {
                return ret;
            }
        }
    }

    public static void playRandomSound(ReadOnlySpan<AudioClip> array, AudioSource audioSource)
    {
        if (array.Length == 0)
        {
            return;
        }
        int index = PickUniqueNumber(0, array.Length, ExcludedValues.None);
        AudioClip soundToPlay = array[index];
        audioSource.PlayOneShot(soundToPlay);
    }

    public static void playRandomSound(ReadOnlySpan<AudioClip> array, AudioSource audioSource, ref ExcludedValues excludedNumber)
    {
        if (array.Length == 0)
        {
            return;
        }

        int index = PickUniqueNumber(0, array.Length, excludedNumber);
        AudioClip soundToPlay = array[index];
        audioSource.PlayOneShot(soundToPlay);
        excludedNumber = new(index);
    }
}
