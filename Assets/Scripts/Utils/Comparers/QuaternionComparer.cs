using Quaternion = UnityEngine.Quaternion;

namespace Utils.Comparers
{
    public static class QuaternionComparer
    {
        public static int Compare(Quaternion x, Quaternion y, int accuracy = Constants.Accuracy)
        {
            return (int)(Quaternion.Angle(x, y) * accuracy);
        }
    }
}