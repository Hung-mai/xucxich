#if UNITY_ANDROID || UNITY_IPHONE || UNITY_STANDALONE_OSX || UNITY_TVOS
// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("n3YqunC7VQaBfDQ0YNDGee0oODeOO09RfRj4G/jsWPtlXf30ZIZYESt2kzzOIPb+WcXDmfpk4vET6MXfSPp5Wkh1fnFS/jD+j3V5eXl9eHswa4yxHzlBhrf1lAtmR3O2nA8HoBKmJb1gNhYlLFn81O6W0+jCxf9rvlsgYwfZTb/xrn7o8J8YpNMWbD7rFk4JhoTUiUa6bdxao1QSiDvKfhrGmi/+lA/Kz++6ObjggkZQGwOKOK5Ix9sCMPNFV7uwZCxu8oJM60j6eXd4SPp5cnr6eXl4xJWUsGfUb/CeO0utPfJU479GLQwlVO+ilsGYVykB6HY2h9eh97OQDs4+R9MU/eKmOoqCygmQR3cGPS+34qxI6Y34zj6yrF+BjBoHU3p7eXh5");
        private static int[] order = new int[] { 10,8,9,10,4,10,13,10,11,12,11,13,13,13,14 };
        private static int key = 120;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
#endif
