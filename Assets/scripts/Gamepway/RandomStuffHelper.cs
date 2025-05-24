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
}
