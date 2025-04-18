using UnityEngine;
using UnityEngine.UI;

public class LevelHUDController : MonoBehaviour
{
    [Header("HUD")]
    public Image progressOfLevel;
    public RectTransform zombieHead;
    public GameObject flagPrefab; // Prefab do GameObject que representa a wave

    private void Start()
    {
        SetWaveMarkers();
    }

    public void UpdateHUD()
    {
        float progress = ZombiesManager.instance.totalZombiesSpawned /ZombiesManager.instance.totalZombiesInLevel;
        progressOfLevel.fillAmount = progress;

        UpdateObjectPosition(progress);
    }

    void UpdateObjectPosition(float progress)
    {
        // Posição inicial e final da barra em coordenadas mundiais
        Vector3[] corners = new Vector3[4];
        progressOfLevel.rectTransform.GetWorldCorners(corners);

        Vector3 startPos = corners[0]; // canto inferior esquerdo
        Vector3 endPos = corners[3]; // canto superior direito

        // Calcula a altura (diferença no eixo Y) da barra
        float heightOffset = (corners[1].y - corners[0].y) / 2f;

        // Ajuste a posição no eixo Y para centralizar o objeto
        startPos.y += heightOffset;
        endPos.y += heightOffset;

        // Calcule a nova posição do objeto ao longo da barra
        zombieHead.position = Vector3.Lerp(startPos, endPos, progress);
    }

    public void SetWaveMarkers()
    {
        //Para cada wave (ou seja cada flag)
        foreach (var wave in ZombiesManager.WavesInLevel.Keys)
        {
            //Instancia a flag dentro do progressLevel
            GameObject waveObject = Instantiate(flagPrefab, progressOfLevel.transform);
            Flag flag = waveObject.GetComponent<Flag>();
            flag.SetWaveOfFlag(wave);
            RectTransform waveObjectRect = waveObject.GetComponent<RectTransform>();

            float zombiesBeforeWave = 0;

            //Para cada zumbi de spawn normal
            foreach (var regularSpawn in GameController.LevelData.regularSpawns)
            {
                //Se o zumbi for spawnar antes dessa wave começar
                if (regularSpawn.spawnTime <= wave.hordeTime)
                {
                    zombiesBeforeWave++;
                }
            }

            //Para cada outra wave que ocorra antes dessa
            foreach (var previousWave in ZombiesManager.WavesInLevel.Keys)
            {
                if (previousWave.hordeTime < wave.hordeTime && previousWave != wave)
                {
                    zombiesBeforeWave += previousWave.zombiesInHorde.Count;
                }
            }

            float wavePosition = zombiesBeforeWave / ZombiesManager.instance.totalZombiesInLevel;
            float markerX = wavePosition * progressOfLevel.rectTransform.rect.width;
            waveObjectRect.anchoredPosition = new Vector2(markerX, waveObjectRect.anchoredPosition.y);
        }
    }
}