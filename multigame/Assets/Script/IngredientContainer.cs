using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngredientContainer : MonoBehaviour
{
    public enum Type { StapleFood, Meat, Vegetable };
    public Type ingredientType;
    public Ingredient[] ingredients;
    public int[] counts;
}