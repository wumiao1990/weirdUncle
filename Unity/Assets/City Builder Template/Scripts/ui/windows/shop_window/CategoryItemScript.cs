using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CategoryItemScript : MonoBehaviour {
	/* prefabs */
	public Sprite ArmySprite;
	public Sprite DecorationsSprite;
	public Sprite DefenceSprite;
	public Sprite OtherSprite;
	public Sprite ResourcesSprite;
	public Sprite TreasureSprite;
	

	/* references */
	public Text Name;
	public Image Image;


	/* private variables */
	private ShopWindowScript.Category _category;

	public void SetCategory(ShopWindowScript.Category category){
		this._category = category;

		switch (this._category) {
		case ShopWindowScript.Category.ARMY:
			//this.Name.text = "ARMY";
			this.Image.sprite = this.ArmySprite;
			break;
		case ShopWindowScript.Category.DEFENCE:
			//this.Name.text = "DEFENCE";
			this.Image.sprite = this.DefenceSprite;
			break;
		case ShopWindowScript.Category.OTHER:
			//this.Name.text = "OTHER";
			this.Image.sprite = this.OtherSprite;
			break;
		case ShopWindowScript.Category.RESOURCES:
			//this.Name.text = "RESOURCES";
			this.Image.sprite = this.ResourcesSprite;
			break;
		case ShopWindowScript.Category.TREASURE:
			//this.Name.text = "TREASURE";
			this.Image.sprite = this.TreasureSprite;
			break;
		case ShopWindowScript.Category.DECORATIONS:
			//this.Name.text = "DECORATIONS";
			this.Image.sprite = this.DecorationsSprite;
			break;
		}
	}

	public void OnClick(){
		//this.GetComponentInParent<ShopWindowScript> ().OnClickCategory (this._category);
		
		int itemId = 0;

		switch (this._category) {
			case ShopWindowScript.Category.ARMY:
				itemId = 3823;
				break;
			case ShopWindowScript.Category.DECORATIONS:
				itemId = 8833;
				break;
			case ShopWindowScript.Category.DEFENCE:
				itemId = 6871;
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
