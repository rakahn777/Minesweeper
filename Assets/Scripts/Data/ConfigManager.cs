using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConfigManager : MonoBehaviour
{
    public TextureConfig textureConfig;
	public BoardConfig boardConfig;

    private static ConfigManager instance;

    public static ConfigManager Instance
    {
        get
        {
            return instance;
        }
    }

	void Awake()
	{
		if (instance != null)
			Destroy (gameObject);
		else
			instance = this;
	}
}
