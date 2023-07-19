using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Debug = UnityEngine.Debug;

public class BuildingSystem : MonoBehaviour
{
    public string buildTag = "Build"; // Tag para identificar os objetos de construção
    public GameObject[] blockPrefabs; // Vetor de prefabs dos blocos de construção
    public Transform raycastOriginTransform; // Ponto de referência para origem do raio
    public Camera playerCamera;
    public float raycastDistance = 10f;
    public int selectedBlockIndex = 0; // Índice do bloco selecionado
    public GameObject blockPreviewInstance; // Instância do bloco de visualização
    public int gridSize = 10; // Tamanho da grade (número de células em cada direção)
    public float cellSize = 1.0f; // Tamanho de cada célula do grid
    private Quaternion rotation; // Rotação do bloco
    private bool rotateBlock = false; // Flag para indicar se deve rotacionar o bloco

    public Material previewMaterial; // Material para o bloco de visualização
    public bool isActive = true;
    private void Start()
    {
        // Instancia o bloco de visualização como filho do jogador e o ativa
        blockPreviewInstance = Instantiate(blockPrefabs[selectedBlockIndex], transform.position, Quaternion.identity, transform);
        blockPreviewInstance.SetActive(true); // Ativa o bloco de visualização no início
        rotation = Quaternion.identity; // Inicializa a rotação como identidade (sem rotação)

        // Muda o material do bloco de visualização para verde
        MeshRenderer renderer = blockPreviewInstance.GetComponent<MeshRenderer>();
        if (renderer != null)
        {
            renderer.material = previewMaterial;
        }
    }

    private void Update()
    {
        if (isActive)
        {
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                if (raycastOriginTransform == null || playerCamera == null)
                {
                    Debug.LogError("Raycast Origin Transform or Player Camera not assigned in RaycastBuilder script!");
                    return;
                }

                Vector3 raycastOrigin = raycastOriginTransform.position;
                Vector3 raycastDirection = playerCamera.transform.forward;
                Ray ray = new Ray(raycastOrigin, raycastDirection);

                RaycastHit hit;
                // Ignora a tag "Player" ao verificar a colisão
                if (Physics.Raycast(ray, out hit, raycastDistance) && !hit.collider.CompareTag("Player"))
                {
                    // Obtém o ponto de colisão acima do bloco atingido
                    Vector3 blockPosition = hit.point + Vector3.up * blockPrefabs[selectedBlockIndex].transform.localScale.y / 2f;
                    GameObject newBlock = Instantiate(blockPrefabs[selectedBlockIndex], blockPosition, rotation);
                    newBlock.SetActive(true); // Ativa o novo bloco que foi instanciado
                }

            }
            // Atualiza a posição do bloco de visualização
            UpdateBlockPreviewPosition();

            // Verifica se a tecla "R" foi pressionada e define a flag para rotacionar o bloco
            if (Keyboard.current.rKey.wasPressedThisFrame)
            {
                rotateBlock = true;
            }

            // Rotaciona o bloco apenas se a flag estiver ativa
            if (rotateBlock)
            {
                rotation *= Quaternion.Euler(0f, 0f, 90f); // Rotaciona o bloco 90 graus no eixo R
                blockPreviewInstance.transform.rotation = rotation;
                rotateBlock = false; // Desativa a flag para evitar rotação contínua
            }
        }

        // Verifica se a tecla "Y" foi pressionada para ativar/desativar o BuildingSystem
        if (Keyboard.current.yKey.wasPressedThisFrame)
        {
            isActive = !isActive;
            // Ativa o bloco de visualização apenas se o BuildingSystem estiver ativo
            blockPreviewInstance.SetActive(isActive);
            // Atualiza o material do bloco de visualização somente quando o BuildingSystem estiver ativo
            if (isActive)
            {
                UpdateBlockPreviewAppearance(previewMaterial); // Material verde quando ativado
            }
            else
            {
                UpdateBlockPreviewAppearance(previewMaterial); // Material vermelho quando desativado
            }
        }
    }
    private void UpdateBlockPreviewAppearance(Material material)
    {
        MeshRenderer renderer = blockPreviewInstance.GetComponent<MeshRenderer>();
        if (renderer != null)
        {
            renderer.material = material;
        }
    }

    private void UpdateBlockPreviewPosition()
    {
        if (blockPreviewInstance != null && raycastOriginTransform != null && playerCamera != null)
        {
            Vector3 raycastOrigin = raycastOriginTransform.position;
            Vector3 raycastDirection = playerCamera.transform.forward;
            Ray ray = new Ray(raycastOrigin, raycastDirection);

            RaycastHit hit;
            // Ignora a tag "Player" ao verificar a colisão
            if (Physics.Raycast(ray, out hit, raycastDistance) && !hit.collider.CompareTag("Player"))
            {
                // Encontra a posição do bloco na grade mais próxima
                Vector3 blockPosition = hit.point;
                blockPosition.x = Mathf.Floor(blockPosition.x / cellSize) * cellSize + cellSize / 2f;
                blockPosition.y = Mathf.Floor(blockPosition.y / cellSize) * cellSize + cellSize / 2f;
                blockPosition.z = Mathf.Floor(blockPosition.z / cellSize) * cellSize + cellSize / 2f;

                // Define a posição do bloco de visualização para a posição central da célula da grade
                blockPreviewInstance.SetActive(true);
                blockPreviewInstance.transform.position = blockPosition;

                // Faz o bloco de visualização ter a mesma rotação da câmera
                blockPreviewInstance.transform.rotation = Quaternion.Euler(0f, playerCamera.transform.eulerAngles.y, 0f);

                // Altera o material do bloco de visualização para verde
                MeshRenderer renderer = blockPreviewInstance.GetComponent<MeshRenderer>();
                if (renderer != null)
                {
                    renderer.material.color = Color.green;
                }
            }
            else
            {
                blockPreviewInstance.SetActive(false);
            }
        }
    }

    public void SelectBlock(int blockIndex)
    {
        if (blockIndex >= 0 && blockIndex < blockPrefabs.Length)
        {
            selectedBlockIndex = blockIndex;
            // Atualiza o bloco de visualização com o novo bloco selecionado
            Destroy(blockPreviewInstance);
            
            blockPreviewInstance = Instantiate(blockPrefabs[selectedBlockIndex], transform.position, Quaternion.identity, transform);
            blockPreviewInstance.SetActive(true);

            // Encontra a posição do bloco na grade mais próxima
            Vector3 blockPosition = transform.position;
            blockPosition.x = Mathf.Floor(blockPosition.x / cellSize) * cellSize + cellSize / 2f;
            blockPosition.y = Mathf.Floor(blockPosition.y / cellSize) * cellSize + cellSize / 2f;
            blockPosition.z = Mathf.Floor(blockPosition.z / cellSize) * cellSize + cellSize / 2f;

            // Define a posição do bloco de visualização para a posição central da célula da grade
            blockPreviewInstance.transform.position = blockPosition;

            // Faz o bloco de visualização ter a mesma rotação da câmera
            blockPreviewInstance.transform.rotation = Quaternion.Euler(0f, playerCamera.transform.eulerAngles.y, 0f);

            // Muda o material do bloco de visualização para verde
            MeshRenderer renderer = blockPreviewInstance.GetComponent<MeshRenderer>();
            if (renderer != null)
            {
                renderer.material = previewMaterial;
            }
        }
    }

    // Método para mudar o material do bloco de visualização
    private void ChangeMaterial(Material material)
    {
        MeshRenderer renderer = blockPreviewInstance.GetComponent<MeshRenderer>();
        if (renderer != null)
        {
            renderer.material = material;
        }
    }
    public void ToggleBuildingSystem(bool? setActive = null)
    {
        if (setActive.HasValue)
        {
            isActive = setActive.Value;
        }
        else
        {
            isActive = !isActive;
        }

        // Ativa o bloco de visualização apenas se o BuildingSystem estiver ativo
        blockPreviewInstance.SetActive(isActive);
        // Atualiza o material do bloco de visualização somente quando o BuildingSystem estiver ativo
        
        
    }
    private void OnDrawGizmos()
    {
        // O resto do código permanece o mesmo
        if (raycastOriginTransform != null && playerCamera != null)
        {
            Vector3 raycastOrigin = raycastOriginTransform.position;
            Vector3 raycastDirection = playerCamera.transform.forward;

            Gizmos.color = Color.blue;
            Gizmos.DrawLine(raycastOrigin, raycastOrigin + raycastDirection * raycastDistance);
        }
    }
}
