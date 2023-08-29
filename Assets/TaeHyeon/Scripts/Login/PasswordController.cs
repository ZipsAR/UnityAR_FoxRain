using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Logger = ZipsAR.Logger;

public class PasswordController : MonoBehaviour
{
    [SerializeField] private List<PasswordDot> passwordDots;
    [SerializeField] private GameObject dotPrefab;
    [SerializeField] private Material defaultMaterial;
    [SerializeField] private Material selectedMaterial;
    
    private LineRenderer lineRenderer;
    private List<int> selectedPasswordIndices;
    private int validateFingerId;
    
    private float xInterval;
    private float yInterval;
    
    private void Awake()
    {
        xInterval = 0.2f;
        yInterval = 0.2f;
        validateFingerId = -1;
        
        lineRenderer = GetComponent<LineRenderer>();
        selectedPasswordIndices = new List<int>();
        
        int passwordNum = 1;
        
        // Create password dots
        for(int yIdx = 1; yIdx >= -1; yIdx--)
        {
            for (int xIdx = -1; xIdx <= 1; xIdx++)
            {
                GameObject dot = Instantiate(dotPrefab, transform);
                dot.transform.localPosition = new Vector3(xInterval * xIdx, yInterval * yIdx, 0f);
                dot.GetComponent<PasswordDot>().Init(this, passwordNum++, defaultMaterial, selectedMaterial);
                passwordDots.Add(dot.GetComponent<PasswordDot>());
            }
        }
    }

    private void Update()
    {
        if (IsAllPointsNotSelected())
        {
            lineRenderer.positionCount = 0;
            return;
        }
        
        List<Vector3> selectedPoints = new List<Vector3>();
        foreach (var dotIdx in selectedPasswordIndices)
        {
            selectedPoints.Add(passwordDots[dotIdx].transform.position);
        }

        lineRenderer.positionCount = selectedPoints.Count;
        lineRenderer.SetPositions(selectedPoints.ToArray());
    }
    
    private bool IsAllPointsNotSelected()
    {
        foreach(var dot in passwordDots)
        {
            if (dot.isSelected)
                return false;
        }

        return true;
    }
    
    public void AddSelectedPasswordIndex(int num)
    {
        selectedPasswordIndices.Add(num - 1);
    }

    public void NotifyFingerIdSelected(int fingerId)
    {
        // If first dot is selected
        if (IsAllPointsNotSelected())
        {
            validateFingerId = fingerId;
            
            foreach (var dot in passwordDots)
            {
                dot.SetActiveFingerId(validateFingerId);
            }
        }
    }
    
    public void ResetPassword()
    {
        validateFingerId = -1;
        foreach (var dot in passwordDots)
        {
            dot.UnSelected();
        }
        selectedPasswordIndices.Clear();
    }
    
    public string GetPassword()
    {
        string password = "";
        foreach (var dotIdx in selectedPasswordIndices)
        {
            password += dotIdx + 1;
        }

        return password;
    }
}
