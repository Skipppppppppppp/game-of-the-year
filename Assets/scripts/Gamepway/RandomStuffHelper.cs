using UnityEngine;

public class RandomStuffHelper : MonoBehaviour
{
    public static int PickUniqueNumber(int minInclude, int maxExclude, int prevNumber)
    {
        int ret = Random.Range(minInclude, maxExclude);

        if (ret == prevNumber)
        {
            ret = PickUniqueNumber(minInclude, maxExclude, prevNumber);
        }

        return ret;
    }

    public static void playRandomSound(AudioClip[] array, AudioSource audioSource)
    {
        if (array.Length == 0)
        {
            return;
        }
        int index = PickUniqueNumber(0, array.Length, array.Length + 1);
        AudioClip soundToPlay = array[index];
        audioSource.PlayOneShot(soundToPlay);
    }

    public static void playRandomSound(AudioClip[] array, AudioSource audioSource, ref int excludedNumber)
    {
        if (array.Length == 0)
        {
            return;
        }

        int index = PickUniqueNumber(0, array.Length, excludedNumber);
        AudioClip soundToPlay = array[index];
        audioSource.PlayOneShot(soundToPlay);
        excludedNumber = index;
    }
}
