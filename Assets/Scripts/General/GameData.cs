using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName="GameData", menuName = "Data/GameData")]
public class GameData : ScriptableObject
{
	[System.Serializable]
	public class KVP<T>
	{
		public string key;
		public T value;

		public KVP(string key, T value)
		{
			this.key = key;
			this.value = value;
		}
	}

	[System.Serializable]
	public class KeyValuePairList<T>
	{
		public List<KVP<T>> values = new List<KVP<T>>();

		public T this[string key]
		{
			get 
			{
				var kvp = values.Find(kvp => kvp.key == key);
				if (kvp != null)
				{
					return kvp.value;
				}

				return default (T);
			}
			set
			{
				var kvp = values.Find(kvp => kvp.key == key);
				if (kvp != null)
				{
					kvp.value = value;
				}
				else
				{
					values.Add(new KVP<T>(key, value));
				}
			}
		}

		public bool HasKey(string key)
		{
			return (values.Find(kvp => kvp.key == key) != null);
		}

		public void Clear()
		{
			values.Clear();
		}
	}

	public KeyValuePairList<bool> boolData = new KeyValuePairList<bool>();
	public KeyValuePairList<int> intData = new KeyValuePairList<int>();
	public KeyValuePairList<float> floatData = new KeyValuePairList<float>();
	public KeyValuePairList<string> stringData = new KeyValuePairList<string>();
	public KeyValuePairList<Vector3> vector3Data = new KeyValuePairList<Vector3>();
	public KeyValuePairList<Quaternion> quaternionData = new KeyValuePairList<Quaternion>();

	void Reset()
	{
		boolData.Clear();
		intData.Clear();
		floatData.Clear();
		stringData.Clear();
		vector3Data.Clear();
		quaternionData.Clear();
	}

	public void Save(string key, bool value)
	{
		boolData[key] = value;
		
	}

	public bool Load(string key, ref bool value)
	{
		if (boolData.HasKey(key))
		{
			value = boolData[key];
			return true;
		}

		return false;
	}

	public void Save(string key, int value)
	{
		intData[key] = value;

	}

	public bool Load(string key, ref int value)
	{
		if (intData.HasKey(key))
		{
			value = intData[key];
			return true;
		}

		return false;
	}

	public void Save(string key, float value)
	{
		floatData[key] = value;

	}

	public bool Load(string key, ref float value)
	{
		if (floatData.HasKey(key))
		{
			value = floatData[key];
			return true;
		}

		return false;
	}

	public void Save(string key, string value)
	{
		stringData[key] = value;

	}

	public bool Load(string key, ref string value)
	{
		if (stringData.HasKey(key))
		{
			value = stringData[key];
			return true;
		}

		return false;
	}

	public void Save(string key, Vector3 value)
	{
		vector3Data[key] = value;

	}

	public bool Load(string key, ref Vector3 value)
	{
		if (vector3Data.HasKey(key))
		{
			value = vector3Data[key];
			return true;
		}

		return false;
	}

	public void Save(string key, Quaternion value)
	{
		quaternionData[key] = value;

	}

	public bool Load(string key, ref Quaternion value)
	{
		if (boolData.HasKey(key))
		{
			value = quaternionData[key];
			return true;
		}

		return false;
	}
}
