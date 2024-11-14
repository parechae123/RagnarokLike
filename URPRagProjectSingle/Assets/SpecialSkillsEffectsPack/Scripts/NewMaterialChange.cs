using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewMaterialChange : MonoBehaviour
{
    // 파티클 시스템인지 여부를 지정하는 플래그
    public bool isParticleSystem;
    // 입력할 머티리얼 (대체할 머티리얼)
    public Material m_inputMaterial;
    // 실제로 사용할 오브젝트의 머티리얼
    Material m_objectMaterial;
    // MeshRenderer와 ParticleSystemRenderer 변수 선언
    MeshRenderer m_meshRenderer;
    ParticleSystemRenderer m_particleRenderer;
    // 시간 경과에 따른 감소를 위한 변수들
    public float m_timeToReduce;    // 감소가 시작되는 시간
    public float m_reduceFactor = 0.0f;    // 감소 비율
    float m_time;    // 경과 시간 추적
    float m_submitReduceFactor;    // 감소에 적용될 현재 감소 비율
    float m_cutOutFactor;    // 컷 아웃 팩터
    // 증가율 설정 변수
    public float m_upFactor;
    float upFactor = 1;
    bool isupfactor = true;

    // 초기화: 적절한 머티리얼 설정 및 변수 초기화
    void Awake()
    {
        if (isParticleSystem)
        {
            // 파티클 시스템이 활성화된 경우 ParticleSystemRenderer를 가져와 머티리얼 적용
            m_particleRenderer = gameObject.GetComponent<ParticleSystemRenderer>();
            m_particleRenderer.material = m_inputMaterial;
            m_objectMaterial = m_particleRenderer.material;
        }
        else
        {
            // 파티클 시스템이 아닌 경우 MeshRenderer를 가져와 머티리얼 적용
            m_meshRenderer = gameObject.GetComponent<MeshRenderer>();
            m_meshRenderer.material = m_inputMaterial;
            m_objectMaterial = m_meshRenderer.material;
        }
        // 감소율 및 컷아웃 팩터 초기화
        m_submitReduceFactor = 0.0f;
        m_cutOutFactor = 1.0f;
    }

    // LateUpdate: 매 프레임마다 머티리얼의 컷아웃 값 및 증가율을 업데이트
    void LateUpdate()
    {
        // 경과 시간 증가
        m_time += Time.deltaTime;

        // 설정된 감소 시간이 지난 경우 컷 아웃 팩터 감소
        if (m_time > m_timeToReduce)
        {
            // 컷 아웃 팩터 감소 및 감소 비율 점진적으로 증가
            m_cutOutFactor -= m_submitReduceFactor;
            m_submitReduceFactor = Mathf.Lerp(m_submitReduceFactor, m_reduceFactor, Time.deltaTime / 50);
        }

        // 컷 아웃 팩터의 범위를 0과 1 사이로 제한
        m_cutOutFactor = Mathf.Clamp01(m_cutOutFactor);
        // 컷 아웃 팩터가 0이 되고 감소 시간이 지났으면 비활성화
        if (m_cutOutFactor <= 0 && m_time > m_timeToReduce)
        {
            m_time = 0;
            gameObject.SetActive(false);
        }

        // 컷 아웃 팩터 값을 쉐이더에 적용
        m_objectMaterial.SetFloat("_MaskCutOut", m_cutOutFactor);

        // 증가율이 설정되어 있고, 증가가 완료되지 않았다면 upFactor 증가
        if (m_upFactor != 0 && isupfactor != false)
        {
            upFactor += m_upFactor * Time.deltaTime;
            upFactor = Mathf.Clamp01(upFactor);
            // 증가된 컷 아웃 팩터 값을 쉐이더에 적용
            m_objectMaterial.SetFloat("_MaskCutOut", upFactor);
            if (upFactor >= 1)
                isupfactor = false; // 증가가 완료되면 더 이상 증가하지 않도록 설정
        }
    }

    // OnDisable: 비활성화될 때 모든 변수를 초기화
    private void OnDisable()
    {
        m_time = 0f;
        upFactor = 1f;
        m_upFactor = -1f;
        m_cutOutFactor = 1.0f;
        m_objectMaterial.SetFloat("_MaskCutOut", upFactor);
        m_objectMaterial.SetFloat("_MaskCutOut", m_cutOutFactor);
        isupfactor = true;
        m_reduceFactor = 1f;
    }
}
