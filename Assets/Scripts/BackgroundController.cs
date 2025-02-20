using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundController : MonoBehaviour
{
    public List<MoveBackground> moveBackgrounds = new List<MoveBackground>();
    
    private List<float> originalSpeeds = new List<float>();

    // Start is called before the first frame update
    void Start()
    {
        foreach(var background in moveBackgrounds)
        {
            originalSpeeds.Add(background.speed);
            background.speed = 0;
        }
    }

    // Update is called once per frame
    void Update()
    {
        var moveX = Input.GetAxis("Horizontal");

        foreach(var background in moveBackgrounds)
        {
            background.speed = originalSpeeds[moveBackgrounds.IndexOf(background)] * moveX;
        }
    }
}
