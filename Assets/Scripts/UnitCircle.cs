using System;
using TMPro;
using UnityEngine;


public class UnitCircle : MonoBehaviour
{
    public int segments = 100; // Çemberin pürüzsüzlüğü
    public float radius = 1.0f; // Çemberin yarıçapı
    public LineRenderer lineRenderer;
    public LineRenderer lineToInters;

    public LineRenderer horizontalAxis;
    public LineRenderer verticalAxis;
    
    
    public RectTransform canvasRect; // Canvas'ın RectTransform'ı

    public Transform point; // Kesişim noktasını gösterecek nesne
    public RectTransform uiPoint; // Eğer UI kullanıyorsanız

    [SerializeField] private bool isItNonLinear = false; //Curved or Lineear?


    [SerializeField] private TextMeshProUGUI horizontalAxisValueTextField, verticalAxisValueTextField;
    
    
    private void Start()
    {
        lineRenderer.positionCount = segments + 1;
        lineRenderer.loop = true;
        DrawCircle();
        DrawAxis();
    }

    private void DrawAxis()
    {
        horizontalAxis.SetPosition(0, new Vector3(-radius,0,0));
        horizontalAxis.SetPosition(1, new Vector3(radius,0,0));
        verticalAxis.SetPosition(0, new Vector3(0,-radius,0));
        verticalAxis.SetPosition(1, new Vector3(0,radius,0));
    } 
    
    private void DrawCircle()
    {
        float angle = 0f;
        for (int i = 0; i <= segments; i++)
        {
            float x = Mathf.Cos(angle) * radius;
            float y = Mathf.Sin(angle) * radius;
            lineRenderer.SetPosition(i, new Vector3(x, y, 0));
            angle += 2 * Mathf.PI / segments;
        }
    }
    

    void Update()
    {
            // Fare pozisyonunu dünya koordinatlarına çevir
            Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z));

            Vector2 position2D = new Vector2(mouseWorldPosition.x, mouseWorldPosition.y); // 2D pozisyon
            if (position2D.magnitude > radius) 
            {
                position2D = position2D.normalized * radius;
            }

            mouseWorldPosition.x = position2D.x;
            mouseWorldPosition.y = position2D.y;
            
            // LineRenderer'ın pozisyonlarını güncelle
            lineToInters.SetPosition(0, Vector3.zero); // Çizginin başlangıç noktası
            lineToInters.SetPosition(1, mouseWorldPosition);  // Çizginin bitiş noktası (fare)

            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvasRect, Input.mousePosition, null, out localPoint);

            float localPointRadius = radius * canvasRect.lossyScale.x * 100;
            
            if (localPoint.magnitude > localPointRadius)
            {
                
                localPoint = localPoint.normalized * localPointRadius;
            }

            uiPoint.anchoredPosition = localPoint;
            
            //point.position = new Vector3(mouseWorldPosition.x, mouseWorldPosition.y, -Camera.main.transform.position.z);

            horizontalAxisValueTextField.text = Calculate(lineToInters.GetPosition(1).x, -5,5).ToString();
            verticalAxisValueTextField.text = Calculate(lineToInters.GetPosition(1).y, -5,5).ToString();

    }
    
    private float Calculate(float input, float min, float max)
    {
        return isItNonLinear
            ? Mathf.Pow(Mathf.Clamp(input, min, max) / max, 2)
            : Mathf.Clamp(input, min, max) / max;
    }
    
}
