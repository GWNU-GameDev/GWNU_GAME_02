using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageManager : MonoBehaviour
{
    public Image panelImage; // 패널에 있는 Image 컴포넌트
    public List<Sprite> imageList; // 이미지 목록
    public float changeInterval = 2.0f; // 이미지 변경 간격 (초 단위)

    private List<Sprite> unusedImages; // 사용하지 않는 이미지 목록

    void Start()
    {
        if (imageList.Count > 0)
        {
            // 초기화
            unusedImages = new List<Sprite>(imageList);

            // 패널 이미지 활성화
            panelImage.gameObject.SetActive(true);

            // 코루틴 시작
            StartCoroutine(ChangeImage());
        }
        else
        {
            // 이미지 목록이 비어있을 때 패널을 비활성화
            panelImage.gameObject.SetActive(false);
            Debug.LogWarning("Image list is empty. No images to display.");
        }
    }

    IEnumerator ChangeImage()
    {
        while (true)
        {
            if (unusedImages.Count == 0)
            {
                // 모든 이미지를 사용한 경우
                Debug.Log("All images have been shown. Resetting the image list.");

                // 사용하지 않은 이미지 목록을 재설정
                unusedImages = new List<Sprite>(imageList);

                // 이미지가 없을 때는 패널을 비활성화
                if (unusedImages.Count == 0)
                {
                    panelImage.gameObject.SetActive(false);
                    yield break; // 코루틴 종료
                }
            }

            // 랜덤으로 다음 이미지를 선택
            int randomIndex = Random.Range(0, unusedImages.Count);
            Sprite newImage = unusedImages[randomIndex];

            // 이미지 변경
            panelImage.sprite = newImage;

            // 사용한 이미지를 목록에서 제거
            unusedImages.RemoveAt(randomIndex);

            // 변경 간격 동안 대기
            yield return new WaitForSeconds(changeInterval);
        }
    }
}
