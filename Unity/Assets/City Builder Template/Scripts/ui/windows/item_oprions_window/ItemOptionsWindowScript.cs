using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemOptionsWindowScript : WindowScript {

	public static ItemOptionsWindowScript instance;

	/* object references */
	public GameObject InfoButton;
	public GameObject UpgradeButton;
	public GameObject TrainButton;
	public GameObject RemoveButton;

	private void Awake()
	{
		if(SceneManager.instance == null)
		{
			return;
		}

		instance = this;
		this.ShowOptions();
	}
    
	public void ShowOptions()
	{
		this.StartCoroutine(this._ShowOptions());
	}

	private float _waitTime = 0.08f;
	bool haveInfoButton = true;
    bool haveUpgradeButton = true;
	bool haveTrainButton = false;
    bool haveRemoveButton = true;

	private IEnumerator _ShowOptions()
	{
		BaseItemScript selectedItem = SceneManager.instance.selectedItem;
        if (selectedItem.itemData.name == "Barrack")
            haveTrainButton = true;

		haveInfoButton = true;
		haveUpgradeButton = true;
		haveRemoveButton = true;

		InfoButton.SetActive(haveInfoButton);
		UpgradeButton.SetActive(haveUpgradeButton);
		TrainButton.SetActive(haveTrainButton);
		RemoveButton.SetActive(haveRemoveButton);

		if (haveInfoButton)
		{
			RemoveButton.GetComponent<Animator>().SetTrigger("show");
			yield return new WaitForSeconds(_waitTime);
		}

		if (haveTrainButton)
        {
            TrainButton.GetComponent<Animator>().SetTrigger("show");
            yield return new WaitForSeconds(_waitTime);
        }

		if (haveUpgradeButton)
		{
			UpgradeButton.GetComponent<Animator>().SetTrigger("show");
			yield return new WaitForSeconds(_waitTime);
		}
      
		if (haveRemoveButton)
		{
			InfoButton.GetComponent<Animator>().SetTrigger("show");
		}
	}

	public void HideOptions()
    {
		this.StartCoroutine(this._HideOptions());
    }

	private IEnumerator _HideOptions()
    {

		if (haveInfoButton)
		{
			InfoButton.GetComponent<Animator>().SetTrigger("hide");
			yield return new WaitForSeconds(_waitTime);
		}

		if (haveUpgradeButton)
		{
			UpgradeButton.GetComponent<Animator>().SetTrigger("hide");
			yield return new WaitForSeconds(_waitTime);
		}

		if (haveTrainButton)
        {
			TrainButton.GetComponent<Animator>().SetTrigger("hide");
            yield return new WaitForSeconds(_waitTime);
        }

		if (haveRemoveButton)
		{
			RemoveButton.GetComponent<Animator>().SetTrigger("hide");
			yield return new WaitForSeconds(_waitTime);
		}

		base.Close();
    }

	public void OnClickInfoButton()
    {
		UIManager.instance.ShowInfoWindow();
    }

	public void OnClickUpgradeButton()
    {

    }

	public void OnClickTrainButton()
    {
		UIManager.instance.ShowTrainTroopsWindow();
    }

	public void OnClickRemoveButton()
	{
		UIManager.instance.HideItemOptions();
		DataBaseManager.instance.RemoveItem(SceneManager.instance.selectedItem);
		SceneManager.instance.RemoveItem(SceneManager.instance.selectedItem);
	}

	public override void Close()
	{
		HideOptions();
	}
}
