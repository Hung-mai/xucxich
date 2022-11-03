#if UNITY_ANDROID || UNITY_IPHONE || UNITY_STANDALONE_OSX || UNITY_TVOS
// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class AppleTangle
    {
        private static byte[] data = System.Convert.FromBase64String("rT+fUYnrirhqXGwXG6x7/L50aZ/Wys3Qy9bbk7SStqSh96ahsa/j0ufcve7J8jTjK2bWwKmyIeMlkSgjLdEjwmS5+auNMBBa5upSwpo8t1cKftyAl2iHd3utdMl2AIaBs1UDDvsFp6veteL0s7zWcRUpgZnlAXfNjOIEVeXv3ar8kr2kofe/gaa6krSOgsHH0NbLxMvBw9bHgtLNzsvB26eioSCjraKSIKOooCCjo6JGMwur0MPB1svBx4LR1sPWx8/HzNbRjJKvpKuIJOokVa+jo6enoqEgo6Oi/o2SI2Gkqomko6enpaCgkiMUuCMRFbkfMeCGsIhlrb8U7z78wWrpIrWCw8zGgsHH0NbLxMvBw9bLzcyC0mLBkdVVmKWO9El4rYOseBjRu+0XxS2qFoJVaQ6Ogs3SFJ2jki4V4W2fhMWCKJHIVa8gbXxJAY1b8cj5xtXVjMPS0s7HjMHNz43D0tLOx8HDqvySIKOzpKH3v4KmIKOqkiCjppKSs6Sh96aosajj0tLOx4LrzMGMk70nISe5O5/llVALOeIsjnYTMrB6e5TdYyX3ewU7G5DgWXp30zzcA/CRlPiSwJOpkqukofempLGg9/GTsaSh97+sprSmtolyy+U21KtcVskvIraJcsvlNtSrXFbJL4ziBFXl793bgsPR0dfPx9GCw8HBx9LWw8zBx5IgphmSIKEBAqGgo6Cgo6CSr6SrtJK2pKH3pqGxr+PS0s7HgvDNzdaC4eOSIKOAkq+kq4gk6iRVr6Ojo8zGgsHNzMbL1svNzNGCzcSC19HH0s7HgvDNzdaC4eOSvLWvkpSSlpAJAdMw5fH3Yw2N4xFaWUHSb0QB7oSShqSh96apsb/j0tLOx4Lhx9DWpU7fmyEp8YJxmmYTHTjtqMldiV6GQElzFdJ9redDhWhTz9pPRRe1tYLNxILWyseC1srHzILD0tLOy8HD1svEy8HD1seCwNuCw8zbgtLD0NaqiaSjp6eloKO0vMrW1tLRmI2N1Ygk6iRVr6Ojp6eiksCTqZKrpKH3a7vQV/+sd939OVCHoRj3Le//r1PLxMvBw9bLzcyC49fWys3Qy9bbk+t61D2RtscD1TZrj6Cho6KjASCjHFbROUxwxq1p2+2WegCcW9pdyWrSzseC4cfQ1svEy8HD1svNzILj16aksaD38ZOxkrOkofemqLGo49LSKbsrfFvpzlelCYCSoEq6nFryq3HAzseC0dbDzMbD0MaC1sfQz9GCw93jCjpbc2jEPobJs3IBGUa5iGG9NzzYrgblKfl2tJWRaWat72y2y3O9M3m85fJJp0/82yaPSZQA9e73TtiSIKPUkqykofe/raOjXaamoaCjpJKtpKH3v7Gjo12mp5Kho6Ndkr+UO+6P2hVPLjl+UdU5UNRw1ZLtY87HguvMwYyThJKGpKH3pqmxv+PSl5CTlpKRlPi1r5GXkpCSm5CTlpITkvpO+KaQLsoRLb98x9FdxfzHHheYD1atrKIwqRODtIzWd56vecC0IKOipKuIJOokVcHGp6OSI1CSiKTwx87Lw8zBx4LNzILWysvRgsHH0MaXgbfpt/u/ETZVVD48bfIYY/ry8ggod3hGXnKrpZUS19eD");
        private static int[] order = new int[] { 22,5,25,55,54,52,31,44,14,46,47,37,59,45,57,21,56,36,33,46,49,58,57,30,32,40,57,59,45,43,31,32,47,51,53,41,39,41,52,46,41,59,50,54,59,56,50,50,51,58,56,54,54,55,59,57,56,57,59,59,60 };
        private static int key = 162;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
#endif
