using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopWindowScript : WindowScript {
	/* prefabs */
	public GameObject CategoryItem;
	public GameObject SubCategoryItem;

    /* references */
    public ScrollRect ScrollView;
	public GameObject ItemsList;
	public GameObject BackButton;

	public enum Category {
		ARMY,
		DEFENCE,
		RESOURCES,
		OTHER,
		TREASURE,
		DECORATIONS
	}

	public enum SubCategory{
		BARRACK,
		BOAT,
		BUILDER_HUT,
		CAMP,
		CANNON,
		ELIXIR_COLLECTOR,
		ELIXIR_STORAGE,
		GEMS,
		GOLD_MINE,
		GOLD_STORAGE,
		TOWER,
		TOWN_CENTER,
		TREE1,
		TREE2,
		TREE3,
		WINDMILL,
		WALL

	}


	void Awake(){
		this.Init ();
	}

	public void Init(){
		this.RenderCategories ();
	}

	public void RenderCategories(){
		this.BackButton.SetActive (false);

		this.ClearItemsList ();

		Category[] categories = new Category[] {
			Category.ARMY,
			Category.DECORATIONS,
			Category.DEFENCE,
			Category.OTHER,
			Category.RESOURCES,
			Category.TREASURE
		};

		for (int index = 0; index < categories.Length; index++) {
			GameObject inst = Utilities.CreateInstance (this.CategoryItem, this.ItemsList, true);
			inst.GetComponent<CategoryItemScript> ().SetCategory (categories [index]);
		}

        RectTransform rt = this.ItemsList.GetComponent<RectTransform>();
        Vector2 sizeDelta = this.ItemsList.GetComponent<RectTransform>().sizeDelta;
        sizeDelta.x = categories.Length * 250 + categories.Length * this.ItemsList.GetComponent<GridLayoutGroup>().spacing.x;
        rt.sizeDelta = sizeDelta;

        this.ResetScrollPosition();
	}

	public void RenderSubCategories(Category category){
		this.BackButton.SetActive (true);

		this.ClearItemsList ();

		SubCategory[] subItems = new SubCategory[0];

		switch (category) {
		case Category.ARMY:
			subItems = new SubCategory[]{ SubCategory.BARRACK, SubCategory.CAMP, SubCategory.BOAT};
			break;
		case Category.DECORATIONS:
			subItems = new SubCategory[]{ SubCategory.TREE1, SubCategory.TREE2, SubCategory.TREE3};
			break;
		case Category.DEFENCE:
			subItems = new SubCategory[]{ SubCategory.CANNON, SubCategory.TOWER};
			break;
		case Category.OTHER:
			subItems = new SubCategory[]{ SubCategory.TOWN_CENTER, SubCategory.BUILDER_HUT, SubCategory.WALL};
			break;
		case Category.RESOURCES:
			subItems = new SubCategory[]{ SubCategory.ELIXIR_COLLECTOR, SubCategory.ELIXIR_STORAGE, SubCategory.GOLD_MINE, SubCategory.GOLD_STORAGE, SubCategory.WINDMILL};
			break;
		case Category.TREASURE:
			subItems = new SubCategory[]{ SubCategory.GEMS};
			break;
		}

		for(int index=0; index<subItems.Length; index++){
			GameObject inst = Utilities.CreateInstance (this.SubCategoryItem, this.ItemsList, true);
			inst.GetComponent<SubCategoryItemScript> ().SetSubCategory (subItems [index]);
		}

        RectTransform rt = this.ItemsList.GetComponent<RectTransform>();
        Vector2 sizeDelta = this.ItemsList.GetComponent<RectTransform>().sizeDelta;
        sizeDelta.x = subItems.Length*250 + subItems.Length * this.ItemsList.GetComponent<GridLayoutGroup>().spacing.x;
        rt.sizeDelta = sizeDelta;

        this.ResetScrollPosition();
	}
		
	public void OnClickCategory(Category category){
		this.RenderSubCategories (category);
	}

	public void ClearItemsList(){
		foreach (Transform child in this.ItemsList.transform) {
			Destroy (child.gameObject);
		}
	}

	public void OnClickBackButton(){
		this.RenderCategories ();
	}

    public void ResetScrollPosition(){
        this.ScrollView.horizontalNormalizedPosition = 0.0f;
    }

}
