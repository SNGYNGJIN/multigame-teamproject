using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;
using Photon.Pun;

public class Ingredient : MonoBehaviourPunCallbacks
{
  public IngredientData ingredientData;

    [SerializeField]
    public string ingredientName { get; set; }
    [SerializeField]
    public int Price { get; set; }
    [SerializeField]
    public IngredientType Type { get; set; }
    [SerializeField]
    public GameObject PrepIngredient { get; set; }

    public void Start()
  {
    ingredientName = ingredientData.IngredientName;
    Price = ingredientData.Price;
    Type = ingredientData.IngredientType;
    PrepIngredient = ingredientData.PrepIngredient;
  }


  public void ingredientInfo()
  {
    Debug.Log("재료 이름 :: " + ingredientData.name);
    Debug.Log("재료 가격 :: " + ingredientData.Price);
    Debug.Log("재료 종류 :: " + ingredientData.IngredientType);
  }


    [PunRPC]
    void UpdateInteractingObjectPosition(Vector3 position, int id)
    {
        Debug.Log("찾아보자");
        PhotonView targetView = PhotonView.Find(id);
        Debug.Log("찾는거실행했음");
        if (targetView != null)
        {
            Debug.Log("찾았음");
            GameObject targetObject = targetView.gameObject;
            targetView.RequestOwnership();
            targetObject.transform.localPosition = position;
            Debug.Log(targetObject.name);
        }

        Debug.Log("끝");
    }


}
