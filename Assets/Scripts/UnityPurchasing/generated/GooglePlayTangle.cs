// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("a+jm6dlr6OPra+jo6QrIOIY5PGqwkMGmcYgbc6jb03zAKW6wMhNmd4o4PMs1jW2FiJlGBBr/ipytl/cRImOTRqnXq2/UeAXLaC3yNK5Zli1AYSCDglHN78sv1taS2DxDE9i02Ytj2Pp6mml6swA89tnoo1ndjR56i+asZW2eAUnNlBnz+Q896W8RC23Za+jL2eTv4MNvoW8e5Ojo6Ozp6mrSI1TybxYT+q+641k1hEnAX5eDb/foiw1AqdNNmtDI+ZEISRWPkME8PdIo2n+hOH6lg1U0gv7Fa8sSpPyRwSgt4L/+01FlWB6Z85hLRzrZt5aQxB970glKIlKoJUnz3ukhJ5nWLzuBEUmFYUawvqeyQyXOBQJOx6yGqW8MXICObOvq6Ono");
        private static int[] order = new int[] { 1,4,7,11,13,6,8,13,12,12,11,11,12,13,14 };
        private static int key = 233;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
