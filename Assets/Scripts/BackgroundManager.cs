// using UnityEngine;

// public class BackgroundManager : MonoBehaviour
// {
//     [SerializeField] private float baseScrollSpeed = 5f;
//     [SerializeField] private ParallaxEffect[] parallaxLayers;
    
//     // Kann von anderen Skripten aufgerufen werden
//     public void SetScrollSpeed(float newSpeed)
//     {
//         baseScrollSpeed = newSpeed;
        
//         // Alle Parallax-Ebenen aktualisieren
//         foreach (var layer in parallaxLayers)
//         {
//             layer.SetSpeed(baseScrollSpeed);
//         }
//     }
    
//     // Füge diese Methode zum ParallaxEffect-Skript hinzu
//     public void SetSpeed(float newSpeed)
//     {
//         scrollSpeed = newSpeed;
//     }
// }
