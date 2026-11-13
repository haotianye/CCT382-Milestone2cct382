
using UnityEngine;

[CreateAssetMenu(
    fileName = "NewFloatVariable",
    menuName = "CCT382/Float Variable"
)]
public class FloatVariableSO : ScriptableObject
{
    public float value = 0f;

    public void SetValue(float newValue)
    {
        value = newValue;
    }
}
