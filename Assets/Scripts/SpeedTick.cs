using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SpeedTick : MonoBehaviour
{
    [SerializeField] private Image panelImage;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        panelImage.color = new Color(Random.Range(0, 255), Random.Range(0, 255), Random.Range(0, 255));
    }
}
