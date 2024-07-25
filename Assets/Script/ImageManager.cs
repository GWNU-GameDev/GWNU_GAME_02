using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageManager : MonoBehaviour
{
    public Image panelImage; // 패널에 있는 Image 컴포넌트
    public List<Sprite> imageList; // 이미지 목록
    public float changeInterval = 2.0f; // 이미지 변경 간격 (초 단위)

    private int currentImageIndex = 0;

    void Start()
    {
        if (imageList.Count > 0)
        {
            StartCoroutine(ChangeImage());
        }
        else
        {
            Debug.LogWarning("Image list is empty.");
        }
    }

    IEnumerator ChangeImage()
    {
        while (true)
        {
            // 이미지 변경
            panelImage.sprite = imageList[currentImageIndex];

            // 다음 이미지를 가리키도록 인덱스를 업데이트
            currentImageIndex = (currentImageIndex + 1) % imageList.Count;

            // 변경 간격 동안 대기
            yield return new WaitForSeconds(changeInterval);
        }
    }
}