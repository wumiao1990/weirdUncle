using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

[System.Serializable]
public class ItemData
{
	public int instanceId;
	public int itemId;
	public int posX;
	public int posZ;
}

[System.Serializable]
public class SceneData
{
	public List<ItemData> items;

	public SceneData()
	{
		items = new List<ItemData>();
	}

	public void AddOrUpdateItem(int instanceId, int itemId, int posX, int posZ)
	{
		ItemData itemData = null;
		foreach (ItemData item in this.items)
		{
			if (item.instanceId == instanceId)
			{
				itemData = item;
			}
		}

		if (itemData == null)
		{
			itemData = new ItemData();
			itemData.instanceId = instanceId;
			itemData.itemId = itemId;
			this.items.Add(itemData);
		}

		itemData.posX = posX;
		itemData.posZ = posZ;
	}

	public void RemoveItem(int instanceId)
	{
		ItemData targetItem = this.GetItem(instanceId);

		if (targetItem != null)
		{
			this.items.Remove(targetItem);
		}
	}

	public ItemData GetItem(int instanceId)
	{
		ItemData targetItem = null;
		foreach (ItemData itemData in this.items)
		{
			if (itemData.instanceId == instanceId)
			{
				targetItem = itemData;
			}
		}
		return targetItem;
	}
}

[System.Serializable]
public class GameData
{
	public SceneData sceneData;
}

public class DataBaseManager : MonoBehaviour
{

	public static DataBaseManager instance;

	private string gameDataFilePath = "/StreamingAssets/db.json";
	private GameData _gameData;

	private string _defaultSceneData =
	    "{\"items\":[" + 
	    "{\"instanceId\":1,\"itemId\":3635,\"posX\":37,\"posZ\":19}," +
	    "{\"instanceId\":2,\"itemId\":2496,\"posX\":22,\"posZ\":23},"+
	    "{\"instanceId\":3,\"itemId\":2728,\"posX\":28,\"posZ\":15},"+
	    "{\"instanceId\":4,\"itemId\":2728,\"posX\":18,\"posZ\":24},"+
	    "{\"instanceId\":59892,\"itemId\":4764,\"posX\":32,\"posZ\":14},"+
	    "{\"instanceId\":53849,\"itemId\":7666,\"posX\":27,\"posZ\":30},"+
	    "{\"instanceId\":35433,\"itemId\":7666,\"posX\":26,\"posZ\":15},"+
	    "{\"instanceId\":77656,\"itemId\":7666,\"posX\":13,\"posZ\":22},"+
	    "{\"instanceId\":23515,\"itemId\":7666,\"posX\":14,\"posZ\":22},"+
	    "{\"instanceId\":65764,\"itemId\":7666,\"posX\":31,\"posZ\":22},"+
	    "{\"instanceId\":44381,\"itemId\":7666,\"posX\":28,\"posZ\":18},"+
	    "{\"instanceId\":98990,\"itemId\":7666,\"posX\":26,\"posZ\":14},"+
	    "{\"instanceId\":26088,\"itemId\":7666,\"posX\":21,\"posZ\":22},"+
	    "{\"instanceId\":67953,\"itemId\":7666,\"posX\":31,\"posZ\":18},"+
	    "{\"instanceId\":66357,\"itemId\":7666,\"posX\":31,\"posZ\":20},"+
	    "{\"instanceId\":47611,\"itemId\":7666,\"posX\":15,\"posZ\":22},"+
	    "{\"instanceId\":33764,\"itemId\":7666,\"posX\":21,\"posZ\":23},"+
	    "{\"instanceId\":63502,\"itemId\":7666,\"posX\":27,\"posZ\":23},"+
	    "{\"instanceId\":45198,\"itemId\":7666,\"posX\":27,\"posZ\":31},"+
	    "{\"instanceId\":77620,\"itemId\":7666,\"posX\":27,\"posZ\":29},"+
	    "{\"instanceId\":67559,\"itemId\":7666,\"posX\":30,\"posZ\":18},"+
	    "{\"instanceId\":44433,\"itemId\":7666,\"posX\":27,\"posZ\":18},"+
	    "{\"instanceId\":97899,\"itemId\":7666,\"posX\":29,\"posZ\":18},"+
	    "{\"instanceId\":12916,\"itemId\":7666,\"posX\":31,\"posZ\":19},"+
	    "{\"instanceId\":16064,\"itemId\":7666,\"posX\":31,\"posZ\":21},"+
	    "{\"instanceId\":70900,\"itemId\":7666,\"posX\":21,\"posZ\":21},"+
	    "{\"instanceId\":99623,\"itemId\":7666,\"posX\":26,\"posZ\":13},"+
	    "{\"instanceId\":40703,\"itemId\":7666,\"posX\":21,\"posZ\":25},"+
	    "{\"instanceId\":92164,\"itemId\":7666,\"posX\":27,\"posZ\":26},"+
	    "{\"instanceId\":91485,\"itemId\":7666,\"posX\":21,\"posZ\":24},"+
	    "{\"instanceId\":75443,\"itemId\":7666,\"posX\":25,\"posZ\":27},"+
	    "{\"instanceId\":99455,\"itemId\":7666,\"posX\":27,\"posZ\":25},"+
	    "{\"instanceId\":77706,\"itemId\":7666,\"posX\":27,\"posZ\":27},"+
	    "{\"instanceId\":87534,\"itemId\":7666,\"posX\":21,\"posZ\":26},"+
	    "{\"instanceId\":67667,\"itemId\":7666,\"posX\":27,\"posZ\":24},"+
	    "{\"instanceId\":58080,\"itemId\":7666,\"posX\":26,\"posZ\":18},"+
	    "{\"instanceId\":10131,\"itemId\":7666,\"posX\":21,\"posZ\":27},"+
	    "{\"instanceId\":12489,\"itemId\":7666,\"posX\":22,\"posZ\":27},"+
	    "{\"instanceId\":27546,\"itemId\":7666,\"posX\":23,\"posZ\":27},"+
	    "{\"instanceId\":24874,\"itemId\":7666,\"posX\":24,\"posZ\":27},"+
	    "{\"instanceId\":60340,\"itemId\":7666,\"posX\":26,\"posZ\":27},"+
	    "{\"instanceId\":36894,\"itemId\":4764,\"posX\":28,\"posZ\":20},"+
	    "{\"instanceId\":66286,\"itemId\":9074,\"posX\":20,\"posZ\":30},"+
	    "{\"instanceId\":21809,\"itemId\":8833,\"posX\":23,\"posZ\":18},"+
	    "{\"instanceId\":31911,\"itemId\":3265,\"posX\":18,\"posZ\":19},"+
	    "{\"instanceId\":63140,\"itemId\":7666,\"posX\":31,\"posZ\":23},"+
	    "{\"instanceId\":11908,\"itemId\":7666,\"posX\":28,\"posZ\":23},"+
	    "{\"instanceId\":17874,\"itemId\":7666,\"posX\":29,\"posZ\":23},"+
	    "{\"instanceId\":34391,\"itemId\":7666,\"posX\":30,\"posZ\":23},"+
	    "{\"instanceId\":51153,\"itemId\":7666,\"posX\":20,\"posZ\":27},"+
	    "{\"instanceId\":73339,\"itemId\":7666,\"posX\":19,\"posZ\":27},"+
	    "{\"instanceId\":55737,\"itemId\":7666,\"posX\":16,\"posZ\":26},"+
	    "{\"instanceId\":76912,\"itemId\":7666,\"posX\":23,\"posZ\":16},"+
	    "{\"instanceId\":89407,\"itemId\":7666,\"posX\":16,\"posZ\":23},"+
	    "{\"instanceId\":92266,\"itemId\":7666,\"posX\":16,\"posZ\":24},"+
	    "{\"instanceId\":27460,\"itemId\":7666,\"posX\":16,\"posZ\":25},"+
	    "{\"instanceId\":77491,\"itemId\":7666,\"posX\":22,\"posZ\":16},"+
	    "{\"instanceId\":63022,\"itemId\":7666,\"posX\":18,\"posZ\":27},"+
	    "{\"instanceId\":63776,\"itemId\":7666,\"posX\":26,\"posZ\":16},"+
	    "{\"instanceId\":50152,\"itemId\":7666,\"posX\":17,\"posZ\":27},"+
	    "{\"instanceId\":43781,\"itemId\":7666,\"posX\":21,\"posZ\":18},"+
	    "{\"instanceId\":16508,\"itemId\":7666,\"posX\":21,\"posZ\":16},"+
	    "{\"instanceId\":80940,\"itemId\":7666,\"posX\":25,\"posZ\":16},"+
	    "{\"instanceId\":19060,\"itemId\":7666,\"posX\":27,\"posZ\":28},"+
	    "{\"instanceId\":43053,\"itemId\":7666,\"posX\":21,\"posZ\":19},"+
	    "{\"instanceId\":18532,\"itemId\":7666,\"posX\":20,\"posZ\":22},"+
	    "{\"instanceId\":27092,\"itemId\":7666,\"posX\":16,\"posZ\":27},"+
	    "{\"instanceId\":40296,\"itemId\":7666,\"posX\":18,\"posZ\":22},"+
	    "{\"instanceId\":56852,\"itemId\":7666,\"posX\":26,\"posZ\":17},"+
	    "{\"instanceId\":16410,\"itemId\":7666,\"posX\":21,\"posZ\":20},"+
	    "{\"instanceId\":37891,\"itemId\":7666,\"posX\":21,\"posZ\":17},"+
	    "{\"instanceId\":36493,\"itemId\":7666,\"posX\":17,\"posZ\":22},"+
	    "{\"instanceId\":88305,\"itemId\":7666,\"posX\":16,\"posZ\":22},"+
	    "{\"instanceId\":16780,\"itemId\":7666,\"posX\":24,\"posZ\":16},"+
	    "{\"instanceId\":90787,\"itemId\":7666,\"posX\":19,\"posZ\":22},"+
	    "{\"instanceId\":15113,\"itemId\":4856,\"posX\":15,\"posZ\":29},"+
	    "{\"instanceId\":56078,\"itemId\":2949,\"posX\":41,\"posZ\":41},"+
	    "{\"instanceId\":42821,\"itemId\":4764,\"posX\":30,\"posZ\":26},"+
	    "{\"instanceId\":61823,\"itemId\":4764,\"posX\":14,\"posZ\":19},"+
	    "{\"instanceId\":71678,\"itemId\":1712,\"posX\":22,\"posZ\":12},"+
	    "{\"instanceId\":20344,\"itemId\":1712,\"posX\":11,\"posZ\":24},"+
	    "{\"instanceId\":29249,\"itemId\":2728,\"posX\":33,\"posZ\":18},"+
	    "{\"instanceId\":86916,\"itemId\":5341,\"posX\":42,\"posZ\":39},"+
	    "{\"instanceId\":77332,\"itemId\":5341,\"posX\":31,\"posZ\":40},"+
	    "{\"instanceId\":21622,\"itemId\":5341,\"posX\":33,\"posZ\":41},"+
	    "{\"instanceId\":27640,\"itemId\":5341,\"posX\":35,\"posZ\":41},"+
	    "{\"instanceId\":49978,\"itemId\":5341,\"posX\":40,\"posZ\":39},"+
	    "{\"instanceId\":17499,\"itemId\":5341,\"posX\":43,\"posZ\":30},"+
	    "{\"instanceId\":80451,\"itemId\":5341,\"posX\":40,\"posZ\":35},"+
	    "{\"instanceId\":85859,\"itemId\":5341,\"posX\":32,\"posZ\":39},"+
	    "{\"instanceId\":58342,\"itemId\":1251,\"posX\":30,\"posZ\":40},"+
	    "{\"instanceId\":67801,\"itemId\":5341,\"posX\":30,\"posZ\":41},"+
	    "{\"instanceId\":41858,\"itemId\":5341,\"posX\":34,\"posZ\":40},"+
	    "{\"instanceId\":31873,\"itemId\":5341,\"posX\":33,\"posZ\":39},"+
	    "{\"instanceId\":37453,\"itemId\":5341,\"posX\":36,\"posZ\":39},"+
	    "{\"instanceId\":63226,\"itemId\":5341,\"posX\":35,\"posZ\":39},"+
	    "{\"instanceId\":26527,\"itemId\":5341,\"posX\":37,\"posZ\":41},"+
	    "{\"instanceId\":51181,\"itemId\":5341,\"posX\":38,\"posZ\":39},"+
	    "{\"instanceId\":58088,\"itemId\":5341,\"posX\":39,\"posZ\":37},"+
	    "{\"instanceId\":50036,\"itemId\":5341,\"posX\":42,\"posZ\":37},"+
	    "{\"instanceId\":15387,\"itemId\":5341,\"posX\":42,\"posZ\":35},"+
	    "{\"instanceId\":47032,\"itemId\":5341,\"posX\":41,\"posZ\":34},"+
	    "{\"instanceId\":66832,\"itemId\":5341,\"posX\":43,\"posZ\":33},"+
	    "{\"instanceId\":17459,\"itemId\":5341,\"posX\":39,\"posZ\":41},"+
	    "{\"instanceId\":75306,\"itemId\":5341,\"posX\":43,\"posZ\":32},"+
	    "{\"instanceId\":38803,\"itemId\":5341,\"posX\":43,\"posZ\":28},"+
	    "{\"instanceId\":77374,\"itemId\":5341,\"posX\":29,\"posZ\":42},"+
	    "{\"instanceId\":47941,\"itemId\":5341,\"posX\":28,\"posZ\":41},"+
	    "{\"instanceId\":62227,\"itemId\":5341,\"posX\":28,\"posZ\":39},"+
	    "{\"instanceId\":43477,\"itemId\":5341,\"posX\":38,\"posZ\":35},"+
	    "{\"instanceId\":45500,\"itemId\":5341,\"posX\":42,\"posZ\":26},"+
	    "{\"instanceId\":20055,\"itemId\":5341,\"posX\":38,\"posZ\":33},"+
	    "{\"instanceId\":31352,\"itemId\":5341,\"posX\":30,\"posZ\":38},"+
	    "{\"instanceId\":80700,\"itemId\":5341,\"posX\":32,\"posZ\":38},"+
	    "{\"instanceId\":66682,\"itemId\":5341,\"posX\":31,\"posZ\":37},"+
	    "{\"instanceId\":92946,\"itemId\":5341,\"posX\":27,\"posZ\":41},"+
	    "{\"instanceId\":25391,\"itemId\":5341,\"posX\":24,\"posZ\":40}]}";

	   
	private string _enemySceneData =
        "{\"items\":[" +
        "{\"instanceId\":2,\"itemId\":2496,\"posX\":22,\"posZ\":22}," +
        "{\"instanceId\":3,\"itemId\":2728,\"posX\":13,\"posZ\":32}," +
        "{\"instanceId\":4,\"itemId\":2728,\"posX\":9,\"posZ\":26}," +
        "{\"instanceId\":59892,\"itemId\":4764,\"posX\":15,\"posZ\":26}," +
        "{\"instanceId\":53849,\"itemId\":7666,\"posX\":27,\"posZ\":20}," +
        "{\"instanceId\":35433,\"itemId\":7666,\"posX\":27,\"posZ\":21}," +
        "{\"instanceId\":77656,\"itemId\":7666,\"posX\":27,\"posZ\":19}," +
        "{\"instanceId\":23515,\"itemId\":7666,\"posX\":26,\"posZ\":19}," +
        "{\"instanceId\":65764,\"itemId\":7666,\"posX\":19,\"posZ\":28}," +
        "{\"instanceId\":44381,\"itemId\":7666,\"posX\":20,\"posZ\":19}," +
        "{\"instanceId\":98990,\"itemId\":7666,\"posX\":25,\"posZ\":19}," +
        "{\"instanceId\":26088,\"itemId\":7666,\"posX\":19,\"posZ\":22}," +
        "{\"instanceId\":67953,\"itemId\":7666,\"posX\":19,\"posZ\":25}," +
        "{\"instanceId\":66357,\"itemId\":7666,\"posX\":19,\"posZ\":27}," +
        "{\"instanceId\":47611,\"itemId\":7666,\"posX\":23,\"posZ\":19}," +
        "{\"instanceId\":33764,\"itemId\":7666,\"posX\":19,\"posZ\":23}," +
        "{\"instanceId\":63502,\"itemId\":7666,\"posX\":27,\"posZ\":23}," +
        "{\"instanceId\":45198,\"itemId\":7666,\"posX\":27,\"posZ\":22}," +
        "{\"instanceId\":77620,\"itemId\":7666,\"posX\":24,\"posZ\":19}," +
        "{\"instanceId\":67559,\"itemId\":7666,\"posX\":19,\"posZ\":21}," +
        "{\"instanceId\":44433,\"itemId\":7666,\"posX\":19,\"posZ\":19}," +
        "{\"instanceId\":97899,\"itemId\":7666,\"posX\":19,\"posZ\":20}," +
        "{\"instanceId\":12916,\"itemId\":7666,\"posX\":19,\"posZ\":26}," +
        "{\"instanceId\":16064,\"itemId\":7666,\"posX\":19,\"posZ\":29}," +
        "{\"instanceId\":70900,\"itemId\":7666,\"posX\":21,\"posZ\":19}," +
        "{\"instanceId\":99623,\"itemId\":7666,\"posX\":22,\"posZ\":19}," +
        "{\"instanceId\":40703,\"itemId\":7666,\"posX\":21,\"posZ\":29}," +
        "{\"instanceId\":92164,\"itemId\":7666,\"posX\":27,\"posZ\":26}," +
        "{\"instanceId\":91485,\"itemId\":7666,\"posX\":20,\"posZ\":29}," +
        "{\"instanceId\":75443,\"itemId\":7666,\"posX\":27,\"posZ\":29}," +
        "{\"instanceId\":99455,\"itemId\":7666,\"posX\":27,\"posZ\":25}," +
        "{\"instanceId\":77706,\"itemId\":7666,\"posX\":27,\"posZ\":27}," +
        "{\"instanceId\":87534,\"itemId\":7666,\"posX\":22,\"posZ\":29}," +
        "{\"instanceId\":67667,\"itemId\":7666,\"posX\":27,\"posZ\":24}," +
        "{\"instanceId\":58080,\"itemId\":7666,\"posX\":19,\"posZ\":18}," +
        "{\"instanceId\":10131,\"itemId\":7666,\"posX\":23,\"posZ\":29}," +
        "{\"instanceId\":12489,\"itemId\":7666,\"posX\":24,\"posZ\":29}," +
        "{\"instanceId\":27546,\"itemId\":7666,\"posX\":25,\"posZ\":29}," +
        "{\"instanceId\":24874,\"itemId\":7666,\"posX\":26,\"posZ\":29}," +
        "{\"instanceId\":60340,\"itemId\":7666,\"posX\":27,\"posZ\":28}," +
        "{\"instanceId\":36894,\"itemId\":4764,\"posX\":15,\"posZ\":19}," +
        "{\"instanceId\":66286,\"itemId\":9074,\"posX\":21,\"posZ\":30}," +
        "{\"instanceId\":21809,\"itemId\":8833,\"posX\":9,\"posZ\":18}," +
        "{\"instanceId\":31911,\"itemId\":3265,\"posX\":12,\"posZ\":12}]}";

	void Awake()
	{
		instance = this;
		this.EnsureGameDataFileExists();
	}

	public void EnsureGameDataFileExists()
	{
		this._gameData = new GameData();
		this._gameData.sceneData = new SceneData();

		if (Application.platform == RuntimePlatform.WebGLPlayer)
		{
			return;
		}

		string filePath = Application.persistentDataPath + gameDataFilePath;
		string directoryPath = Application.persistentDataPath + "/StreamingAssets";

		if (Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.LinuxEditor)
		{
			filePath = Application.dataPath + gameDataFilePath;
			directoryPath = Application.dataPath + "/StreamingAssets";
		}

		if (!Directory.Exists(directoryPath))
		{
			Directory.CreateDirectory(directoryPath);
		}

		if (File.Exists(filePath))
		{
			string jsonData = File.ReadAllText(filePath);
			this._gameData = JsonUtility.FromJson<GameData>(jsonData);
		}
		else
		{
			this.SaveDataBase();
		}
	}

	public SceneData GetScene()
	{
		if (this._gameData.sceneData.items.Count == 0)
		{
			this._gameData.sceneData = JsonUtility.FromJson<SceneData>(this._defaultSceneData);
			this.SaveDataBase();
		}
		return this._gameData.sceneData;
	}

	public SceneData GetEnemyScene()
	{
		SceneData sceneData = JsonUtility.FromJson<SceneData>(this._enemySceneData);
		return sceneData;
	}

	public void SaveScene()
	{
		foreach (BaseItemScript item in SceneManager.instance.GetAllItems())
		{
			this._gameData.sceneData.AddOrUpdateItem(item.instanceId, item.itemData.id, item.GetPositionX(), item.GetPositionZ());
		}
		this.SaveDataBase();
	}

	public void UpdateItemData(BaseItemScript item)
	{
		this._gameData.sceneData.AddOrUpdateItem(item.instanceId, item.itemData.id, item.GetPositionX(), item.GetPositionZ());
		this.SaveDataBase();
	}

	public void RemoveItem(BaseItemScript item)
    {
		this._gameData.sceneData.RemoveItem(item.instanceId);
        this.SaveDataBase();
    }

	public void SaveDataBase()
	{
		if (Application.platform == RuntimePlatform.WebGLPlayer)
		{
			return;
		}

		string filePath = Application.persistentDataPath + gameDataFilePath;

		if (Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.LinuxEditor)
		{
			filePath = Application.dataPath + gameDataFilePath;
		}

		string jsonData = JsonUtility.ToJson(this._gameData);
		File.WriteAllText(filePath, jsonData);
	}


}
