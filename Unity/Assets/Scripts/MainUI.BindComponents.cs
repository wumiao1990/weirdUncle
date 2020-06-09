using UnityEngine;
using UnityEngine.UI;

//自动生成于：6/9/2020 6:15:14 PM
	public partial class MainUI
	{

		private Image m_ImgTest1;
		private Button m_BtnTest2;
		private Text m_TxtTest3;
		private Dropdown m_DropTest4;
		private Image m_ImgTest4;

		private void GetBindComponents(GameObject go)
		{
			ComponentAutoBindTool autoBindTool = go.GetComponent<ComponentAutoBindTool>();

			m_ImgTest1 = autoBindTool.GetBindComponent<Image>(0);
			m_BtnTest2 = autoBindTool.GetBindComponent<Button>(1);
			m_TxtTest3 = autoBindTool.GetBindComponent<Text>(2);
			m_DropTest4 = autoBindTool.GetBindComponent<Dropdown>(3);
			m_ImgTest4 = autoBindTool.GetBindComponent<Image>(4);
		}
	}
