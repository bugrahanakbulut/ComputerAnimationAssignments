using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InputUI : MonoBehaviour
{
    [SerializeField] private MovementSimulator _simulator = null;
    
    [SerializeField] private TextMeshProUGUI _initialPosText = null;

    [SerializeField] private TextMeshProUGUI _initialVelocityText = null;

    [SerializeField] private Slider _timeStepSlider = null;

    [SerializeField] private TextMeshProUGUI _timeStepText = null;
    
    [SerializeField] private Slider _displaySlider = null;

    [SerializeField] private TextMeshProUGUI _stepText = null;

    [SerializeField] private TextMeshProUGUI _windVelocityText = null;

    [SerializeField] private TextMeshProUGUI _massText = null;

    [SerializeField] private TextMeshProUGUI _airResis = null;
    
    public void OnBallPositionEditFinished()
    {
        String s = _initialPosText.GetParsedText();
        
        _simulator.UpdateInitialPosition(VectorParser(s));
    }

    public void OnBallVelocityEditFinished()
    {
        String s = _initialVelocityText.GetParsedText();

        _simulator.UpdateInitialVelocity(VectorParser(s));
    }

    public void OnTimeStepValueUpdated()
    {
        float value = _timeStepSlider.value;
        
        _timeStepText.SetText(value.ToString("0.00"));

        _simulator.UpdateTimeStep(value);
    }

    public void OnDisplayStepUpdated()
    {
        int step = (int) _displaySlider.value;
        
        _stepText.SetText(step.ToString());
        
        _simulator.DisplayStep(step);
    }

    public void InitTimeStepSlider(float val)
    {
        _timeStepSlider.value = val;
        
        _timeStepText.SetText(val.ToString("0.00"));
    }

    public void InitStepSlider(int maxCount, int currentStep)
    {
        _displaySlider.maxValue = maxCount;

        _displaySlider.value = currentStep;
        
        _stepText.SetText(currentStep.ToString());
    }
    
    public void OnWindVelocityEditFinished()
    {
        String s = _windVelocityText.GetParsedText();
        
        _simulator.UpdateWindPosition(VectorParser(s));
    }

    public void OnMassEditFinished()
    {
        String s = _massText.GetParsedText();

        _simulator.UpdateMass(FloatParser(s));
    }

    public void OnAirResEditFinished()
    {
        String s = _airResis.GetParsedText();

        _simulator.UpdateAirResFloatParser(FloatParser(s));
    }
    
    private Vector3 VectorParser(string s)
    {
        s = s.Trim();
        string[] split = s.Split(',');

        if (split.Length < 3) return Vector3.zero;

        float x = float.Parse(split[0].Trim());
        float y = float.Parse(split[1].Trim());
        float z = float.Parse(split[2].Trim().Split((char)8203)[0]);
        
        return new Vector3(x, y, z);
    }

    private float FloatParser(string s)
    {
        s = s.Trim();

        s = s.Trim((char) 8203);

        return float.Parse(s);
    }
}
