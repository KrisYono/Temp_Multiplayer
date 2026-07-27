using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class MaterialManager : NetworkBehaviour
{
    public Renderer headRenderer;
    public Renderer topRenderer;
    public Renderer bottomRenderer;
    public Renderer shoesRenderer;

    public Material[] headMaterials;
    public Material[] topMaterials;
    public Material[] bottomMaterials;
    public Material[] shoesMaterials;

    // UI Dropdown references
    private Dropdown headDropdown;
    private Dropdown topDropdown;
    private Dropdown bottomDropdown;
    private Dropdown shoesDropdown;

    private void Start()
    {
        if (IsOwner)
        {
            // 通过名称查找 UI 元素（确保这些名称与场景中的 UI 元素名称一致）
            headDropdown = GameObject.Find("Change_Hat").GetComponent<Dropdown>();
            topDropdown = GameObject.Find("Change_Top").GetComponent<Dropdown>();
            bottomDropdown = GameObject.Find("Change_Bottom").GetComponent<Dropdown>();
            shoesDropdown = GameObject.Find("Change_Shoes").GetComponent<Dropdown>();

            // 为下拉菜单添加事件监听
            headDropdown.onValueChanged.AddListener(delegate { ChangeMaterial(0, headDropdown.value); });
            topDropdown.onValueChanged.AddListener(delegate { ChangeMaterial(1, topDropdown.value); });
            bottomDropdown.onValueChanged.AddListener(delegate { ChangeMaterial(2, bottomDropdown.value); });
            shoesDropdown.onValueChanged.AddListener(delegate { ChangeMaterial(3, shoesDropdown.value); });
        }
    }

    // 更改材质
    public void ChangeMaterial(int partIndex, int materialIndex)
    {
        if (IsOwner)
        {
            UpdateMaterialServerRpc(partIndex, materialIndex);
        }
    }

    [ServerRpc]
    private void UpdateMaterialServerRpc(int partIndex, int materialIndex)
    {
        UpdateMaterialClientRpc(partIndex, materialIndex);
    }

    [ClientRpc]
    private void UpdateMaterialClientRpc(int partIndex, int materialIndex)
    {
        switch (partIndex)
        {
            case 0:
                headRenderer.material = headMaterials[materialIndex];
                break;
            case 1:
                topRenderer.material = topMaterials[materialIndex];
                break;
            case 2:
                bottomRenderer.material = bottomMaterials[materialIndex];
                break;
            case 3:
                shoesRenderer.material = shoesMaterials[materialIndex];
                break;
        }
    }
}