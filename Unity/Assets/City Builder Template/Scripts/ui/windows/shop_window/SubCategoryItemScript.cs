using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SubCategoryItemScript : MonoBehaviour {
	/* prefabs */
	public Sprite BarrackSprite;
	public Sprite BoatSprite;
	public Sprite BuilderHutSprite;
	public Sprite CampSprite;
	public Sprite CannonSprite;
	public Sprite ElixirCollectorSprite;
	public Sprite ElixirStorageSprite;
	public Sprite GemsSprite;
	public Sprite GoldMineSprite;
	public Sprite GoldStorageSprite;
	public Sprite TowerSprite;
	public Sprite TownCenterSprite;
	public Sprite Tree1Sprite;
	public Sprite Tree2Sprite;
	public Sprite WindMillSprite;
	public Sprite WallSprite;
	public Sprite Tree3Sprite;

	/* references */
	public Text Name;
	public Image Image;


	/* private variables */
	private ShopWindowScript.SubCategory _subCategory;

	public void SetSubCategory(ShopWindowScript.SubCategory subCategory){
		this._subCategory = subCategory;

		switch (this._subCategory) {
		case ShopWindowScript.SubCategory.BARRACK:
			this.Name.text = "BARRACK";
			this.Image.sprite = this.BarrackSprite;
			break;

		case ShopWindowScript.SubCategory.BOAT:
			this.Name.text = "BOAT";
			this.Image.sprite = this.BoatSprite;
			break;

		case ShopWindowScript.SubCategory.BUILDER_HUT:
			this.Name.text = "BUILDER HUT";
			this.Image.sprite = this.BuilderHutSprite;
			break;

		case ShopWindowScript.SubCategory.CAMP:
			this.Name.text = "CAMP";
			this.Image.sprite = this.CampSprite;
			break;

		case ShopWindowScript.SubCategory.CANNON:
			this.Name.text = "CANNON";
			this.Image.sprite = this.CannonSprite;
			break;

		case ShopWindowScript.SubCategory.ELIXIR_COLLECTOR:
			this.Name.text = "ELIXIR COLLECTOR";
			this.Image.sprite = this.ElixirCollectorSprite;
			break;

		case ShopWindowScript.SubCategory.ELIXIR_STORAGE:
			this.Name.text = "ELIXIR STORAGE";
			this.Image.sprite = this.ElixirStorageSprite;
			break;

		case ShopWindowScript.SubCategory.GEMS:
			this.Name.text = "GEMS";
			this.Image.sprite = this.GemsSprite;
			break;

		case ShopWindowScript.SubCategory.GOLD_MINE:
			this.Name.text = "GOLD MINE";
			this.Image.sprite = this.GoldMineSprite;
			break;

		case ShopWindowScript.SubCategory.GOLD_STORAGE:
			this.Name.text = "GOLD STORAGE";
			this.Image.sprite = this.GoldStorageSprite;
			break;

		case ShopWindowScript.SubCategory.TOWER:
			this.Name.text = "TOWER";
			this.Image.sprite = this.TowerSprite;
			break;

		case ShopWindowScript.SubCategory.TOWN_CENTER:
			this.Name.text = "TOWN CENTER";
			this.Image.sprite = this.TownCenterSprite;
			break;

		case ShopWindowScript.SubCategory.TREE1:
			this.Name.text = "TREE1";
			this.Image.sprite = this.Tree1Sprite;
			break;

		case ShopWindowScript.SubCategory.TREE2:
			this.Name.text = "TREE2";
			this.Image.sprite = this.Tree2Sprite;
			break;

		case ShopWindowScript.SubCategory.WINDMILL:
			this.Name.text = "WINDMILL";
			this.Image.sprite = this.WindMillSprite;
			break;
		
		case ShopWindowScript.SubCategory.WALL:
			this.Name.text = "WALL";
			this.Image.sprite = this.WallSprite;
			break;

		case ShopWindowScript.SubCategory.TREE3:
			this.Name.text = "TREE3";
			this.Image.sprite = this.Tree3Sprite;
			break;
		}
	}

	public void OnClick(){
		int itemId = 0;

		switch (this._subCategory) {
		case ShopWindowScript.SubCategory.BARRACK:
			itemId = 8833;
			break;
		case ShopWindowScript.SubCategory.BOAT:
			itemId = 6871;
			break;
		case ShopWindowScript.SubCategory.BUILDER_HUT:
			itemId = 3635;
			break;
		case ShopWindowScript.SubCategory.CAMP:
			itemId = 2728;
			break;
		case ShopWindowScript.SubCategory.CANNON:
			itemId = 1712;
			break;
		case ShopWindowScript.SubCategory.ELIXIR_COLLECTOR:
			itemId = 4856;
			break;
		case ShopWindowScript.SubCategory.ELIXIR_STORAGE:
			itemId = 2090;
			break;
		case ShopWindowScript.SubCategory.GEMS:
			itemId = 3336;
			break;
		case ShopWindowScript.SubCategory.GOLD_MINE:
			itemId = 3265;
			break;
		case ShopWindowScript.SubCategory.GOLD_STORAGE:
			itemId = 9074;
			break;
		case ShopWindowScript.SubCategory.TOWER:
			itemId = 4764;
			break;
		case ShopWindowScript.SubCategory.TOWN_CENTER:
			itemId = 2496;
			break;
		case ShopWindowScript.SubCategory.TREE1:
			itemId = 2949;
			break;
		case ShopWindowScript.SubCategory.TREE2:
			itemId = 1251;
			break;
		case ShopWindowScript.SubCategory.WINDMILL:
			itemId = 6677;
			break;
		case ShopWindowScript.SubCategory.WALL:
			itemId = 7666;
			break;
		case ShopWindowScript.SubCategory.TREE3:
			itemId = 5341;
			break;



		}

		//ItemsCollection.ItemData itemData = Items.GetItem (itemId);
		//Vector3 freePosition = GroundManager.instance.GetRandomFreePositionForItem (itemData.gridSize, itemData.gridSize);

		BaseItemScript item = SceneManager.instance.AddItem (itemId, false, true);
		//item.SetPosition (freePosition);
		if (item != null) {
			DataBaseManager.instance.UpdateItemData (item);
		}
        
		this.GetComponentInParent<ShopWindowScript> ().Close ();
	}


}
