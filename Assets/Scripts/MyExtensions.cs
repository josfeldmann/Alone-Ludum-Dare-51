using System.Collections.Generic;

public static class ListExtension {
    public static T PickRandomNotNull<T>(this T[] arr) {
        List<T> res = new List<T>();

        foreach (T t in arr) {
            if (t != null) res.Add(t);
        }

        return res.PickRandom();
    }

   



    public static T PickRandom<T>(this List<T> list) {
        return list[UnityEngine.Random.Range(0, list.Count)];
    }

    public static int GetCount<T>(this Dictionary<T, int> dict, T t) {
        if (dict.ContainsKey(t)) {
            return dict[t];
        } else {
            return 0;
        }
    }

    public static void AddI<T>(this Dictionary<T, int> dict, T t, int amt) {
        if (dict.ContainsKey(t)) {
            dict[t] += amt;
        } else {
            dict[t] = amt;
        }
    }



    public static bool ContainsAny<T>(this List<T> list, List<T> other) {

        foreach (T t in list) {
            if (other.Contains(t)) return true;
        }

        return false;
    }


    public static void AddValue<T>(this Dictionary<T, int> dictionary, T key, int amount) {
        int count;
        dictionary.TryGetValue(key, out count);
        dictionary[key] = count + amount;
    }

        private static System.Random rng = new System.Random();

        public static void Shuffle<T>(this IList<T> list) {
            int n = list.Count;
            while (n > 1) {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }


    }


