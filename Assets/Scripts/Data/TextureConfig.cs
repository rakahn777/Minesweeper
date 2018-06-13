using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class TextureConfig : ScriptableObject
{
    [SerializeField] private Sprite[] cellTextures;
    [SerializeField] private Sprite groundTexture;
    [SerializeField] private Sprite flagTexture;
    [SerializeField] private Sprite deadTexture;
    [SerializeField] private Sprite mineTexture;

    public Sprite GetGroundTexture()
    {
        return groundTexture;
    }

    public Sprite GetTexture(int cellValue)
    {
        if (cellValue == Constants.MINE_VALUE)
            return mineTexture;
        if (cellValue == Constants.DEAD_VALUE)
            return deadTexture;
        if (cellValue == Constants.GROUND_VALUE)
            return groundTexture;
        if (cellValue == Constants.FLAG_VALUE)
            return flagTexture;

		if (cellValue >= 0 && cellValue < cellTextures.Length)
			return cellTextures[cellValue];
		
		return null;
    }
}
