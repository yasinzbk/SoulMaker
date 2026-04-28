using UnityEngine;
using UnityEngine.SceneManagement; // Sahne geçişleri için gerekli

public class MainMenu : MonoBehaviour
{
    // Oyunu başlatmak için kullanılacak fonksiyon
    public void PlayGame()
    {
        // "1" indexli sahneyi yükler. 
        // Build Settings'ten oyun sahnesinin index'ini kontrol etmelisin.
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    // Oyundan çıkmak için kullanılacak fonksiyon
    public void QuitGame()
    {
        Debug.Log("Oyundan çıkıldı!"); // Editörde çalıştığını anlamak için
        Application.Quit(); // Derlenmiş oyunda uygulamayı kapatır
    }

    public void ReturnToMainMenu()
    {
        // Genelde ana menü Build Settings'te 0. sıradadır.
        SceneManager.LoadScene(0);
    }
}