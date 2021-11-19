using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Main : MonoBehaviour
{
    #region Fields

    public List<BaseStressBallConfiguration> StressBallOptions = new List<BaseStressBallConfiguration>();
    public GameObject StressBallObject;

    [Header("UI")]
    [Space(20)]
    public TMP_Dropdown UIDropdown;
    public Slider UISliderSize;
    public TMP_Text TextMinSize;
    public TMP_Text TextMaxSize;
    public TMP_Text TextCurrentSize;
    public UnityEngine.UIElements.ScrollView ScrollViewColorOptions;
    public RectTransform ScrollViewColorOptionsContainer;
    public Button PrefabColorOption;

    Dictionary<TMP_Dropdown.OptionData, BaseStressBallConfiguration> _dropDownCombinations = new Dictionary<TMP_Dropdown.OptionData, BaseStressBallConfiguration>();

    Material _ballMaterial;

    List<Button> _pooledColorOptionButtons = new List<Button>();

    #endregion Fields

    #region Unity / Base

    void Start()
    {
        _ballMaterial = StressBallObject.GetComponent<MeshRenderer>().material;

        PopulateDropDownBallType();
        DropDownTypeChanged();
    }

    #endregion Unity / Base

    #region Methods

    /// <summary>
    /// Ball type changed.
    /// </summary>
    public void DropDownTypeChanged()
    {
        TMP_Dropdown.OptionData optionData = UIDropdown.options[UIDropdown.value];

        UpdateUI(_dropDownCombinations[optionData]);
        UpdateBall(_dropDownCombinations[optionData]);
    }

    /// <summary>
    /// Bal size slider changed.
    /// </summary>
    public void SliderSizeChanged()
    {
        UpdateBallSize();

        TextCurrentSize.text = GetBallValue().ToString();
    }

    /// <summary>
    /// Populates the dropdown list with the saved ball configurations.
    /// </summary>
    void PopulateDropDownBallType()
    {
        List<TMP_Dropdown.OptionData> auxList = new List<TMP_Dropdown.OptionData>();
        foreach (BaseStressBallConfiguration stressBallConfig in StressBallOptions)
        {
            TMP_Dropdown.OptionData data = new TMP_Dropdown.OptionData(stressBallConfig.ConfigurationName);

            _dropDownCombinations.Add(data, stressBallConfig);
            auxList.Add(data);
        }

        UIDropdown.AddOptions(auxList);
    }

    /// <summary>
    /// Updates the UI element values.
    /// </summary>
    /// <param name="stressBallConfiguration"> Stress ball data. </param>
    void UpdateUI(BaseStressBallConfiguration stressBallConfiguration)
    {
        TextMinSize.text = stressBallConfiguration.MinimumSize.ToString();
        TextMaxSize.text = stressBallConfiguration.MaximumSize.ToString();
        TextCurrentSize.text = stressBallConfiguration.MaximumSize.ToString();

        UISliderSize.minValue = stressBallConfiguration.MinimumSize;
        UISliderSize.maxValue = stressBallConfiguration.MaximumSize;
        UISliderSize.value = stressBallConfiguration.MinimumSize;

        for (int i = 0; i < stressBallConfiguration.AvailableColors.Count; i++)
        {
            if (i < _pooledColorOptionButtons.Count) //Update pooled buttons
            {
                Button btn = _pooledColorOptionButtons[i];
                btn.gameObject.SetActive(true);
                Image btnImage = btn.GetComponent<Image>();

                btn.onClick.RemoveListener(() => UpdateBallColor(btnImage.color));
                btn.onClick.AddListener(() => UpdateBallColor(btnImage.color));

                btnImage.color = stressBallConfiguration.AvailableColors[i];
            }
            else //Create new buttons
            {
                Button btnColorOption = Instantiate(PrefabColorOption);

                Color color = stressBallConfiguration.AvailableColors[i];
                btnColorOption.GetComponent<Image>().color = color;
                btnColorOption.GetComponent<RectTransform>().parent = ScrollViewColorOptionsContainer;
                btnColorOption.onClick.AddListener(() => UpdateBallColor(color));

                _pooledColorOptionButtons.Add(btnColorOption);
            }
        }

        //Disable the unused buttons
        for (int i = stressBallConfiguration.AvailableColors.Count; i < _pooledColorOptionButtons.Count; i++)
            _pooledColorOptionButtons[i].gameObject.SetActive(false);
    }

    /// <summary>
    /// Updates the stress ball size and material.
    /// </summary>
    /// <param name="baseStressBallConfiguration"> Stress ball data. </param>
    void UpdateBall(BaseStressBallConfiguration baseStressBallConfiguration)
    {
        UpdateBallSize();
        UpdateBallColor(baseStressBallConfiguration.AvailableColors[0]);
    }

    void UpdateBallSize()
    {
        float sliderValue = GetBallValue();

        _ballMaterial.SetFloat("SphereGrouthSize", sliderValue);
    }

    void UpdateBallColor(Color color)
    {
        _ballMaterial.color = color;
        _ballMaterial.SetColor("SphereColor", color);
    }

    /// <summary>
    /// Returns the size of the ball, rounded to match the constrained values.
    /// </summary>
    /// <returns></returns>
    float GetBallValue()
    {
        float toRound = UISliderSize.value - 0.0001f;
        return (toRound - toRound % BaseStressBallConfiguration.IncrementalSizeChange) + BaseStressBallConfiguration.IncrementalSizeChange;
    }

    #endregion Methods
}